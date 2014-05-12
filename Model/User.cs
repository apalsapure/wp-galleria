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

        public User(Appacitive.Sdk.APUser existing)
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
                return true;
            }
            catch (AppacitiveApiException) { return false; }
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

                await Appacitive.Sdk.AppContext.LoginAsync(credentials);

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
                return true;
            }
            catch { return false; }
        }

        public static async Task<bool> Logout()
        {
            try
            {
                //Logout user
                await Appacitive.Sdk.AppContext.LogoutAsync();

                return true;
            }
            catch { return false; }
        }
    }
}
