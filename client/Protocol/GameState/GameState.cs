namespace Protocol;

using System;
using System.Linq;
using System.Numerics;
using Nethereum.ABI;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;
/**
 * The top level game state data structure. Encodes all properties of a toshimon game
 * at one instant in time.
 */
[Struct("GameState")]
public record GameState
{

    [Parameter("tuple[2]", "players", 1)]
    public List<PlayerState> Players { get; set; }

    public PlayerState PlayerA { get => Players[0]; set => Players[0] = value; }
    public PlayerState PlayerB { get => Players[1]; set => Players[1] = value; }

    //////////////////////////////////////////////////

    public GameState() {
        Players = new List<PlayerState>() { new PlayerState(), new PlayerState() };
    }

    public GameState(PlayerState playerA, PlayerState playerB) {
        Players = new List<PlayerState>();
        Players.Add(playerA);
        Players.Add(playerB);
    }

    // indexing helper
    public PlayerState this[int index] {
        get => Players[index];
    }

    public byte[] AbiEncode() {
        ABIEncode abiEncode = new ABIEncode();
        return abiEncode.GetABIParamsEncoded(this);
    }

    public static GameState AbiDecode(byte[] encoded) {
        var decoder = new ParameterDecoder();
        return (GameState) decoder.DecodeAttributes(encoded, typeof(GameState));
    }
}
