// SPDX-License-Identifier: UNLICENSED

/**
 * Created on 2022-06-06
 * @summary: A singleton library for utility toshimon functionality
 * Helpful when implementing moves and status conditions
 * @author: Willem Olding (ChainSafe)
 */

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

import { ToshimonState as TM } from './ToshimonState.sol';

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

}
