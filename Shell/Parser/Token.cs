namespace parser
{
    public class Token {
        // Static
        public static string INT = "INT";
        public static string FLOAT = "FLOAT";
        public static string IDENTIFIER = "IDENTIFIER";
        public static string STR = "STR";
        public static string FUNNEL = "FUNNEL";
        public static string PIPE = "PIPE";
        public static string SWCH = "SWCH";

        // Dynamic
        public string type;
        public dynamic value;
        public Position posStart;
        public Position posEnd;

        public Token(string type, dynamic value=null, Position posStart=null, Position posEnd=null) {
            this.type = type;
            this.value = value;

            if (posStart is not null) {
                this.posStart = posStart.copy();
                this.posEnd = posStart.copy();
                this.posEnd.advance();
            } if (posEnd is not null) this.posEnd = posEnd;
        }
    }
}