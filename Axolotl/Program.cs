using cli;

namespace data
{
    public class Program {
        public static void Main(string[] args) {
            new cli.Boot(args[0]).boot();
            new cli.Cli(args[0]).run();
            // while (true) {
            //     run();
            // }
            // readify();
            // UserCol userCol = Compiler.parse("usr.slo");
            // System.Console.WriteLine(userCol.list.Count);
        }

        public static int run() {
            Console.Write("Axolotl> ");
            string input = Console.ReadLine();

            parser.Lexer lexer = new parser.Lexer("<stdin>", input);
            parser.LexResult lres = lexer.makeTokens();
            if (lres.error is not null) {System.Console.WriteLine(lres.error.repr()); return 1;}

            parser.Parser parse = new parser.Parser(lres.tok);
            parser.ParseResult pres = parse.parse();
            if (pres.error is not null) {System.Console.WriteLine(pres.error.repr()); return 1;}

            parser.RTResult ires = parser.Interpreter.visit(pres.node);

            return 0;
        }

        public static void readify() {
            Console.Write("FileLoc: ");
            string input = Console.ReadLine();
            string file = File.ReadAllText(input);
            string output = "";
            for (int i = 0; i < file.Length; i++) {
                char current = file[i];
                try {output += SeraLib.Symbols.toString(current);}
                catch {output += current;}
            } System.Console.WriteLine(output);
        }
    }
}