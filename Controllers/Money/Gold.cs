using System;

namespace TheKing.Controllers.Money {
	struct Gold : IComparable<Gold> {
		public int Value { get; }

		public Gold(int value) {
			Value = value;
		}

		public override string ToString() {
			return Value.ToString();
		}

		public Gold Add(Gold addGold) {
			return new Gold(Value + addGold.Value);
		}

		public int CompareTo(Gold other) {
			return Value.CompareTo(other.Value);
		}

		public static Gold Zero => new Gold(0);

		public static bool operator >(Gold lhr, Gold rhr) {
			return lhr.CompareTo(rhr) > 0;
		}

		public static bool operator <(Gold lhr, Gold rhr) {
			return lhr.CompareTo(rhr) <= 0;
		}

		public static Gold operator *(Gold gold, int multiplier) {
			return new Gold(gold.Value * multiplier);
		}
	}
}
