using Compiler.Tokenizer;
using Compiler.Parser;
using Compiler.Triads;
using System;
using System.IO;
using System.Linq;
using Compiler.Stack_machine;
using Compiler.Stack_machine.Functions;
using System.Collections.Generic;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            var main_code_text = File.ReadAllText(args[2]);
            //Console.WriteLine(main_code_text.GetHashCode());
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("Сode received in the Gonozov's language: ");
            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - -");
            Console.WriteLine(main_code_text);
            Console.WriteLine("-------------------------------------------------");

            VariableTable<Function> globalFuncTable = new VariableTable<Function>();

            PrecedenceBasedRegexTokenizer tokenizer = new PrecedenceBasedRegexTokenizer();
            PrecedenceBasedRPNParser parser = new PrecedenceBasedRPNParser(ref globalFuncTable);
            TriadsOptimizer triadsOptimizer = new TriadsOptimizer();
            StackMachine stackMachine = new StackMachine(globalFuncTable);
            

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

            globalFuncTable.Print();

            //parseSequence = triadsOptimizer.Optimize(parseSequence);

            //Console.WriteLine("\n-------------------------------------------------");

            //foreach (var result in parseSequence)
            //{
            //    Console.Write(result.ToString() + " , ");
            //}

            //Console.WriteLine("\n-------------------------------------------------");

            stackMachine.Execute(parseSequence);

        }
    }
}
