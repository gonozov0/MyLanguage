using System;
using Compiler.Tokenizer;
using System.Collections.Generic;
using System.Text;

namespace Compiler.Parser
{
    class PrecedenceBasedRPNParser
    {
        private Stack<Token> _tokenSequence;
        private ParseCalculation _parseCalculation;
        //List<string> ExeptionList;
        public Queue<Parse> Parse(List<Token> tokens)
        {
            LoadSequenceStack(tokens);
            _parseCalculation = new ParseCalculation();

            Lang();

            return _parseCalculation.GetParseSequence();
        }

        // Инициализация стека токенов
        private void LoadSequenceStack(List<Token> tokens)
        {
            _tokenSequence = new Stack<Token>();
            int count = tokens.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                _tokenSequence.Push(tokens[i]);
            }
        }

        /*
        Моя грамматика:

        lang -> expr*
        expr -> assign_expr | keyword_expr
        assign_expr -> VAR ( ( A_OP stmt ) | INC | DEC )
        stmt ->  n_value ( OP n_value ) *
        n_value -> ( LB stmt RB ) | value
        value -> VAR | DIGIT
        keyword_expr -> while_expr | if_expr | PRINT
        while_expr -> WHILE LB conditions RB LB_F expr+ RB_F
        if_expr -> IF LB conditions RB LB_F expr+ RB_F ( ELSE LB_F expr+ RB_F) ?
        conditions -> condition ( L_OP condition ) *
        condition -> stmt C_OP stmt
             */
        private void Lang()
        {
            Token token;
            if (_tokenSequence.TryPeek(out token))
            {
                if (token.TokenType != TokenType.END_SIGN && token.TokenType != TokenType.RB_F)
                {
                    token = _tokenSequence.Pop();
                    bool isSuccess = AssignExpr(token);
                    if (!isSuccess)
                        isSuccess = KeywordExpr(token);
                    if (!isSuccess)
                        throw new Exception("Invalid value: '" + token.Value + "'");
                    else Lang();
                }
            }
        }
        
        // Блок реализующий логику по assign_expr
        private bool AssignExpr(Token token)
        {
            if (Var(token))
            {
                //_parseCalculation.Add(token);
                token = _tokenSequence.Pop();
                switch(token.TokenType)
                {
                    case TokenType.A_OP:
                        _parseCalculation.Add(token);
                        token = _tokenSequence.Pop();
                        if (token.TokenType == TokenType.CREATE_LIST || token.TokenType == TokenType.CREATE_HASH_SET)
                            _parseCalculation.Add(token);
                        else
                            Conditions(token);
                        break;
                    /*case TokenType.LB_S:
                        _parseCalculation.Add(token);
                        break;*/
                    case TokenType.DOT:
                        token = _tokenSequence.Pop();
                        if (token.TokenType == TokenType.ADD || token.TokenType == TokenType.DELETE)
                        {
                            _parseCalculation.Add(token);
                            token = _tokenSequence.Pop();
                            if (token.TokenType == TokenType.LB)
                            {
                                _parseCalculation.Add(token);
                                Stmt(_tokenSequence.Pop());
                                token = _tokenSequence.Pop();
                                if (token.TokenType == TokenType.RB)
                                {
                                    _parseCalculation.Add(token);
                                    break;
                                }
                            }    
                        }
                        throw new Exception("Invalid value: '" + token.Value + "'");
                    default:
                        throw new Exception("Invalid value: '" + token.Value + "'");
                }
                if (_tokenSequence.Pop().TokenType == TokenType.SEM)
                {
                    _parseCalculation.ReleaseParseOperationStack();
                    return true;
                }
                else
                    throw new Exception("Invalid value: '" + token.Value + "'");
            }
            else
                return false;
        }
        private bool Stmt(Token token)
        {
            Value(token);
            while (_tokenSequence.TryPeek(out token) && token.TokenType == TokenType.OP)
            {
                _parseCalculation.Add(_tokenSequence.Pop());
                Value(_tokenSequence.Pop());
            }
            return true;
        }
        private bool Value(Token token)
        {
            switch(token.TokenType)
            {
                case TokenType.LB:
                    _parseCalculation.Add(token);
                    var isSuccess = Stmt(_tokenSequence.Pop());
                    if (isSuccess)
                    {
                        token = _tokenSequence.Pop();
                        if (token.TokenType == TokenType.RB)
                        {
                            _parseCalculation.Add(token);
                            return true;
                        }
                    }
                    throw new Exception("Invalid value: '" + token.Value + "'");
                case TokenType.VAR:
                    if (Var(token))
                    {
                        if (_tokenSequence.TryPeek(out token) && (token.TokenType == TokenType.INC || token.TokenType == TokenType.DEC))
                            _parseCalculation.Add(_tokenSequence.Pop());
                        return true;
                    }
                    throw new Exception("Invalid value: '" + token.Value + "'");
                case TokenType.DIGIT:
                    _parseCalculation.Add(token);
                    if (_tokenSequence.TryPeek(out token) && (token.TokenType == TokenType.INC || token.TokenType == TokenType.DEC))
                        _parseCalculation.Add(_tokenSequence.Pop());
                    return true;
                default:
                    throw new Exception("Invalid value: '" + token.Value + "'");
            }
        }
        private bool Var(Token token)
        {
            if (token.TokenType != TokenType.VAR)
                return false;
            _parseCalculation.Add(token);
            if (_tokenSequence.TryPeek(out token) && token.TokenType == TokenType.LB_S)
            {
                _parseCalculation.Add(_tokenSequence.Pop());
                Stmt(_tokenSequence.Pop());
                token = _tokenSequence.Pop();
                if (token.TokenType != TokenType.RB_S)
                    throw new Exception("Invalid value: '" + token.ToString() + "'");
                _parseCalculation.Add(token);
            }
            return true;
        }

        // Блок реализующий логику по keyword_expr
        private bool KeywordExpr(Token token)
        {
            bool isSuccess = IfExpr(token);
            if (!isSuccess)
                isSuccess = WhileExpr(token);
            if (!isSuccess)
                if (token.TokenType == TokenType.PRINT)
                {
                    _parseCalculation.Add(token);
                    isSuccess = true;
                }
            return isSuccess;
        }
        private bool WhileExpr(Token token)
        {
            if (token.TokenType == TokenType.WHILE)
            {
                // bool isSuccess = _tokenSequence.Pop().TokenType == TokenType.LB;
                if (_tokenSequence.Pop().TokenType == TokenType.LB)
                {
                    uint labelNum1 = _parseCalculation.AddLabel();
                    Conditions(_tokenSequence.Pop());
                    if (_tokenSequence.Pop().TokenType == TokenType.RB)
                        if (_tokenSequence.Pop().TokenType == TokenType.LB_F)
                        {
                            _parseCalculation.ReleaseParseOperationStack();
                            uint labelNum2 = _parseCalculation.AddTransition(true);
                            Lang();
                            if (_tokenSequence.Pop().TokenType == TokenType.RB_F)
                            {
                                _parseCalculation.ReleaseParseOperationStack();
                                _parseCalculation.AddTransition(labelNum1);
                                _parseCalculation.AddLabel(labelNum2);
                                return true;
                            }
                        }
                }
                throw new Exception("Invalid value: '" + token.Value + "'");
            }
            else
                return false;
        }
        private bool IfExpr(Token token)
        {
            if (token.TokenType == TokenType.IF)
            {
               // bool isSuccess = _tokenSequence.Pop().TokenType == TokenType.LB;
                if (_tokenSequence.Pop().TokenType == TokenType.LB)
                {
                    Conditions(_tokenSequence.Pop());
                    if (_tokenSequence.Pop().TokenType == TokenType.RB)
                        if (_tokenSequence.Pop().TokenType == TokenType.LB_F)
                        {
                            _parseCalculation.ReleaseParseOperationStack();
                            uint labelNum1 = _parseCalculation.AddTransition(true);
                            Lang();
                            if (_tokenSequence.Pop().TokenType == TokenType.RB_F)
                            {
                                _parseCalculation.ReleaseParseOperationStack();
                                if (_tokenSequence.Pop().TokenType == TokenType.ELSE && _tokenSequence.Pop().TokenType == TokenType.LB_F)
                                {
                                    uint labelNum2 = _parseCalculation.AddTransition(false);
                                    _parseCalculation.AddLabel(labelNum1);
                                    Lang();
                                    if (_tokenSequence.Pop().TokenType == TokenType.RB_F)
                                    {
                                        _parseCalculation.ReleaseParseOperationStack();
                                        _parseCalculation.AddLabel(labelNum2);
                                        return true;
                                    }
                                }
                                else
                                {
                                    _parseCalculation.AddLabel(labelNum1);
                                    return true;
                                }
                            }
                        }
                }
                throw new Exception("Invalid value: '" + token.Value + "'");
            }
            else
                return false;
        }
        private bool Conditions(Token token)
        {
            Condition(token);
            while (_tokenSequence.Count > 0 && _tokenSequence.Peek().TokenType == TokenType.L_OP)
            {
                _parseCalculation.Add(_tokenSequence.Pop());
                Condition(_tokenSequence.Pop());
            }
            return true;
        }
        private bool Condition(Token token)
        {
            if (token.TokenType == TokenType.VAR && _tokenSequence.Peek().TokenType == TokenType.DOT)
            {
                _parseCalculation.Add(token);
                token = _tokenSequence.Pop();
                    token = _tokenSequence.Pop();
                    if (token.TokenType == TokenType.CONTAINS)
                    {
                        _parseCalculation.Add(token);
                        token = _tokenSequence.Pop();
                        if (token.TokenType == TokenType.LB)
                        {
                            _parseCalculation.Add(token);
                            Stmt(_tokenSequence.Pop());
                            token = _tokenSequence.Pop();
                            if (token.TokenType == TokenType.RB)
                            {
                                _parseCalculation.Add(token);
                                return true;
                            }
                        }
                }
                throw new Exception("Invalid value: '" + token.Value + "'");
            }
            else
            {
                Stmt(token);
                while (_tokenSequence.Count > 0 && _tokenSequence.Peek().TokenType == TokenType.C_OP)
                {
                    _parseCalculation.Add(_tokenSequence.Pop());
                    Stmt(_tokenSequence.Pop());
                }
                return true;
            }
        }
    }
}
