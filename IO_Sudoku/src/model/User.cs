using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using IO_Sudoku.src.exceptions;

namespace IO_Sudoku.src.model
{
    public class User
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (!String.IsNullOrEmpty(value) && !Char.IsLetter(value[0]))
                {
                    throw new InitCharIsNotALetterException("Nazwa musi rozpoczynać się od litery!");
                }
                if (value == "")
                {
                    throw new EpmtyFieldException("Pole nie może być puste!");
                }
                
                _name = value;
            }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                if (value == "")
                {
                    throw new EpmtyFieldException("Pole nie może być puste!");
                }
                if (!new EmailAddressAttribute().IsValid(value))
                {
                    throw new IncorrectEmailException("Podany adres e-mail jest błędny!");
                }

                _email = value;
            }
        }
        private int _timePlayed;
        public int TimePlayed
        {
            get { return _timePlayed; }
            set { _timePlayed = value; }
        }
        private DateTime _joinDate;
        public DateTime JoinDate
        {
            get { return _joinDate; }
            set { _joinDate = value; }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        /*private bool _wasPasswordUsed;
        public bool WasPasswordUsed
        {
            get { return _wasPasswordUsed; }
            set { _wasPasswordUsed = value; }
        }*/

        public User()
        {
            _timePlayed = 0;
            _joinDate = DateTime.Now;
        }


    }
}
