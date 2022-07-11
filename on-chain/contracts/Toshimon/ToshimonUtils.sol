// SPDX-License-Identifier: UNLICENSED

/**
 * Created on 2022-06-06
 * @summary: A singleton library for utility toshimon functionality
 * Helpful when implementing moves and status conditions
 * @author: Willem Olding (ChainSafe)
 */

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

import "@openzeppelin/contracts/math/Math.sol";

import { ToshimonState as TM } from './ToshimonState.sol';

// If a function in the library is marked as `public` it will only be deployed once
// and all other contracts that use the function will make an external contract call
// to call the library function
//
// If a function is marked `internal` the function code will be copied into any contract that uses it.
// 
// Use internal for very small functions and public for large ones to balance calling gas cost against contract deployment size 

library ToshimonUtils {
	// Return the active monster on a player
	function getActiveMonster(TM.PlayerState memory playerState) public pure returns (TM.MonsterCard memory) {
        return (playerState.monsters[playerState.activeMonsterIndex]);
    }

    // Return a monster card with given damage applied. Prevents underflow if damage would take Hp below zero
	function applyDamage(TM.MonsterCard memory receiver, uint8 damage) public pure returns (TM.MonsterCard memory) {
		if (damage >= receiver.stats.hp) {
			receiver.stats.hp = 0;
		} else {
			receiver.stats.hp -= damage;
		}
		return (receiver);
	}

	// returns a 1 if x is 0, and 0 if x is 1
	function not(uint8 x) internal pure returns (uint8) {
		return x ^ 0xff & 0x01;
	}

	// Adds to values but ensures this will not exceed the limit parameter
	function limitAdd(uint8 a, uint8 b, uint8 limit) internal pure returns (uint8) {
		// TODO ensure no overflow occurs..
		return uint8(Math.min(a + b, limit));
	}
	
	/**
	 * Select an integer uniformly between min (inclusive) and max (exclusive)
	 * It is important to use a different nonce and mover index if reusing the same randomSeed or else 
	 * outcomes will be identical for each player or at different calls within the function
	 */
	function uniformRandom(uint256 min, uint256 max, bytes32 randomSeed, uint8 mover, string memory nonce) public pure returns (uint256) {
		uint256 val = uint256(keccak256(abi.encodePacked(randomSeed, mover, nonce)));
		return min + val % (max - min);
	}

}
