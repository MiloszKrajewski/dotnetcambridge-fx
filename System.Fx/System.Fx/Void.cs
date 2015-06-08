namespace System.Fx
{
	public sealed class Void
	{
		public static readonly Void Instance = new Void();

		private Void() { }

		public override string ToString() { return "Void"; }
		public override bool Equals(object obj) { return obj is Void; }
		public override int GetHashCode() { return 0; }
	}
}
