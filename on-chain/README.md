# Toshimon State Channels Battle System

WORK IN PROGRESS

This repo contains Solidity contracts and testing code to implement the Toshimon battle game using the ForceMove framework for state channels. 

The Solidity project enviroment uses [hardhat](https://hardhat.org/) for building, deployment and testing.

## Dependencies

Development was done using Node v16.14.2. It may work with other versions but this is untested. It is reccomended to use [NVM](https://github.com/nvm-sh/nvm) to manage node versions.

Package management uses `yarn` and not npm. Tested with v1.22.18. 

## Usage

To build and test the contracts:

Install the depedencies and run tests test
```shell
yarn install
yarn test
```

## Building Contracts and ABIs

```shell
yarn hardhat compile
```

## Start a local testnet with Toshimon deployment

```shell
yarn hardhat node
```

This is required to be running to run the C# tests.

## Contributors

- [Willem Olding (ChainSafe)](github.com/willemolding/)
