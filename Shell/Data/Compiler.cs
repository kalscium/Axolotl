using SeraLib;

namespace data
{
    public class Compiler {
        public static Dictionary<uint, Type> mapped = new Dictionary<uint, Type>() {
            {User.address, typeof(User)},
            {Password.address, typeof(Password)},
            {InterPackage.address, typeof(InterPackage)},
        };

        public static void compile(string loc, dynamic data) {
            string result = data.seralib().compile();
            File.WriteAllText(loc, result);
        }

        public static dynamic parse(string loc) {
            string text = File.ReadAllText(loc);
            Parser parser = new Parser(text, Compiler.mapped);
            return parser.parse();
        }
    }
}