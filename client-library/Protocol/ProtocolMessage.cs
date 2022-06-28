namespace Protocol;

using Nethereum.ABI;

public abstract record ProtocolMessage {
	public byte[] AbiEncode() {
		ABIEncode abiEncode = new ABIEncode();
	    return abiEncode.GetABIParamsEncoded(this);
	}
}
