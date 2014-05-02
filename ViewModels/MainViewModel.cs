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
        public void LoadData()
        {
            // Sample data; replace with real data
            this.FoodItems.Add(new ImageDetails() { Id = "1", Category = "food", Url = "/Images/Dessert.jpg", Title = "Dessert", Message = "Awesome stuff." });
            this.FoodItems.Add(new ImageDetails() { Id = "2", Category = "food", Url = "/Images/Fruits.jpg", Title = "Fruits", Message = "Awesome stuff." });
            this.FoodItems.Add(new ImageDetails() { Id = "3", Category = "food", Url = "/Images/Pretzel.jpg", Title = "Pretzel", Message = "Awesome stuff." });
            this.FoodItems.Add(new ImageDetails() { Id = "4", Category = "food", Url = "/Images/Shrimp.jpg", Title = "Shrimp", Message = "Awesome stuff." });
            this.FoodItems.Add(new ImageDetails() { Id = "5", Category = "food", Url = "/Images/SteakSandwich.jpg", Title = "Steak\r\nSandwich", Message = "Awesome stuff." });
            this.FoodItems.Add(new ImageDetails() { Id = "6", Category = "food", Url = "/Images/Beignets.jpg", Title = "Beignets", Message = "Awesome stuff." });
            this.PlaceItems.Add(new ImageDetails() { Id = "7", Category = "place", Url = "/Images/Mürren.jpg", Title = "Mürren", Message = "Must visit" });
            this.PlaceItems.Add(new ImageDetails() { Id = "8", Category = "place", Url = "/Images/Seattle.jpg", Title = "Seattle", Message = "Must visit" });
            this.PlaceItems.Add(new ImageDetails() { Id = "9", Category = "place", Url = "/Images/Neuschwanstein.jpg", Title = "Neuschwanstein", Message = "Must visit" });
            this.PlaceItems.Add(new ImageDetails() { Id = "10", Category = "place", Url = "/Images/Paris.jpg", Title = "Paris", Message = "Must visit" });
            this.PlaceItems.Add(new ImageDetails() { Id = "11", Category = "place", Url = "/Images/Copenhagen.jpg", Title = "Copenhagen", Message = "Must visit" });
            this.PlaceItems.Add(new ImageDetails() { Id = "12", Category = "place", Url = "/Images/Venice.jpg", Title = "Venice", Message = "Must visit" });

            this.NoFoodItems = this.FoodItems.Count == 0;
            this.NoPlaceItems = this.PlaceItems.Count == 0;
            this.NoPeopleItems = this.PeopleItems.Count == 0;
            //this.Items.Add(new ImageDetails() { Category = "people", Url = "/Images/Fruits.jpg", Title = "Maecenas praesent accumsan bibendum", Message = "Maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur" });
            //this.Items.Add(new ImageDetails() { Category = "people", Url = "/Images/Pretzel.jpg", Title = "Dictumst eleifend facilisi faucibus", Message = "Pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent" });
            //this.Items.Add(new ImageDetails() { Category = "people", Url = "/Images/Shrimp.jpg", Title = "Habitant inceptos interdum lobortis", Message = "Accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat" });
            //this.Items.Add(new ImageDetails() { Category = "people", Url = "/Images/Beignets.jpg", Title = "Nascetur pharetra placerat pulvinar", Message = "Pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum" });

            this.IsDataLoaded = true;
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
    }
}