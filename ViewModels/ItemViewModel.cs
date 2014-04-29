using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Galleria
{
    public class ItemViewModel : INotifyPropertyChanged
    {

        private ImageDetails _imageDetails;

        public ItemViewModel(ImageDetails imageDetails)
        {
            this._imageDetails = imageDetails;
        }

        public ImageDetails ImageDetails
        {
            get { return _imageDetails; }
            private set
            {
                _imageDetails = value;
                NotifyPropertyChanged("ImageDetails");
            }
        }

        private User _author;
        public User Author
        {
            get { return _author; }
            private set
            {
                _author = value;
                NotifyPropertyChanged("Author");
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

        internal void LoadData()
        {
            this.IsDataLoaded = false;

            this.Author = new User
            {
                Email = "someone@example.com",
                FirstName = "John",
                LastName = "Doe"
            };

            this.IsDataLoaded = true;
        }


        private bool _isDataLoaded = false;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public bool IsDataLoaded
        {
            get
            {
                return _isDataLoaded;
            }
            set
            {
                if (value != _isDataLoaded)
                {
                    _isDataLoaded = value;
                    NotifyPropertyChanged("IsDataLoaded");
                }
            }
        }
    }
}