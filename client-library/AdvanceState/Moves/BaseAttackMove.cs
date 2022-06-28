namespace toshimon_state_machine;

using Protocol;

public abstract class BaseAttackMove : BaseMove {

	public abstract ToshimonType MoveType { get; }
	public abstract uint ExtraCrit { get; }

	public abstract uint Power(GameState state, int mover, uint randomSeed);

	public bool isCrit(uint randomSeed) {
		return false;
	}

	public uint applyModifier(uint baseDamage, MonsterCard defender, uint randomSeed) {
		uint critModifier = isCrit(randomSeed) ? 2u : 1u; // double damage for a crit
		uint primaryTypeModifier = (uint) TypeChart.GetEffectiveness(MoveType, defender.Type);
		uint secondaryTypeModifier = (uint) TypeChart.GetEffectiveness(MoveType, defender.SecondaryType);
		uint randomModifier = 1u; // TODO figure out randomness

		return baseDamage * critModifier * randomModifier * primaryTypeModifier * secondaryTypeModifier / 4; // divide by 4 because both the typeModifiesr are doubled
	}

	public override GameState applyMove(GameState state, int mover, uint randomSeed) {

		MonsterCard attacker = state[mover].GetActiveMonster();
		MonsterCard defender = state[not(mover)].GetActiveMonster();

		uint movePower = Power(state, mover, randomSeed);
		uint baseDamage = movePower * attacker.Stats.Attack / defender.Stats.Defense + 2;
		uint damage = applyModifier(baseDamage, defender, randomSeed);

		defender = defender.TakeDamage(damage);

		// update the monsters
		state = state.SetPlayer(state[mover].SetActiveMonster(attacker), mover);
		state = state.SetPlayer(state[not(mover)].SetActiveMonster(defender), not(mover));

		return state;
	}

}
