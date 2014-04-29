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

        public User(string email, string password, string firstName, string lastName)
        {
            this.Username = email;
            this.Email = email;
            this.Password = password;
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public static string Authenticate(string email, string password)
        {
            var faileMessage = "Authentication failed";
            try
            {
                //Code to authenticate user on Appacitive will go here
                //For now get the user from store and check the credentials
                //remove this hard coded user
                var user = new User(email, password, "test user", email);

                Context.User = user;

                return null;
            }
            catch { }
            return faileMessage;
        }

        public bool Save()
        {
            //Save user in the backend
            Context.User = this;
            return true;
        }

        public bool Logout()
        {
            //Invalidate user token in Appacitive API
            Context.User = null;
            return true;
        }
    }
}
