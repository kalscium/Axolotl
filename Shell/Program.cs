using parser;

namespace data
{
    public class Program {
        public static void Main(string[] args) {
            System.Console.WriteLine(cli.Cli.banner);
            new cli.Boot("/media/greenchild/Gata/Ethan/Axolotl/").boot();
            // while (true) {
            //     run();
            // }
        }

        public static int run() {
            Console.Write("Axolotl> ");
            string input = Console.ReadLine();

            Lexer lexer = new Lexer("<stdin>", input);
            LexResult lres = lexer.makeTokens();
            if (lres.error is not null) {System.Console.WriteLine(lres.error.repr()); return 1;}

            Parser parser = new Parser(lres.tok);
            ParseResult pres = parser.parse();
            if (pres.error is not null) {System.Console.WriteLine(pres.error.repr()); return 1;}

            RTResult ires = Interpreter.visit(pres.node);

            return 0;
        }
    }
}