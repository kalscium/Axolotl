namespace program
{
	public class Program {
		public static void Main(string[] args) {
			Console.WriteLine("main function run");
		}
	}
}

namespace axolotl
{
	public class Axolotl {
		public static void entry(dynamic interpackage) {
			Console.WriteLine("Axoltol Function run");
			Console.WriteLine($"Sign: {interpackage.sign}");
		}
	}
}
