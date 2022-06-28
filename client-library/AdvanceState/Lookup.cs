namespace toshimon_state_machine;

// Looks up moves/items/status conditions based on their string
// This might seem like a weird thing to do but it is a mock for the on-chain
// version where moves/items/status will be their own contracts and must be looked up by
// an address
public class Lookup
{
	public static string Null = "0x0000000000000000000000000000000000000000";

    public static Dictionary<string, Move> Moves { get; set; } = new Dictionary<string, Move>()
    {
        { DoTen.Id, new DoTen() },
        { Slap.Id, new Slap() },
    };

    public static Dictionary<string, Item> Items { get; set; } = new Dictionary<string, Item>()
    {
        { Restore.Id, new Restore() },
    };

    public static Dictionary<string, StatusCondition> StatusConditions { get; set; } = new Dictionary<string, StatusCondition>()
    {
        { Paralyze.Id, new Paralyze() },
    };
}
