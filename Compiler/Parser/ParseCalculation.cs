using Compiler.Tokenizer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler.Parser
{
    class ParseCalculation
    {
        private Stack<ParseOperation> _parseOperationStack;
        private Queue<Parse> _parseSequence;
        private uint _labelNum;
        Dictionary<string, string> _transitionsAndLabelsBound;
        

        public ParseCalculation()
        {
            _parseOperationStack = new Stack<ParseOperation>();
            _parseSequence = new Queue<Parse>();
            _labelNum = 1;
            _transitionsAndLabelsBound = new Dictionary<string, string>();
        }

        public void Add(Token token)
        {
            switch (token.TokenType)
            {
                case TokenType.PRINT:
                    ReleaseParseOperationStack();
                    _parseSequence.Enqueue(new Parse(token));
                    break;
                case TokenType.DIGIT:
                case TokenType.VAR:
                case TokenType.CREATE_LIST:
                case TokenType.CREATE_HASH_SET:
                case TokenType.ASYNC:
                    _parseSequence.Enqueue(new Parse(token));
                    break;
                case TokenType.LB_S:
                    _parseOperationStack.Push(new ParseOperation(token));
                    break;
                case TokenType.RB_S:
                    for (ParseOperation stackParseOperation = _parseOperationStack.Pop(); stackParseOperation.parseType != ParseType.LB_S; stackParseOperation = _parseOperationStack.Pop())
                    {
                        _parseSequence.Enqueue(new Parse(stackParseOperation));
                    }
                    _parseSequence.Enqueue(new Parse("I", ParseType.INDEX));
                    break;
                case TokenType.LB:
                    _parseOperationStack.Push(new ParseOperation(token));
                    break;
                case TokenType.RB:
                    for (ParseOperation stackParseOperation = _parseOperationStack.Pop(); stackParseOperation.parseType != ParseType.LB; stackParseOperation = _parseOperationStack.Pop())
                    {
                        _parseSequence.Enqueue(new Parse(stackParseOperation));
                    }
                    break;
                default:
                    ParseOperation parseOperation = new ParseOperation(token);
                    PushToStack(parseOperation);
                    break;
            }
        }

        private void PushToStack(ParseOperation parseOperation)
        {
            if (_parseOperationStack.Count > 0 && _parseOperationStack.Peek().Priority >= parseOperation.Priority)
            {
                _parseSequence.Enqueue(new Parse(_parseOperationStack.Pop()));
                PushToStack(parseOperation);
            }
            else
            {
                _parseOperationStack.Push(parseOperation);
            }
        }

        public uint AddTransition(bool isConditional)
        {
            //_transitionsAndLabelsBound.Add(_labelNum, _labelNum + "");
            _parseSequence.Enqueue(new Parse(_labelNum.ToString(), isConditional ? ParseType.COND_TRAN : ParseType.UNCOND_TRAN));
            return _labelNum++;
        }
        public void AddTransition(uint labelNum)
        {
            //string transitionName = _transitionsAndLabelsBound[labelNum];
            _parseSequence.Enqueue(new Parse(labelNum.ToString(), ParseType.UNCOND_TRAN_L));
        }
        public void AddLabel(uint labelNum)
        {
            //string labelName = _transitionsAndLabelsBound[labelNum];
            _parseSequence.Enqueue(new Parse(labelNum.ToString(), ParseType.LABEL));
        }
        public uint AddLabel()
        {
            /*string transitionName = "p" + _labelNum + "!" + (isConditional ? "F" : "");
            string labelName = "L" + _labelNum + ":";
            _transitionsAndLabelsBound.Add(labelName, transitionName);*/
            _parseSequence.Enqueue(new Parse(_labelNum.ToString(), ParseType.LABEL));
            return _labelNum++;
        }
        public void AddFunc(string name)
        {
            _parseSequence.Enqueue(new Parse(name, ParseType.FUNC_NAME));
            _parseSequence.Enqueue(new Parse("(", ParseType.FUNC_ARG_BEGIN));
        }
        public void AddFuncArg()
        {
            _parseSequence.Enqueue(new Parse(";", ParseType.FUNC_ARG_DELIMITER));
        }
        public void CloseFuncArg()
        {
            _parseSequence.Enqueue(new Parse(")", ParseType.FUNC_ARG_CLOSE));
        }

        public Queue<Parse> GetParseSequence()
        {
            ReleaseParseOperationStack();
            return _parseSequence;
        }

        public void ReleaseParseOperationStack()
        {
            ParseOperation parseOperation;
            while (_parseOperationStack.TryPop(out parseOperation))
            {
                _parseSequence.Enqueue(new Parse(parseOperation));
            }
        }
    }
}
