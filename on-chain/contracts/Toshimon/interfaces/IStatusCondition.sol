// SPDX-License-Identifier: UNLICENSED

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

import { ToshimonState as TM } from '../ToshimonState.sol';

interface IStatusCondition {
	function onBeforeMove(TM.GameState memory state, uint8 receiver, uint8 monster, bytes32 randomSeed) external pure returns (TM.GameState memory, bool);

	function onAfterTurn(TM.GameState memory state, uint8 receiver, uint8 monster, bytes32 randomSeed) external pure returns (TM.GameState memory);
}
