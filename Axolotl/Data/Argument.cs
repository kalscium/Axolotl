namespace data
{
    public class Argument {
        public List<string> cmds;
        public List<string> switches;
        public List<dynamic> values;
        public List<ushort> order;

        public Argument(List<ushort> order, List<string> switches, List<dynamic> values) {
            this.order = order;
            this.switches = switches;
            this.values = values;
        }
    }
}