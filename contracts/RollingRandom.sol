// SPDX-License-Identifier: MIT

pragma solidity 0.7.4;
pragma experimental ABIEncoderV2;

import '@statechannels/nitro-protocol/contracts/interfaces/IForceMoveApp.sol';
import '@statechannels/nitro-protocol/contracts/Outcome.sol';

contract RollingRandom is IForceMoveApp {

    /**
     * The data required for a RollingRandom application with two participants
     */
    struct RollingRandomAppData {
        bytes32 commit; // The fresh new commit being added by this move
        bytes32 prev_commit; // commit from state i-1, required due to state cacheing
        uint256 reveal; // the reveal of the commit from state i-2
        bytes32 random_seed; // This is the result of combining the previons n reveals. In a proper app this need not be included, only the result of using this to randomly progress the state
    }

    /**
     * @notice Decodes the appData.
     * @dev Decodes the appData.
     * @param appDataBytes The abi.encode of a RollingRandomAppData struct describing the application-specific data.
     * @return A RollingRandomAppDatat struct containing the application-specific data.
     */
     function appData(bytes memory appDataBytes) internal pure returns (RollingRandomAppData memory) {
        return abi.decode(appDataBytes, (RollingRandomAppData));
    }

    /**
     * @notice Combines two pieces of randomness to produce a single random seed
     * @dev Combines two pieces of randomness to produce a single random seed
     * @param partSeedA A random value
     * @param partSeedB A random value
     * @return A random seed from the combined randomness
     */
    function mergeSeeds(uint256 partSeedA, uint256 partSeedB) internal pure returns (bytes32) {
        return keccak256(abi.encode(partSeedA, partSeedB));
    }


    /**
     * @notice Encodes the RollingRandom rules.
     * @dev Encodes the RollingRandom rules.
     * @param a State being transitioned from.
     * @param b State being transitioned to.
     * @return true if the transition conforms to the rules, false otherwise.
     */
    function validTransition(
        VariablePart calldata a,
        VariablePart calldata b,
        uint48 turnNumB,
        uint256 nParticipants
        ) public override pure returns (bool) {
        // Outcome.OutcomeItem[] memory outcomeA = abi.decode(a.outcome, (Outcome.OutcomeItem[]));
        // Outcome.OutcomeItem[] memory outcomeB = abi.decode(b.outcome, (Outcome.OutcomeItem[]));

        RollingRandomAppData memory prevState = appData(a.appData);
        RollingRandomAppData memory newState = appData(b.appData);
        
        // we only support 2 participants at this time otherwise the channel is frozen
        require(nParticipants == 2, "Only two participant channels are allowed");

        /* The rest of your logic */

        // for the first two turns only commitments are made, so no migrations or reveals required
        // No need to enforce these because players who fail to update the commits only disadvantage themselves
        if( turnNumB > 1) {
            // ensure the previous commit is migrated correctly
            require(newState.prev_commit == prevState.commit, "Commit from prior state was not moved to prev_commit in new state");

            // ensure the new reveal is for the i-2 commit
            require(prevState.prev_commit == keccak256(abi.encode(newState.reveal)), "The revealed value is not the keccak256 preimage of the commitment stored in the prev_state.prev_commit");
        }

        // for turn 3 and onward we have access to randomness and so must put it in the state
        if ( turnNumB > 2 ) {
            // ensure the random seed is produced correctly. In an actual application this should would instead be for a state transition
            // that made use of the random seed. In this case the state transition is just adding the seed to the state.
            require(newState.random_seed == mergeSeeds(prevState.reveal, newState.reveal), "The combined random seed included in the new state is not correctly produced from the seeds");
        }


        return true;
    }
}
