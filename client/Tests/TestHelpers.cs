namespace test;

using System.Collections.Immutable;

public static class TestHelpers {

	// an example monster card used throughout tests
    // It only has a single move and this is in every move slot 
	public static MonsterCard testMonster1(string moveAddress) {
        Stats stats = new Stats() {
            Hp = 70,
            Attack = 70,
            Defense = 70,
            SpAttack = 90,
            SpDefense = 90,
            Speed = 90,
            PP = new List<uint>(new uint[]{10, 0, 0, 0}),
        };

        return new MonsterCard() with {
            Stats = stats with {},
            BaseStats = stats with {},
            Moves = Enumerable.Repeat(moveAddress, 4).ToList(),
        };
    }

    // build a game state quickly where each player only has one monster
    public static GameState build1v1(MonsterCard one, MonsterCard two) {
        var gameState = new GameState();
        gameState.PlayerA.Monsters[0] = one;
        gameState.PlayerB.Monsters[0] = two;        
    	return gameState;
    }

    public static GameState build2v2(MonsterCard one, MonsterCard two) {
        var gameState = new GameState();
        gameState.PlayerA.Monsters[0] = one;
        gameState.PlayerA.Monsters[1] = one;

        gameState.PlayerB.Monsters[0] = two;
        gameState.PlayerB.Monsters[1] = two;
        return gameState;
    }

}