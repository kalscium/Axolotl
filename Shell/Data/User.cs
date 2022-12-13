namespace data
{
    public class User {
        public string username;
        public Password password;

        public User(string username, Password password) {
            this.username = username;
            this.password = password;
        }

        /////////////////////
        // SeraLib Stuff
        /////////////////////

        public static uint address = 1001;

        public User(SeraLib.SeraData data) {
            this.username = data.data[0];
            this.password = data.data[1];
        }

        public SeraLib.SeraBall seralib() {
            return new SeraLib.SeraBall(User.address) {this.username, new SeraLib.SeraData(this.password)};
        }
    }
}