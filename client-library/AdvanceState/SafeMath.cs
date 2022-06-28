namespace toshimon_state_machine;

static class SafeMath {
	public static uint subtract(uint x, uint y) {
		if (y > x) return 0;
		else return x - y;
	}
}
