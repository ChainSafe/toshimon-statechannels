namespace test;

public class ExampleTests
{
    private StateEngine eng;
    private ToshimonDeployment.ToshimonDeployment deployment;
    private uint seed = 0;

    private const int A = 0;
    private const int B = 1;

    // setup - called before each test
    public ExampleTests() {

        this.deployment = new ToshimonDeployment.ToshimonDeployment(Environment.GetEnvironmentVariable("DEPLOYMENT"));

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
            TestHelpers.testMonster1(deployment.Moves[000].Address)
        );

        Console.WriteLine("{0}", gs0.PlayerA.Monsters[0].Moves[0]);

        eng.Init(gs0);
        eng.next(GameAction.Move1, GameAction.Noop, seed);

        // player A takes 0 damage
        Assert.True(eng.Last().Player(A).Monster(0).HasTakenDamage(0));

        // player B takes 20 damage
        Assert.True(eng.Last().Player(B).Monster(0).HasTakenDamage(20));

        // Player A PP for move 0 decreases by 1
        Assert.True(eng.Last().Player(A).Monster(0).HasDecreasedPP(0, 1));

        // Player B PP not decreased
        Assert.True(eng.Last().Player(B).Monster(0).HasDecreasedPP(0, 0));
    }

    // [Fact]
    // public void MoldSporeAttack()
    // {
    //     // initial state
    //     GameState gs0 = TestHelpers.build1v1 (
    //         TestHelpers.testMonster1(deployment.Moves[141].Address),
    //         TestHelpers.testMonster1(deployment.Moves[000].Address)
    //     );
    //     eng.Init(gs0);
    //     eng.next(GameAction.Move1, GameAction.Move1, seed);

    //     // player B has the poison status
    //     Assert.Equal(deployment.StatusConditions[001].Address ,eng.Last().Player(B).Monster(0).after.StatusCondition);
    //     // this value is deterministic random and will change if the seed changes
    //     Assert.Equal(1, eng.Last().Player(B).Monster(0).after.StatusConditionCounter);
    // }

    [Fact]
    public void LeafBlowerAttack()
    {
        // initial state
        GameState gs0 = TestHelpers.build1v1 (
            TestHelpers.testMonster1(deployment.Moves[147].Address),
            TestHelpers.testMonster1(deployment.Moves[000].Address)
        );
        eng.Init(gs0);
        eng.next(GameAction.Move1, GameAction.Noop, seed);

        // player B takes some damage determined by type matchup, move power and random variation
        // this will change if the damage calculation changes        
        Assert.Equal(28, eng.Last().Player(B).Monster(0).after.Stats.Hp);
    }

    [Fact]
    public void PoisonStatus()
    {
        // initial state
        GameState gs0 = TestHelpers.build1v1 (
            TestHelpers.testMonster1(deployment.Moves[141].Address),
            TestHelpers.testMonster1(deployment.Moves[000].Address)
        );
        // assign the poison status
        gs0.PlayerA.Monsters[0].StatusCondition = deployment.StatusConditions[001].Address;
        gs0.PlayerA.Monsters[0].StatusConditionCounter = 3;

        eng.Init(gs0);
        eng.next(GameAction.Noop, GameAction.Noop, seed);

        // A should have taken 1/16th max HP damage
        Assert.True(eng.Last().Player(A).Monster(0).HasTakenDamage((uint) gs0.PlayerA.Monsters[0].BaseStats.Hp / 16));
        // counter decremented
        Assert.Equal(2, eng.Last().Player(A).Monster(0).after.StatusConditionCounter);

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
        gs0[A].Items[0] = potionCard;
        
        // reduce A active monster HP to 10
        var playerA = gs0.PlayerA.SetActiveMonster(gs0.PlayerA.GetActiveMonster() with { Stats = gs0.PlayerA.GetActiveMonster().Stats with { Hp = 10 } } );
        gs0.PlayerA = playerA;
        eng.Init(gs0);

        // Player A used a restore potion. That should heal them to 10+25 = 35
        eng.next(GameAction.Item1, GameAction.Noop, seed);

        Assert.Equal(10+25, eng.Last().after.PlayerA.GetActiveMonster().Stats.Hp);
        
    }
}