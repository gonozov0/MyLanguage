using Compiler.Tokenizer;
using Compiler.Parser;
using System;
using System.IO;
using System.Linq;
using Compiler.Stack_machine;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            var main_code_text = File.ReadAllText(args[1]);
            Console.WriteLine(main_code_text.GetHashCode());
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("Сode received in the Gonozov's language: ");
            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - -");
            Console.WriteLine(main_code_text);
            Console.WriteLine("-------------------------------------------------");


            PrecedenceBasedRegexTokenizer tokenizer = new PrecedenceBasedRegexTokenizer();
            PrecedenceBasedRPNParser parser = new PrecedenceBasedRPNParser();
            StackMachine stackMachine = new StackMachine();

            var tokenSeqence = tokenizer.Tokenize(main_code_text).ToList();

            foreach (var token in tokenSeqence)
            {
                Console.Write(token.ToString() + " , ");
            }
            Console.WriteLine("\n-------------------------------------------------");

            var parseSequence = parser.Parse(tokenSeqence);

            foreach (var result in parseSequence)
            {
                Console.Write(result.ToString() + " , ");
            }
            Console.WriteLine("\n-------------------------------------------------");
            
            stackMachine.Execute(parseSequence);

        }
    }
}
