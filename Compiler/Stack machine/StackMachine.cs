using Compiler.Parser;
using Compiler.Stack_machine.Linked_list;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler.Stack_machine
{
    class StackMachine
    {
        private VariableTable<int> _intVariableTable;
        private VariableTable<bool> _boolVariableTable;
        private VariableTable<MyLinkedList<int>> _linkedListVariableTable;
        private VariableTable<HashSet<int>> _hashSetVariableTable;

        public StackMachine()
        {
            _intVariableTable = new VariableTable<int>();
            _boolVariableTable = new VariableTable<bool>();
            _linkedListVariableTable = new VariableTable<MyLinkedList<int>>();
            _hashSetVariableTable = new VariableTable<HashSet<int>>();
        }

        public void Execute(Queue<Parse> parseSequense)
        {
            Stack<Parse> _auxiliaryStack = new Stack<Parse>();
            Stack<Parse> _parseStack = new Stack<Parse>();
            while (parseSequense.Count > 0)
            {
                var parse = parseSequense.Dequeue();
                _auxiliaryStack.Push(parse);
                Parse parse1, parse2;
                int operand1 = -1, operand2 = -1;
                MyLinkedList<int> linkedListObject1;
                HashSet<int> hashSetObject1;
                bool logicalOp1 = false, logicalOp2 = false, createList = false, createHashSet = false, isListElem = false, isBool = false, isInt = false;
                string key = "";
                switch (parse.ParseType)
                {
                    case ParseType.LABEL:
                        break;
                    case ParseType.DIGIT:
                    case ParseType.BOOLEAN:
                    case ParseType.CREATE_LIST:
                    case ParseType.CREATE_HASH_SET:
                    case ParseType.VAR:
                    //case ParseType.LABEL:
                        _parseStack.Push(parse);
                        break;
                    case ParseType.A_OP:
                        parse1 = _parseStack.Pop();
                        parse2 = _parseStack.Pop();
                        switch (parse1.ParseType)
                        {
                            case ParseType.DIGIT:
                                operand1 = Int32.Parse(parse1.Value);
                                isInt = true;
                                break;
                            case ParseType.BOOLEAN:
                                logicalOp1 = Boolean.Parse(parse1.Value);
                                isBool = true;
                                break;
                            case ParseType.VAR:
                                try
                                {
                                    operand1 = _intVariableTable[parse1.Value];
                                    isInt = true;
                                }
                                catch (Exception ex)
                                {
                                    logicalOp1 = _boolVariableTable[parse1.Value];
                                    isBool = true;
                                }
                                break;
                            case ParseType.CREATE_LIST:
                                createList = true;
                                break;
                            case ParseType.CREATE_HASH_SET:
                                createHashSet = true;
                                break;
                            case ParseType.LIST:
                                operand1 = Int32.Parse(parse1.Value);
                                isInt = true;
                                //isListElem = true;
                                break;
                            default:
                                throw new Exception("Got not a number/bool and not variable");
                        }
                        switch (parse2.ParseType)
                        {
                            case ParseType.VAR:
                                key = parse2.Value;
                                //isInt = true;
                                break;
                            case ParseType.LIST:
                                isListElem = true;
                                break;
                            default:
                                throw new Exception("Got not a number/bool and not variable");
                        }
                        //key = parse2.ParseType == ParseType.VAR ? parse2.Value : throw new Exception("Got not a variable");
                        if (isInt)
                        {
                            if (isListElem)
                            {
                                switch (parse.Value)
                                {
                                    case "=":
                                        _linkedListVariableTable[parse2.VarName][parse2.Index] = operand1;
                                        break;
                                    case "+=":
                                        _linkedListVariableTable[parse2.VarName][parse2.Index] += operand1;
                                        break;
                                    case "-=":
                                        _linkedListVariableTable[parse2.VarName][parse2.Index] -= operand1;
                                        break;
                                    case "*=":
                                        _linkedListVariableTable[parse2.VarName][parse2.Index] *= operand1;
                                        break;
                                    case "/=":
                                        _linkedListVariableTable[parse2.VarName][parse2.Index] /= operand1;
                                        break;
                                    default:
                                        throw new Exception("Not valid operation received for int type");
                                }
                            }
                            else
                            {
                                switch (parse.Value)
                                {
                                    case "=":
                                        _intVariableTable[key] = operand1;
                                        break;
                                    case "+=":
                                        _intVariableTable[key] += operand1;
                                        break;
                                    case "-=":
                                        _intVariableTable[key] -= operand1;
                                        break;
                                    case "*=":
                                        _intVariableTable[key] *= operand1;
                                        break;
                                    case "/=":
                                        _intVariableTable[key] /= operand1;
                                        break;
                                    default:
                                        throw new Exception("Not valid operation received for int type");
                                }
                            }
                        }
                        else if (isBool)
                        {
                            switch (parse.Value)
                            {
                                case "=":
                                    _boolVariableTable[key] = logicalOp1;
                                    break;
                                default:
                                    throw new Exception("Not valid operation received for bool type");
                            }
                        }
                     
                        else if (createList)
                        {
                            _linkedListVariableTable[key] = new MyLinkedList<int>();
                        }
                        else if (createHashSet)
                        {
                            _hashSetVariableTable[key] = new HashSet<int>();
                        }
                        break;
                    case ParseType.C_OP:
                        parse1 = _parseStack.Pop();
                        parse2 = _parseStack.Pop();
                        operand1 = parse1.ParseType == ParseType.DIGIT || parse1.ParseType == ParseType.LIST ? Int32.Parse(parse1.Value) :
                            (parse1.ParseType == ParseType.VAR ? _intVariableTable[parse1.Value] :
                            throw new Exception("Got not a number and not variable"));
                        operand2 = parse2.ParseType == ParseType.DIGIT || parse2.ParseType == ParseType.LIST ? Int32.Parse(parse2.Value) :
                            (parse2.ParseType == ParseType.VAR ? _intVariableTable[parse2.Value] :
                            throw new Exception("Got not a number and not variable"));
                        switch (parse.Value)
                        {
                            case "==":
                                logicalOp1 = operand2 == operand1;
                                break;
                            case "!=":
                                logicalOp1 = operand2 != operand1;
                                break;
                            case "<=":
                                logicalOp1 = operand2 <= operand1;
                                break;
                            case ">=":
                                logicalOp1 = operand2 >= operand1;
                                break;
                            case "<":
                                logicalOp1 = operand2 < operand1;
                                break;
                            case ">":
                                logicalOp1 = operand2 > operand1;
                                break;
                        }
                        _parseStack.Push(new Parse(logicalOp1.ToString(), ParseType.BOOLEAN));
                        break;
                    case ParseType.L_OP:
                        parse1 = _parseStack.Pop();
                        parse2 = _parseStack.Pop();
                        logicalOp1 = parse1.ParseType == ParseType.BOOLEAN ? Boolean.Parse(parse1.Value) :
                            (parse1.ParseType == ParseType.VAR ? _boolVariableTable[parse1.Value] :
                            throw new Exception("Got not a bool and not variable"));
                        logicalOp2 = parse2.ParseType == ParseType.BOOLEAN ? Boolean.Parse(parse2.Value) :
                            (parse2.ParseType == ParseType.VAR ? _boolVariableTable[parse2.Value] :
                            throw new Exception("Got not a bool and not variable"));
                        switch (parse.Value)
                        {
                            case "&&":
                                logicalOp1 = logicalOp1 && logicalOp2;
                                break;
                            case "||":
                                logicalOp1 = logicalOp1 || logicalOp2;
                                break;
                        }
                        _parseStack.Push(new Parse(logicalOp1.ToString(), ParseType.BOOLEAN));
                        break;
                    case ParseType.OP:
                        parse1 = _parseStack.Pop();
                        parse2 = _parseStack.Pop();
                        operand1 = parse1.ParseType == ParseType.DIGIT || parse1.ParseType == ParseType.LIST ? Int32.Parse(parse1.Value) :
                            (parse1.ParseType == ParseType.VAR ? _intVariableTable[parse1.Value] :
                            throw new Exception("Got not a number and not variable"));
                        operand2 = parse2.ParseType == ParseType.DIGIT || parse2.ParseType == ParseType.LIST ? Int32.Parse(parse2.Value) :
                            (parse2.ParseType == ParseType.VAR ? _intVariableTable[parse2.Value] :
                            throw new Exception("Got not a number and not variable"));
                        switch (parse.Value)
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
                        _parseStack.Push(new Parse(operand1.ToString(), ParseType.DIGIT));
                        break;
                    case ParseType.DEC:
                        parse1 = _parseStack.Pop();
                        operand1 = parse1.ParseType == ParseType.VAR ? _intVariableTable[parse1.Value] :
                            throw new Exception("Got not a number and not variable");
                        _intVariableTable[parse1.Value] = --operand1;
                        _parseStack.Push(new Parse(parse1.Value, parse1.ParseType));
                        break;
                    case ParseType.INC:
                        parse1 = _parseStack.Pop();
                        operand1 = parse1.ParseType == ParseType.VAR ? _intVariableTable[parse1.Value] :
                            throw new Exception("Got not a number and not variable");
                        _intVariableTable[parse1.Value] = ++operand1;
                        _parseStack.Push(new Parse(parse1.Value, parse1.ParseType));
                        break;
                    case ParseType.COND_TRAN:
                        parse1 = _parseStack.Pop();
                        logicalOp1 = parse1.ParseType == ParseType.BOOLEAN ? Boolean.Parse(parse1.Value) :
                            (parse1.ParseType == ParseType.VAR ? _boolVariableTable[parse1.Value] :
                            throw new Exception("Got not a bool and not variable"));
                        if (!logicalOp1)
                            for (var promParse = parseSequense.Dequeue();
                                    !(promParse.ParseType == ParseType.LABEL && promParse.Value == parse.Value);
                                    promParse = parseSequense.Dequeue())
                            {
                                continue;
                            }
                        break;
                    case ParseType.UNCOND_TRAN:
                        for (var promParse = parseSequense.Dequeue();
                                promParse.ParseType == ParseType.LABEL && promParse.Value == parse.Value;
                                promParse = parseSequense.Dequeue())
                            continue;
                        break;
                    case ParseType.UNCOND_TRAN_L:
                        Stack<Parse> promStack = new Stack<Parse>();
                        Parse label;
                        if (parseSequense.TryDequeue(out label) && label.ParseType == ParseType.LABEL)
                            promStack.Push(label);
                        string value = "";
                        for (var stackElem = _auxiliaryStack.Pop();; stackElem = _auxiliaryStack.Pop())
                        {
                            if (stackElem.ParseType == ParseType.LABEL && stackElem.Value == parse.Value)
                            {
                                promStack.Push(stackElem);
                                if (!(label != null && label.Value == value))
                                    throw new Exception("Label not found after unconditional loop transition");
                                Queue<Parse> promQueue = new Queue<Parse>(promStack.ToArray());
                                Execute(promQueue);
                                break;
                            }
                            else
                            {
                                promStack.Push(stackElem);
                                if (stackElem.ParseType == ParseType.COND_TRAN)
                                    value = stackElem.Value;
                            }
                        }
                        break;
                    case ParseType.INDEX:
                        parse1 = _parseStack.Pop();
                        parse2 = _parseStack.Pop();
                        operand1 = parse1.ParseType == ParseType.DIGIT ? Int32.Parse(parse1.Value) :
                            (parse1.ParseType == ParseType.VAR ? _intVariableTable[parse1.Value] :
                            throw new Exception("Got not a number and not variable"));
                        if (parse2.ParseType == ParseType.VAR) {
                            /*try
                            {*/
                                linkedListObject1 = _linkedListVariableTable[parse2.Value];
                                _parseStack.Push(new Parse(linkedListObject1[operand1].ToString(), ParseType.LIST, operand1, parse2.Value));
                            /*}
                            catch (Exception ex)
                            {
                                hashSetObject1 = _hashSetVariableTable[parse2.Value];
                                hashSetObject1.
                                _parseStack.Push(new Parse(hashSetObject1[operand1].ToString(), ParseType.LIST, operand1, parse2.Value));
                            }*/
                        }
                        else
                            throw new Exception("Was recieved not a Linked List object");
                        /*linkedListObject1 = parse2.ParseType == ParseType.VAR ? _linkedListVariableTable[parse2.Value] :
                            throw new Exception("Was recieved not a Linked List object");
                        _parseStack.Push(new Parse(linkedListObject1[operand1].ToString(), ParseType.LIST, operand1, parse2.Value));*/
                        break;
                    case ParseType.ADD:
                        parse1 = _parseStack.Pop();
                        parse2 = _parseStack.Pop();
                        operand1 = parse1.ParseType == ParseType.DIGIT ? Int32.Parse(parse1.Value) :
                            (parse1.ParseType == ParseType.VAR ? _intVariableTable[parse1.Value] :
                            throw new Exception("Got not a number and not variable"));
                        if (parse2.ParseType == ParseType.VAR)
                        {
                            try
                            {
                                linkedListObject1 = _linkedListVariableTable[parse2.Value];
                                linkedListObject1.Add(operand1);
                            }
                            catch (Exception ex)
                            {
                                hashSetObject1 = _hashSetVariableTable[parse2.Value];
                                hashSetObject1.Add(operand1);
                            }
                        }
                        else
                            throw new Exception("Was recieved not a Linked List object");
                        /*linkedListObject1 = parse2.ParseType == ParseType.VAR ? _linkedListVariableTable[parse2.Value] :
                            throw new Exception("Was recieved not a Linked List object");
                        linkedListObject1.Add(operand1);*/
                        break;
                    case ParseType.DELETE:
                        parse1 = _parseStack.Pop();
                        parse2 = _parseStack.Pop();
                        operand1 = parse1.ParseType == ParseType.DIGIT ? Int32.Parse(parse1.Value) :
                            (parse1.ParseType == ParseType.VAR ? _intVariableTable[parse1.Value] :
                            throw new Exception("Got not a number and not variable"));
                        if (parse2.ParseType == ParseType.VAR)
                        {
                            try
                            {
                                linkedListObject1 = _linkedListVariableTable[parse2.Value];
                                linkedListObject1.Delete(operand1);
                            }
                            catch (Exception ex)
                            {
                                hashSetObject1 = _hashSetVariableTable[parse2.Value];
                                hashSetObject1.Remove(operand1);
                            }
                        }
                        else
                            throw new Exception("Was recieved not a Linked List object");
                        break;
                    case ParseType.CONTAINS:
                        parse1 = _parseStack.Pop();
                        parse2 = _parseStack.Pop();
                        operand1 = parse1.ParseType == ParseType.DIGIT ? Int32.Parse(parse1.Value) :
                            (parse1.ParseType == ParseType.VAR ? _intVariableTable[parse1.Value] :
                            throw new Exception("Got not a number and not variable"));
                        if (parse2.ParseType == ParseType.VAR)
                        {
                            hashSetObject1 = _hashSetVariableTable[parse2.Value];
                            logicalOp1 = hashSetObject1.Contains(operand1);
                            _parseStack.Push(new Parse(logicalOp1.ToString(), ParseType.BOOLEAN));
                        }
                        else
                            throw new Exception("Was recieved not a Linked List object");
                        break;
                    case ParseType.PRINT:
                        _intVariableTable.Print();
                        _boolVariableTable.Print();
                        _linkedListVariableTable.Print();
                        _hashSetVariableTable.Print();
                        break;
                    default:
                        throw new Exception("Exception with " + parse.ToString());
                }
            }
        }
    }
}
