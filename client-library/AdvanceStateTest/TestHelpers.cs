namespace test;

using System.Collections.Immutable;

public static class TestHelpers {

	// an example monster card used throughout tests
	public static MonsterCard testMonster1() {
        Stats stats = new Stats {
            Hp = 70,
            Attack = 70,
            Defense = 70,
            SpAttack = 90,
            SpDefense = 90,
            Speed = 90,
            PP = ImmutableArray.Create<uint>(new uint[]{10, 0, 0, 0}),
        };

        return new MonsterCard(
            stats,
            stats,
            new string[]{ DoTen.Id, Lookup.Null, Lookup.Null, Lookup.Null }
        );
    }

    // build a game state quickly where each player only has one monster
    public static GameState build1v1(MonsterCard one, MonsterCard two) {
    	return new GameState(
            new PlayerState(new MonsterCard[]{one}, new ItemCard[0]),
            new PlayerState(new MonsterCard[]{two}, new ItemCard[0])
        );
    }

    public static GameState build2v2(MonsterCard one, MonsterCard two) {
        return new GameState(
            new PlayerState(new MonsterCard[]{one, one}, new ItemCard[0]),
            new PlayerState(new MonsterCard[]{two, two}, new ItemCard[0])
        );
    }

}