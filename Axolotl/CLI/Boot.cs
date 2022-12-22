namespace cli
{
    public class Boot {
        public string env;

        public Boot(string env) {
            System.Console.WriteLine(Cli.banner);
            System.Console.WriteLine("\n=== Axolotl Booter ===");
            this.env = env;
        }

        public void boot() {
            log("Booting Axolotl...");
            log($"Loaded Version [{Cli.version}]!");
            this.mkvital();
            log("Tip: Hold 'u' during boot to create extra user!");
            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.U) new Install(env).createUsr(); 
            log("Booted Axolotl!");
            System.Console.WriteLine();
        }

        public void mkvital() {
            if (!Directory.Exists(this.env)) {
                log($"Error: Enviornment [{this.env}] not found!");
                Environment.Exit(1);
            } else log($"Loaded Enviornment [{this.env}]!");

            if (!File.Exists(Path.Combine(this.env, "Axolotl.init"))) {
                log("Warning: Axolotl initialisation file not found!");
                log("Creating Axolotl initialisation file...");
                File.WriteAllText(Path.Combine(this.env, "Axolotl.init"), "Axolotl by Greenchild 2022");
            } log("Found Axolotl initialisation file!");

            if (!Directory.Exists(Path.Combine(this.env, "bin"))) {
                log("Warning: Bin directory not found!");
                log("Creating Bin directory...");
                Directory.CreateDirectory(Path.Combine(this.env, "bin"));
            } log("Loaded Bin Directory!");

            if (!File.Exists(Path.Combine(this.env, "usr.slo"))) {
                log("Error: No users found!");
                log("Performing installation...");
                Install install = new Install(this.env);
                install.createUsrCol();
                install.createUsr();
                Environment.Exit(0);
            }

            try {
                data.Compiler.parse(Path.Combine(this.env, "usr.slo"));
                log("Loaded avalible users!");
            } catch {
                log("Error: Users corrupt!");
                log("Performing installation...");
                Install install = new Install(this.env);
                install.createUsrCol();
                install.createUsr();
                Environment.Exit(0);
            }
        }

        /////////////////////
        // Static Stuff
        /////////////////////

        public static void log(string log) {
            System.Console.WriteLine($"[Axolotl] {log}");
        }
    }
}