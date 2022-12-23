using SeraLib;

namespace data
{
    public class Compiler {
        public static List<Type> mapped = new List<Type>() {
            typeof(User),
            typeof(Password),
            typeof(Interpackage),
            typeof(UserCol),
        };

        public static void compile(string loc, dynamic data) {
            SeraBall.flush();
            string result = data.seralib().compile();
            File.WriteAllText(loc, result);
        }

        public static dynamic parse(string loc) {
            string text = File.ReadAllText(loc);
            Parser parser = new Parser(text, mapped);
            Parser.flush();
            return parser.parse();
        }
    }
}