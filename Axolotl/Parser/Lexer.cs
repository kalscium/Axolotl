namespace parser
{
    // Lexer Class
    public class Lexer {
        // Static
        public static string DIGITS = "0123456789";
        public static string LETTERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
        public static string LETTERS_DIGITS = Lexer.DIGITS + Lexer.LETTERS;

        // Dynamic
        public int idx = -1;
        public string text;
        public string currentChar;
        public LexResult res = new LexResult();

        public Lexer(string fn, string text) {
            this.text = text;
            this.currentChar = null;
            this.advance();
        }

        public void advance() {
            this.idx++;
            this.currentChar = this.idx < this.text.Length ? this.text[this.idx].ToString(): null;
        }

        public LexResult makeTokens() {
            List<Token> tokens = new List<Token>();

            while (this.currentChar is not null) {
                //Error Detection
                if (this.res.error is not null) return res;

                // Skippable Characters
                if (this.currentChar is null) break;
                if (" \t\n".Contains(this.currentChar)) {
                    res.found = true;
                    this.advance();
                }

                // Single Character Toknes
                if (this.currentChar is null) break;
                Token.Lexer_CharTok(">", Token.FUNNEL, this, tokens);
                Token.Lexer_CharTok(".", Token.DOT, this, tokens);
                Token.Lexer_CharTok(";", Token.SEMI, this, tokens);

                // Generated Digit & String Tokens
                if (this.currentChar is null) break;
                this.res.register(Token.Lexer_FuncTok(Lexer.DIGITS.Contains(this.currentChar), this.makeNum, tokens, this));
                if (this.currentChar is null) break;
                this.res.register(Token.Lexer_FuncTok(Lexer.LETTERS.Contains(this.currentChar), this.makeIdentifier, tokens, this));

                // Generated Symbol Tokens
                this.res.register(Token.Lexer_FuncTok(this.currentChar == "\"", this.makeStr, tokens, this));
                this.res.register(Token.Lexer_FuncTok(this.currentChar == "-", this.makeSwtch, tokens, this));

                // Invalid Character Error
                if (!(res.found)) {
                    string currentChar = this.currentChar;
                    this.advance();

                    res.failure(new Error(
                        Error.InvalidCharError,
                        $"'{currentChar}'"
                    ));
                } res.found = false;
            }

            Token.Lexer_AppendTok(true, this, Token.EOF, tokens);
            return res.success(tokens);
        }

        // Token Making Functions
        public LexResult makeSwtch() {
            LexResult res = new LexResult();
            this.advance();

            if (this.currentChar == "-") {
                this.advance();
                return res.success(new Token(Token.DASH), new Token(Token.DASH));
            } Token tok = new Token(Token.DASH, this.currentChar);

            this.advance();
            return res.success(tok);
        }

        public LexResult makeNum() {
            LexResult res = new LexResult();
            string numStr = "";
            byte dotCount = 0;

            while (this.currentChar is not null && (Lexer.DIGITS + ".").Contains(this.currentChar)) {
                if (this.currentChar == ".") {
                    if (dotCount == 1) break;
                    dotCount ++;
                    numStr += ".";
                } else numStr += this.currentChar;
                this.advance();
            }

            if (dotCount == 0)
                try {
                    return res.success(new Token(Token.INT, int.Parse(numStr)));
                } catch {return res.success(new Token(Token.INT, long.Parse(numStr)));}
            else
                try {
                    return res.success(new Token(Token.FLOAT, float.Parse(numStr)));
                } catch { return res.success(new Token(Token.FLOAT, double.Parse(numStr)));}
        }

        public LexResult makeStr() {
            LexResult res = new LexResult();
            string str = "";
            bool escapeCharacter = false;
            this.advance();

            Dictionary<string, string> escapeCharacters = new Dictionary<string, string>() {
                {"n", "\n"},
                {"t", "\t"},
            };

            while (this.currentChar is not null && (this.currentChar != "\"" || escapeCharacter)) {
                if (escapeCharacter) {
                    str += escapeCharacters.GetValueOrDefault(this.currentChar, this.currentChar);
                    escapeCharacter = false;
                } else {
                    if (this.currentChar == "\\") escapeCharacter = true;
                    else str += this.currentChar;
                } this.advance();
            }

            if (this.currentChar != "\"") {
                return res.failure(new Error(
                    Error.ExpectedCharError,
                    ErrorMsg.ECE001
                ));
            } this.advance();

            return res.success(new Token(Token.STR, str));
        }

        public LexResult makeIdentifier() {
            LexResult res = new LexResult();
            string str = "";

            while (this.currentChar is not null && Lexer.LETTERS_DIGITS.Contains(this.currentChar)) {
                str += this.currentChar;
                this.advance();
            }

            return res.success(new Token(Token.IDENTIFIER, str));
        }
    }
}