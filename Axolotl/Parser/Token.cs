namespace parser
{
    public class Token {
        // Static
        public static string INT = "INT";
        public static string FLOAT = "FLOAT";
        public static string IDENTIFIER = "IDENTIFIER";
        public static string STR = "STR";
        public static string FUNNEL = "FUNNEL";
        public static string DASH = "DASH";
        public static string DOT = "DOT";
        public static string SEMI = "SEMI";
        public static string EOF = "EOF";

        // Dynamic
        public string type;
        public object value;

        public Token(string type, object value=null) {
            this.type = type;
            this.value = value;
        }

        public bool matches(string type, object value) {
            return this.type == type && this.value.Equals(value);
        }

        public string repr() {
            return this.value is null ? $"{this.type}": $"{this.type}:{this.value}";
        }

        // For Lexer Shortening
        public static void Lexer_CharTok(string tokenChar, string tokenType, Lexer lexer, List<Token> tokenList) {
            Token.Lexer_AppendTok(lexer.currentChar == tokenChar, lexer, tokenType, tokenList);
        }

        public static LexResult Lexer_FuncTok(bool condition, Func<LexResult> fun, List<Token> tokenList, Lexer lexer) {
            if (condition) {
                lexer.res.found = true;
                LexResult res = new LexResult();
                LexResult output = fun();

                Token tok = (Token) res.register(output);
                if (res.error is not null) return res;

                tokenList.Add(tok);
                if (res.cache is not null) tokenList.Add((Token) res.cache);

                return res;
            } return new LexResult();
        }

        public static void Lexer_AppendTok(bool req, Lexer lexer, string tokenType, List<Token> tokenList) {
            if (req) {
                lexer.res.found = true;
                tokenList.Add(new Token(tokenType));
                lexer.advance();
            }
        }
    }
}