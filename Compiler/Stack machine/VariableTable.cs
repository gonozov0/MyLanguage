using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Compiler.Stack_machine
{
    class VariableTable<T>
    {
        private Dictionary<string, T> _dict;

        public VariableTable()
        {
            _dict = new Dictionary<string, T>();
        }

        public T this[string key]
        {
            get
            {
                return _dict.ContainsKey(key) ? _dict[key] : throw new Exception("key doesn't exist");
            }
            set
            {
                if (_dict.ContainsKey(key)) {
                    _dict[key] = value;
                }
                else
                {
                    _dict.Add(key, value);
                }
            }
        }

        public void Print()
        {
            Console.WriteLine("Variable table:");
            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - -");
            if (_dict.Count > 0 && _dict.Values.First() is HashSet<int>)
            {
                foreach (var var in _dict)
                {
                    string result = "";
                    string template = "{0}\n\t\t\t";
                    //(var.Value as HashSet<int>).Cont
                    foreach (var hashSetItem in var.Value as HashSet<int>)
                    {
                        result += String.Format(template, hashSetItem.ToString());
                    }
                    Console.WriteLine("\t" + var.Key + "\t:\t" + result);
                }
            }
            else
            {
                foreach (var var in _dict)
                {
                    Console.WriteLine("\t" + var.Key + "\t:\t" + var.Value.ToString());
                }
            }
            Console.WriteLine("---------------------------------------------");
        }
    }
}
