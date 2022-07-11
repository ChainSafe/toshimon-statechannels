// SPDX-License-Identifier: UNLICENSED

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

import "@openzeppelin/contracts/math/Math.sol";

import { ToshimonState as TM } from '../ToshimonState.sol';
import { ToshimonUtils as Utils } from '../ToshimonUtils.sol';
import { IMove } from '../interfaces/IMove.sol';
import { PoisonLib } from '../statusConditions/001_Poison.sol';

/**
 * Drains HP from opponent each turn (inflicts the poison status condition in PoC)
 */
contract MoldSpore is IMove {
	function applyMove(TM.GameState memory state, uint8 mover, uint8 repeatsRemaining, bytes32 randomSeed) override external pure returns (TM.GameState memory) {
		uint8 receiver = Utils.not(mover);
		return PoisonLib.onStart(
			state,
			receiver,
			state.players[receiver].activeMonsterIndex,
			randomSeed
		);
	}

	function speed(TM.Stats memory attackerStats, bytes32 randomSeed) override external pure returns (uint8) {
		return attackerStats.speed;
	}
}
