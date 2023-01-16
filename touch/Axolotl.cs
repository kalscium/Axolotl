namespace axoapi
{
    public class Intrapackage {
        public List<string> cmds {get;}
        public List<string> switches {get;}
        public List<dynamic> values {get;}
        public List<byte> order {get;}
        public string sign {get;}

        public Intrapackage(object[] data) {
            this.cmds = (List<string>) data[0];
            this.switches = (List<string>) data[1];
            this.values = (List<dynamic>) data[2];
            this.order = (List<byte>) data[3];
            this.sign = (string) data[4];
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

        public static List<T2> listTo<T, T2>(List<T> list) {
            List<T2> result = new List<T2>();
            for (int i = 0; i < list.Count; i++) {
                result.Add((dynamic) list[i]);
            } return result;
        }
    }

    public class HelpMenu {
        public string cmd;
        public string description;
        public string[] switches;
        public string[] switchDescrpt;
        public string[] examples;
        public HelpMenu[] children;
        public string[] childrenDescrpt;

        public HelpMenu(string cmd, string description, string[] switches, string[] switchDescript, string[] examples, HelpMenu[] children, string[] childrenDescrpt) {
            this.cmd = cmd;
            this.description = description;
            this.switches = switches;
            this.switchDescrpt = switchDescript;
            this.examples = examples;
            this.children = children;
            this.childrenDescrpt = childrenDescrpt;
        }

        public string getTitle() {
            return $"=== Help Menu: The {this.cmd} command ===\nDescription: {this.description}\n";
        }

        public string getSwitches() {
            if (this.switches.Length == 0) return "";
            string result = "\n\tSwitches:\n";
            for (int i = 0; i < this.switches.Length; i++) {
                result += "\t\t" + (this.switches[i].Length > 1 ? "--": "-") + $"{this.switches[i]}: {this.switchDescrpt[i]}\n";
            } return result;
        }

        public string getUsages() {
            if (this.examples.Length == 0) return "";
            string result = "\n\tUsages:\n";
            for (int i = 0; i < this.examples.Length; i++) {
                result += $"\t\tusage: [{this.examples[i]}]\n";
            } return result;
        }

        public string getChildren() {
            if (this.children.Length == 0) return "";
            string result = "\n\tChild Commands:\n";
            for (int i = 0; i < this.children.Length; i++) {
                result += $"\t\t{this.children[i]}: {this.childrenDescrpt[i]}\n";
            } return result;
        }

        public new string ToString() {
            string result = this.getTitle();
            result += this.getSwitches();
            result += this.getUsages();
            result += this.getChildren();
            return result;
        }
    }
}