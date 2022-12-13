namespace parser
{
    public class Error {
        // Class
        public static string InvalidCharError = "Illegal Character";
        public static string InvalidSyntaxError = "Invalid Syntax";
        public static string ExpectedCharError = "Expected Character";

        // Object
        public Position posStart;
        public Position posEnd;
        public string type;
        public string details;

        public Error(Position posStart, Position posEnd, string type, string details) {
            this.posStart = posStart;
            this.posEnd = posEnd;
            this.type = type;
            this.details = details;
        }

        public string repr() {
            string result = $"[Axolotl CLI] Error: {this.type} -> ({this.details})";
            return result;
        }
    }

    public class ErrorMsg {
    }
}