namespace data
{
    public class Password {
        public string hash {get;}
        public string salt {get;}

        public Password(string hash, string salt) {
            this.hash = hash;
            this.salt = salt;
        }

        public Password(string password) {
            string[] result = encryption.PassHashing.hash(password);
            this.hash = result[0];
            this.salt = result[1];
        }

        public bool verify(string password) {
            string[] hash = encryption.PassHashing.hash(password, Convert.FromBase64String(this.salt));
            return hash[0] == this.hash;
        }

        public static string passAsk(string log) {
            Console.Write($"[{log}]: ");
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter) {
                if (info.Key != ConsoleKey.Backspace) {
                    password += info.KeyChar;
                    Console.Write("#");
                    info = Console.ReadKey(true);
                } else if (info.Key == ConsoleKey.Backspace) {
                    if (!string.IsNullOrEmpty(password)) {
                        password = password.Substring
                        (0, password.Length - 1);
                        Console.Write("\b \b");
                    } info = Console.ReadKey(true);
                }
            } System.Console.WriteLine();

            return password;
        }

        /////////////////////
        // SeraLib Stuff
        /////////////////////

        public static uint address = 1002;

        public Password(SeraLib.SeraData data) {
            this.hash = data.data[0];
            this.salt = data.data[1];
        }

        public SeraLib.SeraBall seralib() {
            return new SeraLib.SeraBall(this) {
                this.hash,
                this.salt,
            };
        }
    }
}