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
        public dynamic value;

        public SwitchNode(Token tok) {
            this.tok = tok;
            this.value = tok.value;
        }

        public string repr() {
            return $"-{this.tok.repr()}";
        }
    }

    public class ArgsNode {
        public List<dynamic> args;

        public ArgsNode(List<dynamic> args) {
            this.args = args;
        }

        public string repr() {
            string result = "";
            for (int i = 0; i < args.Count; i++) {
                result += " " + this.args[i].repr();
            } return result;
        }
    }

    public class GetNode {
        public dynamic main;
        public Token sub;

        public GetNode(dynamic main, Token sub) {
            this.main = main;
            this.sub = sub;
        }

        public string repr() {
            return this.sub is null ? this.main.repr(): $"{this.main.repr()}.{this.sub.repr()}";
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
        public List<dynamic> elementNodes;

        public IterNode(List<dynamic> elementNodes) {
            this.elementNodes = elementNodes;
        }

        public string repr() {
            string result = elementNodes[0].repr();
            for (int i = 1; i < elementNodes.Count; i++) {
                result += $"; {elementNodes[i].repr()}";
            } return result;
        }
    }
}