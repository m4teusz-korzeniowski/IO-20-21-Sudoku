using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO_Sudoku.src.exceptions
{
    class EpmtyFieldException : Exception
    {
        public EpmtyFieldException(string message) : base(message) { }
    }
}
