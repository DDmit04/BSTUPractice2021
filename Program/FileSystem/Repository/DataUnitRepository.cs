using System.Collections.Generic;
using System.IO;
using Program.Controller.interfaces;
using Program.FileSystem.Exceptions.dataUnit;
using Program.FileSystem.Utils;
using Program.userInterface;
using Program.Utils;

namespace Program.Controller
{
    public class DataUnitRepository : IDataUnitRepo
    {
        public IDataUnitDataSource DataUnitDataSource { get; }
        public IndexRepository IndexRepository { get; }

        public DataUnitRepository(IDataUnitDataSource dataUnitDataSource, IndexRepository indexRepository)
        {
            DataUnitDataSource = dataUnitDataSource;
            IndexRepository = indexRepository;
        }

        public List<DataUnit> LoadCollectionData(string collectionId)
        {
            return GetAllCollectionDataUnits(collectionId);
        }

        public List<DataUnit> GetDataUnitsByPropsAllCollections(List<DataUnitProp> props)
        {
            var resultList = new List<DataUnit>();
            foreach (var index in IndexRepository.AllIndexes)
            {
                resultList.AddRange(GetDataUnitsByProps(index.Key, props));
            }
            return resultList;
        }

        public List<DataUnit> GetDataUnitsByProps(string collectionId, List<DataUnitProp> props)
        {
            var dataUnits = new List<DataUnit>();
            var filePaths = IndexRepository.GetAllIndexesDataFilePaths(collectionId);
            foreach (var filePath in filePaths)
            {
                var indexDataUnits = DataUnitDataSource.LoadDataUnitsFromFile(filePath);
                indexDataUnits = SearchDataUnits(indexDataUnits, props);
                dataUnits.AddRange(indexDataUnits);
            }
            return dataUnits;
        }

        public DataUnit SaveDataUnit(string collectionId, DataUnit dataUnit)
        {
            var filepath = IndexRepository.GetDataUnitFilepath(collectionId, dataUnit.Id);
            return DataUnitDataSource.SaveDataUnit(filepath, dataUnit);
        }
        public DataUnit CreateDataUnit(string collectionId)
        {
            var id = IdUtils.GenerateDataUnitId();
            var counter = 0;
            while (IndexRepository.ContainsId(collectionId, id))
            {
                id = IdUtils.GenerateDataUnitId();
                counter++;
                if (counter == DbConfig.MAX_DATA_UNIT_ID_COLLISIONS)
                {
                    throw DataUnitIdCollisionException.GenerateException();
                }
            }
            var dataUnit = new DataUnit(id);
            var filepath = IndexRepository.GetDataUnitFilepath(collectionId, dataUnit.Id);
            var savedDataUnit = DataUnitDataSource.SaveDataUnit(filepath, dataUnit);
            try
            {
                IndexRepository.BackupIndex(collectionId);
                var indexDivideRequest = IndexRepository.AddDataUnit(collectionId, dataUnit.Id);
                if (indexDivideRequest != null)
                {
                    try
                    {
                        DivideIndexByTwo(collectionId, indexDivideRequest);
                    }
                    catch
                    {
                        IndexRepository.LoadBackupOfIndex(collectionId);
                        throw;
                    }
                }
            }
            catch
            {
                DataUnitDataSource.DeleteDataUnit(filepath, dataUnit.Id);
                throw;
            }
            return savedDataUnit;
        }
        public void DeleteDataUnit(string collectionId, long dataUnitId)
        {
            var filepath = IndexRepository.GetDataUnitFilepath(collectionId, dataUnitId);
            var dataUnitToDelete = DataUnitDataSource.LoadDataUnitsFromFile(filepath).Find(unit => unit.Id == dataUnitId);
            var dataUnitDeleted = DataUnitDataSource.DeleteDataUnit(filepath, dataUnitId);
            if (dataUnitDeleted)
            {
                try
                {
                    IndexRepository.RemoveDataUnit(collectionId, dataUnitId);
                }
                catch
                {
                    if (dataUnitToDelete != null)
                    {
                        DataUnitDataSource.SaveDataUnit(filepath, dataUnitToDelete);
                    }
                    throw;
                }
            }
        }

        public void DeleteAllCollectionData(string collectionId)
        {
            var deletedIndex = IndexRepository.RemoveIndex(collectionId);
            var collectionDataFilepath = PathUtils.GetCollectionDataFilepath(collectionId);
            try
            {
                Directory.Delete(collectionDataFilepath, true);
            }
            catch
            {
                IndexRepository.AddIndex(collectionId, deletedIndex);
                throw;
            }
        }

        protected List<DataUnit> GetAllCollectionDataUnits(string collectionId)
        {
            var dataUnits = new List<DataUnit>();
            var filePaths = IndexRepository.GetAllIndexesDataFilePaths(collectionId);
            foreach (var filePath in filePaths)
            {
                var indexDataUnits = DataUnitDataSource.LoadDataUnitsFromFile(filePath);
                dataUnits.AddRange(indexDataUnits);
            }
            return dataUnits;
        }
        protected List<DataUnit> SearchDataUnits(List<DataUnit> dataUnits, List<DataUnitProp> searchProps)
        {
            var resultSet = new List<DataUnit>();
            foreach (var dataUnit in dataUnits)
            {
                var matches = dataUnit.MatchWithProps(searchProps);
                if (matches)
                {
                    resultSet.Add(dataUnit);
                }
            }
            return resultSet;
        }
        protected void DivideIndexByTwo(string collectionId, IndexDivideRequest indexDivideRequest)
        {
            var collectionFilepath = PathUtils.GetCollectionDataFilepath(collectionId);
            DataUnitDataSource.DivideIndexDataByTwo(collectionFilepath, indexDivideRequest);
        }
    }
}