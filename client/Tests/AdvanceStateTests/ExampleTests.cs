namespace test;

public class ExampleTests
{
    private StateEngine eng;
    private ToshimonDeployment deployment;
    private uint seed = 0;

    private const int A = 0;
    private const int B = 1;

    // setup - called before each test
    public ExampleTests() {

        this.deployment = new ToshimonDeployment(Environment.GetEnvironmentVariable("DEPLOYMENT"));

        // set up the state engine
        var web3 = new Nethereum.Web3.Web3(Environment.GetEnvironmentVariable("ETH_RPC"));

        var st = new EvmStateTransition(web3, deployment.StateTransitionContractAddress);
        this.eng = new StateEngine(st);
    }

    [Fact]
    public void TwentySavageAttack()
    {
        // initial state
        GameState gs0 = TestHelpers.build1v1 (
            TestHelpers.testMonster1(deployment.Moves[102].Address),
            TestHelpers.testMonster1(deployment.Moves[102].Address)
        );
        eng.Init(gs0);
        eng.next(GameAction.Move1, GameAction.Move1, seed);

        Console.WriteLine("{0}", eng.Last().Player(A).after.Monsters[0]);

        // player A takes 20 damage
        Assert.True(eng.Last().Player(A).Monster(0).HasTakenDamage(20));

        // player B takes 20 damage
        Assert.True(eng.Last().Player(B).Monster(0).HasTakenDamage(20));

        // Player A PP for move 0 decreases by 1
        Assert.True(eng.Last().Player(A).Monster(0).HasDecreasedPP(0, 1));

        // Player B PP for move 0 decreases by 1
        Assert.True(eng.Last().Player(B).Monster(0).HasDecreasedPP(0, 1));


    }

    [Fact]
    public void CanSwitchActiveMonster() {
        // initial state
        GameState gs0 = TestHelpers.build2v2 (
            TestHelpers.testMonster1(deployment.Moves[0].Address),
            TestHelpers.testMonster1(deployment.Moves[0].Address)
        );
        eng.Init(gs0);
        // Player A Swap to monster 2 (index 1) as the active monster
        eng.next(GameAction.Swap2, GameAction.Noop, seed);

        Assert.Equal(1, eng.Last().Player(A).after.ActiveMonsterIndex);
        Assert.Equal(0, eng.Last().Player(B).after.ActiveMonsterIndex);
    }

    [Fact]
    public void CanUsePotion() {
        // initial state
        GameState gs0 = TestHelpers.build1v1 (
            TestHelpers.testMonster1(deployment.Moves[0].Address),
            TestHelpers.testMonster1(deployment.Moves[0].Address)
        );
        // give A a potion in slot 0
        ItemCard potionCard = new ItemCard() { Definition = deployment.Items[012].Address, Used = false };
        gs0 = gs0.SetPlayer(gs0[A].AddItem(potionCard), A);
        
        // reduce A active monster HP to 10
        var playerA = gs0.PlayerA.SetActiveMonster(gs0.PlayerA.GetActiveMonster() with { Stats = gs0.PlayerA.GetActiveMonster().Stats with { Hp = 10 } } );
        gs0.PlayerA = playerA;
        eng.Init(gs0);

        // Player A used a restore potion. That should heal them to 10+25 = 35
        eng.next(GameAction.Item1, GameAction.Noop, seed);

        Assert.Equal(10+25, eng.Last().after.PlayerA.GetActiveMonster().Stats.Hp);
        
    }
}