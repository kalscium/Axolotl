namespace parser
{
    // Lexer Class
    public class Lexer {
        // Class
        public static string DIGITS = "0123456789";
        public static string LETTERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
        public static string LETTERS_DIGITS = Lexer.DIGITS + Lexer.LETTERS;

        // Object
        public string text;
        public Position pos;
        public string currentChar;
        public LexResult res = new LexResult();

        public Lexer(string fn, string text) {
            this.text = text;
            this.pos = new Position(-1, 0, -1, fn, text);
            this.currentChar = null;
            this.advance();
        }

        public void advance() {
            this.pos.advance(this.currentChar);
            this.currentChar = this.pos.idx < this.text.Length ? this.text[this.pos.idx].ToString(): null;
        }

        public LexResult makeTokens() {
            return null;
        }
    }
}