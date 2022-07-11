namespace ToshimonDeployment;

using System.Text.Json;
using System.Collections.Generic;

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

	public static int readChainId(string path) {
		string s = File.ReadAllText(path); 
		return int.Parse(s);
	}
}
