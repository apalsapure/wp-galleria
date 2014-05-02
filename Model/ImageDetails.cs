using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galleria
{
    public class ImageDetails : Appacitive.Sdk.APObject
    {
        public ImageDetails()
            : base("image")
        { }
        //special constructor called by SDK
        public ImageDetails(Appacitive.Sdk.APObject existing)
            : base(existing)
        { }

        public string Category
        {
            get { return this.Get<string>("category"); }
            set
            {
                this.Set<string>("category", value);
                base.FirePropertyChanged("Category");
            }
        }

        public string Url
        {
            get { return this.Get<string>("url"); }
            set
            {
                this.Set<string>("url", value);
                base.FirePropertyChanged("Url");
            }
        }

        public string Title
        {
            get { return this.Get<string>("title"); }
            set
            {
                this.Set<string>("title", value.Trim().Replace(" ", "\r\n"));
                base.FirePropertyChanged("Title");
                base.FirePropertyChanged("EscaptedTitle");
            }
        }

        public string EscaptedTitle { get { return this.Title.Replace("\r\n", " "); } }

        public string Message
        {
            get
            {
                var msg = this.Get<string>("message");
                ShowMessage = !string.IsNullOrEmpty(msg);
                return msg;
            }
            set
            {
                this.Set<string>("message", value.Trim());
                ShowMessage = !string.IsNullOrEmpty(value);
                base.FirePropertyChanged("Message");
            }
        }

        private bool _noMessage;
        public bool ShowMessage
        {
            get { return _noMessage; }
            set
            {
                _noMessage = value;
                base.FirePropertyChanged("ShowMessage");
            }
        }

        public bool IsPublic
        {
            get { return this.Get<bool>("ispublic"); }
            set
            {
                this.Set<bool>("ispublic", value);
                base.FirePropertyChanged("IsPublic");
            }
        }

        public bool IsOwner()
        {
            return string.Equals(this.Get<string>("__createdby"), Context.User.Id);
        }

        public async Task<bool> Save()
        {
            //save object here
            try
            {
                if (string.IsNullOrEmpty(this.Id))
                {                 //as we need to store this image in context of user
                    //we will create a connection between user and the image
                    //when connection is saved, image is automatically created
                    await Appacitive.Sdk.APConnection
                                    .New("author")
                                    .FromExistingObject("user", Context.User.Id)
                                    .ToNewObject("image", this)
                                    .SaveAsync();
                }
                else
                {
                    //update the object
                    await this.SaveAsync();
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> Delete()
        {
            var result = false;
            try
            {
                await Appacitive.Sdk.APObjects.DeleteAsync(this.Type, this.Id, true);
                result = true;
            }
            catch { }
            return result;
        }
    }
}