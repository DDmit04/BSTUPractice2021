namespace Program.Utils
{
    public class PathUtils
    {
        public static string GetCollectionIndexFilepath(string collectionId)
        {
            return $"{FileSystemConfig.COLLECTIONS_DATA_FILEPATH}{collectionId}/{FileSystemConfig.COLLECTION_INDEX_FILENAME}";
        }

        public static string GetIndexBackupFilepath(string collectionId)
        {
            return $"{FileSystemConfig.COLLECTIONS_DATA_FILEPATH}{collectionId}/{FileSystemConfig.COLLECTION_INDEX_BACKUP_FILENAME}";
        }
        public static string GetCollectionDataFilepath(string collectionId)
        {
            return $"{FileSystemConfig.COLLECTIONS_DATA_FILEPATH}{collectionId}/";
        }
    }
}