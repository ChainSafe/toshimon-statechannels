namespace Protocol;

using System;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

/**
 * The top level game state data structure. Encodes all properties of a toshimon game
 * at one instant in time.
 */
public record GameState: ProtocolMessage
{

    [Parameter("tuple", "playerA", 1)]
    public PlayerState PlayerA { get; set; }

    [Parameter("tuple", "playerB", 2)]
    public PlayerState PlayerB { get; set; }


    //////////////////////////////////////////////////

    public GameState(PlayerState playerA, PlayerState playerB) {
        PlayerA = playerA;
        PlayerB = playerB;
    }

    // indexing helper
    public PlayerState this[int index] {
        get => index == 0 ? PlayerA : PlayerB;
    }
}
