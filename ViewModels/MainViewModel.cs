using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Appacitive.Sdk;


namespace Galleria
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.FoodItems = new ObservableCollection<ImageDetails>();
            this.PlaceItems = new ObservableCollection<ImageDetails>();
            this.PeopleItems = new ObservableCollection<ImageDetails>();
        }

        /// <summary>
        /// A collection for ImageDetails objects.
        /// </summary>
        public ObservableCollection<ImageDetails> FoodItems { get; private set; }
        public ObservableCollection<ImageDetails> PlaceItems { get; private set; }
        public ObservableCollection<ImageDetails> PeopleItems { get; private set; }

        private bool _isDataLoaded;
        public bool IsDataLoaded
        {
            get { return _isDataLoaded; }
            set
            {
                _isDataLoaded = value;
                NotifyPropertyChanged("IsDataLoaded");
            }
        }

        private bool _noFoodItems;
        public bool NoFoodItems
        {
            get { return _noFoodItems; }
            set
            {
                _noFoodItems = value;
                NotifyPropertyChanged("NoFoodItems");
            }
        }

        private bool _noPlaceItems;
        public bool NoPlaceItems
        {
            get { return _noPlaceItems; }
            set
            {
                _noPlaceItems = value;
                NotifyPropertyChanged("NoPlaceItems");
            }
        }

        private bool _noPeopleItems;
        public bool NoPeopleItems
        {
            get { return _noPeopleItems; }
            set
            {
                _noPeopleItems = value;
                NotifyPropertyChanged("NoPeopleItems");
            }
        }

        /// <summary>
        /// Creates and adds a few ImageDetails objects into the Items collection.
        /// </summary>
        public async void LoadData()
        {
            this.IsDataLoaded = false;

            //get all images which are public or uploaded by me
            var query = Appacitive.Sdk.Query.Or(new[]{
                                    Appacitive.Sdk.Query.Property("ispublic").IsEqualTo(true),
                                    Appacitive.Sdk.Query.Property("__createdby").IsEqualTo(AppContext.UserContext.LoggedInUser.Id)
                        });

            
            //fire the query
            var result = await Appacitive.Sdk.APObjects.FindAllAsync("image", query, pageSize: 50,
                                                                      fields: new List<string>{ "title", "message", "url", "category", "__createdby"},
                                                                      orderBy: "__utcdatecreated",
                                                                      sortOrder: Appacitive.Sdk.SortOrder.Ascending);
            //iterate over result object and add todolist item to the list 
            while (true)
            {
                result.ForEach(r =>
                {
                    var imageDetails = r as ImageDetails;
                    switch (imageDetails.Category.ToLower())
                    {
                        case "place":
                            App.ViewModel.PlaceItems.Add(imageDetails);
                            break;
                        case "people":
                            App.ViewModel.PeopleItems.Add(imageDetails);
                            break;
                        default:
                            App.ViewModel.FoodItems.Add(imageDetails);
                            break;
                    }
                });
                
                //check if all pages are retrieved
                if (result.IsLastPage) break;
                //fetch next page
                result = await result.NextPageAsync();
            }

            
            this.NoFoodItems = this.FoodItems.Count == 0;
            this.NoPlaceItems = this.PlaceItems.Count == 0;
            this.NoPeopleItems = this.PeopleItems.Count == 0;

            this.IsDataLoaded = true;
        }

        public void AddItem(ImageDetails item)
        {
            //depending upon category, update ViewModel
            switch (item.Category)
            {
                case "place":
                    App.ViewModel.PlaceItems.Add(item);
                    App.ViewModel.NoPlaceItems = false;
                    break;
                case "people":
                    App.ViewModel.PeopleItems.Add(item);
                    App.ViewModel.NoPeopleItems = false;
                    break;
                default:
                    App.ViewModel.FoodItems.Add(item);
                    App.ViewModel.NoFoodItems = false;
                    break;
            }
        }

        public void RemoveItem(ImageDetails item)
        {
            App.ViewModel.FoodItems.Remove(item);
            App.ViewModel.PeopleItems.Remove(item);
            App.ViewModel.PlaceItems.Remove(item);

            this.NoFoodItems = this.FoodItems.Count == 0;
            this.NoPlaceItems = this.PlaceItems.Count == 0;
            this.NoPeopleItems = this.PeopleItems.Count == 0;
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
    }
}