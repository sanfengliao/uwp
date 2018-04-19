using App6.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace App6.ViewModel
{
     [DataContract]
     public class ListItemViewModel
    {
        //Item集合
        private ObservableCollection<Model.ListItem> listItems = new ObservableCollection<Model.ListItem>();
        [DataMember(Order =0)]
        public ObservableCollection<Model.ListItem> Recordings { get { return listItems; } }
        
        //selectItem
        private Model.ListItem seletedItem;
        [DataMember(Order = 1)]
        public Model.ListItem SeletedItem { set { seletedItem = value; } get { return seletedItem; } }

        //构造函数
        public ListItemViewModel()
        {
            //获取数据库中所有的Item
            listItems = DBUtils.DbUitls.GetAllItem();
        }
        //添加Item
        public void AddItem(Model.ListItem item)
        {
            this.listItems.Add(item);
            //更新数据库数据
            DBUtils.DbUitls.SaveItem(item);
            //更新磁贴内容
            TileService.CreateOrUpdateTile(listItems);
        }
        //更新Item
        public void updateItem(string id, ImageSource img, string title, string desc, DateTimeOffset date, string token, StorageFile file)
        {
            Model.ListItem item = GetItem(id);
            item.Date = date;
            item.Pic = img;
            item.Title = title;
            item.Desc = desc;
            //StorageApplicationPermissions.FutureAccessList.Remove(item.Token);
            //item.ImgPath = item.Id + file.Name.Substring(file.Name.LastIndexOf("."));
            item.File = file;
            //更新数据库数据
            DBUtils.DbUitls.UpdateItem(item);
            //更新磁贴
            TileService.CreateOrUpdateTile(listItems);
            this.seletedItem = null;
        }
       //删除Item
        public void removeItem(Model.ListItem item)
        {
            this.listItems.Remove(item);
            DBUtils.DbUitls.DeleteItem(item);
            TileService.CreateOrUpdateTile(listItems);
            this.seletedItem = null;
        }
        //通过Id删除Item
        public void removeItem(String id)
        {
            this.removeItem(GetItem(id));
        }

        public Model.ListItem GetItem(string id)
        {
            foreach (Model.ListItem item in listItems)
            {
                if (item.getId().Equals(id))
                {
                    return item;
                }
            }
            return null;
        }
       

       
    }
}
