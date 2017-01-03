﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UnclePaulsSuperFancyPhotoSoftware
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<ImageHolder> importedImagesList = new List<ImageHolder>();
        List<ImageSource> exportedImagesList = new List<ImageSource>();
        int importOffset = 0;
        int exportOffset = 0;
        List<Image> importUIImages = new List<Image>();
        List<Image> exportUIImages = new List<Image>();

        Grid layoutRoot;
        List<CropSquare> cropSquares = new List<CropSquare>();

        ImageHolder SelectedImage;

        public MainPage()
        {
            this.InitializeComponent();
            importUIImages.Add(importimage0);
            importUIImages.Add(importimage1);
            importUIImages.Add(importimage2);
            importUIImages.Add(importimage3);
            importUIImages.Add(importimage4);
            exportUIImages.Add(exportImage0);
            exportUIImages.Add(exportImage1);
            exportUIImages.Add(exportImage2);
            exportUIImages.Add(exportImage3);
            exportUIImages.Add(exportImage4);
            layoutRoot = this.photoSquare;
            SelectedImage = new ImageHolder();
        }

        private async void importImages_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var files = await picker.PickMultipleFilesAsync();

            ImageHolder tempHolder;
            // Application now has read/write access to the picked file(s)
            foreach (Windows.Storage.StorageFile file in files)
            {
                tempHolder = new ImageHolder();
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                var image = new BitmapImage();
                tempHolder.Image = image;
                tempHolder.File = file;
                image.SetSource(stream);
                importedImagesList.Add(tempHolder
                    );
            }
            UpdateImportUIImages();
        }
        public void UpdateImportUIImages()
        {
            for (int i = 0; i < importUIImages.Count; i++)
            {
                if (importedImagesList.Count > i)
                {
                    importUIImages[i].Source = importedImagesList[i + importOffset].Image;
                }
                else
                {
                    break;
                }
            }
        }
        public void UpdateExportUIImages()
        {
            for (int i = 0; i < exportUIImages.Count; i++)
            {
                if (exportedImagesList.Count > i)
                {
                    exportUIImages[i].Source = exportedImagesList[i];
                }
                else
                {
                    break;
                }
            }
        }
        private void MoveImportedImagesDown_Click(object sender, RoutedEventArgs e)
        {
            if (importOffset + importUIImages.Count < importedImagesList.Count)
            {
                importOffset++;
                UpdateImportUIImages();
            }
        }

        private void MoveImportedImagesUp_Click(object sender, RoutedEventArgs e)
        {
            if (importOffset > 0)
            {
                importOffset--;
                UpdateImportUIImages();
            }
        }

        private void SetMainPicture(int number)
        {
            mainImage.Source = importedImagesList[number + importOffset].Image;
            SelectedImage.File = importedImagesList[number + importOffset].File;
            SelectedImage.Image = importedImagesList[number + importOffset].Image;
        }


        private void importUIImage0Selected_Click(object sender, RoutedEventArgs e)
        {
            SetMainPicture(0);
        }
        private void importUIImage1Selected_Click(object sender, RoutedEventArgs e)
        {
            SetMainPicture(1);
        }
        private void importUIImage2Selected_Click(object sender, RoutedEventArgs e)
        {
            SetMainPicture(2);
        }
        private void importUIImage3Selected_Click(object sender, RoutedEventArgs e)
        {
            SetMainPicture(3);
        }
        private void importUIImage4Selected_Click(object sender, RoutedEventArgs e)
        {
            SetMainPicture(4);
        }

        private void addCutter_Click(object sender, RoutedEventArgs e)
        {
            var newCrop = new CropSquare();
            cropSquares.Add(newCrop);
            layoutRoot.Children.Add(newCrop.Rectangle);
            layoutRoot.Children.Add(newCrop.x);
            layoutRoot.Children.Add(newCrop.y);
            layoutRoot.Children.Add(newCrop.z);
            layoutRoot.Children.Add(newCrop.w);
            layoutRoot.Children.Add(newCrop.rotation);
            RenederCropSquare(newCrop);
        }

        private void photoSquare_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (CropSquare cropSquare in cropSquares)
            {
                RenederCropSquare(cropSquare);
            }
        }
        void RenederCropSquare(CropSquare cropSquare)
        {
            cropSquare.Rectangle.Fill = new SolidColorBrush(Windows.UI.Colors.Transparent);
            cropSquare.Rectangle.Width = mainImage.ActualWidth * cropSquare.Width * Math.Cos(cropSquare.rotate.Angle * (Math.PI / 180)) + mainImage.ActualHeight * cropSquare.Height * Math.Sin(cropSquare.rotate.Angle * (Math.PI / 180));
            cropSquare.Rectangle.Height = mainImage.ActualHeight * cropSquare.Height * Math.Cos(cropSquare.rotate.Angle * (Math.PI / 180)) + mainImage.ActualWidth * cropSquare.Width * Math.Sin(cropSquare.rotate.Angle * (Math.PI / 180));
            cropSquare.Rectangle.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
            cropSquare.Rectangle.StrokeThickness = (mainImage.ActualWidth + mainImage.ActualHeight) * .0005;
            cropSquare.Rectangle.HorizontalAlignment = HorizontalAlignment.Left;
            cropSquare.Rectangle.VerticalAlignment = VerticalAlignment.Top;
            cropSquare.Rectangle.Margin = new Thickness(mainImage.ActualWidth * cropSquare.Left + Math.Abs((layoutRoot.RenderSize.Width - mainImage.RenderSize.Width) / 2), mainImage.ActualHeight * cropSquare.Top + Math.Abs((layoutRoot.RenderSize.Height - mainImage.RenderSize.Height) / 2), 0, 0);
            cropSquare.rotate.Angle = 0;
            cropSquare.Rectangle.RenderTransform = cropSquare.rotate;
            Grid.SetColumn(cropSquare.Rectangle, 1);

            cropSquare.x.Fill = new SolidColorBrush(Windows.UI.Colors.SteelBlue);
            cropSquare.x.Width = (mainImage.ActualWidth + mainImage.ActualHeight) * .004;
            cropSquare.x.Height = (mainImage.ActualWidth + mainImage.ActualHeight) * .004;
            Grid.SetColumn(cropSquare.x, 1);
            cropSquare.x.Margin = new Thickness(cropSquare.Rectangle.Margin.Left - cropSquare.x.Width / 2, cropSquare.Rectangle.Margin.Top - cropSquare.x.Height / 2, 0, 0);
            cropSquare.x.HorizontalAlignment = HorizontalAlignment.Left;
            cropSquare.x.VerticalAlignment = VerticalAlignment.Top;

            cropSquare.y.Fill = new SolidColorBrush(Windows.UI.Colors.SteelBlue);
            cropSquare.y.Width = (mainImage.ActualWidth + mainImage.ActualHeight) * .004;
            cropSquare.y.Height = (mainImage.ActualWidth + mainImage.ActualHeight) * .004;
            Grid.SetColumn(cropSquare.y, 1);
            cropSquare.y.Margin = new Thickness(cropSquare.Rectangle.Margin.Left - cropSquare.y.Width / 2 + cropSquare.Rectangle.Width * Math.Cos(cropSquare.rotate.Angle * (Math.PI / 180)), cropSquare.Rectangle.Margin.Top - cropSquare.y.Height / 2 + cropSquare.Rectangle.Width * Math.Sin(cropSquare.rotate.Angle * (Math.PI / 180)), 0, 0);
            cropSquare.y.HorizontalAlignment = HorizontalAlignment.Left;
            cropSquare.y.VerticalAlignment = VerticalAlignment.Top;

            cropSquare.z.Fill = new SolidColorBrush(Windows.UI.Colors.SteelBlue);
            cropSquare.z.Width = (mainImage.ActualWidth + mainImage.ActualHeight) * .004;
            cropSquare.z.Height = (mainImage.ActualWidth + mainImage.ActualHeight) * .004;
            Grid.SetColumn(cropSquare.z, 1);
            double hyp = Math.Sqrt(cropSquare.Rectangle.Height * cropSquare.Rectangle.Height + cropSquare.Rectangle.Width * cropSquare.Rectangle.Width);
            cropSquare.z.Margin = new Thickness(cropSquare.Rectangle.Margin.Left + hyp * Math.Cos((cropSquare.rotate.Angle) * (Math.PI / 180) + Math.Atan(cropSquare.Rectangle.Height / cropSquare.Rectangle.Width)) - cropSquare.z.Width / 2,
                cropSquare.Rectangle.Margin.Top + hyp * Math.Sin((cropSquare.rotate.Angle) * (Math.PI / 180) + Math.Atan(cropSquare.Rectangle.Height / cropSquare.Rectangle.Width)) - cropSquare.z.Height / 2, 0, 0);
            cropSquare.z.HorizontalAlignment = HorizontalAlignment.Left;
            cropSquare.z.VerticalAlignment = VerticalAlignment.Top;

            cropSquare.w.Fill = new SolidColorBrush(Windows.UI.Colors.SteelBlue);
            cropSquare.w.Width = (mainImage.ActualWidth + mainImage.ActualHeight) * .004;
            cropSquare.w.Height = (mainImage.ActualWidth + mainImage.ActualHeight) * .004;
            Grid.SetColumn(cropSquare.w, 1);
            cropSquare.w.Margin = new Thickness(cropSquare.Rectangle.Margin.Left - cropSquare.z.Width / 2 - cropSquare.Rectangle.Height * Math.Sin(cropSquare.rotate.Angle * (Math.PI / 180)), cropSquare.Rectangle.Margin.Top + cropSquare.Rectangle.Height * Math.Cos(cropSquare.rotate.Angle * (Math.PI / 180)) - cropSquare.z.Height / 2, 0, 0);
            cropSquare.w.HorizontalAlignment = HorizontalAlignment.Left;
            cropSquare.w.VerticalAlignment = VerticalAlignment.Top;

            cropSquare.rotation.Fill = new SolidColorBrush(Windows.UI.Colors.SteelBlue);
            cropSquare.rotation.Width = (mainImage.ActualWidth + mainImage.ActualHeight) * .004;
            cropSquare.rotation.Height = (mainImage.ActualWidth + mainImage.ActualHeight) * .004;
            Grid.SetColumn(cropSquare.rotation, 1);
            hyp = Math.Sqrt(400 + cropSquare.Rectangle.Width * cropSquare.Rectangle.Width / 4);
            double anel = Math.Atan(20 / (cropSquare.Rectangle.Width / 2));
            cropSquare.rotation.Margin = new Thickness(cropSquare.Rectangle.Margin.Left - cropSquare.y.Width / 2 + hyp * Math.Cos(anel - cropSquare.rotate.Angle * (Math.PI / 180)), cropSquare.Rectangle.Margin.Top - cropSquare.y.Height / 2 - hyp * Math.Sin(anel - cropSquare.rotate.Angle * (Math.PI / 180)), 0, 0);
            cropSquare.rotation.HorizontalAlignment = HorizontalAlignment.Left;
            cropSquare.rotation.VerticalAlignment = VerticalAlignment.Top;
        }

        private async void cut_Click(object sender, RoutedEventArgs e)
        {
            Point x;
            Point y;
            Point z;
            Point w;
            ImageHolder TempImage;
            foreach (CropSquare cropSquare in cropSquares)
            {
                x = new Point(cropSquare.Left * SelectedImage.Image.PixelWidth, cropSquare.Top * SelectedImage.Image.PixelHeight);
                y = new Point(cropSquare.Left * SelectedImage.Image.PixelWidth + cropSquare.Width * SelectedImage.Image.PixelWidth * Math.Cos(cropSquare.rotate.Angle * (Math.PI / 180)),
                     (cropSquare.Top * SelectedImage.Image.PixelHeight + cropSquare.Width * SelectedImage.Image.PixelWidth * Math.Sin(cropSquare.rotate.Angle * (Math.PI / 180))));


                double hyp = Math.Sqrt(cropSquare.Height * cropSquare.Height * SelectedImage.Image.PixelHeight * SelectedImage.Image.PixelHeight + cropSquare.Width * cropSquare.Width * SelectedImage.Image.PixelWidth * SelectedImage.Image.PixelWidth);

                z = new Point(cropSquare.Width * SelectedImage.Image.PixelWidth + hyp * Math.Cos((cropSquare.rotate.Angle) * (Math.PI / 180) + Math.Atan((cropSquare.Height * SelectedImage.Image.PixelHeight) / (cropSquare.Width * SelectedImage.Image.PixelWidth))),
                     (cropSquare.Top * SelectedImage.Image.PixelHeight + hyp * Math.Sin((cropSquare.rotate.Angle) * (Math.PI / 180) + Math.Atan((cropSquare.Height * SelectedImage.Image.PixelHeight) / (cropSquare.Width * SelectedImage.Image.PixelWidth)))));

                w = new Point(cropSquare.Left * SelectedImage.Image.PixelWidth - cropSquare.Height * SelectedImage.Image.PixelHeight * Math.Sin(cropSquare.rotate.Angle * (Math.PI / 180)),
                     (cropSquare.Top * SelectedImage.Image.PixelHeight + cropSquare.Height * SelectedImage.Image.PixelHeight * Math.Cos(cropSquare.rotate.Angle * (Math.PI / 180))));
                TempImage = new ImageHolder();
                exportedImagesList.Add( await CSWindowsStoreAppCropBitmap.CropBitmap.GetCroppedBitmapAsync(SelectedImage.File, x, new Size(Math.Abs(x.X - y.X), Math.Abs(x.Y - w.Y)), 1));
                UpdateExportUIImages();
            }

        }
        public class CropSquare
        {
            public Rectangle Rectangle = new Rectangle();
            public Ellipse x = new Ellipse();
            public Ellipse y = new Ellipse();
            public Ellipse z = new Ellipse();
            public Ellipse w = new Ellipse();
            public Ellipse rotation = new Ellipse();
            public RotateTransform rotate = new RotateTransform();
            public double Width = .1;
            public double Height = .1;
            public double Left = .1;
            public double Top = .1;
            public CropSquare()
            {
                Width = .1;
                Height = .1;
                Left = .1;
                Top = .1;
            }
        }

        public class ImageHolder
        {
            public StorageFile File;
            public BitmapImage Image;
        }
    }
}
