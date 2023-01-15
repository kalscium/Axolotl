using cli;

namespace data
{
    public class Program {
        public static void Main(string[] args) {
            new cli.Boot(args[0]).boot();
            new cli.Cli(args[0]).run();
        }
    }
}