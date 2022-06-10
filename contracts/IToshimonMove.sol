// SPDX-License-Identifier: MIT

/**
 * Created on 2022-06-06
 * @summary: An interface for extending the Toshimon battle game with new moves
 * @author: Willem Olding (ChainSafe)
 */
pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

import { GameState } from './ExampleCommitRevealApp.sol';


interface IToshimonMove {

	function power() external pure returns (uint8);
	function accuracy() external pure returns (uint8);
	function toshiType() external pure returns (uint8);

	function applyMove(GameState memory state, uint8 mover, bytes32 seed) external pure returns (GameState memory);
}
