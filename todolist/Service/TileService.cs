using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using App6.Model;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace App6.Service
{
    class TileService
    {
        public static void CreateOrUpdateTile(ObservableCollection<ListItem> list)
        {
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();
            int i = 0;
            foreach (ListItem item in list)
            {
                XmlDocument xmlDoc = CreateTile(item);
                updater.Update(new TileNotification(xmlDoc));
                ++i;
                if (i == 5) //最多只能放五个
                {
                    break;
                }
            }
        }

        public static XmlDocument CreateTile(ListItem item)
        {
            XDocument xDoc = new XDocument(
                new XElement("tile", new XAttribute("version", 3),
                    new XElement("visual",
                        // Small Tile
                        new XElement("binding", new XAttribute("template", "TileSmall"),
                            new XElement("group",
                                new XElement("subgroup",
                                    new XElement("text", item.Title, new XAttribute("hint-style", "caption")),
                                    new XElement("text", item.Desc, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                                )
                            )
                        ),

                        // Medium Tile
                        new XElement("binding", new XAttribute("template", "TileMedium"),
                            new XElement("image", new XAttribute("placement", "background"), new XAttribute("src", "ms-appdata:///local/" + item.ImgPath)),
                            new XElement("group",
                                
                                new XElement("subgroup",
                                    new XElement("text", item.Title, new XAttribute("hint-style", "caption")),
                                    new XElement("text", item.Desc, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                                )
                            )
                        ),
                        new XElement("binding", new XAttribute("template", "TileWide"),
                            new XElement("image", new XAttribute("placement", "background"), new XAttribute("src", "ms-appdata:///local/" + item.ImgPath)),
                            new XElement("group",
                                
                                new XElement("subgroup",
                                    new XElement("text", item.Title, new XAttribute("hint-style", "caption")),
                                    new XElement("text", item.Desc, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                                )
                            )
                        ),
                        new XElement("binding", new XAttribute("template", "TileLarge"),
                            new XElement("image", new XAttribute("placement", "background"), new XAttribute("src", "ms-appdata:///local/" + item.ImgPath)),
                            new XElement("group",
                               
                                new XElement("subgroup",
                                    new XElement("text", item.Title, new XAttribute("hint-style", "caption")),
                                    new XElement("text", item.Desc, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                                )
                            )
                        )

                    )
                )
            );

            Windows.Data.Xml.Dom.XmlDocument xmlDoc = new Windows.Data.Xml.Dom.XmlDocument();
            xmlDoc.LoadXml(xDoc.ToString());
            return xmlDoc;
        }

    }
}
