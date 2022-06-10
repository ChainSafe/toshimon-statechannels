// SPDX-License-Identifier: MIT

/**
 * Created on 2022-06-06
 * @summary: An interface for extending the Toshimon battle game with new moves
 * @author: Willem Olding (ChainSafe)
 */
pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

import './IToshimonMove.sol';
import { GameState } from './ExampleCommitRevealApp.sol';


contract ExampleMove is IToshimonMove {

	function power() override external pure returns (uint8) {
		return (70);
	}
	function accuracy() override external pure returns (uint8) {
		return (80);
	}
	function toshiType() override external pure returns (uint8) {
		return (0);
	}

	function applyMove(GameState memory state, uint8 mover, bytes32 seed) override external pure returns (GameState memory) {
		return (state);
	}
}
