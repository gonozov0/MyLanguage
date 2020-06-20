using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.Tokenizer
{
    class PrecedenceBasedRegexTokenizer
    {
        private List<TokenDefinition> _tokenDefinitions;

        public PrecedenceBasedRegexTokenizer()
        {
            _tokenDefinitions = new List<TokenDefinition>();

            _tokenDefinitions.Add(new TokenDefinition(TokenType.COMMENT, @"\/{2}.*", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.VAR, @"\w+", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.A_OP, @"(=|\+=|\/=|\*=|-=)", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.INC, @"\+\+", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.DEC, @"--", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.DIGIT, @"\d+", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.OP, @"(-|\+|\*|\/)", 2));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.WHILE, @"while", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.LB, @"\(", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.RB, @"\)", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.LB_F, @"\{", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.RB_F, @"\}", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.SEM, @";", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.IF, @"if", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.ELSE, @"else", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.L_OP, @"(\|\||&&)", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.C_OP, @"(==|!=|<=|>=|>|<)", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.PRINT, @"print", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.CREATE_LIST, @"List", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.CREATE_HASH_SET, @"HashSet", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.RB_S, @"\]", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.LB_S, @"\[", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.DOT, @"\.", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.COM, @"\,", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.ADD, @"Add", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.DELETE, @"Delete", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.CONTAINS, @"Contains", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.FUNC, @"Func", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.ASYNC, @"Async", 1));

        }

        public IEnumerable<Token> Tokenize(string text)
        {
            var tokenMatches = FindTokenMatches(text);

            var groupedByIndex = tokenMatches.GroupBy(x => x.StartIndex)
                .OrderBy(x => x.Key)
                .ToList();

            TokenMatch lastMatch = null;
            for (int i = 0; i < groupedByIndex.Count; i++)
            {
                var bestMatch = groupedByIndex[i].OrderBy(x => x.Precedence).First();
                if (lastMatch != null && bestMatch.StartIndex < lastMatch.EndIndex)
                    continue;
                if (bestMatch.TokenType != TokenType.COMMENT)
                    yield return new Token(bestMatch.TokenType, bestMatch.Value);
                lastMatch = bestMatch;
            }

            yield return new Token(TokenType.END_SIGN);
        }

        private List<TokenMatch> FindTokenMatches(string text)
        {
            var tokenMatches = new List<TokenMatch>();

            foreach (var tokenDefinition in _tokenDefinitions)
                tokenMatches.AddRange(tokenDefinition.FindMatches(text).ToList());

            return tokenMatches;
        }
    }
}
