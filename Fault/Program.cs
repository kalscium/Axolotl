using axoapi;

namespace axolotl
{
    public class Program {
        // public static string helpMenu = new HelpMenu(
        //     "fault",
        //     "Used to store text logs within a secure encrypted database.",
        // );

        public static void Main(string[] args) {
            new front.Cli("Hello!\nWelcome to Ethan's Example / Test Story to test out the machanics of this text editor that I have written in c# to act as a front end to my encrypted log taking program utilising axolotl.\n\nThis is bob.\nBob has recently had an accident.\nBob has brain damage.\nThis was bob.\nIntroducing a new and improved Dave!\nDave is much cooler than bob.\nAnd he spends his free time kicking small children and kittens.\nHe is banned from all animal shelters.\nDave's life was ended when he was shot trying to consume all the kangaroos.\nDave was named a hero.\nOh, yeah there is also Stephen.\nNo one cares about Stephen.\nThey always mistake him for his much cooler brother STEVEN.\nStephen is sad.");
        }

        public static void test() {
            char[] alpha = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};
            Console.Clear();
            int w = Console.WindowWidth;

            for (int i = 0; i < alpha.Length; i++) {
                for (int a = 0; a < w; a++) {
                    Console.Write(alpha[i]);
                }
            } Console.SetCursorPosition(0, 0);
            Console.Write('g');
            Console.ReadLine();
        }

        public static void entry(object[] axo) {
            Intrapackage interpackage = new Intrapackage(axo);
        }
    }
}