namespace cli
{
    public class Install {
        public string env;

        public Install(string env) {
            System.Console.WriteLine("\n=== Axolotl Installer ===");
            this.env = env;
        }

        public void createUsr() {
        }

        /////////////////////
        // Static Stuff
        /////////////////////

        public static void log(string log) {
            System.Console.WriteLine($"[Axoltol Installer] {log}");
        }
    }
}