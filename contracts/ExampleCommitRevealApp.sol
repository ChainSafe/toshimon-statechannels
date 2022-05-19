// SPDX-License-Identifier: MIT

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

import './CommitRevealApp.sol';

contract ExampleCommitRevealApp is CommitRevealApp {

    enum Move { ATTACK, DEFEND, COFFEE }

    struct GameState {
        PlayerState playerA;
        PlayerState playerB;
    }

    struct PlayerState {
        uint8 health;
        uint8 speed;
    }

    /**
     * Compute the next game state given a move from each player and the previous state
     * @param  bytes   gameState     Serialized prior game state
     * @param  uint8   moveA         The move by player A
     * @param  uint8   moveB         The move by player B
     * @param  bytes32 randomSeed    A seed produced by the shared randomness protocol. 
     *                               This is safe to use to compute the new state
     * @return bytes        The new game state in byte serialized form
     */
    function advanceState(
        bytes memory gameState,
        uint8 moveA,
        uint8 moveB,
        bytes32 randomSeed
    ) override public pure returns (bytes memory) {
        // fastest player goes first
        // if its a tie then flip a coin to split
        
        
    }
}
