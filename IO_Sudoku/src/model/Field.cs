using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO_Sudoku.src.model
{
    class Field
    {
        private int _value;
        
        public int Value
        {
            get => _value;
            set => _value = value;
        }

        public Field(int value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
