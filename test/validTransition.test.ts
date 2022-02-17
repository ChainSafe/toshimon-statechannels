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
let rrApp: Contract;
let channel: Channel;

// make some state with typical values for all fields
function makeState(data: RRAppData, turnNum: number): State {
	return {
		channel,
		outcome: [],
		turnNum,
		isFinal: false,
		challengeDuration: 0x0,
		appDefinition: rrApp.address,
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
	const RollingRandom = await ethers.getContractFactory("RollingRandom");
	rrApp = await RollingRandom.deploy();
	await rrApp.deployed();
});

describe("RollingRandom - validTransition", function () {

	it("Expects nothing for turn 0", async function () {

		const prevState = makeState(initialAppData(makeCommit(getRandomValue('seed'))), 0)
		const newState: State = {...prevState, turnNum: 1};

		expect(
			await validTransition(prevState, newState, rrApp)
		).to.equal(true);
	});


	// TODO: finish these tests

	it("Fails on later turns if the prev_commit isnt idental to the commit field in prevState", async function () {

		const prevState = makeState(initialAppData(makeCommit(getRandomValue('seed'))), 3)
		const newState: State = {...prevState, turnNum: 4};

		await expect(
			validTransition(prevState, newState, rrApp)
		).to.be.revertedWith('Commit from prior state was not moved to prev_commit in new state');
	});

	it("If the prev commit is set correctly, fails because counter not correctly incremented", async function () {
		let r0 = getRandomValue("r0");
		let c0 = makeCommit(r0);

		const prevAppState = { ...initialAppData(makeCommit(getRandomValue('seed'))), prev_commit: c0 }
		const newAppState = {...prevAppState, prev_commit: prevAppState.commit, reveal: r0 }

		await expect(
			validTransition(makeState(prevAppState, 3), makeState(newAppState, 4), rrApp)
		).to.be.revertedWith('a_counter was not incremented when it should have been');
	});

	it("Succeeds if the correct counter is incremented", async function () {
		let r0 = getRandomValue("r0");
		let c0 = makeCommit(r0);

		const prevAppState = { ...initialAppData(makeCommit(getRandomValue('seed'))), prev_commit: c0 }
		const newAppState = {...prevAppState, prev_commit: prevAppState.commit, reveal: r0, a_counter: 1 } // increment the a_counter

		expect(
			await validTransition(makeState(prevAppState, 3), makeState(newAppState, 4), rrApp)
		).to.equal(true);
	});
});
