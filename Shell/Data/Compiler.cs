using SeraLib;

namespace data
{
    public class Compiler {
        public static List<Type> mapped = new List<Type>() {
            typeof(User),
            typeof(Password),
        };

        public static void compile(string loc, dynamic data) {
            string result = data.seralib().compile();
            File.WriteAllText(loc, result);
        }

        public static dynamic parse(string loc) {
            string text = File.ReadAllText(loc);
            Parser parser = new Parser(text, Compiler.mapftype());
            return parser.parse();
        }

        public static Dictionary<uint, Type> mapftype() {
            Dictionary<uint, Type> mapped = new Dictionary<uint, Type>();
            for (int i = 0; i < Compiler.mapped.Count; i++) {
                uint address = (uint) Compiler.mapped[i].GetField("address").GetValue(null);
                mapped[address] = Compiler.mapped[i];
            } return mapped;
        }
    }
}