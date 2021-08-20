using System;

namespace Program
{
    public class DbConfig
    {
        public static readonly string FILE_EXTENSION = ".dat";
        public static readonly string DB_FILEPATH = "db/";
        public static readonly string COLLECTION_DEFS_FILEPATH = DB_FILEPATH + "colsDefs" + FILE_EXTENSION;
        public static readonly string COLLECTIONS_DATA_FILEPATH = DB_FILEPATH + "collections/";
        public static readonly string COLLECTION_INDEX_FILENAME = "colIndex" + FILE_EXTENSION;
        public static readonly string COLLECTION_INDEX_BACKUP_FILENAME = "colIndex_backup" + FILE_EXTENSION;
        public static readonly long MAX_ID = long.MaxValue;
        public static readonly long MIN_ID = 0;
        public static readonly string LOGS_FILEPATH = DB_FILEPATH + "logs/";
        public static readonly string LOGS_EXTENSION = ".txt";
        public static readonly int MAX_DATA_UNIT_ID_COLLISIONS = (int) Math.Pow(10, 6);
    }
}