using System.Reflection;

namespace data
{
    public class InterPackage {
        public User user;
        public List<string> cmds;
        public List<string> switches;
        public List<dynamic> values;
        public List<UInt16> order;
        public string sign;

        public InterPackage(List<UInt16> order, List<string> switches, List<dynamic> values) {
            this.switches = switches;
            this.values = values;
            this.order = order;
        }

        public string advance() {
            if (this.cmds.Count == 0) return "";
            string cmd = this.cmds[0];
            this.cmds.RemoveAt(0);
            return cmd;
        }

        public string run(Dictionary<string, Type> dict) {
            if (this.cmds.Count == 0) return null;
            string cmd = this.advance();

            if (!dict.ContainsKey(cmd))
                return $"[Axolotl] Error: Cmd '{cmd}' not found within program";

            Type type = dict[cmd];
            type.GetMethod("entry").Invoke(null, new object[] {this});

            return null;
        }

        public string run(string directory) {
            if (this.cmds.Count == 0) return null;
            string cmd = this.advance();

            Directory.SetCurrentDirectory(directory);
            Type result = InterPackage.dll(cmd);
            if (result is null) return $"[Axolotl] Error: Cmd '{cmd}' not found in {directory}";

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
            } throw new Exception("[Axolotl Bin] Error: Cannot find entry point for program");
        }

        public static string getSign(User user, string password, string cmd) {
            string compUsr = user.seralib().compile();
            string contents = new SeraLib.SeraBall(0) {compUsr, cmd}.compile();
            string result = encryption.TwoWay.Encrypt(contents, password);

            return result;
        }
    }
}