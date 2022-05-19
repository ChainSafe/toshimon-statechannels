# Rolling commit-reveal randomness in 2 player state channels

- Idea from here https://statechannels.discourse.group/t/rolling-commit-reveal-an-idea-for-randomness-in-force-move-games/138

# Commit-Reveal Randomness Scheme

Moves in the card game can have an outcome based on randomness. For example most attacks are not guaranteed to hit and might miss with some percentage chance. There is no source of randomness that can be queried in a state channel and so the players must use a two party protocol to produce random seed which can be used to calculate the outcome of certain moves.

This shared randomness protocol must have the following properties:

1. Neither participant can influence the outcome in any predictable way.
2. At no point does one participant have an information advantage over the other that they can act on.

The first point is intuitive. If either participant could influence the random outcome they would likely do so in their favor, breaking the fairness property. The second point is more nuanced. Take for example a protocol where each participant commits to a random seed which is later revealed along with the move they intend to make. When the second player reveals their seed they already know what the shared randomness will be as the other player revealed their seed in the prior move. They have the advantage that they can select their move with knowledge of the outcome in each case. This breaks the second point.

## Proposed Scheme

A simple scheme that satisfies both of the required properties is a commit-reveal scheme where both parties commit to both a random seed and the hash of the move they want to make in the round. The random seed doubles as a salt for the move, without which it would be trivial to perform a hash reversal. After both players have published their commitment either can reveal in any order and the combined random seeds can be used to compute an outcome. The alternate turn taking requirement of a force-move channel determines which player must reveal first.

1. A signs update with commitment to random seed and move.
2. B signs a similar update with their random seed and move.
3. A reveals their seed and move.
4. B reveals their seed and move. This update also contains the game state update.

There is an additional consideration for state channels that despite committing to a move in step 2, B has an additional move available to them at step 4 which is to not sign a new update and exit the game. Recall that this is always available to any player at any time. This requires consideration of the Outcomes at each stage. TODO.

## Generalized Implementation

In the above protocol the only game specific logic is that which relates to the available moves and the game state update. The randomness protocol can be abstracted to its own abstract contract (`CommitRevealApp.sol`) and developers need only implement their own contract which derives this and adds an implementation of `advanceState`, a function which takes an old state as bytes and produces a new state given the player moves and the shared randomness.

```solidity
    function advanceState(
        bytes memory gameState,
        uint8 moveA,
        uint8 moveB,
        bytes32 randomSeed
    ) virtual public pure returns (bytes memory);
```

## Using this repo

### Setup

```shell
npm install 
```

### Running Tests

```shell
npx hardhat test
```

