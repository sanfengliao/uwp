using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Text;
using App6.Model;
using App6.Service;
using App6.ViewModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
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

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace App6
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            this.ViewModel = ((App)App.Current).ViewModel;
            this.InitFile();

        }
        ListItemViewModel ViewModel { get; set; }
        //初始化右边编辑框的图片
        private async void  InitFile()
        {
            file = await ApplicationData.Current.LocalFolder.GetFileAsync("1.png");
        }
        private StorageFile file;
        //点击左边List中的Item
        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            create.Content = "Update";
            ListItem item = e.ClickedItem as ListItem;
            this.ViewModel.SeletedItem = item;
            if (item != null)
            {
                if (this.ActualWidth >= 800)
                {
                    title.Text = item.Title;
                    detail.Text = item.Desc;
                    image.Source = item.Pic;
                    itemId.Text = item.Id;
                    date.Date = item.Date;
                    token.Text = item.ImgPath;
                    file = item.File;
                    deleteAppBarbutton.Visibility = Visibility.Visible;
                }
                else
                {
                    this.Frame.Navigate(typeof(NewPage));
                }
            }

        }
        //选取图片
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
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    imgSrc.SetSource(stream);
                    image.Source = imgSrc;
                }
            }
        }
        //点击添加或修改按钮
        private async void create_Click(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
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
                    ViewModel.AddItem(item);
                    message.Content = "Create success";
                }
                else
                {
                    ViewModel.updateItem(itemId.Text, image.Source, title.Text, detail.Text, date.Date, token.Text, file);
                    message.Content = "Update sucess";
                    bt.Content = "Create";
                }
                itemId.Text = "";
                title.Text = "";
                detail.Text = "";
                image.Source = null;
                date.Date = DateTime.Today;
                //添加或修改成功后，把存在FutureAccessList的图片删除掉
                if (StorageApplicationPermissions.FutureAccessList.ContainsItem(token.Text))
                    StorageApplicationPermissions.FutureAccessList.Remove(token.Text);
                token.Text = "";
               
            }
            await message.ShowAsync();
        }

        private void addAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ActualWidth >= 800)
            {
                return;
            }
            else
            {
                this.Frame.Navigate(typeof(NewPage));
            }
        }

        private void Pick(object sender, RoutedEventArgs e)
        {
            var icon = sender as MenuFlyoutItem;
            ListItem item = ViewModel.GetItem(icon.DataContext.ToString());
            this.ViewModel.SeletedItem = item;
            if (item != null)
            {
                if (this.ActualWidth >= 800)
                {
                    create.Content = "Update";
                    itemId.Text = item.Id;
                    title.Text = item.Title;
                    detail.Text = item.Desc;
                    date.Date = item.Date;
                    image.Source = item.Pic;
                    token.Text = item.ImgPath;
                    deleteAppBarbutton.Visibility = Visibility.Visible;
                }
                else
                {
                    this.Frame.Navigate(typeof(NewPage));
                }
            }
        }
        private void Delete(object sender, RoutedEventArgs e)
        {
            var icon = sender as MenuFlyoutItem;
            ViewModel.removeItem(icon.DataContext.ToString());
        }

        private void deleteAppBarbutton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.SeletedItem != null)
            {
                this.ViewModel.removeItem(this.ViewModel.SeletedItem);
                title.Text = "";
                detail.Text = "";
                image.Source = null;
                itemId.Text = "";
                create.Content = "Create";
                token.Text = "";
                date.Date = DateTime.Today;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            itemId.Text = "";
            title.Text = "";
            detail.Text = "";
            image.Source = null;
            token.Text = "";
            date.Date = DateTime.Now;
            title.Focus(FocusState.Pointer);
            create.Content = "Create";
        }

        private void slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var slider = sender as Slider;
            image.Width = slider.Value;
            image.Height = 200;
        }
        //挂起数据保存
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            bool suspending = ((App)App.Current).issuppend;
            if (suspending)
            {
                ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();
                composite["title"] = title.Text;
                composite["detail"] = detail.Text;
                //隐藏域，Item的Id
                composite["itemId"] = itemId.Text;
                composite["date"] = date.Date;
                //隐藏域，图片的路径
                composite["imageToken"] = token.Text;
                ApplicationData.Current.LocalSettings.Values["newpage"] = composite;
            }
            base.OnNavigatedFrom(e);
        }

        //挂起恢复后重新数据恢复
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                ApplicationData.Current.LocalSettings.Values.Remove("newpage");
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
                            //若图片存在LocalFolder中
                            if (token.Text.Contains("."))
                            {
                                file = await ApplicationData.Current.LocalFolder.GetFileAsync(token.Text);
                            }
                            else //图片存在 StorageApplicationPermissions.FutureAccessList中
                            {
                                file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token.Text);
                            }
                            readImage(file);
                        }

                    }
                    detail.Text = (string)composite["detail"];
                    date.Date = (DateTimeOffset)composite["date"];
                    this.ViewModel = ((App)App.Current).ViewModel;
                    ApplicationData.Current.LocalSettings.Values.Remove("newpage");
                }
            }
            base.OnNavigatedTo(e);
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
        //发邮件代码
        private void Share(object sender, RoutedEventArgs e)
        {
            var icon = sender as MenuFlyoutItem;
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            this.ViewModel.SeletedItem = this.ViewModel.GetItem(icon.DataContext.ToString());
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = "Share";
            request.Data.SetText(this.ViewModel.SeletedItem.Title);
            // request.Data.SetDataProvider(StandardDataFormats.Bitmap,new DataProviderHandler(this.OnDeferredImageRequestedHandler));
            request.Data.SetStorageItems(new List<StorageFile> { this.ViewModel.SeletedItem.File });

            request.Data.SetText(this.ViewModel.SeletedItem.Desc);

        }

        private async void OnDeferredImageRequestedHandler(DataProviderRequest request)
        {
            if (this.ViewModel.SeletedItem.ImgPath != null)
            {
                DataProviderDeferral deferral = request.GetDeferral();

                try
                {
                    InMemoryRandomAccessStream inMemoryStream = new InMemoryRandomAccessStream();

                    // Decode the image
                    StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(this.ViewModel.SeletedItem.ImgPath);

                    BitmapDecoder imageDecoder = await BitmapDecoder.CreateAsync(await file.OpenAsync(FileAccessMode.Read));

                    // Re-encode the image at 50% width and height
                    BitmapEncoder imageEncoder = await BitmapEncoder.CreateForTranscodingAsync(inMemoryStream, imageDecoder);
                    imageEncoder.BitmapTransform.ScaledWidth = (uint)(imageDecoder.OrientedPixelHeight * 0.5);
                    imageEncoder.BitmapTransform.ScaledHeight = (uint)(imageDecoder.OrientedPixelHeight * 0.5);
                    await imageEncoder.FlushAsync();

                    request.SetData(RandomAccessStreamReference.CreateFromStream(inMemoryStream));
                }

                catch (Exception ex)
                {
                    // Handle the exception
                    Debug.WriteLine(ex);

                }

                finally
                {
                    deferral.Complete();
                }
            }
        }
        //点击checkbox是数据库数据的修改
        private void checkbox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            String id = checkBox.DataContext.ToString();
            DBUtils.DbUitls.UpdateItemIsComplete(id, checkBox.IsChecked);
        }

        //搜索
        private void search_Click(object sender, RoutedEventArgs e)
        {
            List<ListItem> list = DBUtils.DbUitls.SearchItems(searchText.Text);
            var message = new Windows.UI.Popups.MessageDialog("");
            StringBuilder sb = new StringBuilder();
            foreach (ListItem item in list)
            {
                sb.Append(item.Title);
                sb.Append(";\t");
                sb.Append(item.Desc);
                sb.Append(";\t");
                sb.Append(item.Date.ToString());
                sb.Append("\n");
            }
            if (!sb.ToString().Trim().Equals(""))
            {
                message.Content = sb.ToString();
                message.ShowAsync();
            }
        }


    }
}
