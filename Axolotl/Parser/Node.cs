namespace parser
{
    public class NumberNode {
        public Token tok;

        public NumberNode(Token tok) {
            this.tok = tok;
        }

        public string repr() {
            return $"{this.tok.repr()}";
        }
    }

    public class StringNode {
        public Token tok;

        public StringNode(Token tok) {
            this.tok = tok;
        }

        public string repr() {
            return $"\"{this.tok.repr()}\"";
        }
    }

    public class SwitchNode {
        public Token tok;
        public object value;

        public SwitchNode(Token tok) {
            this.tok = tok;
            this.value = tok.value;
        }

        public string repr() {
            return $"-{this.tok.repr()}";
        }
    }

    public class ArgsNode {
        public List<object> args;

        public ArgsNode(List<object> args) {
            this.args = args;
        }

        public string repr() {
            string result = "";
            for (int i = 0; i < args.Count; i++) {
                result += " " + this.args[i].GetType().GetMethod("repr").Invoke(this.args[i], new object[0]);
            } return result;
        }
    }

    public class GetNode {
        public object main;
        public Token sub;

        public GetNode(object main, Token sub) {
            this.main = main;
            this.sub = sub;
        }

        public string repr() {
            string main = (string) this.main.GetType().GetMethod("repr").Invoke(this.main, new object[0]);
            return this.sub is null ? main: $"{main}.{this.sub.repr()}";
        }
    }

    public class ExprNode {
        public GetNode getNode;
        public ArgsNode argsNode;

        public ExprNode(GetNode getNode, ArgsNode argsNode) {
            this.getNode = getNode;
            this.argsNode = argsNode;
        }

        public string repr() {
            return $"{this.getNode.repr()}{this.argsNode.repr()}";
        }
    }

    public class IterNode {
        public List<object> elementNodes;

        public IterNode(List<object> elementNodes) {
            this.elementNodes = elementNodes;
        }

        public string repr() {
            string result = (string) elementNodes[0].GetType().GetMethod("repr").Invoke(elementNodes[0], new object[0]);
            for (int i = 1; i < elementNodes.Count; i++) {
                result += $"; {(string) elementNodes[i].GetType().GetMethod("repr").Invoke(elementNodes[i], new object[0])}";
            } return result;
        }
    }
}