namespace parser
{
    public static class Interpreter {
        public static RTResult visit(dynamic node) {
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
                cmds = new List<string>() {node.main.value};
            } else if (node.main is Token) {
                cmds = new List<string>() {node.main.value};
                cmds.Add(node.sub.value);
            } else {
                cmds = res.register(Interpreter.visit_GetNode(node.main));
                cmds.Add(node.sub.value);
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
            List<dynamic> values = new List<dynamic>();
            List<UInt16> order = new List<UInt16>();

            for (int i = 0; i < node.args.Count; i++) {
                dynamic arg = node.args[i];
                if (arg is SwitchNode) {
                    string swtch = res.register(Interpreter.visit_SwitchNode(arg));
                    switches.Add(swtch);
                    order.Add(1);
                } else {
                    dynamic val = res.register(Interpreter.visit(arg));
                    values.Add(val);
                    order.Add(0);
                }
            } return res.success(new data.Argument(order, switches, values));
        }

        public static RTResult visit_ExprNode(ExprNode node) {
            RTResult res = new RTResult();

            List<string> cmds = res.register(Interpreter.visit_GetNode(node.getNode));
            data.Argument argument = res.register(Interpreter.visit_ArgsNode(node.argsNode));
            argument.cmds = cmds;

            return res.success(argument);
        }

        public static RTResult visit_IterNode(IterNode node) {
            RTResult res = new RTResult();
            List<data.Argument> result = new List<data.Argument>();
            for (int i = 0; i < node.elementNodes.Count; i++) {
                data.Argument arg = res.register(Interpreter.visit_ExprNode(node.elementNodes[i]));
                result.Add(arg);
            } return res.success(result);
        }
    }
}