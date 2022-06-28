namespace toshimon_state_machine;

using System.Security.Cryptography;

public static class DetRandom {
	
	/**
	 * Select an integer uniformly between min and max inclusive
	 * It is important to use a different nonce if reusing the same randomSeed or else 
	 * outcomes will be identical for each player
	 */
	public static uint uniform(uint min, uint max, uint randomSeed, uint nonce) {
	    HashAlgorithm algorithm = SHA256.Create();
	    byte[] hash = algorithm.ComputeHash(BitConverter.GetBytes(nonce + randomSeed));
	    uint val = BitConverter.ToUInt32(hash);
	    return min + val % (max - min + 1);
	}
}

