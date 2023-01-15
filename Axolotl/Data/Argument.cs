namespace data
{
    public class Argument {
        public List<string> cmds;
        public List<string> switches;
        public List<object> values;
        public List<byte> order;

        public Argument(List<byte> order, List<string> switches, List<object> values) {
            this.order = order;
            this.switches = switches;
            this.values = values;
        }
    }
}