using System;
using System.Collections;

namespace parser
{
    public class Position {
        public int idx;
        public int ln;
        public int col;
        public string fn;
        public string ftxt;

        public Position(int idx, int ln, int col, string fn, string ftxt) {
            this.idx = idx;
            this.ln = ln;
            this.col = col;
            this.fn = fn;
            this.ftxt = ftxt;
        }

        public Position advance(string currentChar=null) {
            this.idx += 1;
            this.col += 1;

            if (currentChar == "\n") {
                this.ln += 1;
                this.col = 0;
            }

            return this;
        }

        public Position copy() {
            return new Position(this.idx, this.ln, this.col, this.fn, this.ftxt);
        }
    }
}