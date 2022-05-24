// SPDX-License-Identifier: MIT

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;



import "@openzeppelin/contracts/math/SafeMath.sol";
import './CommitRevealApp.sol';

contract ExampleCommitRevealApp is CommitRevealApp {
    using SafeMath for uint8;

    enum Move { ATTACK, DEFEND, COFFEE }

    uint8 constant A = 0;
    uint8 constant B = 1;

    // the damage done in each case, can adjust for balance
    uint8 constant MINIMAL_ATTACK = 2;
    uint8 constant ATTACK = 5;
    uint8 constant COFFEE_SPEED_INCREASE = 1;

    struct GameState {
        PlayerState[2] players;
    }

    struct PlayerState {
        uint8 health;
        uint8 speed;
    }

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
            return (abi.encode(gameState), _setOutcomeWinner(outcome, A), true);
        } else if (gameState.players[A].health == 0) {
            return (abi.encode(gameState), _setOutcomeWinner(outcome, B), true);
        } else {
            return (abi.encode(gameState), outcome, false);
        }


    }

    // For incentive reasons it needs to ensure that each time a player makes
    // a state update they set themselves as the winning player
    // unless the game forces otherwise via a conclusion
    function updateOutcome(
        Outcome.SingleAssetExit[] memory outcome,
        Phase phase
    ) override public pure returns (Outcome.SingleAssetExit[] memory) {
        if        ( phase == Phase.A_COMMIT ) {
            return (_setOutcomeWinner(outcome, A));
        } else if ( phase == Phase.B_COMMIT ) {
            return (_setOutcomeWinner(outcome, B));
        } else if ( phase == Phase.A_REVEAL ) {
            return (_setOutcomeWinner(outcome, A));
        } else if ( phase == Phase.B_REVEAL ) { 
            return (_setOutcomeWinner(outcome, B));
        } else {
            return (outcome);
        }
    }

    // converts an outcome to make a given player the winner
    // Assumes:
    //  The allocations are stored in order of player index
    //  Only the first asset exit should be modified
    function _setOutcomeWinner(
        Outcome.SingleAssetExit[] memory outcome,
        uint8 player
    ) private pure returns (Outcome.SingleAssetExit[] memory) {
        Outcome.SingleAssetExit memory assetExit = outcome[0];
        uint256 total = assetExit.allocations[0].amount + assetExit.allocations[1].amount;

        outcome[0].allocations[player].amount = total;
        outcome[0].allocations[~player].amount = 0;

        return (outcome);
    }

}
