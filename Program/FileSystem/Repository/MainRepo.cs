using System.Collections.Generic;
using System.IO;
using Program.Controller;
using Program.Controller.interfaces;
using Program.Exceptions.collection;
using Program.userInterface;
using Program.Utils;

namespace Program
{
    public class MainRepo : IMainRepo
    {
        protected ICollectionDefDataSource CollectionDefDataSource;
        protected IDataUnitDataSource DataUnitDataSource;
        protected IDataUnitIndexDataSource DataUnitIndexDataSource;
        protected IndexRepository IndexRepository;
        protected  CollectionDefinitionRepository CollectionDefinitionRepository { get; }
        protected  DataUnitRepository DataUnitRepository { get; }

        public MainRepo()
        {
            Directory.CreateDirectory(FileSystemConfig.COLLECTIONS_DATA_FILEPATH);
            DirUtils.CreateDirsForFile(FileSystemConfig.COLLECTION_DEFS_FILEPATH);
            
            DataUnitDataSource = new DataUnitDataSource();
            DataUnitIndexDataSource = new DataUnitIndexDataSource();
            CollectionDefDataSource = new CollectionDefinitionDataSource();

            var colDefs = CollectionDefDataSource.LoadCollectionDefinitions();

            IndexRepository = new IndexRepository(colDefs, DataUnitIndexDataSource);
            CollectionDefinitionRepository = new CollectionDefinitionRepository(CollectionDefDataSource);
            DataUnitRepository = new DataUnitRepository(DataUnitDataSource, IndexRepository);
        }

        public List<DataUnit> LoadCollectionData(string collectionId)
        {
            return DataUnitRepository.LoadCollectionData(collectionId);
        }

        public List<DataUnit> GetDataUnitsByProps(string collectionId, List<DataUnitProp> props)
        {
            return DataUnitRepository.GetDataUnitsByProps(collectionId, props);
        }

        public List<DataUnit> GetDataUnitsByPropsAllCollections(List<DataUnitProp> props)
        {
            var dataUnits = DataUnitRepository.GetDataUnitsByPropsAllCollections(props);
            return dataUnits;
        }

        public DataUnit UpdateDataUnit(string collectionId, DataUnit dataUnit)
        {
            return DataUnitRepository.SaveDataUnit(collectionId, dataUnit);
        }
        public DataUnit CreateDataUnit(string collectionId)
        {
            return DataUnitRepository.CreateDataUnit(collectionId);
        }

        public void DeleteDataUnit(string collectionId, long dataUnitId)
        {
            DataUnitRepository.DeleteDataUnit(collectionId, dataUnitId);
        }

        public List<CollectionDefinition> LoadCollectionDefinitions()
        {
            return CollectionDefinitionRepository.LoadCollectionDefinitions();
        }

        public CollectionDefinition RenameCollection(string collectionId, string newCollectionName)
        {
            var oldColDef = CollectionDefinitionRepository.LoadCollectionDefinitions()
                .Find(def => def.Id == collectionId);
            if (oldColDef != null)
            {
                oldColDef.Name = newCollectionName;
                var savedDefinition = CollectionDefinitionRepository.SaveCollection(oldColDef);
                return savedDefinition;
            }
            throw CollectionNotFoundException.GenerateException(collectionId);
        }

        public CollectionDefinition CreateCollection(string collectionName)
        {
            var savedCollection = CollectionDefinitionRepository.CreateCollection(collectionName);
            try
            {
                IndexRepository.CreateIndex(savedCollection.Id);
            }
            catch
            {
                CollectionDefDataSource.DeleteCollection(savedCollection.Id);
                throw;
            }
            return savedCollection;
        }

        public void DeleteCollection(string collectionId)
        {
            var deletedCollection = CollectionDefinitionRepository.DeleteCollection(collectionId);
            try
            {
                DataUnitRepository.DeleteAllCollectionData(collectionId);
            }
            catch
            {
                CollectionDefinitionRepository.SaveCollection(deletedCollection);
                throw;
            }
        }
    }
}