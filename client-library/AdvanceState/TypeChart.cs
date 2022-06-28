namespace toshimon_state_machine;

using Protocol;

// Type effect multiplier
// Enum value / 2 gives the floating point value of the modifier
public enum M {
	N = 0, // No effect
	H = 1, // half
	U = 2, // unmodified
	D = 4, // Double
}

public class TypeChart
{
    static M[][] chart =
    {
        //                   Flex    Fire   Wat    Lig    Pla    Ice    Bru    Tox    Ear    Eth    Dig   Dark    Hea    Crys   
        /*Flex*/    new M[] { M.U,   M.U,   M.U,   M.U,   M.U,   M.U,   M.H,   M.U,   M.U,   M.U,   M.H,   M.U,   M.U,   M.D},
        /*Fire*/    new M[] { M.D,   M.H,   M.H,   M.H,   M.D,   M.D,   M.U,   M.U,   M.U,   M.D,   M.U,   M.U,   M.U,   M.H},
        /*Water*/   new M[] { M.D,   M.D,   M.U,   M.U,   M.H,   M.H,   M.U,   M.U,   M.U,   M.U,   M.U,   M.U,   M.U,   M.U},
        /*Light*/   new M[] { M.U,   M.N,   M.H,   M.D,   M.D,   M.U,   M.D,   M.U,   M.H,   M.U,   M.H,   M.D,   M.H,   M.D},
        /*Plant*/   new M[] { M.U,   M.H,   M.D,   M.U,   M.H,   M.U,   M.U,   M.D,   M.D,   M.U,   M.U,   M.U,   M.U,   M.H},
        /*Ice*/     new M[] { M.D,   M.H,   M.D,   M.U,   M.U,   M.U,   M.H,   M.D,   M.U,   M.U,   M.U,   M.U,   M.U,   M.U},
        /*Brute*/   new M[] { M.H,   M.U,   M.U,   M.U,   M.U,   M.D,   M.H,   M.U,   M.U,   M.N,   M.U,   M.U,   M.D,   M.D},
        /*Toxic*/   new M[] { M.U,   M.U,   M.D,   M.U,   M.D,   M.U,   M.U,   M.H,   M.D,   M.U,   M.U,   M.H,   M.H,   M.U},
        /*Earth*/   new M[] { M.U,   M.D,   M.H,   M.D,   M.H,   M.U,   M.U,   M.D,   M.U,   M.U,   M.U,   M.U,   M.U,   M.H},
        /*Ether*/   new M[] { M.D,   M.U,   M.U,   M.H,   M.U,   M.U,   M.U,   M.U,   M.U,   M.H,   M.D,   M.D,   M.U,   M.U},
        /*Digital*/ new M[] { M.U,   M.U,   M.U,   M.U,   M.D,   M.D,   M.H,   M.U,   M.U,   M.H,   M.D,   M.U,   M.D,   M.N},
        /*Dark*/    new M[] { M.D,   M.U,   M.U,   M.H,   M.H,   M.U,   M.D,   M.U,   M.U,   M.D,   M.D,   M.U,   M.N,   M.H},
        /*Heart*/   new M[] { M.D,   M.U,   M.U,   M.H,   M.U,   M.U,   M.H,   M.U,   M.U,   M.U,   M.U,   M.D,   M.H,   M.D},
        /*Crystal*/ new M[] { M.H,   M.D,   M.H,   M.U,   M.U,   M.U,   M.D,   M.H,   M.U,   M.D,   M.U,   M.H,   M.D,   M.U}
    };
    public static M GetEffectiveness(ToshimonType attackType, ToshimonType defenseType)
    {
        if (attackType == ToshimonType.None || defenseType == ToshimonType.None)
            return M.U;

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;

        return chart[row][col];
    }
}
