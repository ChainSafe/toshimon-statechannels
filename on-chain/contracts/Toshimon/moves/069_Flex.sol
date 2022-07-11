// SPDX-License-Identifier: UNLICENSED

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

 import "@openzeppelin/contracts/math/Math.sol";

import { ToshimonState as TM } from '../ToshimonState.sol';
import { ToshimonUtils as Utils } from '../ToshimonUtils.sol';
import { IMove } from '../interfaces/IMove.sol';

/**
 * Raises users defense by a fixed amount
 */
contract Flex is IMove {

	uint8 constant defenseGain = 10;

	function applyMove(TM.GameState memory state, uint8 mover, uint8 repeatsRemaining, bytes32 randomSeed) override external pure returns (TM.GameState memory) {
		uint8 defense = state.players[mover].monsters[state.players[mover].activeMonsterIndex].stats.defense;
		state.players[mover].monsters[state.players[mover].activeMonsterIndex].stats.defense += Utils.limitAdd(defense, defenseGain, 100);
		return state;
	}

	function speed(TM.Stats memory attackerStats, bytes32 randomSeed) override external pure returns (uint8) {
		return attackerStats.speed;
	}
}
