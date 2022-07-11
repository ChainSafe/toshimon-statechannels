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

	uint8 constant damage = 20;

	function applyMove(TM.GameState memory state, uint8 mover, uint8 repeatsRemaining, bytes32 randomSeed) override external pure returns (TM.GameState memory) {
		return state;
	}

	function speed(TM.Stats memory attackerStats, bytes32 randomSeed) override external pure returns (uint8) {
		return attackerStats.speed;
	}
}
