import { expect } from "chai";
import { ethers } from "hardhat";
import { Contract, Wallet, utils } from "ethers";
import {
	Allocation,
	encodeOutcome,
	Channel,
	ContractArtifacts,
	getChannelId,
	AssetOutcomeShortHand,
	replaceAddressesAndBigNumberify,
	randomExternalDestination,
	VariablePart,
	State,
	validTransition,
	getVariablePart,
} from '@statechannels/nitro-protocol';

import { getRandomNonce, encodeAppData, initialAppData, RRAppData, getRandomValue, makeCommit } from "./test-helpers";

// global test vars
let app: Contract;
let channel: Channel;

// make some state with typical values for all fields
function makeState(data: RRAppData, turnNum: number): State {
	return {
		channel,
		outcome: [],
		turnNum,
		isFinal: false,
		challengeDuration: 0x0,
		appDefinition: app.address,
		appData: encodeAppData(data),
	};
}

before(async function () {
	// setup a channel
	channel = {
		participants: [Wallet.createRandom().address, Wallet.createRandom().address],
		chainId: process.env.CHAIN_NETWORK_ID as string,
		channelNonce: getRandomNonce('rollingRandomApp'),
	};
	// deploy our app contract
	const contract = await ethers.getContractFactory("ExampleCommitRevealApp");
	app = await contract.deploy();
	await app.deployed();
});

describe("RollingRandom - validTransition", function () {

	it("Expects nothing for turn 0", async function () {

		const prevState = makeState(initialAppData(makeCommit(getRandomValue('seed'))), 0)
		const newState: State = {...prevState, turnNum: 1};

		expect(
			await validTransition(prevState, newState, app)
		).to.equal(true);
	});
});
