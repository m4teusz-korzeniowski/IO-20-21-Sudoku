using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO_Sudoku.src.exceptions
{
    class IncorrectEmailException : Exception
    {
        public IncorrectEmailException(string message) : base(message) { }
    }
}
