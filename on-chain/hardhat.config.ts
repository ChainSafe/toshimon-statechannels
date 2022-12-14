import { task } from "hardhat/config";
import "@nomiclabs/hardhat-waffle";
import "hardhat-gas-reporter";
import 'hardhat-deploy';

require('@symblox/hardhat-abi-gen');

require("dotenv").config();

// This is a sample Hardhat task. To learn how to create your own go to
// https://hardhat.org/guides/create-task.html
task("accounts", "Prints the list of accounts", async (args, hre) => {
  const accounts = await hre.ethers.getSigners();

  for (const account of accounts) {
    console.log(account.address);
  }
});

// You need to export an object to set up your config
// Go to https://hardhat.org/config/ to learn more

export default {
  solidity: {
    version: "0.7.6",
    settings: {
      optimizer: {
        enabled: true,
        runs: 2000,
        details: {
          yul: true,
          yulDetails: {
            stackAllocation: true,
            optimizerSteps: "dhfoDgvulfnTUtnIf"
          }
        }
      },
    },
  },
  gasReporter: {
    currency: 'USD',
    // token: 'MATIC',
    // gasPriceApi: 'https://api.polygonscan.com/api?module=proxy&action=eth_gasPrice',
    coinmarketcap: "ebaf9c41-43a2-4afa-9fe3-16e7d9484e5b",
  },
  abiExporter: {
    path: './abi',
    clear: true,
    flat: true,
    only: [':Adjudicator$', ':ToshimonState$', ':ToshimonStateTransition$', ':CommitRevealApp$', ':TESTNitroUtils$', ':TESTEncoding$'],
    spacing: 2
  },
  namedAccounts: {
    deployer: 0
  }
};
