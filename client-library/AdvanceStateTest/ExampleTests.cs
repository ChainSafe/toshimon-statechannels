namespace test;

public class ExampleTests
{
    private StateEngine<LocalStateTransition> eng;
    private uint seed = 0;

    private const int A = 0;
    private const int B = 1;

    // setup - called before each test
    public ExampleTests() {
        this.eng = new StateEngine<LocalStateTransition>();
    }

    [Fact]
    public void SimpleAttackWorks()
    {
        // initial state
        GameState gs0 = TestHelpers.build1v1 (
            TestHelpers.testMonster1(),
            TestHelpers.testMonster1()
        );
        eng.Init(gs0);
        eng.next(GameAction.Move1, GameAction.Move1, seed);
        Assert.True(eng.isConsistent());

        // player A takes 10 damage
        Assert.True(eng.Last().Player(A).Monster(0).HasTakenDamage(10));

        // player B takes 10 damage
        Assert.True(eng.Last().Player(B).Monster(0).HasTakenDamage(10));

        // Player A PP for move 0 decreases by 1
        Assert.True(eng.Last().Player(A).Monster(0).HasDecreasedPP(0, 1));

        // Player B PP for move 0 decreases by 1
        Assert.True(eng.Last().Player(B).Monster(0).HasDecreasedPP(0, 1));

        Console.WriteLine(eng.Last().after.AbiEncode());
    }

    [Fact]
    public void CanSwitchActiveMonster() {
        // initial state
        GameState gs0 = TestHelpers.build2v2 (
            TestHelpers.testMonster1(),
            TestHelpers.testMonster1()
        );
        eng.Init(gs0);
        // Swap to monster 2 (index 1) as the active monster
        eng.next(GameAction.Swap2, GameAction.Move1, seed);

        // the swap happens before the attack so A Monster 2 takes the damage
        // player A takes 10 damage on M2, None on M1
        Assert.True(eng.Last().Player(A).Monster(0).HasTakenDamage(0));
        Assert.True(eng.Last().Player(A).Monster(1).HasTakenDamage(10));
    }

    [Fact]
    public void CanUsePotion() {
        // initial state
        GameState gs0 = TestHelpers.build1v1 (
            TestHelpers.testMonster1(),
            TestHelpers.testMonster1()
        );
        // give A a potion in slot 0
        ItemCard potionCard = new ItemCard() { Definition = Restore.Id, Used = false };
        gs0 = gs0.SetPlayer(gs0[A].AddItem(potionCard), A);
        eng.Init(gs0);

        // both players attack
        eng.next(GameAction.Noop, GameAction.Move1, seed);
        Assert.True(eng.Last().Player(A).Monster(0).HasTakenDamage(10));

        // Player A used the potion. That should heal them to full health then they take 10 damage again
        eng.next(GameAction.Item1, GameAction.Noop, seed);
        // damave to A monster should be 10 from full HP
        Console.WriteLine(eng.ToString());
        Console.WriteLine(eng.Last().after[0].Monsters[0]);
        Assert.True(gs0[0].diff(eng.Last().after[0]).Monster(0).HasTakenDamage(0));

        
    }
}