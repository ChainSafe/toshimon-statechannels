// SPDX-License-Identifier: MIT

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

import './nitro-protocol/contracts/interfaces/IForceMoveApp.sol';
import {ExitFormat as Outcome} from '@statechannels/exit-format/contracts/ExitFormat.sol';

abstract contract CommitRevealApp is IForceMoveApp {

    struct Reveal {
        uint8 move;
        bytes32 seed;
    }

    struct AppData {
        bytes32 preCommitA;
        bytes32 preCommitB;

        Reveal revealA;
        Reveal revealB;

        bytes gameState;
    }

    function advanceState(
        bytes memory gameState,
        uint8 moveA,
        uint8 moveB,
        bytes32 randomSeed
    ) virtual public pure returns (bytes memory);

    /**
     * @notice Decodes the appData.
     * @dev Decodes the appData.
     * @param appDataBytes The abi.encode of an AppData struct describing the application-specific data.
     * @return A AppData struct containing the application-specific data.
     */
    function appData(bytes memory appDataBytes) internal pure returns (AppData memory) {
        bytes memory decodedAppData = abi.decode(appDataBytes, (bytes));
        return abi.decode(decodedAppData, (AppData));
    }

    /**
     * @notice Combines two pieces of randomness to produce a single random seed
     * @dev Combines two pieces of randomness to produce a single random seed
     * @param partSeedA A random value
     * @param partSeedB A random value
     * @return A random seed from the combined randomness
     */
    function mergeSeeds(bytes32 partSeedA, bytes32 partSeedB) internal pure returns (bytes32) {
        return keccak256(abi.encode(partSeedA, partSeedB));
    }

    // helper to do byte array comparison
    function compareBytes(bytes memory a, bytes memory b) public pure returns (bool) {
        return (keccak256(abi.encodePacked((a))) == keccak256(abi.encodePacked((b))));
    }

    // helper to do byte array comparison
    function compareReveals(Reveal memory a, Reveal memory b) public pure returns (bool) {
        return (keccak256(abi.encode((a))) == keccak256(abi.encode((b))));
    }

    function validTransition(
        VariablePart memory a,
        VariablePart memory b,
        uint256 nParticipants
    ) public pure override returns (bool) {
        require(nParticipants == 2, "Only two participant commit/reveal games are supported");
        require(a.turnNum == b.turnNum + 1, "Transition must increment turn number"); // not sure if we have to check this...

        // we are in the commit reveal cycle of gameplay
        uint48 phase = b.turnNum % 4;

        AppData memory aData = appData(a.appData);
        AppData memory bData = appData(b.appData);

        if        (phase == 0) { // A commit
            // no change constraints
            require(compareBytes(aData.gameState, bData.gameState), "Cannot mutate the game state in [A commitment] move");
        } else if (phase == 1) { // B commit
            // no change constraints
            require(compareBytes(aData.gameState, bData.gameState), "Cannot mutate the game state in [B commitment] move");
            require(aData.preCommitA == bData.preCommitA, "Cannot mutate A's preCommit in [B commitment] move");
        } else if (phase == 2) { // A reveal
            // no change constraints
            require(compareBytes(aData.gameState, bData.gameState), "Cannot mutate the game state in [A reveal] move");
            require(aData.preCommitA == bData.preCommitA, "Cannot mutate A's preCommit in [A reveal] move");
            require(aData.preCommitB == bData.preCommitB, "Cannot mutate B's preCommit in [A reveal] move");
            // reveal matches preCommit
            require(aData.preCommitA ==  keccak256(abi.encode(bData.revealA)));
        } else if (phase == 3) { // B reveal
            // no change constraints
            require(aData.preCommitA == bData.preCommitA, "Cannot mutate A's preCommit in [B reveal] move");
            require(aData.preCommitB == bData.preCommitB, "Cannot mutate B's preCommit in [B reveal] move");
            require(compareReveals(aData.revealA, bData.revealA), "Cannot mutate A's reveal in [B reveal] move");
            // reveal matches preCommit
            require(aData.preCommitB == keccak256(abi.encode(bData.revealB)));
            // game state update is made correctly with respect to the committed moves and random seeds
            bytes32 randomSeed = mergeSeeds(bData.revealA.seed, bData.revealB.seed);
            require(
                compareBytes(
                    advanceState(aData.gameState, bData.revealA.move, bData.revealB.move, randomSeed),
                    bData.gameState
                ), "New state must be computed based on preCommit moves in [B reveal] move"
            );
        }

        return true;
    }

    // checks a transition between two outcomes for validity
    // ensures the outcomes follow the requirements of:
    //  - single asset
    //  - always redistributed (no balance is created or destroyed)
    //  - all allocations are simple and can be withdrawns
    //  - there are two allocations for the single asset (one per player)
    function _validOutcomeTransition(
        Outcome.SingleAssetExit[] memory outcomeA,
        Outcome.SingleAssetExit[] memory outcomeB
    ) private pure returns (bool) {
        require(_validOutcome(outcomeA), "a state does not have a valid outcome");
        require(_validOutcome(outcomeB), "a state does not have a valid outcome");

        Outcome.SingleAssetExit memory assetOutcomeA = outcomeA[0];
        Outcome.SingleAssetExit memory assetOutcomeB = outcomeB[0];

        Outcome.Allocation[] memory allocationsA = assetOutcomeA.allocations;
        Outcome.Allocation[] memory allocationsB = assetOutcomeB.allocations;


        // Interprets the nth outcome as benefiting participant n
        // checks the destinations have not changed
        // Checks that the sum of assets hasn't changed
        // And that for all non-movers
        // the balance hasn't decreased
        require(
            allocationsB[0].destination == allocationsA[0].destination,
            'Destinations may not change'
        );
        require(
            allocationsB[1].destination == allocationsA[1].destination,
            'Destinations may not change'
        );

        require(allocationsA[0].amount + allocationsA[1].amount == allocationsB[0].amount + allocationsB[1].amount, 
            'Total allocated cannot change');

        return (true);
    }

    // checks a single outcome for validity
    function _validOutcome(
        Outcome.SingleAssetExit[] memory outcome
    ) private pure returns (bool) {
        require(outcome.length == 1, 'Only one asset allowed');

        Outcome.SingleAssetExit memory assetOutcome = outcome[0];

        // Throws unless that allocation has exactly 2 outcomes
        Outcome.Allocation[] memory allocations = assetOutcome.allocations;

        require(allocations.length == 2, '|AllocationA|!=|participants|');

        // require all to be simple allocations
        require(
            allocations[0].allocationType == uint8(Outcome.AllocationType.simple),
            'not a simple allocation'
        );
        require(
            allocations[1].allocationType == uint8(Outcome.AllocationType.simple),
            'not a simple allocation'
        );

        return (true);
    }


}
