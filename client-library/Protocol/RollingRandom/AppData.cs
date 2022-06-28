namespace Protocol;

using System;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

/**
 * This is the AppData to be used in the VariablePart of a ForceMove state update when participating in a RollingRandom state
 * channel application. This wrapps the serialized game state itself in the GameState field
 */
public record AppData
{
	[Parameter("bytes32", "preCommitA", 1)]
    public byte[] PreCommitA { get; set; }

    [Parameter("bytes32", "preCommitB", 1)]
    public byte[] PreCommitB { get; set; }

    [Parameter("tuple", "revealA", 3)]
    public Reveal RevealA { get; set; }

    [Parameter("tuple", "revealB", 4)]
    public Reveal RevealB { get; set; }

    // in Toshimon protocol this is the ABI encoded GameState
    [Parameter("bytes", "gameState", 5)]
    public byte[] GameState { get; set; }
}

public record Reveal {
    [Parameter("uint8", "move", 1)]
    public byte Move { get; set; }

    [Parameter("bytes32", "seed", 1)]
    public byte[] Seed { get; set; }
}
