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

The disadvantage of the above protocol is that:

- It requires 4 messages to be sent/signed before the randomness is ready to use. If randomness is required for every move this is a significant messaging overhead
- Neither party commited to what they plan to do with the randomness. In games where a player has multiple possible actions this could allow them to always pick the most advantageous

## Proposed Solution

The solution is to use a 3-stage scheme for each move. If it is player A's move:

- A commits to their move and a random seed.
	- The seed is used to salt the move so lookup tables are not possible if the space of possible moves is small.
- B submits a seed, no commitment is required
- A reveals their move, their seed and takes any actions to modify the state based on their move

The roles are then reversed and B can make a move

## Example Game

For an example this repo implements a simplified version of blackjack. Each turn a player chose to either Hit or Stand. A dice is rolled based on the shared randomness and the value added to the players total. If their total exceeds 21 then the player busts and loses. If a player Stands while the other is standing them the game is over and the player with the highest score wins of if they are equal it is a tie.

## Using this repo

### Setup

```shell
npm install 
```

### Running Tests

```shell
npx hardhat test
```

