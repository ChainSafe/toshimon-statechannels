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

import { getRandomNonce, encodeAppData, initialAppData, RRAppData } from "./test-helpers";

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

describe("RollingRandom - validTransition", function () {

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

	it("Expects nothing for turn 0", async function () {

		const prevState = makeState(initialAppData(), 0)
		const newState: State = {...prevState, turnNum: 1};

		expect(
			await validTransition(prevState, newState, rrApp)
		).to.equal(true);
	});


	// TODO: finish these tests

	it("Fails on later turns because a valid commit/reveal is expected", async function () {

		const prevState = makeState(initialAppData(), 3)
		const newState: State = {...prevState, turnNum: 4};

		expect(
			await validTransition(prevState, newState, rrApp)
		).to.equal(true);
	});

	it("Succeeds if the prev commit is set correctly", async function () {

		const prevAppState = initialAppData()
		const newAppState = {...prevAppState, prev_commit: prevAppState.commit }

		expect(
			await validTransition(makeState(prevAppState, 3), makeState(newAppState, 4), rrApp)
		).to.equal(true);
	});
});
