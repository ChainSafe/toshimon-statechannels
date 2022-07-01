using Spectre.Console;
using Protocol;
using Nethereum.Signer;
using System.Numerics;

using Nethereum.Hex.HexConvertors.Extensions;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.KeyStore.Model;

public static class Utils {
	public static void renderMonster(MonsterRecord mon) {

        AnsiConsole.Write(new Rule(mon.Name));
        AnsiConsole.Write(new Panel(mon.Description));

        var typeTable = new Table();
        typeTable.AddColumn("Primary Type");
        typeTable.AddColumn("Secondary Type"); 
        typeTable.AddRow(((ToshimonType) mon.Type1).ToString(), ((ToshimonType) mon.Type2).ToString());
        AnsiConsole.Write(typeTable);

        var movesTable = new Table();
        movesTable.AddColumn("Move");
        movesTable.AddColumn("SP");
        movesTable.AddColumn("Description");

        var db = new MovesDb("./ToshimonDatabase/moves.csv");

        foreach(var guid in new string[]{mon.Move1, mon.Move2, mon.Move3}) {
            MoveRecord move = db.findByGuid(guid);
            movesTable.AddRow(move.Name, move.Sp.ToString(), move.Description ?? "");
        }

        AnsiConsole.Write(movesTable);

        var statTable = new Table();
        statTable.AddColumn("Stat");
        statTable.AddColumn("Value");
        // Add some rows
        statTable.AddRow("MaxHP", String.Format("{0}", mon.MaxHP));
        statTable.AddRow("Attack", String.Format("{0}", mon.Attack));
        statTable.AddRow("Defense", String.Format("{0}", mon.Defense));
        statTable.AddRow("Sp.Attack", String.Format("{0}", mon.SpAttack));
        statTable.AddRow("Sp.Defense", String.Format("{0}", mon.SpDefense));
        statTable.AddRow("Speed", String.Format("{0}", mon.Speed));

        statTable.Border(TableBorder.Rounded);

        // Render the statTable to the console
        AnsiConsole.Write(statTable);
    }

    public static MonsterCard[] selectToshimonParty() {
      AnsiConsole.Write(new Rule("Select Toshimon Party"));

      List<MonsterCard> monsters = new List<MonsterCard>();

        // TODO make this file path an env var or something
      var db = new ToshimonDb("./ToshimonDatabase/toshimon.csv");

      for (int i = 0; i < 5; i++) {
        MonsterRecord monster;
        do {
            int toshimonNumber = AnsiConsole.Ask<int>(String.Format("Input Toshidex number for party member {0}:", i+1));
            monster = db.findByToshidexNumber(toshimonNumber);
            renderMonster(monster);
        } while (!AnsiConsole.Confirm(String.Format("Use this Toshimon?")));
        monsters.Add(monster.toMonsterCard());
    }
    return monsters.ToArray();
}

public static EthECKey generateNewKey() {
   return EthECKey.GenerateKey();
}

public static EthECKey createOrLoadKey() {
   if (AnsiConsole.Confirm("Generate new ephemeral keypair for this game?")) {
    AnsiConsole.MarkupLine("Generating new keypair...");
            // generate a new keypair
    EthECKey ecKey = generateNewKey();
            // save to file
    var keyStoreService = new Nethereum.KeyStore.KeyStoreScryptService();
    var scryptParams = new ScryptParams {Dklen = 32, N = 262144, R = 1, P = 8};			

    string path = AnsiConsole.Prompt(
        new TextPrompt<string>("Enter path to save keystore file."));

    string password = AnsiConsole.Prompt(
        new TextPrompt<string>("Enter keystore password?")
        .Secret());

    var keyStore = keyStoreService.EncryptAndGenerateKeyStore(password, ecKey.GetPrivateKeyAsBytes(), ecKey.GetPublicAddress(), scryptParams);
    string json = keyStoreService.SerializeKeyStoreToJson(keyStore);

    File.WriteAllText(path, json);  

    return ecKey;
} else {
    string path = AnsiConsole.Ask<string>("Path to existing key store file");
    string password = AnsiConsole.Prompt(
        new TextPrompt<string>("Enter keystore password?")
        .Secret());

    return loadKey(path, password);
}
}

public static EthECKey loadKey(string path, string password) {
    string encryptedJson = File.ReadAllText(path);
    var keyStoreService = new Nethereum.KeyStore.KeyStoreScryptService();
    byte[] privateKey = keyStoreService.DecryptKeyStoreFromJson(password, encryptedJson);
    return new EthECKey(privateKey, true);
}

public static void signAndWriteUpdate(FixedPart fixedPart, VariablePart variablePart, EthECKey key, string path) {
        // sign this state update with this players key
    var signature = new StateUpdate(fixedPart, variablePart).Sign(key);    

        // combine into an acceptance message
    var stateUpdate = new SignedStateUpdate() {
        VariablePart = variablePart,
        Signature = signature,
    };
        // write to std out or a file if output path provided
    using (Stream s = File.Create(path) ) {
        byte[] stateUpdateBytes = stateUpdate.AbiEncode();
        s.Write(stateUpdateBytes, 0, stateUpdateBytes.Length);
    }
}

public static void renderProposal() {

}

public static void renderChannelDef(FixedPart channelSpec) {
    var table = new Table();
    table.HideHeaders();

    var playerTable = new Table();
    playerTable.HideHeaders();
    playerTable.AddColumn("");
    playerTable.AddColumn("");
    playerTable.AddRow("Player A", channelSpec.Participants[0]);
    playerTable.AddRow("Player B", channelSpec.Participants[1]);
    AnsiConsole.Write(playerTable);

    table.AddColumn("");
    table.AddColumn("");
    table.AddRow("Channel Id", Convert.ToBase64String(channelSpec.ChannelId));
    table.AddRow("Chain Id", channelSpec.ChainId.ToString());
    table.AddRow("ChannelNonce",channelSpec.ChannelNonce.ToString());
    table.AddRow("AppDefinition",channelSpec.AppDefinition);
    table.AddRow("ChallengeDuration",channelSpec.ChallengeDuration.ToString());
    AnsiConsole.Write(table);
}

public static void renderState(GameState state, int whoami) {
        // Create a table
    var table = new Table();

    PlayerState me = state[whoami];
    PlayerState other = state[not(whoami)];

        // Add some columns
    table.AddColumn("Enemy");
    table.AddColumn("Yours");

    var db = new ToshimonDb("./ToshimonDatabase/toshimon.csv");

    for( int i = 0; i < 5; i++) {
        table.AddRow(
            monsterSummary(other.Monsters[i], other.ActiveMonsterIndex == i, db),
            monsterSummary(me.Monsters[i], me.ActiveMonsterIndex == i, db)
        );
    }

    table.Border(TableBorder.Rounded);
    table.Expand();

    // Render the table to the console
    AnsiConsole.Write(table);
}

private static Table monsterSummary(MonsterCard m, bool active, ToshimonDb db) {
    var t = new Table();
    t.AddColumn(String.Format("{1} {0}", active ? "(active)" : "",  db.findByCardId(m.CardId).Name));
    t.AddRow(new BarChart()
        .AddItem("HP", m.Stats.Hp, Color.Green)
        .WithMaxValue(m.BaseStats.Hp));
    return t;
}

private static int not(int i) {
    return i switch {
        0 => 1,
        1 => 0,
        _ => throw new IndexOutOfRangeException(),
    };
}

public static MonsterCard GetActiveMonster(PlayerState p) {
    return p.Monsters[p.ActiveMonsterIndex];
}
}