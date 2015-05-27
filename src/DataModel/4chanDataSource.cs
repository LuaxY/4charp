using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Net.Http;
using Windows.Data.Html;

namespace _4charp.Data
{
    public class _4chanDataThread
    {
        public _4chanDataThread(string board, string date, string name, string com, bool hasimage, string ext = "", double time = 0)
        {
            this.Date = date;
            this.Name = name;
            this.Desc = HtmlUtilities.ConvertToText(com);
            this.hasImage = hasimage;
            this.Ext = ext;
            this.Time = time;
            this.Board = board;

            if (this.hasImage)
            {
                string url = "http://i.4cdn.org/" + this.Board + "/" + this.Time.ToString() + this.Ext;
                this.ImagePath = new BitmapImage(new Uri(url));
            }
        }

        public string Date { get; private set; }
        public string Name { get; private set; }
        public string Desc { get; private set; }
        public bool hasImage { get; private set; }
        public string Ext { get; private set; }
        public double Time { get; private set; }
        public string Board { get; private set; }
        public BitmapImage ImagePath { get; private set; }
    }

    public class _4chanDataCatalog
    {
        public _4chanDataCatalog(double id, double time, string date, string author, string desc, string ext, string board)
        {
            this.Id = id;
            this.Time = time;
            this.Date = date;
            this.Author = author;
            this.Desc = HtmlUtilities.ConvertToText(desc);
            this.Ext = ext;
            this.Board = board;
            string url = "http://i.4cdn.org/" + this.Board + "/" + this.Time.ToString() + this.Ext;
            this.ImagePath = new BitmapImage(new Uri(url));
            this.Items = new ObservableCollection<_4chanDataThread>();
        }

        public double Id { get; private set; }
        public double Time { get; private set; }
        public string Date { get; private set; }
        public string Author { get; private set; }
        public string Desc { get; private set; }
        public string Ext { get; private set; }
        public string Board { get; private set; }
        public BitmapImage ImagePath { get; private set; }
        public ObservableCollection<_4chanDataThread> Items { get; private set; }

        public override string ToString()
        {
            return this.Id.ToString();
        }
    }

    public class _4chanDataBoard
    {
        public _4chanDataBoard(String board, String title)
        {
            this.Board = board;
            this.Title = title;
            this.Items = new ObservableCollection<CatalogsGroup>();
            this.ImagePath = "Assets/boards/" + board + ".png";
        }

        public string Board { get; private set; }
        public string Title { get; private set; }
        public ObservableCollection<CatalogsGroup> Items { get; private set; }
        public string ImagePath { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    public class BoardsGroup
    {
        public BoardsGroup()
        {
            this.Title = "Catégories";
            this.Items = new ObservableCollection<_4chanDataBoard>();
        }

        public string Title { get; private set; }
        public ObservableCollection<_4chanDataBoard> Items { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    public class CatalogsGroup
    {
        public CatalogsGroup()
        {
            this.Title = "Catégories";
            this.Items = new ObservableCollection<_4chanDataCatalog>();
        }

        public string Title { get; private set; }
        public ObservableCollection<_4chanDataCatalog> Items { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    public class ThreadGroup
    {
        public ThreadGroup()
        {
            this.Title = "Catégories";
            this.Items = new ObservableCollection<_4chanDataThread>();
        }

        public string Title { get; private set; }
        public ObservableCollection<_4chanDataThread> Items { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    public sealed class _4chanDataSource
    {
        private static _4chanDataSource _4chanDataSrc = new _4chanDataSource();

        /*******************************************************************************************************/

        private ObservableCollection<BoardsGroup> _boards = new ObservableCollection<BoardsGroup>();
        public ObservableCollection<BoardsGroup> Boards
        {
            get { return this._boards;  }
        }

        public static async Task<IEnumerable<BoardsGroup>> GetBoardAsync()
        {
            await _4chanDataSrc.Get4chanBoardsAsync();
            return _4chanDataSrc.Boards;
        }

        private async Task Get4chanBoardsAsync()
        {
            if (this._boards.Count != 0)
                return;

            /*Uri dataUri = new Uri("http://a.4cdn.org/boards.json");
            HttpClient _client = new HttpClient();
            var result = await _client.GetStringAsync(dataUri);*/

            Uri dataUri = new Uri("ms-appx:///DataModel/boards.json");
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string result = await FileIO.ReadTextAsync(file);

            JsonObject jsonObject = JsonObject.Parse(result);
            JsonArray jsonArray = jsonObject["boards"].GetArray();
            BoardsGroup group = new BoardsGroup();

            string[] blacklist = { "b", "d", "e", "gif", "h", "hc", "hm", "k", "lgbt", "mlp", "pol", "r", "r9k", "s", "s4s", "t", "u", "y", "soc", "hr", "f" };

            foreach (JsonValue boardValue in jsonArray)
            {
                JsonObject boardObject = boardValue.GetObject();
                bool isBlacklisted = false;

                foreach (string x in blacklist)
                {
                    if (x == boardObject["board"].GetString())
                    {
                        isBlacklisted = true;
                    }
                }

                if (isBlacklisted)
                    continue;

                _4chanDataBoard board = new _4chanDataBoard(boardObject["board"].GetString(),
                                                            boardObject["title"].GetString());

                group.Items.Add(board);
            }

            this.Boards.Add(group);
        }

        /*******************************************************************************************************/

        public static async Task<IEnumerable<CatalogsGroup>> GetCatalogdAsync(_4chanDataBoard board)
        {
            await _4chanDataSrc.Get4chanCatalogAsync(board);
            return board.Items;
        }

        private async Task Get4chanCatalogAsync(_4chanDataBoard board)
        {
            if (board.Items.Count != 0)
                return;
            
            Uri dataUri = new Uri("http://a.4cdn.org/" + board.Board + "/catalog.json");
            HttpClient _client = new HttpClient();
            var result = await _client.GetStringAsync(dataUri);
            JsonArray pages = JsonArray.Parse(result);
            CatalogsGroup group = new CatalogsGroup();

            foreach (JsonValue pageValue in pages)
            {
                JsonObject page = pageValue.GetObject();
                JsonArray threads = page["threads"].GetArray();

                foreach (JsonValue threadValue in threads)
                {
                    JsonObject threadObject = threadValue.GetObject();

                    if (threadObject.ContainsKey("tim") &&
                        threadObject.ContainsKey("no") &&
                        threadObject.ContainsKey("now") &&
                        threadObject.ContainsKey("name") &&
                        threadObject.ContainsKey("com") &&
                        threadObject.ContainsKey("ext"))
                    {
                        _4chanDataCatalog catalog = new _4chanDataCatalog(threadObject["no"].GetNumber(),
                                                                          threadObject["tim"].GetNumber(),
                                                                          threadObject["now"].GetString(),
                                                                          threadObject["name"].GetString(),
                                                                          threadObject["com"].GetString(),
                                                                          threadObject["ext"].GetString(),
                                                                          board.Board);

                        group.Items.Add(catalog);
                    }

                    
                }
            }

            board.Items.Add(group);
        }


        /*******************************************************************************************************/

        public static async Task<IEnumerable<_4chanDataThread>> GetThreadAsync(_4chanDataCatalog catalog)
        {
            await _4chanDataSrc.Get4chanThreadAsync(catalog);
            return catalog.Items;
        }

        private async Task Get4chanThreadAsync(_4chanDataCatalog catalog)
        {
            if (catalog.Items.Count != 0)
                return;

            Uri dataUri = new Uri("http://a.4cdn.org/" + catalog.Board + "/thread/" + catalog.Id.ToString() + ".json");
            HttpClient _client = new HttpClient();
            var result = await _client.GetStringAsync(dataUri);
            JsonObject jsonObject = JsonObject.Parse(result);

            JsonArray threads = jsonObject["posts"].GetArray();

            foreach (JsonValue threadValue in threads)
            {
                JsonObject threadObject = threadValue.GetObject();

                if (threadObject.ContainsKey("now") &&
                    threadObject.ContainsKey("name") &&
                    threadObject.ContainsKey("com"))
                {
                    _4chanDataThread thread;

                    if (threadObject.ContainsKey("filename"))
                    {
                        thread = new _4chanDataThread(catalog.Board,
                                                        threadObject["now"].GetString(),
                                                        threadObject["name"].GetString(),
                                                        threadObject["com"].GetString(),
                                                        true,
                                                        threadObject["ext"].GetString(),
                                                        threadObject["tim"].GetNumber());
                    }
                    else
                    {
                        thread = new _4chanDataThread(catalog.Board,
                                                        threadObject["now"].GetString(),
                                                        threadObject["name"].GetString(),
                                                        threadObject["com"].GetString(),
                                                        false);
                    }

                    catalog.Items.Add(thread);
                }
            }
        }
    }
}
