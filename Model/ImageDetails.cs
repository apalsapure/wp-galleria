using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Galleria
{
    public class ImageDetails : INotifyPropertyChanged
    {
        public string Id { get; set; }

        private string _category;
        public string Category
        {
            get { return _category; }
            set
            {
                _category = value;
                NotifyPropertyChanged("Category");
            }
        }

        private string _url;
        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                NotifyPropertyChanged("Url");
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value.Trim().Replace(" ", "\r\n");
                NotifyPropertyChanged("Title");
                NotifyPropertyChanged("EscaptedTitle");
            }
        }

        public string EscaptedTitle { get { return this.Title.Replace("\r\n", " "); } }

        private string _message;
        public string Message
        {
            get
            {
                ShowMessage = !string.IsNullOrEmpty(_message);
                return _message;
            }
            set
            {
                _message = value;
                ShowMessage = !string.IsNullOrEmpty(value);
                NotifyPropertyChanged("Comment");
            }
        }

        private bool _noMessage;
        public bool ShowMessage
        {
            get { return _noMessage; }
            set
            {
                _noMessage = value;
                NotifyPropertyChanged("ShowMessage");
            }
        }


        private bool _isPublic;
        public bool IsPublic
        {
            get { return _isPublic; }
            set
            {
                _isPublic = value;
                NotifyPropertyChanged("IsPublic");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool IsOwner { get; set; }

        public bool Save()
        {
            return true;
        }

        public bool Delete()
        {
            // delete item from store
            return true;
        }

    }
}
