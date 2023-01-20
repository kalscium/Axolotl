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
                System.Console.WriteLine(line(text[i].ToString(), ref ln, x - 2, i + 1, ref lines, 0, 0));
            }

            while (ln < y) {
                ln++;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                System.Console.WriteLine('~');
                Console.ResetColor();
            }
            
            return lines;
        }

        private static string line(string str, ref int ln, int x, int i, ref List<int[]> lines, int idx, int before, bool num=true) {
            ln++;
            x -= 2;
            lines.Add(new int[4]);
            int lin = lines.Count - 1; // Replacement word for line

            if (num) {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{Base64Encode(i)} ");
                Console.ResetColor();
            }

            string result = "";
            bool longer = false;

            if (str.Length <= x) result = (num ? "": "- ") + str;
            else {
                result = (num ? "": "- ") + $"{new string(str.ToCharArray(0, x + 2))}\n";
                longer = true;
            }
            
            lines[lin][0] = num ? result.Length: result.Length - 2;
            lines[lin][1] = i - 1;
            lines[lin][2] = idx;
            lines[lin][3] = before;
            idx += lines[lin][0];
            before++;

            if (longer) result += line(new string(str.Skip(x + 2).ToArray()), ref ln, x, i, ref lines, idx, before, false);

            return result;
        }

        private static string Base64Encode(int num)
        {
            char[] base64chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz+/[]|<>!@#$%^&*()-=+".ToCharArray();
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