namespace Protocol;

using System;
using System.Numerics;
using Nethereum.ABI;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;
/**
 * This is the AppData to be used in the VariablePart of a ForceMove state update when participating in a RollingRandom state
 * channel application. This wraps the serialized game state itself in the GameState field
 */
public record AppData
{
	[Parameter("bytes32", "preCommitA", 1)]
    public byte[] PreCommitA { get; set; }

    [Parameter("bytes32", "preCommitB", 2)]
    public byte[] PreCommitB { get; set; }

    [Parameter("tuple", "revealA", 3)]
    public Reveal RevealA { get; set; }

    [Parameter("tuple", "revealB", 4)]
    public Reveal RevealB { get; set; }

    // in Toshimon protocol this is the ABI encoded GameState
    [Parameter("bytes", "gameState", 5)]
    public byte[] GameState { get; set; }

    public AppData() {
        PreCommitA = new Reveal().CommitHash;
        PreCommitB = new Reveal().CommitHash;
        RevealA = new Reveal();
        RevealB = new Reveal();
        GameState = new byte[]{0};
    }

    public AppData(byte[] gameState) : this() {
        GameState = gameState;
    }

    public byte[] AbiEncode() {
        ABIEncode abiEncode = new ABIEncode();
        return abiEncode.GetABIParamsEncoded(this);
    }

    public static AppData AbiDecode(byte[] encoded) {
        var decoder = new ParameterDecoder();
        return (AppData) decoder.DecodeAttributes(encoded, typeof(AppData));
    }
}
