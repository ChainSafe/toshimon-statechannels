// SPDX-License-Identifier: UNLICENSED

/**
 * Created on 2022-06-02 12:37
 * @summary: An example commit-reveal randomness application. It is a simple battle game where
 * players can attack, defend or drink coffee. The player with the highest speed state (obtained from drinking the most coffee)
 * gets to attack first each turn. If the players speed is equal then the combined random seed is used to randomly select which 
 * player is first.
 * @author: Willem Olding (ChainSafe)
 */
pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

import "@openzeppelin/contracts/math/SafeMath.sol";
import './../CommitRevealApp.sol';

struct GameState {
    PlayerState[2] players;
}

struct PlayerState {
    uint8 health;
    uint8 speed;
}

contract ExampleCommitRevealApp is CommitRevealApp {
    using SafeMath for uint8;

    enum Move { ATTACK, DEFEND, COFFEE }

    // the damage done in each case, can adjust for balance
    uint8 constant MINIMAL_ATTACK = 2;
    uint8 constant ATTACK = 5;
    uint8 constant COFFEE_SPEED_INCREASE = 1;

    function _gameState(bytes memory gameStateBytes) internal pure returns (GameState memory) {
        return abi.decode(gameStateBytes, (GameState));
    }

    function _attack(GameState memory gameState, uint8 receiver, uint8 damage) internal pure returns (GameState memory) {
        gameState.players[receiver].health -= damage;
        return (gameState);
    }

    function _speedUp(GameState memory gameState, uint8 receiver, uint8 increase) internal pure returns (GameState memory) {
        gameState.players[receiver].speed += increase;
        return (gameState);
    }

    function _resolveBothAttack(GameState memory gameState, uint8 firstMover) internal pure returns (GameState memory) {
        gameState = _attack(gameState, ~firstMover, ATTACK);
        if (gameState.players[~firstMover].health > 0) {  // other player attacks only if still alive
            gameState = _attack(gameState, firstMover, ATTACK);
        }
        return (gameState);
    }

    function advanceState(
        bytes memory _gameState_,
        Outcome.SingleAssetExit[] memory outcome,
        uint8 _moveA,
        uint8 _moveB,
        bytes32 randomSeed
    ) override public pure returns (bytes memory, Outcome.SingleAssetExit[] memory, bool) {
        GameState memory gameState = _gameState(_gameState_);

        Move moveA = Move(_moveA);
        Move moveB = Move(_moveB);

        // enumerate all the combinations
        if        (moveA == Move.ATTACK && moveB == Move.ATTACK) {
            // Both attack, fastest player hits first
            if (gameState.players[A].speed > gameState.players[B].speed) {
                gameState = _resolveBothAttack(gameState, A);
            } else if (gameState.players[B].speed > gameState.players[A].speed) {
                gameState = _resolveBothAttack(gameState, B);
            } else { // speeds are evenly matched. Randomly choose who goes first
                if (uint(randomSeed) % 2 == 0) {
                    gameState = _resolveBothAttack(gameState, A);
                } else {
                    gameState =_resolveBothAttack(gameState, B);
                }
            }
        } else if (moveA == Move.ATTACK && moveB == Move.DEFEND) {
            // A lands minimal attack on B
             gameState = _attack(gameState, B, MINIMAL_ATTACK);
        } else if (moveA == Move.ATTACK && moveB == Move.COFFEE) {
            // A lands big attack
            gameState = _attack(gameState, B, ATTACK);
        } else if (moveA == Move.DEFEND && moveB == Move.ATTACK) {
            // B lands minimal attack on A
             gameState = _attack(gameState, A, MINIMAL_ATTACK);
        } else if (moveA == Move.DEFEND && moveB == Move.DEFEND) {
            // Nothing happens...
        } else if (moveA == Move.DEFEND && moveB == Move.COFFEE) {
            // B speed goes up
            gameState = _speedUp(gameState, B, COFFEE_SPEED_INCREASE);
        } else if (moveA == Move.COFFEE && moveB == Move.ATTACK) {
            // B lands big attack
            gameState = _attack(gameState, A, ATTACK);
        } else if (moveA == Move.COFFEE && moveB == Move.DEFEND) {
            // A speed goes up
            gameState = _speedUp(gameState, A, COFFEE_SPEED_INCREASE);
        } else if (moveA == Move.COFFEE && moveB == Move.COFFEE) {
            // A and B speed goes up
            gameState = _speedUp(gameState, A, COFFEE_SPEED_INCREASE);
            gameState = _speedUp(gameState, B, COFFEE_SPEED_INCREASE);
        }

        // check if the game is concluded and update the conclude flag and outcome
        // as required
        if (gameState.players[A].health == 0) {
            return (abi.encode(gameState), updateOutcomeFavourPlayer(outcome, A), true);
        } else if (gameState.players[A].health == 0) {
            return (abi.encode(gameState), updateOutcomeFavourPlayer(outcome, B), true);
        } else {
            return (abi.encode(gameState), outcome, false);
        }


    }

    // For incentive reasons it needs to ensure that each time a player makes
    // a state update they set themselves as the winning player
    // unless the game forces otherwise via a conclusion
    // 
    // The entire balance is reallocated to the winning player index by this function
    // 
    // This assumes the outcome is ordered according to the players.
    function updateOutcomeFavourPlayer(
        Outcome.SingleAssetExit[] memory outcome,
        uint8 playerIndex
    ) override public pure returns (Outcome.SingleAssetExit[] memory) {
        Outcome.SingleAssetExit memory assetExit = outcome[0];
        uint256 total = assetExit.allocations[0].amount + assetExit.allocations[1].amount;

        outcome[0].allocations[playerIndex].amount = total;
        outcome[0].allocations[~playerIndex].amount = 0;

        return (outcome);
    }

}
