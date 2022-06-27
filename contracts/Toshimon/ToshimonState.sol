// SPDX-License-Identifier: UNLICENSED

/**
 * Created on 2022-06-06
 * @summary: Types for representing the toshimon game state
 * @author: Willem Olding (ChainSafe)
 */

pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

library ToshimonState {

	struct GameState {
		PlayerState[2] players;
	}

	struct PlayerState {
		MonsterCard[5] monsters;
		ItemCard[5] items;
		uint8 activeMonsterIndex;
	}

	struct MonsterCard {
		uint256 id;
		uint8 mainType;
		uint8 secondaryType;
		Stats baseStats;
		Stats stats;
		address[4] moves;
		address statusCondition;
		uint8 stausConditionCounter;
		address specialStatusCondition;
		uint8 specialStatusConditionCounter;
		uint8 activeMoveIndex;
		uint8 activeMoveCounter;
	}

	struct Stats {
		uint8 hp;
		uint8 attack;
		uint8 defense;
		uint8 spAttack;
		uint8 spDefense;
		uint8 speed;
		uint8[4] pp;
	}

	struct ItemCard {
		address defintion;
		bool used;
	}

}
