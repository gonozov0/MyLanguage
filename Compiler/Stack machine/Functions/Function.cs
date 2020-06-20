using System;
using System.Collections.Generic;
using System.Text;
using Compiler.Parser;

namespace Compiler.Stack_machine.Functions
{
    class Function
    {
        public Function()
        {
            Args = new List<string>();
            Value = new Queue<Parse>();
        }

        public string Name { get; set; }
        public List<string> Args { get; set; }
        public Queue<Parse> Value { get; set; }

        public override string ToString()
        {
            string s = "";
            foreach (var result in this.Value)
            {
                s += result.ToString() + " , ";
            }
            s += "\n";
            return String.Format("Name: {0}; Args: {1}; \n\t\tStack: {2}", this.Name, String.Join(", ", this.Args), s);
        }
    }
}
