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
using System.Windows.Navigation;
using System.Threading.Tasks;

namespace Galleria
{
    public partial class MainPage : PhoneApplicationPage
    {
        private bool _isFirstTime = true;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            gSingUp.Visibility = System.Windows.Visibility.Collapsed;
            gPivot.Visibility = System.Windows.Visibility.Collapsed;
            gAddImage.Visibility = System.Windows.Visibility.Collapsed;
            gSingIn.Visibility = System.Windows.Visibility.Collapsed;

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;

            txtEmail.Text = "a@a.com";
            txtPassword.Password = "aa";

            lstCategory.ItemsSource = new List<string> { "Food", "Place", "People" };
            txtTitle.TextChanged += txtTitle_TextChanged;
        }

        // Load data for the ViewModel Items
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (_isFirstTime)
            {
                _isFirstTime = false;
                progress.Visibility = System.Windows.Visibility.Visible;
                if (await User.IsLoggedIn())
                {
                    gPivot.Visibility = System.Windows.Visibility.Visible;
                    ShowList();
                    return;
                }
                gSingIn.Visibility = System.Windows.Visibility.Visible;
            }
            //hide progress bar
            progress.Visibility = System.Windows.Visibility.Collapsed;
        }

        #region Event Handlers
        //user is signing in
        private async void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            //disable sign in button
            btnSignIn.IsEnabled = false;
            progress.Visibility = System.Windows.Visibility.Visible;

            //Logic to Authenticate User will go here
            //Once user is authenticated, show todo list
            var result = await User.Authenticate(txtEmail.Text, txtPassword.Password);
            if (string.IsNullOrEmpty(result) == false)
            {
                MessageBox.Show("Oops please check your email address and password", "Sign in Failed", MessageBoxButton.OK);
                progress.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                ShowList();
            }

            //enable sign in
            btnSignIn.IsEnabled = true;
        }

        //User is signing up
        private async void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            //disable signup button
            btnSignUp.IsEnabled = false;
            //show progress bar
            progress.Visibility = System.Windows.Visibility.Visible;

            //create a user object and persist locally
            var split = txtRName.Text.Split(' ');
            string lastName = string.Empty;
            if (split.Length > 1) lastName = string.Join(" ", split.Skip(1));

            //save user
            var user = new User(txtREmail.Text, txtRPassword.Password, split[0], lastName);
            if (await user.Save() == false)
            {
                MessageBox.Show("Oops some thing went wrong, check your network connection.", "Sign up failed", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("Login with your email address and password.", "Sign up successful", MessageBoxButton.OK);
                txtEmail.Text = txtREmail.Text;
                txtREmail.Text = txtRName.Text = txtRPassword.Password = string.Empty;
                lnkSignIn_Click();
            }

            //hide progress bar
            progress.Visibility = System.Windows.Visibility.Collapsed;
            //enable signup button
            btnSignUp.IsEnabled = true;
        }

        //user clicked signup link
        private void lnkSignUp_Click(object sender, RoutedEventArgs e)
        {
            gSingIn.Visibility = System.Windows.Visibility.Collapsed;
            gSingUp.Visibility = System.Windows.Visibility.Visible;
        }

        //user clicked signin link
        private void lnkSignIn_Click(object sender = null, RoutedEventArgs e = null)
        {
            gSingIn.Visibility = System.Windows.Visibility.Visible;
            gSingUp.Visibility = System.Windows.Visibility.Collapsed;
        }

        //user signing out
        private async void menuSignOut_Click(object sender, EventArgs e)
        {
            progress.Visibility = System.Windows.Visibility.Visible;
            if (await Context.User.Logout() == false) MessageBox.Show("Some error occurred, could not sign out.", "Sign out operation failed", MessageBoxButton.OK);
            else
            {
                //clean up
                if (App.ViewModel.FoodItems != null) App.ViewModel.FoodItems.Clear();
                if (App.ViewModel.PlaceItems != null) App.ViewModel.PlaceItems.Clear();
                if (App.ViewModel.PeopleItems != null) App.ViewModel.PeopleItems.Clear();
                txtEmail.Text = txtPassword.Password = string.Empty;
                gPivot.Visibility = System.Windows.Visibility.Collapsed;
                gSingIn.Visibility = System.Windows.Visibility.Visible;
            }
            progress.Visibility = System.Windows.Visibility.Collapsed;
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

        private void appBarAdd_Click(object sender, EventArgs e)
        {
            gPivot.Visibility = System.Windows.Visibility.Collapsed;
            gAddImage.Visibility = System.Windows.Visibility.Visible;

            txtTitle.Focus();
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

        private void appBarCancel_Click(object sender = null, EventArgs e = null)
        {
            txtTitle.Text = "";
            txtMessage.Text = "";
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

        private async void appBarSave_Click(object sender, EventArgs e)
        {
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
            progress.Visibility = System.Windows.Visibility.Visible;
            await UploadFile();
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

        void txtTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableSaveButton();
        }

        private void imageTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult != TaskResult.OK) return;
            BitmapImage image = new BitmapImage();
            stream = ImageUtil.HandleOrientation(e.ChosenPhoto, e.OriginalFileName);
            stream = ImageUtil.Compress(stream);
            EnableSaveButton();
        }
        #endregion

        Stream stream;

        private async Task UploadFile()
        {
            try
            {
                string fileName = DateTime.Now.Ticks.ToString() + ".jpg";
                var upload = new Appacitive.Sdk.FileUpload("image/jpeg", fileName, 30);
                var uploadUrl = await upload.GetUploadUrlAsync();

                const int BLOCK_SIZE = 4096;
                WebClient wc = new WebClient();
                wc.Headers["Content-Type"] = "image/jpeg";
                wc.AllowReadStreamBuffering = true;
                wc.AllowWriteStreamBuffering = true;
                wc.OpenWriteCompleted += (s, args) =>
                {
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        using (BinaryWriter bw = new BinaryWriter(args.Result))
                        {
                            long bCount = 0;
                            long fileSize = stream.Length;
                            byte[] bytes = new byte[BLOCK_SIZE];
                            do
                            {
                                bytes = br.ReadBytes(BLOCK_SIZE);
                                bCount += bytes.Length;
                                bw.Write(bytes);
                            } while (bCount < fileSize);
                        }
                    }
                };

                // what to do when writing is complete
                wc.WriteStreamClosed += async (s, args) =>
                {
                    var download = new Appacitive.Sdk.FileDownload(fileName);
                    string publicUrl = await download.GetPublicUrlAsync();

                    await SaveImageDetails(publicUrl);
                };

                // Write to the WebClient
                wc.OpenWriteAsync(new Uri(uploadUrl.Url, UriKind.Absolute), "PUT");
            }
            catch
            {
                MessageBox.Show("Failed to upload the image. Please try again.");
                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
                progress.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private async Task SaveImageDetails(string url)
        {
            var imageDetails = new ImageDetails();
            imageDetails.Title = txtTitle.Text;
            imageDetails.Message = txtMessage.Text;
            imageDetails.Category = lstCategory.SelectedItem.ToString().ToLower();
            imageDetails.Url = url;
            imageDetails.IsPublic = chkPublic.IsChecked == true;

            if (await imageDetails.Save() == false)
            {
                MessageBox.Show("Failed to upload the image. Please try again.");
                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
                progress.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                //add new item to the list
                App.ViewModel.AddItem(imageDetails);
                
                progress.Visibility = System.Windows.Visibility.Collapsed;
                appBarCancel_Click();
            }
        }

        private void EnableSaveButton()
        {
            if (txtTitle.Text.Trim() == string.Empty || stream == null) return;
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
        }

        private void ShowList()
        {
            //hide remaining views
            gSingIn.Visibility = System.Windows.Visibility.Collapsed;
            gSingUp.Visibility = System.Windows.Visibility.Collapsed;

            //show todo list view
            gPivot.Visibility = System.Windows.Visibility.Visible;

            //load todo items
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            //show the application bar
            ApplicationBar.IsVisible = true;
            progress.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}