using SeraLib;

namespace data
{
    public class Compiler {
        public static List<Type> mapped = new List<Type>() {
            typeof(User),
            typeof(Password),
            typeof(InterPackage),
            typeof(UserCol),
        };

        public static void compile(string loc, dynamic data) {
            string result = data.seralib().compile();
            SeraBall.flush();
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