namespace Protocol;

using System.Numerics;
using Nethereum.ABI;
using Nethereum.Signer;

public record StateUpdate {
	private FixedPart fixedPart;
	private VariablePart variablePart;

	public StateUpdate(FixedPart fixedPart, VariablePart variablePart) {
		fixedPart = fixedPart;
		variablePart = variablePart;
	}

	// To compute the state hash the fields of the fixed and variable parts are encoded in a particular way
	// see https://github.com/statechannels/go-nitro/blob/7bf69da2e9df58b9fddf8d02158449667223c8d2/nitro-protocol/contracts/ForceMove.sol#L745
	public byte[] StateHash {
		get {
			ABIEncode abiEncode = new ABIEncode();
    		return abiEncode.GetSha3ABIEncoded(
    			fixedPart.ChannelId, // this is a derived field on fixedPart. Essentially the hash of all fields
        		variablePart.AppData,
        		variablePart.Outcome,
        		variablePart.TurnNum,
        		variablePart.IsFinal
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
