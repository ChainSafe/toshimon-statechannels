namespace test;

public class ExampleTests
{
    private StateEngine eng;
    private uint seed = 0;

    private const int A = 0;
    private const int B = 1;

    // setup - called before each test
    public ExampleTests() {
        // create the state engine
        // var st = new LocalStateTransition();
        // 
        var web3 = new Nethereum.Web3.Web3("http://127.0.0.1:8545");
        var contractAddress = "0xe7f1725E7734CE288F8367e1Bb143E90bb3F0512";
        var st = new EvmStateTransition(web3, contractAddress);

        this.eng = new StateEngine(st);
    }

    // [Fact]
    // public void SimpleAttackWorks()
    // {
    //     // initial state
    //     GameState gs0 = TestHelpers.build1v1 (
    //         TestHelpers.testMonster1(),
    //         TestHelpers.testMonster1()
    //     );
    //     eng.Init(gs0);
    //     eng.next(GameAction.Move1, GameAction.Move1, seed);
    //     Assert.True(eng.isConsistent());

    //     // player A takes 10 damage
    //     Assert.True(eng.Last().Player(A).Monster(0).HasTakenDamage(10));

    //     // player B takes 10 damage
    //     Assert.True(eng.Last().Player(B).Monster(0).HasTakenDamage(10));

    //     // Player A PP for move 0 decreases by 1
    //     Assert.True(eng.Last().Player(A).Monster(0).HasDecreasedPP(0, 1));

    //     // Player B PP for move 0 decreases by 1
    //     Assert.True(eng.Last().Player(B).Monster(0).HasDecreasedPP(0, 1));

    //     Console.WriteLine(eng.Last().after.AbiEncode());
    // }

    [Fact]
    public void CanSwitchActiveMonster() {
        // initial state
        GameState gs0 = TestHelpers.build2v2 (
            TestHelpers.testMonster1(),
            TestHelpers.testMonster1()
        );
        eng.Init(gs0);
        // Player A Swap to monster 2 (index 1) as the active monster
        eng.next(GameAction.Swap2, GameAction.Noop, seed);

        Console.WriteLine("{0}", eng.Last().Player(A).after);

        Assert.True(eng.Last().Player(A).after.ActiveMonsterIndex == 1);
        Assert.True(eng.Last().Player(B).after.ActiveMonsterIndex == 0);
    }

    [Fact]
    public void CanUsePotion() {
        // initial state
        GameState gs0 = TestHelpers.build1v1 (
            TestHelpers.testMonster1(),
            TestHelpers.testMonster1()
        );
        // give A a potion in slot 0
        ItemCard potionCard = new ItemCard() { Definition = "0x9fE46736679d2D9a65F0992F2272dE9f3c7fa6e0", Used = false };
        gs0 = gs0.SetPlayer(gs0[A].AddItem(potionCard), A);
        
        // reduce A active monster HP to 10
        var playerA = gs0.PlayerA.SetActiveMonster(gs0.PlayerA.GetActiveMonster() with { Stats = gs0.PlayerA.GetActiveMonster().Stats with { Hp = 10 } } );
        gs0.PlayerA = playerA;
        eng.Init(gs0);

        // Player A used a restore potion. That should heal them to 10+30 = 40
        eng.next(GameAction.Item1, GameAction.Noop, seed);

        Assert.True(eng.Last().after.PlayerA.GetActiveMonster().Stats.Hp == 40);
        
    }
}