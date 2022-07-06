# Toshimon State Machine

This is a prototype of what a pure function based implementation of the Toshimon battle game might look like.

In order to play a game in a state channel is must be distilled into discrete states and a state evolution function. This repo implements both of these things in C#. It is designed to be integrated with the Unity client but eventually replaced with a Solidity implemlementation.

## Prerequisites

Libs currently build and tested with .NET Core 6.0 and C# 10.0. Mileage may vary with other environments.

Installation instructions for all platforms are available [here](https://docs.microsoft.com/en-us/dotnet/core/install/).

## Usage

The [stateTransition](./AdvanceState/stateTransition.cs) file exports an interface `IStateTransition`. An implementor of this provides a function able to consume Toshimon state objects, a move from each player and a random seed and produce a new state according to the rules of the game.

The class library also exports the data types for the [Toshimon game state itself](./AdvanceState/DataTypes.cs). This is composed of `PlayerState` which each contain a number of `MonsterCard`s and `ItemCard`s. These types are designed to be consumed by the game client and used to display the state to the player.

## Testing

An very important part of this project is the test suite. It is indended that this test suite be kept after the migration to Solidity and so the tests contained in this repo should one day be the definitive specification for the Toshimon game rules.

Tests are contained in their own project `AdvanceStateTest`. It uses [xUnit](https://xunit.net/#documentation) as the testing framework. The test project also contains a number of extensions and helpers to make the written tests as clear as possible. See the [test README](./AdvanceStateTest/README.md).

Running the tests:

```shell
cd AdvanceStateTest
dotnet test

```

## License

- This is a private prototype and not currently under a public license

## Contributors

- [Willem Olding (ChainSafe)](github.com/willemolding/)

