namespace data
{
    public class Password {
        public string hash;
        public string salt;

        public Password(string hash, string salt) {
            this.hash = hash;
            this.salt = salt;
        }

        public Password(string password) {
            string[] result = encryption.PassHashing.hash(password);
            this.hash = result[0];
            this.salt = result[1];
        }

        public Password(SeraDB.Entry entry) {}

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

        public object SeraDB {
            get {
                return new object[] {
                    this.hash,
                    this.salt,
                };
            } set {
                object[] list = (object[]) value;
                this.hash = (string) list[0];
                this.salt = (string) list[1];
            }
        }
    }
}