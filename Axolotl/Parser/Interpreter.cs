namespace parser
{
    public static class Interpreter {
        public static RTResult visit(object node) {
            string methodName = $"visit_{node.GetType().Name}";
            RTResult res = (RTResult) typeof(Interpreter).GetMethod(methodName).Invoke(null, new object[] {node});
            return res;
        }

        /////////////////////
        // Visit Methods
        /////////////////////

        public static RTResult visit_NumberNode(NumberNode node) {
            return new RTResult().success(
                node.tok.value
            );
        }

        public static RTResult visit_StringNode(StringNode node) {
            return new RTResult().success(
                node.tok.value
            );
        }

        public static RTResult visit_GetNode(GetNode node) {
            RTResult res = new RTResult();
            List<string> cmds;

            if (node.sub is null) {
                cmds = new List<string>() {(string) ((Token) node.main).value};
            } else if (node.main is Token) {
                cmds = new List<string>() {(string) ((Token) node.main).value};
                cmds.Add((string) node.sub.value);
            } else {
                cmds = (List<string>) res.register(Interpreter.visit_GetNode((GetNode) node.main));
                cmds.Add((string) node.sub.value);
            } return res.success(cmds);
        }

        public static RTResult visit_SwitchNode(SwitchNode node) {
            return new RTResult().success(
                node.value
            );
        }

        public static RTResult visit_ArgsNode(ArgsNode node) {
            RTResult res = new RTResult();
            List<string> switches = new List<string>();
            List<object> values = new List<object>();
            List<byte> order = new List<byte>();

            for (int i = 0; i < node.args.Count; i++) {
                object arg = node.args[i];
                if (arg is SwitchNode) {
                    string swtch = (string) res.register(Interpreter.visit_SwitchNode((SwitchNode) arg));
                    switches.Add(swtch);
                    order.Add(1);
                } else {
                    object val = res.register(Interpreter.visit(arg));
                    values.Add(val);
                    order.Add(0);
                }
            } return res.success(new data.Argument(order, switches, values));
        }

        public static RTResult visit_ExprNode(ExprNode node) {
            RTResult res = new RTResult();

            List<string> cmds = (List<string>) res.register(Interpreter.visit_GetNode(node.getNode));
            data.Argument argument = (data.Argument) res.register(Interpreter.visit_ArgsNode(node.argsNode));
            argument.cmds = cmds;

            return res.success(argument);
        }

        public static RTResult visit_IterNode(IterNode node) {
            RTResult res = new RTResult();
            List<data.Argument> result = new List<data.Argument>();
            for (int i = 0; i < node.elementNodes.Count; i++) {
                data.Argument arg = (data.Argument) res.register(Interpreter.visit_ExprNode((ExprNode) node.elementNodes[i]));
                result.Add(arg);
            } return res.success(result);
        }
    }
}