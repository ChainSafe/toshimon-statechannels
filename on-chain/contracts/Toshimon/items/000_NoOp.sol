// SPDX-License-Identifier: UNLICENSED

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

 import "@openzeppelin/contracts/math/Math.sol";

import { ToshimonState as TM } from '../ToshimonState.sol';
import { IItem } from '../interfaces/IItem.sol';

/**
 * Does nothing at all
 */
contract NoOp is IItem {
	function applyItem(TM.GameState memory state, uint8 mover, uint8 monster, bytes32 randomSeed) external override pure returns (TM.GameState memory) {
		return state;
	}
}
