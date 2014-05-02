using Appacitive.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galleria
{
    public class User : Appacitive.Sdk.APUser
    {
        public User()
            : base()
        { }

        public User(Appacitive.Sdk.APObject existing)
            : base(existing)
        { }

        public string Name { get { return string.Format("{0} {1}", FirstName, LastName); } }

        public User(string email, string password, string firstName, string lastName)
        {
            this.Username = email;
            this.Email = email;
            this.Password = password;
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public static async Task<bool> IsLoggedIn()
        {
            try
            {
                var user = await Appacitive.Sdk.APUsers.GetLoggedInUserAsync();
                if (user == null) return false;
                Context.User = new User(user);
                return true;
            }
            catch { return false; }
        }

        public async static Task<string> Authenticate(string email, string password)
        {
            var failedMessage = "Authentication failed";
            try
            {
                //authenticate user on Appacitive
                var credentials = new UsernamePasswordCredentials(email, password)
                {
                    TimeoutInSeconds = int.MaxValue,
                    MaxAttempts = int.MaxValue
                };

                var userSession = await Appacitive.Sdk.App.LoginAsync(credentials);

                //Logged in user
                var user = new User(userSession.LoggedInUser);

                Context.User = user;

                return null;
            }
            catch { }
            return failedMessage;
        }

        public async Task<bool> Save()
        {
            try
            {
                //Save user in the backend
                await this.SaveAsync();
                Context.User = this;
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> Logout()
        {
            try
            {
                //Logout user
                await Appacitive.Sdk.App.LogoutAsync();

                Context.User = null;
                return true;
            }
            catch { return false; }
        }
    }
}
