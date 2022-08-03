namespace ToshimonDeployment;

using System.Text.Json;
using System.Collections.Generic;

public record ToshimonDeployment {
	public string StateTransitionContractAddress { get; }
	public string AdjudicatorContractAddress { get; }
	public string NitroTestContractAddress { get; }
	public string EncodingTestContractAddress { get; }
	public Dictionary<uint, NamedDeployment> Moves { get; }
	public Dictionary<uint, NamedDeployment> Items { get; }
	public Dictionary<uint, NamedDeployment> StatusConditions { get; }
	public int ChainId { get; }

	public record NamedDeployment {
		public string Name { get; }
		public string Address { get; }

		public NamedDeployment(string name, string address) {
			Name = name;
			Address = address;
		}

	}

	public ToshimonDeployment(string deploymentRoot) {
		ChainId = DeploymentReader.readChainId(Path.Combine(deploymentRoot, ".chainId"));

		StateTransitionContractAddress = DeploymentReader.readRecord(Path.Combine(deploymentRoot, "ToshimonStateTransition.json")).address;
		AdjudicatorContractAddress = DeploymentReader.readRecord(Path.Combine(deploymentRoot, "Adjudicator.json")).address;
		NitroTestContractAddress = DeploymentReader.readRecord(Path.Combine(deploymentRoot, "TESTNitroUtils.json")).address;
		EncodingTestContractAddress = DeploymentReader.readRecord(Path.Combine(deploymentRoot, "TESTEncoding.json")).address;

		Moves = DeploymentReader.readRecordList(Path.Combine(deploymentRoot, ".moves.json")).ToDictionary(x => x.id, x => new NamedDeployment(x.name, x.address));
		Items = DeploymentReader.readRecordList(Path.Combine(deploymentRoot, ".items.json")).ToDictionary(x => x.id, x => new NamedDeployment(x.name, x.address));
		StatusConditions = DeploymentReader.readRecordList(Path.Combine(deploymentRoot, ".statusConditions.json")).ToDictionary(x => x.id, x => new NamedDeployment(x.name, x.address));
	
	}

	public NamedDeployment? getMoveByAddress(string address) {
		return Moves.Values.SingleOrDefault(x => x.Address == address);
	}

	public NamedDeployment? getStatusConditionByAddress(string address) {
		return StatusConditions.Values.SingleOrDefault(x => x.Address == address);
	}

	public string? getMoveAddressById(uint id) {
		return Moves[id].Address;
	}

}
