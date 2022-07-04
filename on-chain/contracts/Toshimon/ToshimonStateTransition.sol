// SPDX-License-Identifier: MIT

/**
 * Created on 2022-06-23 12:37
 * @summary: The state transition function that captures all the rules of the Toshimon battle game
 * @author: Willem Olding (ChainSafe)
 */
pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

import "@openzeppelin/contracts/math/SafeMath.sol";
import '../CommitReveal/CommitRevealApp.sol';
import { ToshimonState as TM } from './ToshimonState.sol';
import './interfaces/IMove.sol';


contract ToshimonStateTransition is CommitRevealApp {


    function _dummy(TM.GameState calldata gs) public pure returns (bool) {
        return (true);
    }

    function advanceState(
        bytes memory _gameState_,
        Outcome.SingleAssetExit[] memory outcome,
        uint8 moveA,
        uint8 moveB,
        bytes32 randomSeed
    ) override public pure returns (bytes memory, Outcome.SingleAssetExit[] memory, bool) {
        
        TM.GameState memory gameState = abi.decode(_gameState_, (TM.GameState));

        return (_gameState_, outcome, true);

        // if either player is unconcious then no more moves can be made
        // and the game is over. No further state updates possible.
        // if (_is_unconcious(gameState.players[0]) || _is_unconcious(gameState.players[1])) {
        //     return (_gameState_, outcome, true);
        // }
        
        // first up resolve any switch monster actions
        // These occur first and order between players doesn't matter
        // if ( _isSwapAction(moveA) ) {
        //     gameState.players[0].activeMonsterIndex = moveA - 4;
        // }
        // if ( _isSwapAction(moveB) ) {
        //     gameState.players[1].activeMonsterIndex = moveB - 4;
        // }

        // // next up resolve attacks. Speed should be used to resolve
        // // if both players are attackign but here A always goes first
        // // for demo purposes
        // if ( _isMoveAction(moveA) ) {
        //     gameState = _makeMove(gameState, moveA,  0, randomSeed);
        // }
        // if ( _isMoveAction(moveB) ) {
        //     gameState = _makeMove(gameState, moveB,  1, randomSeed);
        // }

        // return (abi.encode(gameState), outcome, false);

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
        Outcome.SingleAssetExit memory wagerAssetExit = outcome[0];
        uint256 total = wagerAssetExit.allocations[0].amount + wagerAssetExit.allocations[1].amount;

        outcome[0].allocations[playerIndex].amount = total;
        outcome[0].allocations[~playerIndex].amount = 0;

        return (outcome);
    }

    // A player is unconcious if all their monsters have HP == 0
    function _is_unconcious(TM.PlayerState memory playerState) pure internal returns (bool) {
        bool alive = false;
        for (uint8 i = 0; i < 5; i++) {
            if (playerState.monsters[i].stats.hp > 0) {
                alive = true;
            }
        }
        return (alive);
    }

    function _isSwapAction(uint8 move) pure internal returns (bool) {
        return (move >=4 && move <=8);
    }

    function _isMoveAction(uint8 move) pure internal returns (bool) {
        return (move < 4);
    }

    function _makeMove(TM.GameState memory gameState, uint8 moveIndex, uint8 mover, bytes32 randomSeed) pure internal returns (TM.GameState memory) {
        TM.MonsterCard memory attacker = _getActiveMonster(gameState.players[mover]);
        TM.MonsterCard memory defender = _getActiveMonster(gameState.players[~mover]);

        // bail if attacker is unconcious or no PP available
        if (attacker.stats.hp == 0) {
            return gameState;
        }
        if (attacker.stats.pp[moveIndex] == 0) {
            return gameState;
        }        

        // reduce the PP of the attacker on that move
        attacker.stats.pp[moveIndex] -= 1;

        // apply move
        // This can fail, calling code must catch errors
        gameState = IMove(attacker.moves[moveIndex]).applyMove(gameState, mover, randomSeed);

        return gameState;

    }

    function _getActiveMonster(TM.PlayerState memory playerState) pure internal returns (TM.MonsterCard memory) {
        return (playerState.monsters[playerState.activeMonsterIndex]);
    }

}
