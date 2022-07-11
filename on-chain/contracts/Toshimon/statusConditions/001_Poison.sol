// SPDX-License-Identifier: UNLICENSED

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

 import "@openzeppelin/contracts/math/Math.sol";

import { ToshimonState as TM } from '../ToshimonState.sol';
import { ToshimonUtils as Utils } from '../ToshimonUtils.sol';
import { IStatusCondition } from '../interfaces/IStatusCondition.sol';

/**
 * Inflicts 1/16 max HP damage after each turn for between 2-6 turns
 */
contract Poison is IStatusCondition {
	function onStart(TM.GameState memory state, uint8 mover, uint8 monster, bytes32 randomSeed) override external pure returns (TM.GameState memory) {
		// set the counter to a random value between 2 and 6
		state.players[mover].monsters[monster].statusConditionCounter = uint8(Utils.uniformRandom(2, 6, randomSeed, mover, "POISON"));
		return (state);
	}

	function onBeforeMove(TM.GameState memory state, uint8 mover, uint8 monster, bytes32 randomSeed) override external pure returns (TM.GameState memory, bool) {
		// poison does not limit ability to attack so return true
		return (state, true);
	}

	function onAfterTurn(TM.GameState memory state, uint8 mover, uint8 monster, bytes32 randomSeed) override external pure returns (TM.GameState memory) {
		// cure if counter has elapsed else decrement counter
		if (state.players[mover].monsters[monster].statusConditionCounter == 0) {
			state.players[mover].monsters[monster].statusCondition = address(0);
		} else {
			state.players[mover].monsters[monster].statusConditionCounter -= 1;
		}

		// do 1/16th of max HP in damage
		state.players[mover].monsters[monster] = Utils.applyDamage(state.players[mover].monsters[monster], state.players[mover].monsters[monster].baseStats.hp / 16);

		return (state);
	}
}
