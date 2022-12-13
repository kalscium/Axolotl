using System.Reflection;
using System.Security.Cryptography;


namespace data
{
    public class InterPackage {
        public User user;
        public List<dynamic> cmds;
        public string sign;

        public InterPackage(User user, List<dynamic> cmds, string sign) {
            this.user = user;
            this.cmds = cmds;
            this.sign = sign;
        }

        public string advance() {
            if (this.cmds.Count == 0) return "";
            string cmd = this.cmds[0];
            this.cmds.Remove(0);
            return cmd;
        }

        public string run(Dictionary<string, Type> dict) {
            if (this.cmds.Count == 0) return null;
            if (!dict.ContainsKey(this.cmds[0])) return $"[Axolotl] Error: Cmd '{this.cmds[0]}' not found within program";

            Type type = dict[this.cmds[0]];
            type.GetMethod("entry").Invoke(null, new object[] {this});

            return null;
        }

        public string run(string directory) {
            if (this.cmds.Count == 0) return null;
            Type result = InterPackage.dll(directory + this.cmds[0]);
            if (result is null) return $"[Axolotl] Error: Cmd '{this.cmds[0]}' not found in !/bin";

            result.GetMethod("entry").Invoke(null, new object[] {this});

            return null;
        }

        /////////////////////
        // Static Stuff
        /////////////////////

        public static Type dll(string loc) {
            if (!File.Exists(loc)) return null;
            Assembly dll = Assembly.LoadFile(loc);

            foreach (Type type in dll.GetExportedTypes()) {
                if (type.Namespace == "axolotl") return type;
            } throw new Exception("[Axolotl Bin] Cannot find entry point for program");
        }

        public static string getSign(User user, string password, string cmd) {
            string compUsr = user.seralib().compile();
            byte[] sugar = RandomNumberGenerator.GetBytes(8);
            string contents = new SeraLib.SeraBall(InterPackage.address * 10) {compUsr, cmd, Convert.ToBase64String(sugar)}.compile();
            string result = encryption.TwoWay.Encrypt(contents, password);

            return result;
        }

        /////////////////////
        // SeraLib Stuff
        /////////////////////

        public static uint address = 1003;

        public InterPackage(SeraLib.SeraData data) {
            this.user = data.data[0];
            this.cmds = data.data[1];
            this.sign = data.data[2];
        }

        public SeraLib.SeraBall seralib() {
            return new SeraLib.SeraBall(InterPackage.address) {new SeraLib.SeraData(this.user), this.cmds, this.sign};
        }
    }
}