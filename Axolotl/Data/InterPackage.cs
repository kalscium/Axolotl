using System.Reflection;

namespace data
{
    public class InterPackage {
        public User user;
        public List<string> cmds;
        public List<string> switches;
        public List<dynamic> values;
        public List<ushort> order;
        public string sign;

        public InterPackage(User user, string password, Argument arg) {
            this.user = user;
            this.cmds = arg.cmds;

            this.switches = arg.switches;
            this.values = arg.values;
            this.order = arg.order;

            this.sign = InterPackage.getSign(this.user, password, cmds[0]);
        }

        public InterPackage(List<ushort> order, List<string> switches, List<dynamic> values) {
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

            Type result = InterPackage.dll(Path.Combine(directory, cmd));
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
            string contents = new SeraLib.SeraGenerics.List(new List<dynamic> {compUsr, cmd}).seralib().compile();
            string result = encryption.TwoWay.Encrypt(contents, password);

            return result;
        }

        public static List<T2> listTo<T, T2>(List<T> list) {
            List<T2> result = new List<T2>();
            for (int i = 0; i < list.Count; i++) {
                result.Add((dynamic) list[i]);
            } return result;
        }

        /////////////////////
        // SeraLib Stuff
        /////////////////////

        public static uint address = 1003;

        public InterPackage(SeraLib.SeraData data) {
            this.user = data.data[0];
            this.cmds = listTo<dynamic, string>(data.data[1]);
            this.switches = listTo<dynamic, string>(data.data[2]);
            this.values = data.data[3];
            this.order = listTo<dynamic, ushort>(data.data[4]);
            this.sign = data.data[5];
        }

        public SeraLib.SeraBall seralib() {
            return new SeraLib.SeraBall(this) {
                this.user,
                listTo<string, dynamic>(this.cmds),
                listTo<string, dynamic>(this.switches),
                this.values,
                listTo<ushort, dynamic>(this.order),
                this.sign,
            };
        }
    }
}