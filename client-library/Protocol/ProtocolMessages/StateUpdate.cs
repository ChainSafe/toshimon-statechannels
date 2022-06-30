namespace Protocol;

using System.Numerics;
using Nethereum.ABI;
using Nethereum.Signer;
using Nethereum.ABI.FunctionEncoding.Attributes;

public record StateUpdate {

    [Parameter("bytes32", "chanelId", 1)]
    public byte[] ChannelId { get; set; }

    [Parameter("bytes", "appData", 2)]
    public byte[] AppData { get; set; }
    
    [Parameter("tuple[]", "outcome", 3)]
    public List<SingleAssetExit> Outcome { get; set; }

    [Parameter("uint48", "turnNum", 4)]
    public ulong TurnNum { get; set; }

    [Parameter("bool", "isFinal", 5)]
    public bool IsFinal { get; set; }

	public StateUpdate(FixedPart fixedPart, VariablePart variablePart) {
		ChannelId = fixedPart.ChannelId;
		AppData = variablePart.AppData;
		Outcome = variablePart.Outcome;
		TurnNum = variablePart.TurnNum;
		IsFinal = variablePart.IsFinal;
	}

	// To compute the state hash the fields of the fixed and variable parts are encoded in a particular way
	// see https://github.com/statechannels/go-nitro/blob/7bf69da2e9df58b9fddf8d02158449667223c8d2/nitro-protocol/contracts/ForceMove.sol#L745
	public byte[] StateHash {
		get {
			ABIEncode abiEncode = new ABIEncode();
    		return abiEncode.GetSha3ABIEncoded(
    			ChannelId, // this is a derived field on fixedPart. Essentially the hash of all fields
        		AppData,
        		Outcome,
        		TurnNum,
        		IsFinal
        	);
		}
	}

	public Signature Sign(EthECKey key) {
		EthereumMessageSigner signer = new EthereumMessageSigner();
		// this takes care of adding the special message prefix '\u0019Ethereum Signed Message:\n' !! Do we need this??!!
		// byte[] hash = signer.HashPrefixedMessage(this.StateHash);
		EthECDSASignature sig = signer.SignAndCalculateV(this.StateHash, key);
		return new Signature() {
			V = sig.V[0], // this indexing should always be right as v should always be exactly 1 byte
			R = sig.R,
			S = sig.S,
		};
	}
}
