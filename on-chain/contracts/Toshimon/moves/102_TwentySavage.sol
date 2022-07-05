// SPDX-License-Identifier: UNLICENSED

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

 import "@openzeppelin/contracts/math/Math.sol";

import { ToshimonState as TM } from '../ToshimonState.sol';
import { ToshimonUtils as Utils } from '../ToshimonUtils.sol';
import { IMove } from '../interfaces/IMove.sol';

/**
 * Always does exactly 20 HP of damage to the opponent
 */
contract TwentySavage is IMove {

	uint8 constant damage = 20;

	function applyMove(TM.GameState memory state, uint8 mover, uint8 repeatsRemaining, bytes32 randomSeed) override external pure returns (TM.GameState memory) {
		Utils.applyDamage(Utils.getActiveMonster(state.players[~mover]), damage);
		return state;
	}

	function speed(TM.Stats memory attackerStats, bytes32 randomSeed) override external pure returns (uint8) {
		return attackerStats.speed;
	}
}
