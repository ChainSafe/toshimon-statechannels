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

	// Get the total damage for this matchup
	// TODO: Need to be very careful of overflow and underflow!!
	function attackDamage(uint8 movePower, TM.Stats memory attackerStats, uint8 moveType, TM.Stats memory defenderStats, uint8 defenderMainType, uint8 defenderSecondaryType, uint8 mover, bytes32 randomSeed) public pure returns (uint8) {
		// base damage is ratio of attackers attack to defenders defense multiplied by move power
		uint256 baseDamage = uint(movePower) * attackerStats.attack / defenderStats.defense;
		// apply modifiers
		// - type effectiviness factor
		baseDamage = baseDamage * typeEffectivenessFactor(moveType, defenderMainType, defenderSecondaryType) / 4;
		// - Crit modifier (5.5% chance of double damage)
		baseDamage *= uniformRandom(0, 200, randomSeed, mover, "CRIT") < 11 ? 2 : 1;
		// - random variation (multiplier randomly selected between 0.9 and 1.0) 
		baseDamage = baseDamage * uniformRandom(900, 1000, randomSeed, mover, "RANDOMVARIATION") / 1000;
		return uint8(baseDamage);
		
	}


	// returns quadruple the type effectiveness factor for a match up
	function typeEffectivenessFactor(uint8 attackType, uint8 defenderMainType, uint8 defenderSecondaryType) public pure returns (uint8) {
		return typeChart(attackType, defenderMainType) * typeChart(attackType, defenderSecondaryType);
	}

	// returns double the type chart factor for a type matchup
	// It returns double so this can be represented as an integer (e.g. 0.5 = 1)
	function typeChart(uint8 attackType, uint8 defendingType) public pure returns (uint8) {
		uint8[14][14] memory CHART = 
	      //       Flex   Fire  Wat Lig  Pla  Ice  Bru  Tox  Ear  Eth  Dig  Dark  Hea Crys   
        /*Flex*/   [[ 2,   2,   2,   2,   2,   2,   1,   2,   2,   2,   1,   2,   2,   4],
        /*Fire*/    [ 4,   1,   1,   1,   4,   4,   2,   2,   2,   4,   2,   2,   2,   1],
        /*Water*/   [ 4,   4,   2,   2,   1,   1,   2,   2,   2,   2,   2,   2,   2,   2],
        /*Light*/   [ 2,   0,   1,   4,   4,   2,   4,   2,   1,   2,   1,   4,   1,   4],
        /*Plant*/   [ 2,   1,   4,   2,   1,   2,   2,   4,   4,   2,   2,   2,   2,   1],
        /*Ice*/     [ 4,   1,   4,   2,   2,   2,   1,   4,   2,   2,   2,   2,   2,   2],
        /*Brute*/   [ 1,   2,   2,   2,   2,   4,   1,   2,   2,   0,   2,   2,   4,   4],
        /*Toxic*/   [ 2,   2,   4,   2,   4,   2,   2,   1,   4,   2,   2,   1,   1,   2],
        /*Earth*/   [ 2,   4,   1,   4,   1,   2,   2,   4,   2,   2,   2,   2,   2,   1],
        /*Ether*/   [ 4,   2,   2,   1,   2,   2,   2,   2,   2,   1,   4,   4,   2,   2],
        /*Digital*/ [ 2,   2,   2,   2,   4,   4,   1,   2,   2,   1,   4,   2,   4,   0],
        /*Dark*/    [ 4,   2,   2,   1,   1,   2,   4,   2,   2,   4,   4,   2,   0,   1],
        /*Heart*/   [ 4,   2,   2,   1,   2,   2,   1,   2,   2,   2,   2,   4,   1,   4],
        /*Crystal*/ [ 1,   4,   1,   2,   2,   2,   4,   1,   2,   4,   2,   1,   4,   1]];

		if (attackType == 0 || defendingType == 0) {
	        return 2;
		} else {
			return CHART[attackType - 1][defendingType - 1];
	    }
    }

}

