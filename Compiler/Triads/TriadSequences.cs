using System;
using System.Collections.Generic;
using System.Text;
using Compiler.Parser;
using Compiler.Stack_machine;

namespace Compiler.Triads
{
    class TriadSequences
    {
        private List<Triad> _triadSequences;
        private VariableTable<int> _intVariableTable;

        public TriadSequences()
        {
            _triadSequences = new List<Triad>();
            _intVariableTable = new VariableTable<int>();
        }

        public void Add(Triad triad)
        {
            _triadSequences.Add(triad);
        }

        public int Count()
        {
            return _triadSequences.Count;
        }

        private Queue<Parse> TriadsToParse()
        {
            Queue<Parse> _parseQueue = new Queue<Parse>();
            foreach (Triad triad in _triadSequences)
            {
                Console.WriteLine(triad.ToString());
                if (triad.TriadType == TriadType.FINAL)
                {
                    _parseQueue.Enqueue(triad.Operand1);
                    _parseQueue.Enqueue(triad.Operand2);
                    _parseQueue.Enqueue(triad.Operation);
                }
            }
            return _parseQueue;
        }

        private void PrintTriads()
        {
            foreach (Triad triad in _triadSequences)
            {
                Console.WriteLine(triad.ToString());
            }
            Console.WriteLine("\n-------------------------------------------------");
        }

        public Queue<Parse> Optimize()
        {
            PrintTriads();
            int triadOptimizedCount = 0;
            while (true)
            {
                int _triadOptimizedCount = 0;
                foreach (Triad triad in _triadSequences)
                {
                    if (triad.TriadType == TriadType.CONST)
                    {
                        _triadOptimizedCount++;
                        continue;
                    }
                    TriadType? _newTriadType = null;
                    int operand1 = -1;
                    int operand2 = -1;
                    string key;
                    switch (triad.Operation.ParseType)
                    {
                        case ParseType.A_OP:
                            switch (triad.Operand2.ParseType)
                            {
                                case ParseType.DIGIT:
                                    operand2 = Int32.Parse(triad.Operand2.Value);
                                    break;
                                case ParseType.VAR:
                                    try
                                    {
                                        operand2 = _intVariableTable[triad.Operand2.Value];
                                    }
                                    catch (Exception ex)
                                    {
                                        continue;
                                    }
                                    break;
                                case ParseType.TRIAD_LINK:
                                    Triad _triad = _triadSequences[Int32.Parse(triad.Operand2.Value)];
                                    if (_triad.TriadType == TriadType.CONST)
                                    {
                                        operand2 = Int32.Parse(_triad.Operand1.Value);
                                    }
                                    else continue;
                                    break;
                                default:
                                    throw new Exception("Got not a number/bool and not variable (Triad Optimizer)");
                            }
                            switch (triad.Operand1.ParseType)
                            {
                                case ParseType.VAR:
                                    key = triad.Operand1.Value;
                                    break;
                                default:
                                    throw new Exception("Got not a number/bool and not variable (Triad Optimizer)");
                            }
                            switch (triad.Operation.Value)
                            {
                                case "=":
                                    _intVariableTable[key] = operand2;
                                    break;
                                case "+=":
                                    _intVariableTable[key] += operand2;
                                    break;
                                case "-=":
                                    _intVariableTable[key] -= operand2;
                                    break;
                                case "*=":
                                    _intVariableTable[key] *= operand2;
                                    break;
                                case "/=":
                                    _intVariableTable[key] /= operand2;
                                    break;
                                default:
                                    throw new Exception("Not valid operation received for int type");
                            }
                            triad.TriadType = TriadType.FINAL;
                            triad.Operand2 = new Parse(operand2.ToString(), ParseType.DIGIT);
                            break;
                        case ParseType.OP:
                            switch (triad.Operand1.ParseType)
                            {
                                case ParseType.DIGIT:
                                    operand1 = Int32.Parse(triad.Operand1.Value);
                                    break;
                                case ParseType.VAR:
                                    try
                                    {
                                        operand1 = _intVariableTable[triad.Operand1.Value];
                                    }
                                    catch (Exception ex)
                                    {
                                        continue;
                                    }
                                    break;
                                 case ParseType.TRIAD_LINK:
                                    Triad _triad = _triadSequences[Int32.Parse(triad.Operand1.Value)];
                                    if (_triad.TriadType == TriadType.CONST)
                                    {
                                        operand1 = Int32.Parse(_triad.Operand1.Value);
                                    }
                                    else continue;
                                    break;
                                default:
                                    throw new Exception("Got not a number/bool and not variable (Triad Optimizer)");
                            }
                            switch (triad.Operand2.ParseType)
                            {
                                case ParseType.DIGIT:
                                    operand2 = Int32.Parse(triad.Operand2.Value);
                                    break;
                                case ParseType.VAR:
                                    try
                                    {
                                        operand2 = _intVariableTable[triad.Operand2.Value];
                                    }
                                    catch (Exception ex)
                                    {
                                        continue;
                                    }
                                    break;
                                case ParseType.TRIAD_LINK:
                                    Triad _triad = _triadSequences[Int32.Parse(triad.Operand2.Value)];
                                    if (_triad.TriadType == TriadType.CONST)
                                    {
                                        operand2 = Int32.Parse(_triad.Operand1.Value);
                                    }
                                    else continue;
                                    break;
                                default:
                                    throw new Exception("Got not a number/bool and not variable (Triad Optimizer)");
                            }
                            switch (triad.Operation.Value)
                            {
                                case "+":
                                    operand1 += operand2;
                                    break;
                                case "-":
                                    operand1 -= operand2;
                                    break;
                                case "*":
                                    operand1 *= operand2;
                                    break;
                                case "/":
                                    operand1 /= operand2;
                                    break;
                            }
                            triad.Operand1.Value = operand1.ToString();
                            triad.TriadType = TriadType.CONST;
                            break;
                        case ParseType.DEC:
                            switch (triad.Operand1.ParseType)
                            {
                                case ParseType.VAR:
                                    try
                                    {
                                        operand1 = _intVariableTable[triad.Operand1.Value];
                                    }
                                    catch (Exception ex)
                                    {
                                        continue;
                                    }
                                    break;
                                default:
                                    throw new Exception("Got not a number/bool and not variable (Triad Optimizer)");
                            }
                            _intVariableTable[triad.Operand1.Value] = --operand1;
                            triad.TriadType = TriadType.FINAL;
                            break;
                        case ParseType.INC:
                            switch (triad.Operand1.ParseType)
                            {
                                case ParseType.VAR:
                                    try
                                    {
                                        operand1 = _intVariableTable[triad.Operand1.Value];
                                    }
                                    catch (Exception ex)
                                    {
                                        continue;
                                    }
                                    break;
                                default:
                                    throw new Exception("Got not a number/bool and not variable (Triad Optimizer)");
                            }
                            _intVariableTable[triad.Operand1.Value] = ++operand1;
                            triad.TriadType = TriadType.FINAL;
                            break;

                        default:
                            throw new Exception("Exception with " + triad.ToString());
                    }
                }
                if (_triadOptimizedCount == triadOptimizedCount)
                    break;
            }
            return TriadsToParse();
        }
    }
}
