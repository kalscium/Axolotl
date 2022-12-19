namespace parser
{
    // Main Parser Class
    public class Parser {
        public List<Token> tokens;
        public int tokIdx;
        public Token currentTok;

        public Parser(List<Token> tokens) {
            this.tokens = tokens;
            this.tokIdx = -1;
            this.currentTok = null;
            this.advance();
        }

        public dynamic advance() {
            this.tokIdx++;
            if (this.tokIdx < this.tokens.Count) {
                this.currentTok = this.tokens[this.tokIdx];
            } return null;
        }

        public ParseResult parse() {
            ParseResult res = this.iterExpr(); // Default Statement
            if (res.error is null && this.currentTok.type != Token.EOF) {
                return res.failure(new Error(
                    Error.InvalidSyntaxError,
                    ErrorMsg.ECE002
                ));
            } return res;
        }

        /////////////////////
        // Parser Methods
        /////////////////////

        // (The functions are order according to the parse heiarchy)

        public ParseResult iterExpr() {
            ParseResult res = new ParseResult();
            List<dynamic> elementNodes = new List<dynamic>();
            
            elementNodes.Add(res.register(this.expr()));
            if (res.error is not null) return res;
            while (this.currentTok.type == Token.SEMI) {
                res.registerAdvance(this.advance());

                if (!(this.currentTok.type == Token.EOF)) {
                    elementNodes.Add(res.register(this.expr()));
                    if (res.error is not null) return res;
                }
            }

            return res.success(new IterNode(
                elementNodes
            ));
        }

        public ParseResult expr() {
            ParseResult res = new ParseResult();

            dynamic get = res.register(this.get());
            if (res.error is not null) return res;
            if (get is Token) get = new GetNode(get, null);

            ArgsNode args = res.register(this.args());
            if (res.error is not null) return res;

            return res.success(new ExprNode(get, args));
        }

        public ParseResult get() {
            ParseResult res = new ParseResult();

            if (this.currentTok.type != Token.IDENTIFIER) {
                return res.failure(new Error(
                    Error.InvalidSyntaxError,
                    ErrorMsg.ISE002
                ));
            } dynamic main = this.currentTok;

            res.registerAdvance(this.advance());

            while (this.currentTok.type == Token.DOT) {
                res.registerAdvance(this.advance());
                Token sub = this.currentTok;
                main = new GetNode(main, sub);
                res.registerAdvance(this.advance());
            } return res.success(main);
        }

        public ParseResult args() {
            ParseResult res = new ParseResult();
            List<dynamic> args = new List<dynamic>();
            bool isArg = true;

            while (isArg) {
                if (this.currentTok.type == Token.DASH) {
                    args.Add(res.register(this.switchExpr()));
                    if (res.error is not null) return res;
                } else if (this.currentTok.type == Token.INT || this.currentTok.type == Token.FLOAT || this.currentTok.type == Token.STR || this.currentTok.type == Token.IDENTIFIER) {
                    args.Add(res.register(this.atom()));
                    if (res.error is not null) return res;
                } else isArg = false;
            }

            return res.success(new ArgsNode(args));
        }

        public ParseResult switchExpr() {
            ParseResult res = new ParseResult();
            Token tok = this.currentTok;

            if (tok.type != Token.DASH) {
                return res.failure(new Error(
                    Error.ExpectedCharError,
                    ErrorMsg.ECE003
                ));
            } res.registerAdvance(this.advance());

            if (this.currentTok.type == Token.DASH) {
                res.registerAdvance(this.advance());
                dynamic atom = res.register(this.atom());
                if (res.error is not null) return res;
                return res.success(new SwitchNode(atom.tok));
            }

            if (tok is null) return res.failure(new Error(
                Error.ExpectedCharError,
                ErrorMsg.ECE004
            ));

            return res.success(new SwitchNode(tok));
        }

        public ParseResult atom() {
            ParseResult res = new ParseResult();
            Token tok = this.currentTok;

            if (tok.type == Token.INT || tok.type == Token.FLOAT) {
                res.registerAdvance(this.advance());
                return res.success(new NumberNode(tok));
            } else if (tok.type == Token.STR || tok.type == Token.IDENTIFIER) {
                res.registerAdvance(this.advance());
                return res.success(new StringNode(tok));
            }

            return res.failure(new Error(
                Error.InvalidSyntaxError,
                ErrorMsg.ISE001
            ));
        }
    }
}