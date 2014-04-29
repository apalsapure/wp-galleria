using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace Galleria
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        private ItemViewModel _context;
        // Constructor
        public DetailsPage()
        {
            InitializeComponent();
        }

        // When page is navigated to set data context to selected item in list
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string selectedIndex = "";
            if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
            {
                var list = new List<ImageDetails>();
                var category = "food";
                NavigationContext.QueryString.TryGetValue("category", out category);

                switch (category)
                {
                    case "food": list = App.ViewModel.FoodItems.ToList(); break;
                    case "place": list = App.ViewModel.PlaceItems.ToList(); break;
                    case "people": list = App.ViewModel.PeopleItems.ToList(); break;
                }

                var item = list.Where(i => i.Id == selectedIndex).FirstOrDefault();
                _context = new ItemViewModel(item);
                DataContext = _context;
                if (!_context.IsDataLoaded)
                {
                    _context.LoadData();
                }

            }
        }
    }
}