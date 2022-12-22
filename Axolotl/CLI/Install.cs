namespace cli
{
    public class Install {
        public string env;

        public Install(string env) {
            System.Console.WriteLine("\n=== Axolotl Installer ===");
            this.env = env;
        }

        ~Install() {
            log("Installation Finished!");
        }

        // Creating Users

        public void createUsrCol() {
            log("Creating User Collection...");
            data.UserCol userCol = new data.UserCol();
            data.Compiler.compile(Path.Combine(this.env, "usr.slo"), userCol);
            log("Created User Collection!");
        }

        public void createUsr() {
            log("Creating User...");
            CreateUsr createUsr = new CreateUsr();
            createUsr.gather();
            data.UserCol userCol = data.Compiler.parse(Path.Combine(this.env, "usr.slo"));
            userCol.list.Add(createUsr.user);
            data.Compiler.compile(Path.Combine(this.env, "usr.slo"), userCol);
            log("Created User!");
        }

        class CreateUsr {
            public data.User user = new data.User();

            public void gather() {
                System.Console.WriteLine();
                log("Enter your information below!");

                this.user.fullName = ask("Full Name");
                if (!byte.TryParse(ask("Age"), out this.user.age)) this.user.age = 0;
                this.user.username = ask("Username");
                this.user.password = getPassword();
            }

            public data.Password getPassword() {
                System.Console.WriteLine();
                string pass = data.Password.passAsk("Password");
                string confirm = data.Password.passAsk("Password Confirm");

                if (pass != confirm) {
                    log("Error: Passwords do not match!");
                    return this.getPassword();
                } return new data.Password(confirm);
            }

            public static string ask(string log) {
                Console.Write($"[User Creation] {log}: ");
                return Console.ReadLine();
            }
        }

        /////////////////////
        // Static Stuff
        /////////////////////

        public static void log(string log) {
            System.Console.WriteLine($"[Axoltol Installer] {log}");
        }
    }
}