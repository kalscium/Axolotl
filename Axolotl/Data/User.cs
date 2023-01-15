namespace data
{
    public class User {
        public string username;
        public Password password;

        public string fullName;
        public byte age;

        public User() {}

        public User(SeraDB.Entry entry) {}

        public object SeraDB {
            get {
                return new object[] {
                    this.username,
                    this.password,
                    this.fullName,
                    this.age,
                };
            } set {
                object[] list = (object[]) value;
                this.username = (string) list[0];
                this.password = (Password) list[1];
                this.fullName = (string) list[2];
                this.age = (byte) list[3];
            }
        }
    }

    public class UserCol {
        public SeraDB.DataBase database;

        public UserCol(string filename) {
            this.database = SeraDB.DataBase.load(filename);
        }

        public bool getusr(string username, string password, out User user) {
            try {
                user = (User) this.database.search(username);
                if (!user.password.verify(password)) return false;
                return true;
            } catch {
                user = null;
                return false;
            }
        }

        public static void init(string filename) {
            SeraDB.DataBase database = new SeraDB.DataBase(filename, new Dictionary<string, object> {
                {"__init__", "__init__"},
            });
        }
    }
}