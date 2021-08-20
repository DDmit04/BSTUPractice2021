using System.Collections.Generic;
using Program.Exceptions;
using Program.userInterface;
using Program.Utils;

namespace Program
{
    public class IndexRepository
    {
        protected IDataUnitIndexDataSource DataUnitIndexDataSource { get; }
        public Dictionary<string, IdIndex> AllIndexes { get; }

        public IndexRepository(List<CollectionDefinition> colDefs,
            IDataUnitIndexDataSource dataUnitIndexDataSource)
        {
            DataUnitIndexDataSource = dataUnitIndexDataSource;
            AllIndexes = DataUnitIndexDataSource.LoadRootIndexes(colDefs);
        }

        public void CreateIndex(string collectionId)
        {
            var index = FindIndexByCollectionId(collectionId);
            if (index == null)
            {
                var newIndex = new IdIndex();
                DataUnitIndexDataSource.UpdateIndexFile(collectionId, newIndex);
                AllIndexes.Add(collectionId, newIndex);
                var firstCollectionDataFilePath = PathUtils.GetCollectionDataFilepath(collectionId) + newIndex.DataFileName;
                DirUtils.CreateFile(firstCollectionDataFilePath);
            }
        }

        public IdIndex RemoveIndex(string collectionId)
        {
            var indexToRemove = FindIndexByCollectionId(collectionId);
            AllIndexes.Remove(collectionId);
            return indexToRemove;
        }
        public void AddIndex(string collectionId, IdIndex index)
        {
            AllIndexes.Add(collectionId, index);
        }

        public IndexDivideRequest AddDataUnit(string collectionId, long dataUnitId)
        {
            var index = FindIndexByCollectionIdOrThrow(collectionId);
            var idRewriteLocations = index.AddDataUnitIndex(dataUnitId);
            if (index.Parent != null)
            {
                AllIndexes[collectionId] = index.Parent;
                index = index.Parent;
            }
            DataUnitIndexDataSource.UpdateIndexFile(collectionId, index);
            return idRewriteLocations;
        }

        public IdIndex RemoveDataUnit(string collectionId, long dataUnitId)
        {
            var index = FindIndexByCollectionIdOrThrow(collectionId);
            var indexToDelete = index.RemoveDataUnitIndex(dataUnitId);
            DataUnitIndexDataSource.UpdateIndexFile(null, index);
            return indexToDelete;
        }

        public List<string> GetAllIndexesDataFilePaths(string collectionId)
        {
            var index = FindIndexByCollectionIdOrThrow(collectionId);
            var filenames = index.GetAllIndexesFileNames();
            var collectionDataFilePath = PathUtils.GetCollectionDataFilepath(collectionId);
            var resFilenames = new List<string>();
            foreach (var filename in filenames)
            {
                resFilenames.Add(filename.Insert(0, collectionDataFilePath));
            }
            return resFilenames;
        }

        public bool ContainsId(string collectionId, long dataUnitId)
        {
            var index = FindIndexByCollectionIdOrThrow(collectionId);
            return index.ContainsId(dataUnitId);
        }
        public string GetDataUnitFilepath(string collectionId, long dataUnitId)
        {
            var index = FindIndexByCollectionIdOrThrow(collectionId);
            var filename = index.FindDataFileNameByUnitId(dataUnitId);
            return PathUtils.GetCollectionDataFilepath(collectionId) + filename;
        }

        protected IdIndex FindIndexByCollectionId(string collectionId)
        {
            if (AllIndexes.ContainsKey(collectionId))
            {
                return AllIndexes[collectionId];
            }
            return null;
        }

        protected  IdIndex FindIndexByCollectionIdOrThrow(string collectionId)
        {
            var index = FindIndexByCollectionId(collectionId);
            if (index != null)
            {
                return index;
            }

            throw IndexNotFoundException.GenerateException(collectionId);
        }

        public void BackupIndex(string collectionId)
        {
            var index = FindIndexByCollectionIdOrThrow(collectionId);
            var backupFilepath = PathUtils.GetIndexBackupFilepath(collectionId);
            DataUnitIndexDataSource.SaveIndexToFile(backupFilepath, index);
        }

        public void LoadBackupOfIndex(string collectionId)
        {
            var backupFilepath = PathUtils.GetIndexBackupFilepath(collectionId);
            var loadedIndex = DataUnitIndexDataSource.LoadRootIndexFromFile(backupFilepath, collectionId);
            AllIndexes.Remove(collectionId);
            AllIndexes.Add(collectionId, loadedIndex);
            DataUnitIndexDataSource.UpdateIndexFile(null, loadedIndex);
        }
    }
}