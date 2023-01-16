namespace Clear
{
    public class Clear {
        public static void Main(string[] args) {
            Console.Clear();
        }
    }
}

namespace axolotl
{
    public class Clear {
        public static string helpMenu = new axoapi.HelpMenu(
            "clear",
            "Used to clear the console",
            new string[0],
            new string[0],
            new string[] {
                "clear"
            }, new axoapi.HelpMenu[0],
            new string[0]
        ).ToString();

        public static void entry(object[] interpackage) {
            Console.Clear();
        }
    }
}