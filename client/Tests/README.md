# Toshimon Advance State Tests

To limit the number of languages used in the project, the tests for the Toshimon state machine implementation are written in C#. This should be familiar to the game developers who should be responsible for writing the tests.

These tests only test the game logic itself as implemented by the `advanceState` function in the `ToshimonStateTransition` contract. The other state contract logic is tested using javascript tests from within Hardhat.

## Running the tests

Firstly ensure that a local ethereum node is running with a Toshimon contract deployment. This can be started by running 

```shell
yarn hardhat node
```
from the `/on-chain` directory in this repo. It can also be run against a real network deployment since all function calls are view only.

---

The tests need to be configured to use the correct RPC url. This can be set using the environment variable `ETH_RPC`. This is most likely `http://127.0.0.1:8545` for a local node but could also be an Infura RPC endpoint.

The tests also need to know the details of the deployment on that network (e.g. the addresses of all the contracts). This description is produced automatically by hardhat-deploy and usually lives in the directory `/on-chain/deployments/localhost`. This will be automatically created when the hardhat node is started with the command above.

Pass the location of the deployment directory in the `DEPLOYMENT` environment variable.

---

Finally, the tests can be run using

```shell
ETH_RPC=http://127.0.0.1:8545 DEPLOYMENT=<full-path>/on-chain/deployments/locahost/ dotnet test
```

## License

- This is a private prototype and not currently under a public license

## Contributors

- [Willem Olding (ChainSafe)](github.com/willemolding/)
