using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Galleria
{
    public class User
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Name { get { return string.Format("{0} {1}", FirstName, LastName); } }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
    }
}
