// SPDX-License-Identifier: MIT

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

import './nitro-protocol/contracts/interfaces/IForceMoveApp.sol';
import {ExitFormat as Outcome} from '@statechannels/exit-format/contracts/ExitFormat.sol';

abstract contract CommitRevealApp is IForceMoveApp {

    // player indices
    uint8 constant A = 0;
    uint8 constant B = 1;

    // The phases of the protocol
    enum Phase { A_COMMIT, B_COMMIT, A_REVEAL, B_REVEAL }

    struct Reveal {
        uint8 move;
        bytes32 seed;
    }

    struct AppData {
        uint256 wager;
        uint256 bond;

        bytes32 preCommitA;
        bytes32 preCommitB;

        Reveal revealA;
        Reveal revealB;

        bytes gameState;
    }

    function advanceState(
        bytes memory gameState,
        Outcome.SingleAssetExit[] memory outcome,
        uint8 moveA,
        uint8 moveB,
        bytes32 randomSeed
    ) virtual public pure returns (bytes memory, Outcome.SingleAssetExit[] memory, bool);

    /// Take an old outcome and update it to favour a given player
    /// Assumes the outcome allocations can be indexed by the playerIndex
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

    function _phase(uint48 turnNum) internal pure returns (Phase) {
        // This conversion is safe as the modulo is always < 4
        return Phase(turnNum % 4);
    }

    function validTransition(
        VariablePart memory prev,
        VariablePart memory next,
        uint256 nParticipants
    ) public pure override returns (bool) {
        require(nParticipants == 2, "Only two participant commit/reveal games are supported");

        // we are in the commit reveal cycle of gameplay
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
            // no change constraints
            // require(prevData.preCommitA == nextData.preCommitA, "Cannot mutate A's preCommit in [B reveal] move");
            // require(prevData.preCommitB == nextData.preCommitB, "Cannot mutate B's preCommit in [B reveal] move");
            // require(_compareReveals(prevData.revealA, nextData.revealA), "Cannot mutate A's reveal in [B reveal] move");
            // reveal matches preCommit
            require(prevData.preCommitB == keccak256(abi.encode(nextData.revealB)));
            // outcome
            require(_compareOutcomes(next.outcome, updateOutcomeFavourPlayer(prev.outcome, B)));
            // game state update is made correctly with respect to the committed moves and random seeds
            bytes32 randomSeed = _mergeSeeds(nextData.revealA.seed, nextData.revealB.seed);
            (bytes memory newState,,) = advanceState(prevData.gameState, prev.outcome, nextData.revealA.move, nextData.revealB.move, randomSeed);
            require(
                _compareBytes(
                    newState,
                    nextData.gameState
                ), "New state must be computed based on preCommit moves in [B reveal] move"
            );
        }

        return true;
    }
}
