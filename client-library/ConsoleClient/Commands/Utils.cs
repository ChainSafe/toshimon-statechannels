using Spectre.Console;
using Protocol;
using Nethereum.Signer;

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
        var db = new ToshimonDb("./toshimon.csv");

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
			string password = ""; // use no password for ephemeral keys
			var keyStore = keyStoreService.EncryptAndGenerateKeyStore(password, ecKey.GetPrivateKeyAsBytes(), ecKey.GetPublicAddress(), scryptParams);
            string json = keyStoreService.SerializeKeyStoreToJson(keyStore);
			File.WriteAllText("./keystore.json", json);  

            return ecKey;
        } else {
            string accountFilePath = AnsiConsole.Ask<string>("Path to existing key store file");
            string password = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter keystore password?")
                    .Secret());

			string encryptedJson = File.ReadAllText(accountFilePath);
			var keyStoreService = new Nethereum.KeyStore.KeyStoreScryptService();
			byte[] privateKey = keyStoreService.DecryptKeyStoreFromJson(password, encryptedJson);
			return new EthECKey(privateKey, true);
        }
    }
}