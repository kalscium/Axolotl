using axoapi;

namespace axolotl
{
    public class Program {
        public static string helpMenu = new HelpMenu(
            "fault",
            "Used to store text logs within a secure encrypted database. Or in other words, a secure digital diary.",
            new string[] {"e"},
            new string[] {"Exports the entries as an unencrypted directory, for reading and or viewing"},
            new string[] {
                "fault <database loc> <(optional) title> <(optional) description>",
                "fault \"Diary.sldb\" \"The Incident\" \"A incident happened with dave today.\"",
                "fault -e \"Diary.sldb\""
            }, new HelpMenu[0],
            new string[0]
        ).ToString();

        public static void Main(string[] args) {
            new back.Wrapper("Diary.sldb").edit();
            // new front.Front("Hello!\nWelcome to Ethan's Example / Test Story to test out the machanics of this text editor that I have written in c# to act as a front end to my encrypted log taking program utilising axolotl.\n\nThis is bob.\nBob has recently had an accident.\nBob has brain damage.\nThis was bob.\nIntroducing a new and improved Dave!\nDave is much cooler than bob.\nAnd he spends his free time kicking small children and kittens.\nHe is banned from all animal shelters.\nDave's life was ended when he was shot trying to consume all the kangaroos.\nDave was named a hero.\nOh, yeah there is also Stephen.\nNo one cares about Stephen.\nThey always mistake him for his much cooler brother STEVEN.\nStephen is sad.").run();
        }

        public static void entry(object[] axo) {
            Intrapackage interpackage = new Intrapackage(axo);
            back.Entry.sign = interpackage.sign;

            if (interpackage.values.Count == 0) {
                Console.WriteLine("[Fault] Error: DataBase location required");
                return;
            }

            if (interpackage.checkSwitch("e")) {
                SeraDB.DataBase database = back.Export.getDataBase(interpackage.values[0]);
                if (database is null) return;
                back.Export.export(database);
                return;
            }

            back.Wrapper wrapper = null;

            if (interpackage.values.Count == 1) {
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