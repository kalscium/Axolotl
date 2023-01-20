using System.Text;

namespace front
{
    public class Cli {
        public List<StringBuilder> text;
        public int[] pos = {2, 0};
        public int offset = 0;
        public List<int[]> map;

        public Cli(string str) {
            string[] lines = str.Split('\n');
            this.text = new List<StringBuilder>();

            for (int i = 0; i < lines.Length; i++) {
                this.text.Add(new StringBuilder(lines[i]));
            } this.map = Print.print(this.text, offset);

            Move.go(this);
            while (true) this.split();
        }

        public void split() {
            ConsoleKeyInfo key = Console.ReadKey(true);
            byte suc = key.Key switch {
                ConsoleKey i when new ConsoleKey[] {ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow}.Contains(i) => Move.move(i, this),
                ConsoleKey.CrSel => 0,
                _ => Edit.edit(this, key.KeyChar),
            };
        }

        public static class Edit {
            public static byte edit(Cli cli, char key) {
                store(cli, key);
                cli.map[cli.pos[1]][0]++;

                console(cli, key);
                cli.pos[0]++;

                Move.go(cli);
                return 0;
            }

            private static void store(Cli cli, char key) {
                int line = cli.map[cli.pos[1]][1];
                int idx = cli.map[cli.pos[1]][2] + cli.pos[0] - 2 - cli.map[cli.pos[1]][3];
                cli.text[line].Insert(idx, key);
                File.AppendAllText("lines.log", cli.text[line].ToString() + "\n");
            }

            private static void console(Cli cli, char key) {
                Console.SetCursorPosition(2, cli.pos[1]);
                int line = cli.map[cli.pos[1]][1];
                int idx = cli.map[cli.pos[1]][2];
                string text = new string(cli.text[line].ToString().Skip(idx).ToArray());

                Console.Write(text);

                // int line = cli.map[cli.pos[1]][1];
                // int idx = cli.map[cli.pos[1]][2] + cli.pos[0] - 2;
                // int cap = cli.map[cli.pos[1]][0];

                // for (int i = idx; i < cap; i++) {
                //     Console.Write(cli.text[line][i]);
                // }
            }
        }

        public static class Move {
            public static byte move(ConsoleKey key, Cli cli) {
                if (key == ConsoleKey.UpArrow) safeGo(1, false, cli);
                else if (key == ConsoleKey.DownArrow) safeGo(-1, false, cli);
                else if (key == ConsoleKey.RightArrow) safeGo(1, true, cli);
                else if (key == ConsoleKey.LeftArrow) safeGo(-1, true, cli);
                return 0;
            }

            public static void go(Cli cli, int i=0) => Console.SetCursorPosition(cli.pos[0], cli.pos[1]);

            public static void safeGo(int change, bool isX, Cli cli) {
                if (isX) cli.pos[0] += change;
                else cli.pos[1] -= change;
                mkValid(cli);
                go(cli);
            }

            public static void mkValid(Cli cli) {
                if (cli.pos[1] < 0) if (cli.offset > 0) {cli.offset--; cli.map = Print.print(cli.text, cli.offset); cli.pos[1] = 0;}
                else cli.pos[1] = 0;
                else if (cli.pos[1] >= cli.map.Count) if (cli.offset < cli.map.Count - 1) {cli.offset++; cli.map = Print.print(cli.text, cli.offset); cli.pos[1] = cli.map.Count - 1;}
                else cli.pos[1] = cli.map.Count - 1;

                if (cli.pos[0] < 2) cli.pos[0] = 2;
                else if (cli.pos[0] >= cli.map[cli.pos[1]][0] + 3) cli.pos[0] = cli.map[cli.pos[1]][0] + 2;
                else if (cli.pos[0] >= Console.WindowWidth) cli.pos[0] = Console.WindowWidth - 1;
            }

            public static bool isValid(int pos, bool isX, Cli cli) {
                if (isX) {
                    if (pos < 2) return false;
                    if (pos >= cli.map[cli.pos[1]][0] + 3) return false;
                    if (pos >= Console.WindowWidth) return false;
                } else {
                    if (pos < 0) return false;
                    if (pos >= cli.map.Count) return false;
                    if (pos >= Console.WindowHeight) return false;
                } return true;
            }
        }
    }
}