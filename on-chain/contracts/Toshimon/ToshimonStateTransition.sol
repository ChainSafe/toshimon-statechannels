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
 import './interfaces/IItem.sol';
 import './interfaces/IStatusCondition.sol';

 contract ToshimonStateTransition is CommitRevealApp {

    function advanceState(
        bytes memory _gameState_,
        Outcome.SingleAssetExit[] memory outcome,
        uint8 moveA,
        uint8 moveB,
        bytes32 randomSeed
        ) public pure override returns (bytes memory, Outcome.SingleAssetExit[] memory, bool) {
        TM.GameState memory gameState = abi.decode(_gameState_, (TM.GameState));
        (TM.GameState memory newState,  Outcome.SingleAssetExit[] memory newOutcome, bool isFinal) = advanceStateTyped(gameState, outcome, moveA, moveB, randomSeed);
        return (abi.encode(newState), newOutcome, isFinal);
    }

    function advanceStateTyped(
        TM.GameState memory gameState,
        Outcome.SingleAssetExit[] memory outcome,
        uint8 moveA,
        uint8 moveB,
        bytes32 randomSeed
        ) public pure returns (TM.GameState memory, Outcome.SingleAssetExit[] memory, bool) {

        // if either player is unconcious then no more moves can be made
        // and the game is over. No further state updates possible.
        if (_is_unconcious(gameState.players[A]) || _is_unconcious(gameState.players[B])) {
            return (gameState, outcome, true);
        }
        
        // if either player has an active multi-turn move then the move passed to this function
        // should be disregarded and the multi-turn move applied instead
        moveA = _swapMoveIfMultiTurnMoveActive(gameState.players[A], moveA);
        moveB = _swapMoveIfMultiTurnMoveActive(gameState.players[B], moveB);

        // first up resolve any switch monster actions
        // These occur first and order between players doesn't matter
        if ( _isSwapAction(moveA) ) {
            gameState.players[A].activeMonsterIndex = moveA - 4;
        }
        if ( _isSwapAction(moveB) ) {
            gameState.players[A].activeMonsterIndex = moveB - 4;
        }

        // next up resolve items. These can only be applied to the active monster
        // and they are resolved before attacks so again order doesn't matter here
        if ( _isItemAction(moveA) ) {
            gameState = _useItem(gameState, moveA - 9, A, randomSeed);
        }
        if ( _isItemAction(moveB) ) {
            gameState = _useItem(gameState, moveA - 9, B, randomSeed);
        }        

        // next up resolve attacks. Speed should be used to resolve
        // if both players are attackign but here A always goes first
        // for demo purposes
        if ( _isMoveAction(moveA) ) {
            gameState = _makeMove(gameState, moveA,  A, randomSeed);
        }
        if ( _isMoveAction(moveB) ) {
            gameState = _makeMove(gameState, moveB,  B, randomSeed);
        }

        // apply the status condition `onAfterTurn` callback on all monsters, not just the
        // active ones
        for (uint8 i = 0; i < gameState.players[A].monsters.length; i++) {
            TM.MonsterCard memory monster = gameState.players[A].monsters[i];
            if (monster.statusCondition != address(0)) {
                gameState = IStatusCondition(monster.statusCondition).onAfterTurn(gameState, A, i, randomSeed);
            }
        }
        for (uint8 i = 0; i < gameState.players[B].monsters.length; i++) {
            TM.MonsterCard memory monster = gameState.players[B].monsters[i];
            if (monster.statusCondition != address(0)) {
                gameState = IStatusCondition(monster.statusCondition).onAfterTurn(gameState, B, i, randomSeed);
            }
        }

        return (gameState, outcome, true);

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
        bool unconcious = true;
        for (uint8 i = 0; i < playerState.monsters.length; i++) {
            if (playerState.monsters[i].stats.hp > 0) {
                unconcious = false;
            }
        }
        return (unconcious);
    }

    function _isItemAction(uint8 move) pure internal returns (bool) {
        return (move >=9 && move <=13);
    }

    function _isSwapAction(uint8 move) pure internal returns (bool) {
        return (move >=4 && move <=8);
    }

    function _isMoveAction(uint8 move) pure internal returns (bool) {
        return (move < 4);
    }

    function _swapMoveIfMultiTurnMoveActive(TM.PlayerState memory playerState, uint8 move) pure internal returns (uint8) {
        TM.MonsterCard memory activeMonster = _getActiveMonster(playerState);
        if (activeMonster.activeMoveCounter > 0) {
            return (activeMonster.activeMoveIndex);
        } else {
            return (move);
        }
    }

    function _makeMove(TM.GameState memory gameState, uint8 moveIndex, uint8 mover, bytes32 randomSeed) pure internal returns (TM.GameState memory) {
        TM.MonsterCard memory attacker = _getActiveMonster(gameState.players[mover]);

        // bail if attacker is unconcious or no PP available
        if (attacker.stats.hp == 0) {
            return gameState;
        }
        if (attacker.stats.pp[moveIndex] == 0) {
            return gameState;
        }        

        // reduce the PP of the attacker on that move
        attacker.stats.pp[moveIndex] -= 1;

        // check for active status conditions. Apply the effects and see if they allow the attack
        // TODO
        
        // Try and make the move
        // There is a possibility this is a malformed state and the move contract does not exist
        // This must not error!! If it errors then the state channel is locked.
        // Just silently fail to make the move and return the unchanged state
        // 
        return IMove(attacker.moves[moveIndex]).applyMove(gameState, mover, attacker.activeMoveIndex, randomSeed);
        // try IMove(attacker.moves[moveIndex]).applyMove(gameState, mover, attacker.activeMoveIndex, randomSeed) returns (TM.GameState memory newState) {
        //     return newState;
        // } catch {
        //     return gameState;
        // }
    }

    function _useItem(TM.GameState memory gameState, uint8 itemIndex, uint8 mover, bytes32 randomSeed) pure internal returns (TM.GameState memory) {
        TM.PlayerState memory user = gameState.players[mover];

        // mark the item as used
        user.items[itemIndex].used = true;

        // apply the item
        gameState = IItem(user.items[itemIndex].definition).applyItem(gameState, mover, user.activeMonsterIndex, randomSeed);

        return gameState;

    }

    function _getActiveMonster(TM.PlayerState memory playerState) pure internal returns (TM.MonsterCard memory) {
        return (playerState.monsters[playerState.activeMonsterIndex]);
    }

}
