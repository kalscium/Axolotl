using data;

namespace cli
{
    public class Cli {   
        public static string banner = " $$$$$$\\                      $$\\            $$\\     $$\\ \n$$  __$$\\                     $$ |           $$ |    $$ |\n$$ /  $$ |$$\\   $$\\  $$$$$$\\  $$ | $$$$$$\\ $$$$$$\\   $$ |\n$$$$$$$$ |\\$$\\ $$  |$$  __$$\\ $$ |$$  __$$\\\\_$$  _|  $$ |\n$$  __$$ | \\$$$$  / $$ /  $$ |$$ |$$ /  $$ | $$ |    $$ |\n$$ |  $$ | $$  $$<  $$ |  $$ |$$ |$$ |  $$ | $$ |$$\\ $$ |\n$$ |  $$ |$$  /\\$$\\ \\$$$$$$  |$$ |\\$$$$$$  | \\$$$$  |$$ |\n\\__|  \\__|\\__/  \\__| \\______/ \\__| \\______/   \\____/ \\__|";
        public static string version = "1.0.0";

        public User user;
        public InterPackage interPackage;
        public string env;
        public string bin;

        public Cli(string env) {
            this.env = env;
            this.bin = Path.Combine(env, "bin");
        }

        public void run() {
            Directory.SetCurrentDirectory(this.env);
            string password = this.login();
            while (true) this.console(password);
        }

        public int console(string password) {
            string username = user.username;

            writeColour(ConsoleColor.Green, $"{username}@Axolotl");
            Console.Write(":");
            writeColour(ConsoleColor.Blue, this.getCwd());
            Console.Write("$ ");

            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) return 0;

            List<Argument> args = parse(input);
            if (args is null) return 1;
            
            for (int i = 0; i < args.Count; i++) {
                Argument arg = args[i];
                if (arg.cmds[0] == "exit") Environment.Exit(0);
                InterPackage interpackage = new InterPackage(this.user, password, arg);
                string output = interpackage.run(this.bin);
                if (output is not null) System.Console.WriteLine(output);
            }

            return 0;
        }

        public static void writeColour(ConsoleColor colour, string log) {
            Console.ForegroundColor = colour;
            Console.Write(log);
            Console.ResetColor();
        }

        public static List<Argument> parse(string input) {
            parser.Lexer lexer = new parser.Lexer("<stdin>", input);
            parser.LexResult lres = lexer.makeTokens();
            if (lres.error is not null) {System.Console.WriteLine(lres.error.repr()); return null;}

            parser.Parser parse = new parser.Parser(lres.tok);
            parser.ParseResult pres = parse.parse();
            if (pres.error is not null) {System.Console.WriteLine(pres.error.repr()); return null;}

            List<Argument> args = parser.Interpreter.visit_IterNode(pres.node).value;
            return args;
        }

        public string getCwd() {
            string real = Directory.GetCurrentDirectory();

            if (real == this.env) return "^";
            if (this.env.Contains(real)) return "^";

            if (real.Contains(this.env)) {
                string latter = real.Split(this.env)[1];
                latter = latter.TrimStart('/');
                latter = latter.TrimStart('\\');
                return Path.Combine(this.env, latter);
            } return real;
        }

        public string login() {
            System.Console.WriteLine("=== Login ===");
            string username = ask("Username");
            string password = Password.passAsk("Password");
            UserCol userCol = Compiler.parse(Path.Combine(this.env, "usr.slo"));
            System.Console.WriteLine();

            this.user = userCol.getusr(username, password);

            if (this.user is null) {
                log("Error: Username or password incorrect!");
                System.Console.WriteLine();
                return this.login();
            }

            log("Successfully logged in!");
            System.Console.WriteLine();

            return password;
        }

        public static string ask(string log) {
            Console.Write($"[{log}]: ");
            return Console.ReadLine();
        }

        public static void log(string log) {
            Console.WriteLine($"[Axolotl] {log}");
        }
    }
}