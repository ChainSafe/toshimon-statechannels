// SPDX-License-Identifier: UNLICENSED

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

import { ToshimonState as TM } from '../ToshimonState.sol';

interface IMove {
	function applyMove(TM.GameState memory state, uint8 mover, uint randomSeed) external pure returns (TM.GameState memory);
}
