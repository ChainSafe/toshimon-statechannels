import { ethers } from "hardhat";
import {
	Bytes32,
	Uint256,
} from '@statechannels/nitro-protocol';

type Uint8 = number;

export interface RRAppData {
	commit: Bytes32,
	seed: Bytes32,
	reveal_salt_seed: Bytes32,
	reveal_move: Uint8,

	a_counter: Uint8,
	b_counter: Uint8,
};

// Convert some RR App data to its byte array form to be send in a tx
export function encodeAppData(data: RRAppData): string {
  return ethers.utils.defaultAbiCoder.encode(
    ['tuple(bytes32 commit, bytes32 seed, bytes32 reveal_salt_seed, uint8 reveal_move, uint8 a_counter, uint8 b_counter)'],
    [data]
  );
}

export function makeCommit(value: Uint256): Bytes32 {
		return ethers.utils.keccak256(value);
}

// Some random intitial app data
// Probably need to think this through a bit more at some point
export function initialAppData(commit: Bytes32): RRAppData {
	return {
		commit,
		seed: ethers.utils.keccak256("0x"),
		reveal_salt_seed: ethers.utils.keccak256("0x"),
		reveal_move: 0,
		a_counter: 0,
		b_counter: 0,
	}
}

export function getRandomNonce(seed: string): number {
  return Number.parseInt(ethers.utils.id(seed).slice(2, 11), 16);
}

export function getRandomValue(seed: string): Uint256 {
	return ethers.utils.id(seed);
}
