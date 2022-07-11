// SPDX-License-Identifier: UNLICENSED

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

 import "@openzeppelin/contracts/math/Math.sol";

import { ToshimonState as TM } from '../ToshimonState.sol';
import { IStatusCondition } from '../interfaces/IStatusCondition.sol';

/**
 * Does nothing at all
 */
contract NoOpCondition is IStatusCondition {
	function onBeforeMove(TM.GameState memory state, uint8 receiver, uint8 monster, bytes32 randomSeed) override external pure returns (TM.GameState memory, bool) {
		return (state, true);
	}

	function onAfterTurn(TM.GameState memory state, uint8 receiver, uint8 monster, bytes32 randomSeed) override external pure returns (TM.GameState memory) {
		return (state);
	}
}

library NoOpConditionLib {
	function onStart(TM.GameState memory state, uint8 receiver, uint8 monster, bytes32 randomSeed) public pure returns (TM.GameState memory) {
		return (state);
	}	
}
