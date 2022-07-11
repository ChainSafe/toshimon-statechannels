// SPDX-License-Identifier: UNLICENSED

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

 import "@openzeppelin/contracts/math/Math.sol";

import { ToshimonState as TM } from '../ToshimonState.sol';
import { ToshimonUtils as Utils } from '../ToshimonUtils.sol';
import { IMove } from '../interfaces/IMove.sol';

/**
 * A leaf-type attack
 */
contract LeafBlower is IMove {

	uint8 constant power = 45;
	uint8 constant moveType = 5;

	function applyMove(TM.GameState memory state, uint8 mover, uint8 repeatsRemaining, bytes32 randomSeed) override external pure returns (TM.GameState memory) {
		uint8 receiver = Utils.not(mover);
		TM.MonsterCard memory attacker = Utils.getActiveMonster(state.players[mover]);
		TM.MonsterCard memory defender = Utils.getActiveMonster(state.players[receiver]);

		uint8 damage = Utils.attackDamage(power, attacker.stats, moveType, defender.stats, defender.mainType, defender.secondaryType, mover, randomSeed);
		state.players[receiver].monsters[state.players[receiver].activeMonsterIndex] = Utils.applyDamage(Utils.getActiveMonster(state.players[receiver]), damage);
		return state;	
	}

	function speed(TM.Stats memory attackerStats, bytes32 randomSeed) override external pure returns (uint8) {
		return attackerStats.speed;
	}
}
