import { ethers } from "hardhat";
import {
	Bytes32,
	Uint256,
} from '@statechannels/nitro-protocol';

type Uint8 = number;

export interface RRAppData {
	commit: Bytes32,
	prev_commit: Bytes32,
	reveal: Uint256,
	a_counter: Uint8,
	b_counter: Uint8,
};

// Convert some RR App data to its byte array form to be send in a tx
export function encodeAppData(data: RRAppData): string {
  return ethers.utils.defaultAbiCoder.encode(
    ['tuple(bytes32 commit, bytes32 prev_commit, uint256 reveal, uint8 a_counter, uint8 b_counter)'],
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
		prev_commit: ethers.utils.keccak256("0x"),
		reveal: "0x00",
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
