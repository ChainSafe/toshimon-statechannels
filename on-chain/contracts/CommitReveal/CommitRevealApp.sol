// SPDX-License-Identifier: MIT

/**
 * Created on 2022-06-02 12:37
 * @summary: An abstract contract for implementing a ForceMove application with shared randomness
 * @author: Willem Olding (ChainSafe)
 */
pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

import '@statechannels/nitro-protocol/contracts/interfaces/IForceMoveApp.sol';
import { StrictTurnTaking } from '@statechannels/nitro-protocol/contracts/libraries/signature-logic/StrictTurnTaking.sol';
import {ExitFormat as Outcome} from '@statechannels/exit-format/contracts/ExitFormat.sol';

abstract contract CommitRevealApp is IForceMoveApp {

    // player indices
    uint8 constant A = 0;
    uint8 constant B = 1;

    // The phases of the protocol
    enum Phase { A_COMMIT, B_COMMIT, A_REVEAL, B_REVEAL }

    // Data that must be included in each reveal phase state update
    struct Reveal {
        uint8 move;
        bytes32 seed;
    }

    // Application specific data for the ForceMove app
    // This itself contains game specific data
    struct AppData {
        bytes32 preCommitA;
        bytes32 preCommitB;

        Reveal revealA;
        Reveal revealB;

        bytes gameState;
    }

    /**
     * @dev Takes a game state and outcome and mutates it using a move from each player
     *      collaboratively produced randomness
     * @param gameState The prior game state to be mutated and returned
     * @param outcome The prior outcome that can be mutated and returned
     * @param moveA Move from player A
     * @param moveB Move from player B
     * @param randomSeed The combined random seed produced by the protocol 
     * @return The outcome resulting in a rebalance to the given player
     */
    function advanceState(
        bytes memory gameState,
        Outcome.SingleAssetExit[] memory outcome,
        uint8 moveA,
        uint8 moveB,
        bytes32 randomSeed
    ) virtual public pure returns (bytes memory, Outcome.SingleAssetExit[] memory, bool);

    /**
     * @dev Take an old outcome and update it to favour a given player
     *       Assumes the outcome allocations can be indexed by the playerIndex
     * @param outcome An outcome object to be rebalanced
     * @param playerIndex Zero based index of the player to be assigned the favoured outcome
     * @return The outcome resulting in a rebalance to the given player
     */
    function updateOutcomeFavourPlayer(
        Outcome.SingleAssetExit[] memory outcome,
        uint8 playerIndex
    ) virtual public pure returns (Outcome.SingleAssetExit[] memory);


    /**
     * @notice Decodes the appData.
     * @dev Decodes the appData.
     * @param appDataBytes The abi.encode of an AppData struct describing the application-specific data.
     * @return A AppData struct containing the application-specific data.
     */
    function _appData(bytes memory appDataBytes) internal pure returns (AppData memory) {
        return abi.decode(appDataBytes, (AppData));
    }

    /**
     * @notice Combines two pieces of randomness to produce a single random seed
     * @dev Combines two pieces of randomness to produce a single random seed
     * @param partSeedA A random value
     * @param partSeedB A random value
     * @return A random seed from the combined randomness
     */
    function _mergeSeeds(bytes32 partSeedA, bytes32 partSeedB) internal pure returns (bytes32) {
        return keccak256(abi.encode(partSeedA, partSeedB));
    }

    // helper to do byte array comparison
    // can replace with a more efficient version later
    function _compareBytes(bytes memory a, bytes memory b) internal pure returns (bool) {
        return (keccak256(abi.encodePacked((a))) == keccak256(abi.encodePacked((b))));
    }

    // helper to do byte array comparison
    function _compareReveals(Reveal memory a, Reveal memory b) internal pure returns (bool) {
        return (keccak256(abi.encode((a))) == keccak256(abi.encode((b))));
    }

    // helper to do byte array comparison
    function _compareOutcomes(Outcome.SingleAssetExit[] memory a, Outcome.SingleAssetExit[] memory b) internal pure returns (bool) {
        return (keccak256(abi.encode((a))) == keccak256(abi.encode((b))));
    }

    /**
     * @dev Get the current phase given the turn number
     * @param turnNum The current sequence/turn number
     * @return Phase
     */
    function _phase(uint48 turnNum) internal pure returns (Phase) {

        // The first 4 turnNums (0,1,2,3) are used for the pre-fund and post-fund setup phase
        // This function will be called at all with those turnNums.
        // Coincidentally this is a multiple of the number of phases in CommitReveal so there 
        // is no need to adjust.
        
        // This conversion is safe as the modulo is always < 4
        return Phase(turnNum % 4);
    }


    /**
     * @notice Encodes application-specific rules for a particular ForceMove-compliant state channel.
     * @dev Encodes application-specific rules for a particular ForceMove-compliant state channel.
     * @param fixedPart Fixed part of the state channel.
     * @param signedVariableParts Array of variable parts to find the latest of.
     * @return VariablePart Latest supported by application variable part from supplied array.
     */    
    function latestSupportedState(
        FixedPart calldata fixedPart,
        SignedVariablePart[] calldata signedVariableParts
    ) external pure override returns (VariablePart memory) {
        StrictTurnTaking.requireValidTurnTaking(fixedPart, signedVariableParts);
        require(fixedPart.participants.length == 2, "Only two participant commit/reveal games are supported");

        for (uint i = 1; i < signedVariableParts.length; i++) {
            require(_validTransition(signedVariableParts[i].variablePart, signedVariableParts[i-1].variablePart));
        }

        return signedVariableParts[signedVariableParts.length - 1].variablePart;
    }

    /**
     * @dev Checks for a valid transition between two states
     */
    function _validTransition(
        VariablePart memory prev,
        VariablePart memory next
    ) internal pure returns (bool) {
        
        if (next.turnNum < 4) {
            require(
                Outcome.exitsEqual(prev.outcome, next.outcome),
                'Outcome change forbidden in pre-find and post-fund stages'
            );
            require(keccak256(abi.encodePacked(prev.appData)) == 
                    keccak256(abi.encodePacked(next.appData)),
                'appData change forbidden in pre-find and post-fund stages'
            );
        }

        // we are in the commit/reveal cycle of gameplay (any turnNum >= 4 when the channel isn't concluded)
        Phase phase = _phase(next.turnNum);

        AppData memory prevData = _appData(prev.appData);
        AppData memory nextData = _appData(next.appData);

        if        (phase == Phase.A_COMMIT) {
            // no change constraints
            require(_compareBytes(prevData.gameState, nextData.gameState), "Cannot mutate the game state in [A commitment] move");
            // outcome
            require(_compareOutcomes(next.outcome, updateOutcomeFavourPlayer(prev.outcome, A)));
        } else if (phase == Phase.B_COMMIT) {
            // no change constraints
            require(_compareBytes(prevData.gameState, nextData.gameState), "Cannot mutate the game state in [B commitment] move");
            require(prevData.preCommitA == nextData.preCommitA, "Cannot mutate A's preCommit in [B commitment] move");
            // outcome
            require(_compareOutcomes(next.outcome, updateOutcomeFavourPlayer(prev.outcome, B)));
        } else if (phase == Phase.A_REVEAL) {
            // no change constraints
            require(_compareBytes(prevData.gameState, nextData.gameState), "Cannot mutate the game state in [A reveal] move");
            // require(prevData.preCommitA == nextData.preCommitA, "Cannot mutate A's preCommit in [A reveal] move");
            require(prevData.preCommitB == nextData.preCommitB, "Cannot mutate B's preCommit in [A reveal] move");
            // reveal matches preCommit
            require(prevData.preCommitA ==  keccak256(abi.encode(nextData.revealA)));
            // outcome
            require(_compareOutcomes(next.outcome, updateOutcomeFavourPlayer(prev.outcome, A)));
        } else if (phase == Phase.B_REVEAL) {
            // reveal matches preCommit
            require(prevData.preCommitB == keccak256(abi.encode(nextData.revealB)));
            // outcome
            require(_compareOutcomes(next.outcome, updateOutcomeFavourPlayer(prev.outcome, B)));
            // game state update is made correctly with respect to the committed moves and random seeds
            bytes32 randomSeed = _mergeSeeds(nextData.revealA.seed, nextData.revealB.seed);
            (bytes memory newState,, bool isFinal) = advanceState(prevData.gameState, prev.outcome, nextData.revealA.move, nextData.revealB.move, randomSeed);
            require(
                _compareBytes(
                    newState,
                    nextData.gameState
                ), "New state must be computed based on preCommit moves in [B reveal] move"
            );
            // if the game state concluded this must be reflected in the variable part of the channel state
            require(isFinal == next.isFinal);
        }

        return true;
    }
}
