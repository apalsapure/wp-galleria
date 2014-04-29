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
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.IO;

namespace Galleria
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            gPivot.Visibility = System.Windows.Visibility.Visible;
            gAddImage.Visibility = System.Windows.Visibility.Collapsed;
            lstCategory.ItemsSource = new List<string> { "Food", "Places", "People" };
            txtTitle.TextChanged += txtTitle_TextChanged;
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pivot = (sender as Pivot);
            if (pivot.SelectedIndex == 0)
            {
                HubTileService.UnfreezeGroup("food");
                HubTileService.FreezeGroup("places");
                HubTileService.FreezeGroup("people");
            }
            else if (pivot.SelectedIndex == 1)
            {
                HubTileService.UnfreezeGroup("places");
                HubTileService.FreezeGroup("food");
                HubTileService.FreezeGroup("people");
            }
            else
            {
                HubTileService.FreezeGroup("food");
                HubTileService.FreezeGroup("places");
                HubTileService.UnfreezeGroup("people");
            }
        }

        // Handle selection changed on ListBox
        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            // If selected index is -1 (no selection) do nothing
            if (listBox.SelectedIndex == -1) return;

            var selectedItem = listBox.SelectedItem as ImageDetails;

            // Navigate to the new page
            NavigationService.Navigate(new Uri(String.Format("/DetailsPage.xaml?selectedItem={0}&category={1}", selectedItem.Id, selectedItem.Category), UriKind.Relative));

            // Reset selected index to -1 (no selection)
            listBox.SelectedIndex = -1;
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void appBarAdd_Click(object sender, EventArgs e)
        {
            gPivot.Visibility = System.Windows.Visibility.Collapsed;
            gAddImage.Visibility = System.Windows.Visibility.Visible;

            //txtListName.Focus();
            ApplicationBar.Buttons.RemoveAt(0);
            ApplicationBarIconButton appBarSaveButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.save.rest.png", UriKind.Relative));
            appBarSaveButton.Text = "save";
            appBarSaveButton.IsEnabled = false;
            appBarSaveButton.Click += appBarSave_Click;
            ApplicationBar.Buttons.Add(appBarSaveButton);

            ApplicationBarIconButton appBarCancelButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.close.rest.png", UriKind.Relative));
            appBarCancelButton.Text = "cancel";
            appBarCancelButton.Click += appBarCancel_Click;
            ApplicationBar.Buttons.Add(appBarCancelButton);
        }

        private void appBarCancel_Click(object sender, EventArgs e)
        {
            gPivot.Visibility = System.Windows.Visibility.Visible;
            gAddImage.Visibility = System.Windows.Visibility.Collapsed;
            lock (App.ViewModel)
            {
                ApplicationBar.Buttons.RemoveAt(0);
                ApplicationBar.Buttons.RemoveAt(0);
            }
            ApplicationBarIconButton appBarAddButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
            appBarAddButton.Text = "add";
            appBarAddButton.Click += appBarAdd_Click;
            ApplicationBar.Buttons.Add(appBarAddButton);
        }

        private void appBarSave_Click(object sender, EventArgs e)
        {

        }

        private void Camera_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            CameraCaptureTask cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += imageTask_Completed;

            cameraCaptureTask.Show();
        }

        private void Media_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += imageTask_Completed;
            photoChooserTask.Show();
        }

        MemoryStream imageStream;

        private void imageTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult != TaskResult.OK) return;
            BitmapImage image = new BitmapImage();
            image.SetSource(e.ChosenPhoto);
            Image imageC = new Image();
            imageC.Source = image;
            WriteBitmap(imageC);

            imageStream.Seek(0, SeekOrigin.Begin);
            EnableSaveButton();
        }

        void txtTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableSaveButton();
        }

        private void EnableSaveButton()
        {
            if (txtTitle.Text.Trim() == string.Empty || imageStream == null) return;
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
        }

        private void WriteBitmap(FrameworkElement element)
        {
            WriteableBitmap wBitmap = new WriteableBitmap(element, null);
            imageStream = new MemoryStream();
            wBitmap.SaveJpeg(imageStream, (int)element.ActualWidth, (int)element.ActualHeight, 0, 100);
        }
    }

}