// SPDX-License-Identifier: UNLICENSED

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

import { ToshimonState as TM } from '../ToshimonState.sol';

interface IStatusCondition {
	function onStart(TM.GameState memory state, uint8 mover, uint8 monster, uint randomSeed) external pure returns (TM.GameState memory);

	function onBeforeMove(TM.GameState memory state, uint8 mover, uint8 monster, uint randomSeed) external pure returns (TM.GameState memory, bool);

	function onAfterTurn(TM.GameState memory state, uint8 mover, uint8 monster, uint randomSeed) external pure returns (TM.GameState memory);
}
