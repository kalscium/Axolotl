namespace cd
{
    public class Cd {
        public static void Main(string[] args) {
            if (args.Length < 1) return;
            try {Directory.SetCurrentDirectory(args[0]);}
            catch {System.Console.WriteLine($"[Change Directory] Error: Directory \"{Path.Combine(Directory.GetCurrentDirectory(), args[0])}\" not found!");}
        }
    }
}

namespace axolotl
{
    public class Cd {
        public static string helpMenu = new axoapi.HelpMenu(
            "cd",
            "Used to change the current working directory",
            new string[0],
            new string[0],
            new string[] {
                "cd directory",
            }, new axoapi.HelpMenu[0],
            new string[0]
        ).ToString();

        public static void entry(object[] interPackage) {
            axoapi.Intrapackage interpackage = new axoapi.Intrapackage(interPackage);
            if (interpackage.values.Count < 1) return;
            try {Directory.SetCurrentDirectory(interpackage.values[0]);}
            catch {System.Console.WriteLine($"[Change Directory] Error: Directory \"{Path.Combine(Directory.GetCurrentDirectory(), interpackage.values[0])}\" not found!");}
        }
    }
}