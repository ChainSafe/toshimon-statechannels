// SPDX-License-Identifier: UNLICENSED

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

 import "@openzeppelin/contracts/math/Math.sol";

import { ToshimonState as TM } from '../ToshimonState.sol';
import { IStatusCondition } from '../interfaces/IStatusCondition.sol';

/**
 * Does nothing at all
 */
contract NoOp is IStatusCondition {
	function onStart(TM.GameState memory state, uint8 mover, uint8 monster, bytes32 randomSeed) override external pure returns (TM.GameState memory) {
		return (state);
	}

	function onBeforeMove(TM.GameState memory state, uint8 mover, uint8 monster, bytes32 randomSeed) override external pure returns (TM.GameState memory, bool) {
		return (state, true);
	}

	function onAfterTurn(TM.GameState memory state, uint8 mover, uint8 monster, bytes32 randomSeed) override external pure returns (TM.GameState memory) {
		return (state);
	}
}
