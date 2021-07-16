using System.Collections.Generic;

namespace Program.Controller.interfaces
{
    public interface IMainRepo
    {
        public List<DataUnit> LoadCollectionData(string collectionId);
        public List<DataUnit> GetDataUnitsByProps(string collectionId, List<DataUnitProp> props);
        public List<DataUnit> GetDataUnitsByPropsAllCollections(List<DataUnitProp> props);
        public DataUnit CreateDataUnit(string collectionId);
        public DataUnit UpdateDataUnit(string collectionId, DataUnit dataUnit);
        public void DeleteDataUnit(string collectionId, string dataUnitId);

        public List<CollectionDefinition> LoadCollectionDefinitions();
        public CollectionDefinition RenameCollection(string collectionId, string newCollectionName);
        public CollectionDefinition CreateCollection(string collectionName);
        public void DeleteCollection(string collectionId);
    }
}