namespace DeploymentReader;

using System.Text.Json;
using System.Collections.Generic;

public class ToshimonDeployment {
	public string StateTransitionContractAddress { get; }
	public string AdjudicatorContractAddress { get; }
	public Dictionary<uint, NamedDeployment> Moves { get; }
	public Dictionary<uint, NamedDeployment> Items { get; }
	public Dictionary<uint, NamedDeployment> StatusConditions { get; }

	public record NamedDeployment {
		public string Name { get; }
		public string Address { get; }

		public NamedDeployment(string name, string address) {
			Name = name;
			Address = address;
		}

	}

	public ToshimonDeployment(string deploymentRoot) {
		StateTransitionContractAddress = DeploymentReader.readRecord(Path.Combine(deploymentRoot, "ToshimonStateTransition.json")).address;
		AdjudicatorContractAddress = DeploymentReader.readRecord(Path.Combine(deploymentRoot, "Adjudicator.json")).address;

		Moves = DeploymentReader.readRecordList(Path.Combine(deploymentRoot, ".moves.json")).ToDictionary(x => x.id, x => new NamedDeployment(x.name, x.address));
		Items = DeploymentReader.readRecordList(Path.Combine(deploymentRoot, ".items.json")).ToDictionary(x => x.id, x => new NamedDeployment(x.name, x.address));
		StatusConditions = DeploymentReader.readRecordList(Path.Combine(deploymentRoot, ".statusConditions.json")).ToDictionary(x => x.id, x => new NamedDeployment(x.name, x.address));
	}

}

public record DeploymentRecord {
	public uint id { get; set; }
	public string name { get; set; }
	public string address { get; set; }
}

public class DeploymentReader
{
	public static DeploymentRecord readRecord(string path) {
		string jsonString = File.ReadAllText(path);
		return JsonSerializer.Deserialize<DeploymentRecord>(jsonString)!;
	}

	public static List<DeploymentRecord> readRecordList(string path) {
		string jsonString = File.ReadAllText(path);
		return JsonSerializer.Deserialize<List<DeploymentRecord>>(jsonString)!;
	}
}
