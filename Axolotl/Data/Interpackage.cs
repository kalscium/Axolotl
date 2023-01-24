using System.Reflection;

namespace data
{
    public class Interpackage {
        public User user {get;}
        public List<string> cmds {get;}
        public List<string> switches {get;}
        public List<dynamic> values {get;}
        public List<byte> order {get;}
        public string sign {get;}

        public Interpackage(User user, string password, Argument arg) {
            this.user = user;
            this.cmds = arg.cmds;

            this.switches = arg.switches;
            this.values = arg.values;
            this.order = arg.order;

            this.sign = Interpackage.getSign(password, cmds[0]);
        }

        public Interpackage(List<byte> order, List<string> switches, List<dynamic> values) {
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

        public object[] compile() {
            return new object[] {
                this.cmds,
                this.switches,
                this.values,
                this.order,
                this.sign,
            };
        }

        // 'Dictionary Run' should not be used in interpackage
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

            Type result = Interpackage.dll(Path.Combine(directory, cmd));
            if (result is null) return $"[Axolotl] Error: Cmd '{cmd}' not found in {directory}";

            if (this.switches.Contains("h") || this.switches.Contains("help")) {
                object helpMenu = result.GetField("helpMenu").GetValue(null);
                System.Console.WriteLine(helpMenu.ToString());
                return null;
            }

            result.GetMethod("entry").Invoke(null, new object[] {this.compile()});

            return null;
        }

        public bool checkSwitch(params string[] swtches) {
            bool isSwitch = true;
            for (int i = 0; i < swtches.Length; i++) {
                if (!this.switches.Contains(swtches[i])) isSwitch = false;
            } return isSwitch;
        }

        public int getSwitch(string swtch) {
            if (!this.checkSwitch(swtch)) return -1;
            int idx = this.switches.IndexOf(swtch);
            return this.order[idx];
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

        public static string getSign(string password, string cmd) => encryption.PassHashing.hash($"{password}.{cmd}")[0];

        public static List<T2> listTo<T, T2>(List<T> list) {
            List<T2> result = new List<T2>();
            for (int i = 0; i < list.Count; i++) {
                result.Add((dynamic) list[i]);
            } return result;
        }
    }
}