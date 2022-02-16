import { ethers } from "hardhat";
import {
	Bytes32,
	Uint256,
} from '@statechannels/nitro-protocol';


export interface RRAppData {
	commit: Bytes32,
	prev_commit: Bytes32,
	reveal: Uint256,
	random_seed: Bytes32,
};

// Convert some RR App data to its byte array form to be send in a tx
export function encodeAppData(data: RRAppData): string {
  return ethers.utils.defaultAbiCoder.encode(
    ['tuple(bytes32 commit, bytes32 prev_commit, uint256 reveal, bytes32 random_seed)'],
    [data]
  );
}

// Some random intitial app data
// Probably need to think this through a bit more at some point
export function initialAppData(): RRAppData {
	return {
		commit: ethers.utils.keccak256("0x"),
		prev_commit: ethers.utils.keccak256("0x"),
		reveal: "0x00",
		random_seed: ethers.utils.keccak256("0x"),
	}
}

export function getRandomNonce(seed: string): number {
  return Number.parseInt(ethers.utils.id(seed).slice(2, 11), 16);
}
