using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace App6.Model
{
	//json序列化是需要的注解
    [DataContract]
    public class ListItem : INotifyPropertyChanged
    {
        private String id; // id 唯一标识符
        private StorageFile file; // 图片的StorageFile
        private string img_path; //图片在LocalFolder的存储路径
        private ImageSource pic; //图片的ImageSource
		private string title; // 标题
        private string desc; // 详情
        private bool? isComplete; //是否完成
        private DateTimeOffset date;//时间
		
		public event PropertyChangedEventHandler PropertyChanged;
        [DataMember(Order =0)]
        public string Id { get { return id; } set { id = value; } }
        [DataMember(Order = 1)]
        public string Title {
            get { return title; }
            set {
                title = value;
                PropertyChangedHanlder("Title");
            } }
        [DataMember(Order =1)]
        public string ImgPath { get { return img_path; } set { img_path = value; PropertyChangedHanlder("ImgPath"); } }
        public  ImageSource Pic {  get {
                if (pic == null)
                {
                    setImage();
                }
                return pic;
            } set {pic = value; PropertyChangedHanlder("Pic"); } }
        [DataMember(Order = 3)]
        public string Desc { get { return desc; } set { desc = value; PropertyChangedHanlder("Desc"); } }
        [DataMember(Order = 4)]
        public bool? IsComplete { get { return isComplete; } set { isComplete = value; PropertyChangedHanlder("IsComplete"); } }
        [DataMember(Order = 5)]
        public DateTimeOffset Date { set { date = value; PropertyChangedHanlder("Date"); } get { return date; } }
        public StorageFile File{ set { file = value; PropertyChangedHanlder("File"); } get { if (file == null) { setStorageFileAsync(); } return file; } }
        public ListItem()
        {

        }
        public ListItem(ImageSource pic, string title, string desc, DateTimeOffset date, string token,StorageFile file )
        {
            this.id = Guid.NewGuid().ToString();
            this.pic = pic;
            this.title = title;
            this.desc = desc;
            this.date = date;
            this.isComplete = false;
            //this.img_path = id + file.Name.Substring(file.Name.LastIndexOf("."));
            this.file = file;
        }
        protected void PropertyChangedHanlder(string str)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(str));
            }
        }
        public string getId()
        {
            return this.id;
        }
		//设置ImageSource
        public async void setImage()
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(img_path);
            BitmapImage imgSrc = new BitmapImage();
            if (file != null)
            {
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    imgSrc.SetSource(stream);
                    this.Pic = imgSrc;
                }
            }

        }
		//设置StorageFile
        public async void setStorageFileAsync()
        {
            this.file = await ApplicationData.Current.LocalFolder.GetFileAsync(img_path);
        }
        
    }
}
