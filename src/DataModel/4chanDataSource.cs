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

namespace _4charp.Data
{
    public class _4chanDataBoard
    {
        public _4chanDataBoard(String board, String title)
        {
            this.Board = board;
            this.Title = title;
        }

        public string Board { get; private set; }
        public string Title { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    public class FakeGroup
    {
        public FakeGroup()
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

    public sealed class _4chanDataSource
    {
        private static _4chanDataSource _4chanDataSrc = new _4chanDataSource();

        private ObservableCollection<FakeGroup> _boards = new ObservableCollection<FakeGroup>();
        public ObservableCollection<FakeGroup> Boards
        {
            get { return this._boards;  }
        }

        public static async Task<IEnumerable<FakeGroup>> GetBoardAsync()
        {
            await _4chanDataSrc.Get4chanDataAsync();
            return _4chanDataSrc.Boards;
        }

        private async Task Get4chanDataAsync()
        {
            if (this._boards.Count != 0)
                return;

            //Uri dataUri = new Uri("http://a.4cdn.org/boards.json");
            Uri dataUri = new Uri("ms-appx:///DataModel/boards.json");

            /*HttpClient _client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, dataUri);
            var response = await _client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();*/

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);

            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["boards"].GetArray();

            FakeGroup group = new FakeGroup();

            string[] blacklist = { "b", "d", "e", "gif", "h", "hc", "hm", "k", "lgbt", "mlp", "pol", "r", "r9k", "s", "s4s", "t", "u", "y", "soc", "hr" };

            foreach (JsonValue boardValue in jsonArray)
            {
                JsonObject boardObject = boardValue.GetObject();
                bool isBlacklisted = false;

                foreach (string x in blacklist)
                {
                    if (x.Contains(boardObject["board"].GetString()))
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
    }
}
