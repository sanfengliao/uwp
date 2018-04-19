using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App6.Model;
using Windows.Storage;
namespace App6.DBUtils
{
    class DbUitls
    {
        //初始化数据库
        public static void Init()
        {
            var con = new SQLiteConnection("todolist.db");
            string sql = @"create table if not exists Tasks (
                                id varchar(50) primary key,
                                title varchar(20),
                                detail varchar(1024),
                                img_path varchar(1024),
                                create_time datetime,
                                isComplete varchar(20)
                                
                            )";
            var statement = con.Prepare(sql);
            statement.Step();
        }
        //增
        public static async void SaveItem(ListItem item)
        {
           
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            //获取图片的名字
            String ImageName = item.File.Name;
            //获取后缀名
            String ext = ImageName.Substring(ImageName.LastIndexOf("."));
            //图片拷贝到localfolder的路径
            var file = await storageFolder.CreateFileAsync(item.Id + Guid.NewGuid().ToString() + ext, CreationCollisionOption.ReplaceExisting);
            //拷贝
            await item.File.CopyAndReplaceAsync(file);
            //图片拷贝完成后，改变item的ImgPath,在localfloder中的具体路径
            item.ImgPath = file.Name;

            //将信息保存到数据库
            SQLiteConnection con = new SQLiteConnection("todolist.db");
            SQLiteStatement statement = con.Prepare("insert into Tasks values(?, ?, ?, ?, ?, ?)") as SQLiteStatement;
            statement.Bind(1, item.Id);
            statement.Bind(2, item.Title);
            statement.Bind(3, item.Desc);
            statement.Bind(4, item.ImgPath);
            statement.Bind(5, item.Date.ToString());
            statement.Bind(6, item.IsComplete.ToString());
            statement.Step();
        }

        //改
        public static async void UpdateItem(ListItem item)
        {
            //获取选择的图片的文件名
            String ImageName = item.File.Name;
            //获取图片后缀
            String ext = ImageName.Substring(ImageName.LastIndexOf("."));
            //将图片拷贝到localfoloder中
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            var file = await storageFolder.CreateFileAsync(item.Id + Guid.NewGuid().ToString() + ext, CreationCollisionOption.ReplaceExisting);
            await item.File.CopyAndReplaceAsync(file);
            //删除原来的图片
            var deleteFile = await storageFolder.GetFileAsync(item.ImgPath);
            await deleteFile.DeleteAsync();
            //将item中ImgPath更新
            item.ImgPath = file.Name;

            //将数据保存到数据库
            SQLiteConnection con = new SQLiteConnection("todolist.db");
            var statement = con.Prepare("update Tasks set title = ? , detail = ?, img_path=?, create_time=?, isComplete=? where id = ?");
            statement.Bind(1, item.Title);
            statement.Bind(2, item.Desc);
            statement.Bind(3, item.ImgPath);
            statement.Bind(4, item.Date.ToString());
            statement.Bind(5, item.IsComplete.ToString());
            statement.Bind(6, item.Id);
            statement.Step();
        }

        //删
        public static async void DeleteItem(ListItem item)
        {
            //删除图片
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.GetFileAsync(item.ImgPath);
            SQLiteConnection con = new SQLiteConnection("todolist.db");
            await file.DeleteAsync();
            //删除数据库数据
            var statement = con.Prepare("delete from Tasks where id = ?");
            statement.Bind(1, item.Id);
            statement.Step();
            
        }

        //查询所有的数据
        public static ObservableCollection<ListItem> GetAllItem()
        {
            ObservableCollection<ListItem> list = new ObservableCollection<ListItem>();
            SQLiteConnection con = new SQLiteConnection("todolist.db");
            var statement = con.Prepare("select * from Tasks");

            while (statement.Step() == SQLiteResult.ROW)
            {
                ListItem item = new ListItem();
                item.Id = (string)statement[0];
                item.Title = (string)statement[1];
                item.Desc = (string)statement[2];
                item.ImgPath = (string)statement[3];
                item.Date = DateTimeOffset.Parse((string)statement[4]);
                item.IsComplete = Boolean.Parse((string)statement[5]);
                item.setStorageFileAsync();
                //item.setImage();
                list.Add(item);
            }
            return list;
        }

        //改变isComplete
        public static void UpdateItemIsComplete(String id, Boolean? isComplete)
        {
            SQLiteConnection con = new SQLiteConnection("todolist.db");
            var statement = con.Prepare("update Tasks set isComplete=? where id = ?");
            statement.Bind(1, isComplete.ToString());
            statement.Bind(2, id);
            statement.Step();
        }

        //搜索
        public static List<ListItem> SearchItems(String test)
        {
            List<ListItem> list = new List<ListItem>();
            SQLiteConnection con = new SQLiteConnection("todolist.db");
            var statement = con.Prepare("select * from Tasks where title like ? or detail like ?");
            //绑定参数
            statement.Bind(1, "%" + test + "%");
            statement.Bind(2, "%" + test + "%");
            while (statement.Step() == SQLiteResult.ROW)
            {
                ListItem item = new ListItem();
                item.Id = (string)statement[0];
                item.Title = (string)statement[1];
                item.Desc = (string)statement[2];
                item.ImgPath = (string)statement[3];
                item.Date = DateTimeOffset.Parse((string)statement[4]);
                item.IsComplete = Boolean.Parse((string)statement[5]);
                item.setStorageFileAsync();
                //item.setImage();
                list.Add(item);
            }
            return list;

        }
    }
}
