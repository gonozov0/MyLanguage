using System;
using System.Collections.Generic;
using System.Text;
using Compiler.Parser;

namespace Compiler.Triads
{
    class TriadsOptimizer
    {
        private List<TriadSequences> _triadList;
        private Queue<Parse> _parseSequense;
        private Stack<Parse> _parseStack;
        private TriadSequences _triadSequence;

        public TriadsOptimizer()
        {
            _triadList = new List<TriadSequences>();
            _parseSequense = new Queue<Parse>();
            _parseStack = new Stack<Parse>();
            _triadSequence = new TriadSequences();
        }

        private void FreeStack()
        {
            foreach (Parse item in _parseStack)
            {
                _parseSequense.Enqueue(item);
            }
        }

        private void CreateTriads(Queue<Parse> parseSequense)
        {
            
            while (parseSequense.Count > 0)
            {
                var parse = parseSequense.Dequeue();
                Parse parse1, parse2;
                int index;
                switch (parse.ParseType)
                {
                    case ParseType.DIGIT:
                    case ParseType.VAR:
                        _parseStack.Push(parse);
                        break;
                    case ParseType.A_OP:
                    case ParseType.OP:
                        parse1 = _parseStack.Pop();
                        parse2 = _parseStack.Pop();
                        index = _triadSequence.Count();
                        _parseStack.Push(new Parse(index.ToString(), ParseType.TRIAD_LINK));
                        _triadSequence.Add(new Triad(parse, parse2, parse1));
                        break;
                    case ParseType.INC:
                    case ParseType.DEC:
                        parse1 = _parseStack.Pop();
                        _triadSequence.Add(new Triad(parse, parse1, new Parse("", ParseType.NULL)));
                        break;
                    case ParseType.PRINT:
                        _parseSequense = _triadSequence.Optimize();
                        _parseSequense.Enqueue(parse);
                        break;
                    default:

                        throw new Exception("Triads non working here");
                }
            }
        }

        public Queue<Parse> Optimize(Queue<Parse> parseSequense)
        {
            CreateTriads(parseSequense);
            return _parseSequense;
        }
    }
}
