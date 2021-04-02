using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO_Sudoku.src.model
{
    class Board
    {
        private List<List<Field>> fields = new List<List<Field>>();
        private Random randomizer = new Random();

        public List<List<Field>> Fields => fields;

        public Board()
        {
            for(int i = 0; i < 9; i++)
            {
                List<Field> tempFields = new List<Field>();
                for (int j = 0; j < 9; j++)
                {
                    tempFields.Add(new Field(randomizer.Next(0, 10)));
                }
                fields.Add(tempFields);
            }
        }
        
    }
}
