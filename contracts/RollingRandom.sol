// SPDX-License-Identifier: MIT

pragma solidity 0.7.4;
pragma experimental ABIEncoderV2;

import '@statechannels/nitro-protocol/contracts/interfaces/IForceMoveApp.sol';
import '@statechannels/nitro-protocol/contracts/Outcome.sol';

contract RollingRandom is IForceMoveApp {

    enum Mover { A, B }
    enum Phase { Commit, Seed, Reveal }
    enum Move { Hit, Stand }

    /**
     * The data required for a RollingRandom application with two participants
     */
    struct RollingRandomAppData {
        bytes32 commit; // A salted (with salt_seed) commitment to a move
        bytes32 seed; // A seed provided by a player to seed randomness for the other players move
        bytes32 reveal_salt_seed; // Salt used in the commitment that also is used as the other part of the random seed
        Move reveal_move; // She reveal of the move commit to in the most recent commit

        // Depending on the combined random seed either A or Bs counter will increase.
        // If the seed is even A will increase, if it is odd B will increase.
        uint8 a_counter;
        uint8 b_counter;
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
    function mergeSeeds(bytes32 partSeedA, bytes32 partSeedB) internal pure returns (bytes32) {
        return keccak256(abi.encode(partSeedA, partSeedB));
    }

    /**
     * @notice Combines two pieces of randomness to produce a single random seed
     * @dev Combines two pieces of randomness to produce a single random seed
     * @param combinedSeed A random value
     * @return A random number that could come from a dice
     */
    function getDiceRoll(bytes32 combinedSeed) internal pure returns (uint) {
        return uint(combinedSeed) % 6 + 1;
    }

    function getMover(uint48 turnNum) internal pure returns (Mover) {
        if ((turnNum / 3) % 2 == 0) {
            return Mover.A;
        } else {
            return Mover.B;
        }
    }

    function getPhase(uint48 turnNum) internal pure returns (Phase) {
        if (turnNum % 3 == 0) {    
            return Phase.Commit;
        } else if (turnNum % 3 == 1) {
            return Phase.Seed;
        } else {
            return Phase.Reveal;
        }
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
        uint48 turnNum,
        uint256 nParticipants
        ) public override pure returns (bool) {
        // Outcome.OutcomeItem[] memory outcomeA = abi.decode(a.outcome, (Outcome.OutcomeItem[]));
        // Outcome.OutcomeItem[] memory outcomeB = abi.decode(b.outcome, (Outcome.OutcomeItem[]));

        RollingRandomAppData memory prevState = appData(a.appData);
        RollingRandomAppData memory newState = appData(b.appData);
        
        // we only support 2 participants at this time otherwise the channel is frozen
        require(nParticipants == 2, "Only two participant channels are allowed");

        /* The rest of your logic */
        Mover mover = getMover(turnNum);
        Phase phase = getPhase(turnNum);

        if (phase == Phase.Commit) {    
            // mover commit phase
            // actually no requirements here (mover can shaft themselves if they like by not providing fresh commitments)   
            // copy contraints
            require(prevState.a_counter == newState.a_counter, "Counter was illegally updated");
            require(prevState.b_counter == newState.b_counter, "Counter was illegally updated");

        } else if (phase == Phase.Seed) {
            // other seed phase
            // copy contraints
            require(prevState.commit == newState.commit, "Updated state did not bring forward commit from prior state");
            require(prevState.a_counter == newState.a_counter, "Counter was illegally updated");
            require(prevState.b_counter == newState.b_counter, "Counter was illegally updated");
        } else  { // (phase == Phase.Reveal)
            // reveal and execute phase
            require(prevState.commit == keccak256(abi.encode(newState.reveal_move, newState.reveal_salt_seed)), "The revealed move and salt_seed is not the keccak256 preimage of the commitment stored in the prev_state.commit");
            // also ensure any changes to the game state are consistent with the move
            
            bytes32 combinedSeed = mergeSeeds(prevState.seed, prevState.commit);
            uint diceRoll = getDiceRoll(combinedSeed);

            if (newState.reveal_move == Move.Hit) {
                if (mover == Mover.A) {
                    require(newState.a_counter == prevState.a_counter + diceRoll, "State update did not correctly implement incrementing the counter");
                } else { // mover == Mover.B
                    require(newState.b_counter == prevState.b_counter + diceRoll, "State update did not correctly implement incrementing the counter");
                }
            } else { // Move.Stand
                // nothing required
            }
        }

        return true;
    }
}
