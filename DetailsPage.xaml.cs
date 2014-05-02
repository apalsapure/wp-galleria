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
using Microsoft.Phone.Shell;

namespace Galleria
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        private ItemViewModel _context;
        private List<ImageDetails> _contextItems;
        private int _index = 0;

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
                _contextItems = list;
                _index = list.IndexOf(item);
                DataContext = _context;

                txtTitle.Text = item.EscaptedTitle;
                txtMessage.Text = item.Message;
                if (!_context.IsDataLoaded)
                    _context.LoadData();
            }
        }

        private void appBarEdit_Click(object sender, EventArgs e)
        {
            gDetails.Visibility = System.Windows.Visibility.Collapsed;
            gEdit.Visibility = System.Windows.Visibility.Visible;
            progress.Visibility = System.Windows.Visibility.Collapsed;

            ApplicationBar.Buttons.RemoveAt(0);
            ApplicationBar.Buttons.RemoveAt(0);
            ApplicationBarIconButton appBarSaveButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.save.rest.png", UriKind.Relative));
            appBarSaveButton.Text = "save";
            appBarSaveButton.Click += appBarSave_Click;
            ApplicationBar.Buttons.Add(appBarSaveButton);

            ApplicationBarIconButton appBarCancelButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.close.rest.png", UriKind.Relative));
            appBarCancelButton.Text = "cancel";
            appBarCancelButton.Click += appBarCancel_Click;
            ApplicationBar.Buttons.Add(appBarCancelButton);
        }

        private async void appBarSave_Click(object sender, EventArgs e)
        {
            progress.Visibility = System.Windows.Visibility.Visible;
            _context.ImageDetails.Title = txtTitle.Text;
            _context.ImageDetails.Message = txtMessage.Text;
            if (await _context.ImageDetails.Save())
                appBarCancel_Click();
            else
            {
                _context.ImageDetails.Title = lblTitle.Text;
                _context.ImageDetails.Message = lblMessage.Text;
                MessageBox.Show("Failed to update the information. Please try again.", "Information !!!", MessageBoxButton.OK);
            }
            progress.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void appBarCancel_Click(object sender = null, EventArgs e = null)
        {
            gDetails.Visibility = System.Windows.Visibility.Visible;
            gEdit.Visibility = System.Windows.Visibility.Collapsed;

            ApplicationBar.Buttons.RemoveAt(0);
            ApplicationBar.Buttons.RemoveAt(0);
            ApplicationBarIconButton appBarEditButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.edit.rest.png", UriKind.Relative));
            appBarEditButton.Text = "edit";
            appBarEditButton.Click += appBarEdit_Click;
            ApplicationBar.Buttons.Add(appBarEditButton);

            ApplicationBarIconButton appBarDeleteButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.delete.rest.png", UriKind.Relative));
            appBarDeleteButton.Text = "delete";
            appBarDeleteButton.Click += appBarDelete_Click;
            ApplicationBar.Buttons.Add(appBarDeleteButton);
        }

        private async void appBarDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("This action can not be undone. \nAre you sure you want to delete the image?", "Warning !!!", MessageBoxButton.OKCancel);
            if (result != MessageBoxResult.OK) return;
            if (await _context.ImageDetails.Delete() == false)
            {
                MessageBox.Show("Failed to delete the image. Please try again.");
            }
            else
            {
                App.ViewModel.RemoveItem(_context.ImageDetails);
                NavigationService.GoBack();
            }

        }

        private void GestureListener_Flick(object sender, FlickGestureEventArgs e)
        {
            if (gEdit.Visibility == System.Windows.Visibility.Visible) return;
            ImageDetails item = null;
            if (e.HorizontalVelocity < 0)
            {
                if (_index + 1 == _contextItems.Count) return;
                _index++;
                //load next
                item = _contextItems[_index];
            }
            else if (e.HorizontalVelocity > 0)
            {
                if (_index == 0) return;
                //load previous
                _index--;
                item = _contextItems[_index];
            }
            _context = new ItemViewModel(item);
            DataContext = _context;
            if (!_context.IsDataLoaded)
            {
                _context.LoadData();
            }
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Image.xaml?selectedItem=" + _context.ImageDetails.Id + "&category=" + _context.ImageDetails.Category, UriKind.Relative));
        }
    }
}