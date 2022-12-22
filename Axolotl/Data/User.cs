namespace data
{
    public class User {
        public string username;
        public Password password;

        public string fullName;
        public byte age;

        public User() {}

        /////////////////////
        // SeraLib Stuff
        /////////////////////

        public static uint address = 1001;

        public User(SeraLib.SeraData data) {
            this.username = data.data[0];
            this.password = data.data[1];
            this.fullName = data.data[2];
            this.age = data.data[3];
        }

        public SeraLib.SeraBall seralib() {
            return new SeraLib.SeraBall(this) {
                this.username,
                this.password,
                this.fullName,
                this.age,
            };
        }
    }

    public class UserCol {
        public List<dynamic> list = new List<dynamic>();

        public UserCol() {}

        public User getusr(string username, string password) {
            for (int i = 0; i < this.list.Count; i++) {
                if (username != this.list[i].username) continue;
                if (!this.list[i].password.verify(password)) continue;
                return this.list[i];
            } return null;
        }

        public static uint address = 1004;

        public UserCol(SeraLib.SeraData data) {
            this.list = data.data[0];
        }

        public SeraLib.SeraBall seralib() {
            return new SeraLib.SeraBall(this) {
                this.list,
            };
        }
    }
}