// SPDX-License-Identifier: UNLICENSED

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

import { ToshimonState as TM } from '../ToshimonState.sol';
import { IMove } from '../interfaces/IMove.sol';

contract Tackle is IMove {
	function applyMove(TM.GameState memory state, uint8 mover, bytes32 randomSeed) override external pure returns (TM.GameState memory) {
		return state;
	}
}
