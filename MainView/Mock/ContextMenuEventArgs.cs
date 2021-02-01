using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainView.Mock
{
    public class ContextMenuEventArgs
    {
        public int RowIndex { get; set; } = -1;
        public int ColumnIndex { get; set; } = -1;
        public string CellValue { get; set; } = "";

        public IWebsite Website { get; set; } = null;
    }

    public enum EventType
    {
        BeginAdding, EndAdding,
        Exit,
        Remove,
        RowValidating
    }
}
