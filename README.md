# Rolling commit-reveal randomness in 2 player state channels

- Idea from here https://statechannels.discourse.group/t/rolling-commit-reveal-an-idea-for-randomness-in-force-move-games/138

## Problem Statement

Many games require a source of randomness to determine the outcomes of particular moves. With two non-colluding players it is actually trivial to compute shared randomness via the following interactive commit/reveal protocol:

- A sends $\text{hash}(r_A)$ to B
- B sends $\text{hash}(r_B)$ to A
- A sends $r_A$ to B
- B sends $r_B$ to A
- A and B both compute randomness as $f(r_A, r_B)$


The above protocol can take place in a state channel such that the result of the shared randomness generation is determinisitic and can be used to determine future state transitions.

The disadvantage of the above protocol is that is requires 4 messages to be sent/signed before the randomness is ready to use. If randomness is required for every move this is a significant messaging overhead

## Proposed Solution

To reduce messaging overhead the idea is to pipeline the above process such that, after an initialization period, each player both commits and reveals some randomness at each turn and the reveal from the current and previous turn can be used to generate a shared random value. 

Because the randomness at turn $i$ is produced from values committed at rounds $i-1 and i-2$ (2-player case) it is unmanipulatable. 

The state transition function must enforce that the reveals do indeed correspond to their commitment, and that any state transition which use the random value for that round do indeed have the expected outcome. There is one small dififculty that the reveal depends on a commitment made $n$ states ago, where $n$ is the number of participants. So that the state transition can be validated given only the current and previous states there needs to be state caching in place the stores the prior $n-1$ commits in addition to the current commit. 

## Using this repo

### Setup

```shell
npm install 
```

### Running Tests

```shell
npx hardhat test
```

