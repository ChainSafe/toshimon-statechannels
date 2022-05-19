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
