namespace cli
{
    public class Boot {
        public string env;

        public Boot(string env) {
            System.Console.WriteLine("\n=== Axolotl Booter ===");
            this.env = env;
        }

        public void boot() {
            log("Booting Axolotl...");
            log($"Loaded Version [{Cli.version}]!");
            this.mkvital();
            log("Booted Axolotl!");
        }

        public void mkvital() {
            if (!Directory.Exists(this.env)) {
                log($"Error: Enviornment [{this.env}] not found!");
                Environment.Exit(1);
            } else log($"Loaded Enviornment [{this.env}]!");

            if (!Directory.Exists(this.env + "bin")) {
                log("Warning: Bin folder not found!");
                log("Creating bin folder...");
                Directory.CreateDirectory(this.env + "bin");
            } log("Loaded Bin Folder!");

            if (!File.Exists($"{this.env}bin/usr.slo")) {
                log("Error: No users found!");
                log("Performing installation...");
                new Install(this.env).createUsr();
                Environment.Exit(0);
            } else log("Loaded avalible users!");
        }

        /////////////////////
        // Static Stuff
        /////////////////////

        public static void log(string log) {
            System.Console.WriteLine($"[Axolotl] {log}");
        }
    }
}