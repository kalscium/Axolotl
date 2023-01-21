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
                ConsoleKey i when new ConsoleKey[] {ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow}.Contains(i) => Move.move(key, this),
                ConsoleKey i when key.Modifiers == ConsoleModifiers.Control && i == ConsoleKey.S => 0,
                ConsoleKey.Backspace => Edit.backspace(this),
                ConsoleKey.Enter => Edit.enter(this),
                _ => Edit.edit(this, key.KeyChar),
            };
        }

        public static class Edit {
            public static byte edit(Cli cli, char key) {
                if (store(cli, key) == 1) return 0;
                cli.map[cli.pos[1]][0]++;

                console(cli);
                cli.pos[0]++;

                Move.go(cli);
                return 0;
            }

            public static byte backspace(Cli cli) {
                (int line, int idx) = point(cli);
                if (idx == 0) return renter(cli);

                cli.text[line].Remove(idx - 1, 1);

                cli.pos[0]--;
                Move.go(cli);

                if (cli.map[cli.pos[1]][0] > Console.WindowWidth - 2) {
                    cli.map = Print.print(cli.text, cli.offset);
                } else {
                    console(cli);
                    Console.Write(' ');
                    cli.map[cli.pos[1]][0]--;
                } Move.go(cli);
                return 0;
            }

            public static byte renter(Cli cli) {
                int line = cli.map[cli.pos[1]][1];
                if (line == 0) return 1;
                if (cli.pos[1] == 0) {cli.offset--; cli.map = Print.print(cli.text, cli.offset); cli.pos[1]++;}

                cli.text[line - 1].Append(cli.text[line]);
                cli.text.RemoveAt(line);

                cli.pos[1]--;
                cli.pos[0] = cli.map[cli.pos[1]][0] + 2;
                cli.map = Print.print(cli.text, cli.offset);
                Move.go(cli);

                return 0;
            }

            public static byte enter(Cli cli) {
                (int line, int idx) = point(cli);

                cli.text.Insert(line + 1, new StringBuilder(cli.text[line].ToString(idx, cli.text[line].Length - idx)));
                cli.text[line].Remove(idx, cli.text[line].Length - idx);
                cli.pos[0] = cli.text[line + 1].Length;
                cli.map = Print.print(cli.text, cli.offset);

                cli.pos[0] = 2;
                Move.go(cli);
                Move.safeGo(-1, false, cli);

                return 0;
            }

            private static byte store(Cli cli, char key) {
                (int line, int idx) = point(cli);
                cli.text[line].Insert(idx, key);
                if (cli.map[cli.pos[1]][0] > Console.WindowWidth - 2) {
                    cli.map = Print.print(cli.text, cli.offset);
                    Move.safeGo(1, true, cli);
                    return 1;
                } return 0;
            }

            private static (int, int) point(Cli cli) {
                int line = cli.map[cli.pos[1]][1];
                int idx = cli.map[cli.pos[1]][2] + cli.pos[0] - 2;
                return (line, idx);
            } 

            private static void console(Cli cli) {
                (int line, int idx) = point(cli);

                for (int i = idx; i < cli.text[line].Length; i++) {
                    Console.Write(cli.text[line][i]);
                }
            }
        }

        public static class Move {
            public static byte move(ConsoleKeyInfo key, Cli cli) {
                int booster = key.Modifiers == ConsoleModifiers.Control ? 1: 0;

                if (key.Key == ConsoleKey.UpArrow) safeGo(1 + booster, false, cli);
                else if (key.Key == ConsoleKey.DownArrow) safeGo(-1 - booster, false, cli);
                else if (key.Key == ConsoleKey.RightArrow) safeGo(1 + booster, true, cli);
                else if (key.Key == ConsoleKey.LeftArrow) safeGo(-1 - booster, true, cli);
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
                else if (cli.pos[1] >= cli.map.Count) if (cli.offset < cli.text.Count - 1) {cli.offset++; cli.map = Print.print(cli.text, cli.offset); cli.pos[1] = cli.map.Count - 1;}
                else cli.pos[1] = cli.map.Count - 1;

                if (cli.pos[0] < 2 && cli.map[cli.pos[1]][2] > 0) {cli.pos[0] = 2; safeGo(1, false, cli); cli.pos[0] = cli.map[cli.pos[1]][0];}
                else if (cli.pos[0] < 2 && cli.map[cli.pos[1]][1] != 0) {cli.pos[0] = Console.WindowWidth; safeGo(1, false, cli);}
                else if (cli.pos[0] < 2) cli.pos[0] = 2;
                else if (cli.pos[0] == cli.map[cli.pos[1]][0] + 3) {cli.pos[0] = 2; safeGo(-1, false, cli);}
                else if (cli.pos[0] >= cli.map[cli.pos[1]][0] + 3) cli.pos[0] = cli.map[cli.pos[1]][0] + 2;
                else if (cli.pos[0] == Console.WindowWidth) {cli.pos[0] = 2; safeGo(-1, false, cli);}
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