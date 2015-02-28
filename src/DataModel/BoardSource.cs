using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace _4charp.Data
{
    class BoardItem
    {
        public BoardItem(String board, String title)
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

    public sealed class BoardSource
    {

    }

}
