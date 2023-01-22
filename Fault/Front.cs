using System.Text;

namespace front
{
    public class Front {
        public List<StringBuilder> text;
        public int[] pos = {2, 0};
        public int offset = 0;
        public List<int[]> map;
        public string filename = "null";

        public Front(string str) {
            string[] lines = str.Split('\n');
            this.text = new List<StringBuilder>();

            for (int i = 0; i < lines.Length; i++) {
                this.text.Add(new StringBuilder(lines[i]));
            } this.map = Print.print(this.text, offset);

            Move.go(this);
            while (true) this.split();
        }

        public Front(string filename, bool exists) {
            this.filename = filename;
            if (exists)this.text = back.Store.load(filename, "Entry");
            else {
                back.Store.init(filename);
                this.text = new List<StringBuilder>() {new StringBuilder()};
            } this.map = Print.print(this.text, offset);

            Move.go(this);
            while (true) this.split();
        }

        public void split() {
            ConsoleKeyInfo key = Console.ReadKey(true);
            byte suc = key.Key switch {
                ConsoleKey i when new ConsoleKey[] {ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow}.Contains(i) => Move.move(key, this),
                ConsoleKey i when key.Modifiers == ConsoleModifiers.Control && i == ConsoleKey.S => back.Store.store(this.filename, "Entry", this.text),
                ConsoleKey.Backspace => Edit.backspace(this),
                ConsoleKey.Enter => Edit.enter(this),
                _ => Edit.edit(this, key.KeyChar),
            };
        }

        public static class Edit {
            public static byte edit(Front cli, char key) {
                if (store(cli, key) == 1) return 0;
                cli.map[cli.pos[1]][0]++;

                console(cli);
                cli.pos[0]++;

                Move.go(cli);
                return 0;
            }

            public static byte backspace(Front cli) {
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

            public static byte renter(Front cli) {
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

            public static byte enter(Front cli) {
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

            private static byte store(Front cli, char key) {
                (int line, int idx) = point(cli);
                cli.text[line].Insert(idx, key);
                if (cli.map[cli.pos[1]][0] > Console.WindowWidth - 2) {
                    cli.map = Print.print(cli.text, cli.offset);
                    Move.safeGo(1, true, cli);
                    return 1;
                } return 0;
            }

            private static (int, int) point(Front cli) {
                int line = cli.map[cli.pos[1]][1];
                int idx = cli.map[cli.pos[1]][2] + cli.pos[0] - 2;
                return (line, idx);
            } 

            private static void console(Front cli) {
                Console.BackgroundColor = Print.background;
                Console.ForegroundColor = Print.foreground;
                (int line, int idx) = point(cli);

                for (int i = idx; i < cli.text[line].Length; i++) {
                    Console.Write(cli.text[line][i]);
                }
            }
        }

        public static class Move {
            public static byte move(ConsoleKeyInfo key, Front cli) {
                int booster = key.Modifiers == ConsoleModifiers.Control ? 1: 0;

                if (key.Key == ConsoleKey.UpArrow) safeGo(1 + booster, false, cli);
                else if (key.Key == ConsoleKey.DownArrow) safeGo(-1 - booster, false, cli);
                else if (key.Key == ConsoleKey.RightArrow) safeGo(1 + booster, true, cli);
                else if (key.Key == ConsoleKey.LeftArrow) safeGo(-1 - booster, true, cli);
                return 0;
            }

            public static void go(Front cli, int i=0) => Console.SetCursorPosition(cli.pos[0], cli.pos[1]);

            public static void safeGo(int change, bool isX, Front cli) {
                if (isX) cli.pos[0] += change;
                else cli.pos[1] -= change;
                mkValid(cli);
                go(cli);
            }

            public static void mkValid(Front cli) {
                if (cli.pos[1] < 0) if (cli.offset > 0) {cli.offset--; cli.map = Print.print(cli.text, cli.offset); cli.pos[1] = 0;}
                else cli.pos[1] = 0;
                else if (cli.pos[1] >= cli.map.Count) if (cli.offset < cli.text.Count - 1) {cli.offset++; cli.map = Print.print(cli.text, cli.offset); cli.pos[1] = cli.map.Count - 1;}
                else cli.pos[1] = cli.map.Count - 1;

                if (cli.pos[0] < 2 && cli.map[cli.pos[1]][2] > 0) {cli.pos[0] = 2; safeGo(1, false, cli); cli.pos[0] = cli.map[cli.pos[1]][0];}
                else if (cli.pos[0] < 2 && cli.map[cli.pos[1]][1] != 0) {cli.pos[0] = Console.WindowWidth; safeGo(1, false, cli);}
                else if (cli.pos[0] < 2) cli.pos[0] = 2;
                else if (cli.pos[0] == cli.map[cli.pos[1]][0] + 3 && !(cli.pos[1] == cli.map.Count - 1 && cli.map[cli.pos[1]][1] != cli.map.Count - 1)) {cli.pos[0] = 2; safeGo(-1, false, cli);}
                else if (cli.pos[0] >= cli.map[cli.pos[1]][0] + 3) cli.pos[0] = cli.map[cli.pos[1]][0] + 2;
                else if (cli.pos[0] == Console.WindowWidth && !(cli.pos[1] == cli.map.Count - 1 && cli.map[cli.pos[1]][1] != cli.map.Count - 1)) {cli.pos[0] = 2; safeGo(-1, false, cli);}
                else if (cli.pos[0] >= Console.WindowWidth) cli.pos[0] = Console.WindowWidth - 1;
            }

            public static bool isValid(int pos, bool isX, Front cli) {
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

    public class Print {
        public static ConsoleColor background = ConsoleColor.Black;
        public static ConsoleColor foreground = ConsoleColor.Gray;
        public string text;

        public Print(string text) {
            this.text = text;
        }

        public static List<int[]> print(List<System.Text.StringBuilder> text, int offset) {
            Console.BackgroundColor = background;
            Console.Clear();
            int x = Console.WindowWidth;
            int y = Console.WindowHeight - 1;
            List<int[]> lines = new List<int[]>();
            int ln = 0;

            for (int i = offset; ln < y && i < text.Count; i++) {
                string[] result = line(text[i].ToString(), ref ln, x - 2, i, ref lines, 0).Split('\n');

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{Base85Encode(i + 1)}\u2502");
                Console.ForegroundColor = foreground;
                System.Console.WriteLine(result[0]);

                for (int a = 1; a < result.Length; a++) {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("-\u2502");
                    Console.ForegroundColor = foreground;
                    System.Console.WriteLine(result[a]);
                }
            }

            while (ln < y) {
                ln++;
                Console.BackgroundColor = background;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                System.Console.WriteLine('~');
                Console.ResetColor();
            }
            
            return lines;
        }

        private static string line(string str, ref int ln, int x, int i, ref List<int[]> lines, int idx, bool num=true) {
            ln++;
            lines.Add(new int[3]);
            int lin = lines.Count - 1; // Replacement word for line

            string result = "";
            bool longer = false;

            if (str.Length <= x) result = str;
            else {
                result = $"{new string(str.ToCharArray(0, x))}\n";
                longer = true;
            }
            
            lines[lin][0] = result.Length;
            lines[lin][1] = i;
            lines[lin][2] = idx;
            idx += result.Length - 1;

            if (longer) result += line(new string(str.Skip(x).ToArray()), ref ln, x, i, ref lines, idx, false);

            return result;
        }

        private static string Base85Encode(int num)
        {
            char[] base85chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!#$%&()*+-;<=>?@^_`{|}~".ToCharArray();
            var base85 = new char[5];
            int i = 4;
            while (num > 0)
            {
                int remainder = num % 85;
                base85[i--] = base85chars[remainder];
                num = num / 85;
            }
            return new string(base85);
        }
    }
}