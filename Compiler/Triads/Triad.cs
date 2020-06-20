using System;
using System.Collections.Generic;
using System.Text;
using Compiler.Parser;

namespace Compiler.Triads
{
    class Triad
    {
        public Triad(Parse operation, Parse operand1, Parse operand2)
        {
            TriadType = TriadType.NEW;
            Operation = operation;
            Operand1 = operand1;
            Operand2 = operand2;
        }

        private TriadType _triadType;
        public TriadType TriadType { 
            get {
                return _triadType;
            }
            set {
                if (value == TriadType.CONST)
                {
                    Operation.Value = "C";
                    Operand2.Value = "";
                }
                _triadType = value;
            } 
        }

        public Parse Operation { get; set; }

        public Parse Operand1 { get; set; }

        public Parse Operand2 { get; set; }

        public Queue<Parse> ToParses()
        {
            Queue<Parse> parses = new Queue<Parse>();
            parses.Enqueue(Operand1);
            parses.Enqueue(Operand2);
            parses.Enqueue(Operation);
            return parses;
        }

        public override string ToString()
        {
            return String.Format("{0}({1},{2})", Operation.Value, Operand1.Value, Operand2.Value);
        }

    }
}
