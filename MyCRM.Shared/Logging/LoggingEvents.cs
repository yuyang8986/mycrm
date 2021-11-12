using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.Logging
{
    public class LoggingEvents
    {
        public const int GenerateItems = 1000;
        public const int ListItems = 1001;
        public const int GetItem = 1002;
        public const int InsertItem = 1003;
        public const int UpdateItem = 1004;
        public const int DeleteItem = 1005;
        public const int RecoverItem = 1006;

        public const int ListItemsFailed = 2000;
        public const int InsertItemFailed = 2001;
        public const int UpdateItemFailed = 2002;
        public const int DeleteItemFailed = 2003;
        public const int RecoverItemFailed = 2004;
        public const int ReorderItemFailed = 2005;
        public const int SaveToDatabaseFailed = 2005;

        public const int GetItemNotFound = 4000;
        public const int UpdateItemNotFound = 4001;
    }
}
