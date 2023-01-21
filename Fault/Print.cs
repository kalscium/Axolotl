namespace front
{
    public class Print {
        public string text;

        public Print(string text) {
            this.text = text;
        }

        public static List<int[]> print(List<System.Text.StringBuilder> text, int offset) {
            Console.Clear();
            int x = Console.WindowWidth;
            int y = Console.WindowHeight - 1;
            List<int[]> lines = new List<int[]>();
            int ln = 0;

            for (int i = offset; ln < y && i < text.Count; i++) {
                string[] result = line(text[i].ToString(), ref ln, x - 2, i, ref lines, 0).Split('\n');

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{Base64Encode(i + 1)}|");
                Console.ResetColor();
                System.Console.WriteLine(result[0]);

                for (int a = 1; a < result.Length; a++) {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(" |");
                    Console.ResetColor();
                    System.Console.WriteLine(result[a]);
                }
            }

            while (ln < y) {
                ln++;
                Console.ForegroundColor = ConsoleColor.Blue;
                System.Console.WriteLine('~');
                Console.ResetColor();
            }
            
            return lines;
        }

        private static string line(string str, ref int ln, int x, int i, ref List<int[]> lines, int idx, bool num=true) {
            ln++;
            x -= 2;
            lines.Add(new int[3]);
            int lin = lines.Count - 1; // Replacement word for line

            string result = "";
            bool longer = false;

            if (str.Length <= x + 2) result = str;
            else {
                result = $"{new string(str.ToCharArray(0, x + 2))}\n";
                longer = true;
            }
            
            lines[lin][0] = result.Length;
            lines[lin][1] = i;
            lines[lin][2] = idx;
            idx += result.Length - 1;

            if (longer) result += line(new string(str.Skip(x + 2).ToArray()), ref ln, x + 2, i, ref lines, idx, false);

            return result;
        }

        private static string Base64Encode(int num)
        {
            char[] base64chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz+/".ToCharArray();
            var base64 = new char[(int)Math.Ceiling(Math.Log(num + 1, 64))];
            int i = base64.Length - 1;
            while (num > 0)
            {
                int remainder = num & 0x3F;
                base64[i--] = base64chars[remainder];
                num = num >> 6;
            }
            return new string(base64);
        }
    }
}