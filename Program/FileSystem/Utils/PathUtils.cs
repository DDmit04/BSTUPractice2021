namespace Program.Utils
{
    public class PathUtils
    {
        public static string GetCollectionIndexFilepath(string collectionId)
        {
            return $"{DbConfig.COLLECTIONS_DATA_FILEPATH}{collectionId}/{DbConfig.COLLECTION_INDEX_FILENAME}";
        }

        public static string GetIndexBackupFilepath(string collectionId)
        {
            return $"{DbConfig.COLLECTIONS_DATA_FILEPATH}{collectionId}/{DbConfig.COLLECTION_INDEX_BACKUP_FILENAME}";
        }
        public static string GetCollectionDataFilepath(string collectionId)
        {
            return $"{DbConfig.COLLECTIONS_DATA_FILEPATH}{collectionId}/";
        }
    }
}