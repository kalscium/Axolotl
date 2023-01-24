using axoapi;

namespace axolotl
{
    public class Program {
        public static string helpMenu = new HelpMenu(
            "fault",
            "Used to store text logs within a secure encrypted database. Or in other words, a secure digital diary.",
            new string[] {"e"},
            new string[] {"Ejects the entries as an unencrypted directory, for reading and or viewing"},
            new string[] {
                "fault <database loc> <(optional) title> <(optional) description>",
                "fault \"Diary.sldb\" \"The Incident\" \"A incident happened with dave today.\"",
                "fault -e \"Diary.sldb\""
            }, new HelpMenu[0],
            new string[0]
        ).ToString();

        public static void Main(string[] args) {
            new back.Wrapper("Diary.sldb").edit();
        }

        public static void entry(object[] axo) {
            Intrapackage interpackage = new Intrapackage(axo);
            back.Entry.sign = interpackage.sign;

            back.Wrapper wrapper = null;

            if (interpackage.values.Count == 0) {
                Console.WriteLine("[Fault] Error: DataBase location required");
                return;
            } else if (interpackage.values.Count == 1) {
                wrapper = new back.Wrapper(interpackage.values[0]);
            } else if (interpackage.values.Count == 2) {
                wrapper = new back.Wrapper(interpackage.values[0], interpackage.values[1]);
            } else if (interpackage.values.Count == 3) { 
                wrapper = new back.Wrapper(interpackage.values[0], interpackage.values[1], interpackage.values[2]);
            }

            wrapper.edit();
        }
    }
}