using App6.Model;
using App6.Service;
using App6.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App6
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewPage : Page
    {
        public NewPage()
        {
            this.InitializeComponent();
            //this.InitFile();
        }
        private async void InitFile()
        {
            file = await ApplicationData.Current.LocalFolder.GetFileAsync("1.png");
            token.Text = file.Name;
        }
        private ListItemViewModel viewModel;
        private StorageFile file;
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            this.viewModel = ((App)App.Current).ViewModel;
            if (this.viewModel.SeletedItem != null)
            {
                createBtn.Content = "Update";
                title.Text = this.viewModel.SeletedItem.Title;
                detail.Text = this.viewModel.SeletedItem.Desc;
                image.Source = this.viewModel.SeletedItem.Pic;
                date.Date = this.viewModel.SeletedItem.Date;
                itemId.Text = this.viewModel.SeletedItem.Id;
                token.Text = this.viewModel.SeletedItem.ImgPath;
                file = this.viewModel.SeletedItem.File;
                deleteAppBarbutton.Visibility = Visibility.Visible;
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("newpage"))
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["newpage"] as ApplicationDataCompositeValue;
                    title.Text = (string)composite["title"];
                    itemId.Text = (string)composite["itemId"];

                    if (composite["imageToken"] != null)
                    {
                        token.Text = (string)composite["imageToken"];
                        if (token.Text != null && !token.Text.Trim().Equals(""))
                        {
                            if (token.Text.Contains("."))
                            {
                                file = await ApplicationData.Current.LocalFolder.GetFileAsync(token.Text);
                            }
                            else
                            {
                                file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token.Text);
                            }
                            readImage(file);
                        }

                    }
                    detail.Text = (string)composite["detail"];
                    date.Date = (DateTimeOffset)composite["date"];
                    //this.viewModel = ((App)App.Current).ViewModel;
                    ApplicationData.Current.LocalSettings.Values.Remove("newpage");
                }
                else
                {
                    this.InitFile();
                }
            }
            base.OnNavigatedTo(e);

        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            bool suspending = ((App)App.Current).issuppend;
            if (suspending)
            {
                ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();
                composite["imageToken"] = token.Text;
                composite["title"] = title.Text;
                composite["detail"] = detail.Text;
                composite["itemId"] = itemId.Text;
                composite["date"] = date.Date;
                
                ApplicationData.Current.LocalSettings.Values["newpage"] = composite;
            }
            base.OnNavigatingFrom(e);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var bt = sender as Button;
            var message = new Windows.UI.Popups.MessageDialog("");
            if (title.Text == "" || title.Text.Trim() == "")
            {
                message.Content = "Title is empty!!!";
                title.Focus(FocusState.Pointer);
            }
            else if (detail.Text == "" || detail.Text.Trim() == "")
            {
                message.Content = "Detail is empty!!!!";
                detail.Focus(FocusState.Pointer);
            }
            else if (date.Date.CompareTo(DateTime.Today) < 0)
            {
                message.Content = "Date is illegle";
            }
            else
            {
                string text = bt.Content as string;
                if ("Create".Equals(text))
                {
                    ListItem item = new ListItem(image.Source, title.Text, detail.Text, date.Date, token.Text, file);
                    viewModel.AddItem(item);
                    message.Content = "Create success";
                   
                }
                else
                {
                    viewModel.updateItem(itemId.Text, image.Source, title.Text, detail.Text, date.Date, token.Text, file);
                    message.Content = "Update sucess";
                    bt.Content = "Create";
                }
                itemId.Text = "";
                title.Text = "";
                detail.Text = "";
                token.Text = "";
                image.Source = null;
                date.Date = DateTime.Today;
                this.Frame.GoBack();
            }
            TileService.CreateOrUpdateTile(this.viewModel.Recordings);
            await message.ShowAsync();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            itemId.Text = "";
            title.Text = "";
            detail.Text = "";
            token.Text = "";
            date.Date = DateTime.Now;
            title.Focus(FocusState.Pointer);
            image.Source = null;
            createBtn.Content = "Create";
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");
            file = await openPicker.PickSingleFileAsync();
            BitmapImage imgSrc = new BitmapImage();
            if (file != null)
            {
                token.Text = StorageApplicationPermissions.FutureAccessList.Add(file);
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                imgSrc.SetSource(stream);
                image.Source = imgSrc;
                stream.Dispose();
            }
        }
        private void deleteAppBarbutton_Click(object sender, RoutedEventArgs e)
        {
            if (this.viewModel.SeletedItem != null)
            {
                this.viewModel.removeItem(this.viewModel.SeletedItem);
                title.Text = "";
                itemId.Text = "";
                detail.Text = "";
                image.Source = null;
                date.Date = DateTime.Today;
                createBtn.Content = "Create";
                this.Frame.GoBack();
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            bool suspending = ((App)App.Current).issuppend;
            if (suspending)
            {
                ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();
                composite["imageToken"] = token.Text;
                composite["title"] = title.Text;
                composite["detail"] = detail.Text;
                composite["itemId"] = itemId.Text;
                composite["date"] = date.Date;
               
                ApplicationData.Current.LocalSettings.Values["newpage"] = composite;
            }
            base.OnNavigatedFrom(e);
        }
        private async void readImage(StorageFile file)
        {
            BitmapImage imgSrc = new BitmapImage();
            if (file != null)
            {
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    imgSrc.SetSource(stream);
                    image.Source = imgSrc;
                }
            }
        }
    }
}
