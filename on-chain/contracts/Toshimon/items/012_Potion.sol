// SPDX-License-Identifier: UNLICENSED

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

 import "@openzeppelin/contracts/math/Math.sol";

import { ToshimonState as TM } from '../ToshimonState.sol';
import { IItem } from '../interfaces/IItem.sol';

contract Potion is IItem {
	uint8 constant heals = 25;

	function applyItem(TM.GameState memory state, uint8 mover, uint8 monster, bytes32 randomSeed) external override pure returns (TM.GameState memory) {
		// add 25 HP to the receiver up to their max HP
		uint8 currentHp = state.players[mover].monsters[monster].stats.hp;
		uint8 maxHp = state.players[mover].monsters[monster].baseStats.hp;
		state.players[mover].monsters[monster].stats.hp = uint8(Math.min(uint256(currentHp + heals), uint256(maxHp)));
		return state;
	}
}
