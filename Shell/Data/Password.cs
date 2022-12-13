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

        public bool verify(string password) {
            string[] hash = encryption.PassHashing.hash(password, Convert.FromBase64String(this.salt));
            return hash[0] == this.hash;
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
            return new SeraLib.SeraBall(Password.address) {this.hash, this.salt};
        }
    }
}