using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler.Stack_machine.Linked_list
{
    class Item<T>
    {
        public T Data { get; set; }
        public Item<T> Next { get; set; }

        public Item(T data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            Data = data;
        }

        public override string ToString()
        {
            return Data.ToString();
        }
    }
}
