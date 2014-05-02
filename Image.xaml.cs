using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace Galleria
{
    public partial class Image : PhoneApplicationPage
    {
        public Image()
        {
            InitializeComponent();
        }

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
                DataContext = item;
            }
            base.OnNavigatedTo(e);
        }

        private double _imageScale = 1d;
        private Point _imageTranslation = new Point(0, 0);
        private Point _fingerOne;
        private Point _fingerTwo;
        private double _previousScale;

        private void OnPinchStarted(object s, PinchStartedGestureEventArgs e)
        {
            _fingerOne = e.GetPosition(MyImage, 0);
            _fingerTwo = e.GetPosition(MyImage, 1);
            _previousScale = 1;
        }

        private void OnPinchDelta(object s, PinchGestureEventArgs e)
        {
            var newScale = e.DistanceRatio / _previousScale;
            var currentFingerOne = e.GetPosition(MyImage, 0);
            var currentFingerTwo = e.GetPosition(MyImage, 1);
            var translationDelta = GetTranslationOffset(currentFingerOne,
            currentFingerTwo, _fingerOne, _fingerTwo, _imageTranslation, newScale);
            _fingerOne = currentFingerOne;
            _fingerTwo = currentFingerTwo;
            _previousScale = e.DistanceRatio;
            UpdatePicture(newScale, translationDelta);
        }

        private void UpdatePicture(double scaleFactor, Point delta)
        {
            var newscale = _imageScale * scaleFactor;
            var transform = (CompositeTransform)MyImage.RenderTransform;
            if (newscale > 1)
            {
                _imageScale *= scaleFactor;
                _imageTranslation = new Point
                (_imageTranslation.X + delta.X, _imageTranslation.Y + delta.Y);
                transform.ScaleX = _imageScale;
                transform.ScaleY = _imageScale;
                transform.TranslateX = _imageTranslation.X;
                transform.TranslateY = _imageTranslation.Y;
            }
            else
            {
                transform.TranslateX = 0;
                transform.TranslateY = 0;
                transform.ScaleX = transform.ScaleY = 1;
                _imageTranslation = new Point(0, 0);
            }
        }

        private Point GetTranslationOffset(Point currentFingerOne, Point currentFingerTwo,
        Point oldFingerOne, Point oldFingerTwo, Point currentPosition, double scale)
        {
            var newFingerOnePosition = new Point(
                currentFingerOne.X + (currentPosition.X - oldFingerOne.X) * scale,
                currentFingerOne.Y + (currentPosition.Y - oldFingerOne.Y) * scale);
            var newFingerTwoPosition = new Point(
                currentFingerTwo.X + (currentPosition.X - oldFingerTwo.X) * scale,
                currentFingerTwo.Y + (currentPosition.Y - oldFingerTwo.Y) * scale);
            var newPosition = new Point(
                (newFingerOnePosition.X + newFingerTwoPosition.X) / 2,
                (newFingerOnePosition.Y + newFingerTwoPosition.Y) / 2);
            return new Point(
                newPosition.X - currentPosition.X,
                newPosition.Y - currentPosition.Y);
        }

        private void PhoneApplicationPage_OrientationChanged
            (object sender, OrientationChangedEventArgs e)
        {
            if (e.Orientation == PageOrientation.Landscape ||
                e.Orientation == PageOrientation.LandscapeLeft ||
                e.Orientation == PageOrientation.LandscapeRight)
            {
                MyImage.Width = 720;
                MyImage.Height = 480;
            }
            else
            {
                MyImage.Width = 480;
                MyImage.Height = 720;
            }
        }

        private void GestureListener_DragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            var transform = (CompositeTransform)MyImage.RenderTransform;
            var newx = transform.TranslateX + e.HorizontalChange;
            var newy = transform.TranslateY + e.VerticalChange;
            if (newx < 0 && (MyImage.Width * transform.ScaleX - MyImage.Width > Math.Abs(newx)))
                transform.TranslateX = newx;
            if (newy < 0 && (MyImage.Height * transform.ScaleY - MyImage.Height > Math.Abs(newy)))
                transform.TranslateY = newy;
            _imageTranslation = new Point(transform.TranslateX, transform.TranslateY);
        }
    }
}