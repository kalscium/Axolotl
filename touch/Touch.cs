namespace touch
{
    public class Touch {
        public static void Main(string[] args) {
            for (int i = 0; i < args.Length; i++) {
                File.Create(args[i]);
            }
        }
    }
}

namespace axolotl
{
    public class Touch {
        public static string helpMenu = new axoapi.HelpMenu(
            "touch",
            "Used to create empty files",
            new string[0],
            new string[0],
            new string[] {
                "touch file1",
                "touch file1 file2 file3",
            }, new axoapi.HelpMenu[0],
            new string[0]
        ).ToString();

        public static void entry(object[] interPackage) {
            axoapi.Intrapackage interpackage = new axoapi.Intrapackage(interPackage);
            for (int i = 0; i < interpackage.values.Count; i++) {
                File.Create(interpackage.values[i]);
            }
        }
    }
}