using System;
using System.Collections.Generic;

namespace Program.userInterface
{
    public interface IUserInterface
    {
        List<CollectionDefinition> GetCollectionDefinitions();

        CollectionDefinition CreateCollection(string collectionName);
        CollectionDefinition RenameCollection(string collectionId, string newName);
        void DeleteCollection(string collectionId);
        List<DataUnit> GetCollectionData(string collectionId, Comparison<DataUnit> sortFunc = null);
        List<DataUnit> SearchDataUnits(string collectionId, List<DataUnitProp> searchFields, Comparison<DataUnit> sortFunc = null);
        List<DataUnit> SearchDataUnitsAllCollections(List<DataUnitProp> searchFields, Comparison<DataUnit> sortFunc = null);

        DataUnit AddDataUnit(string collectionId);
        DataUnit UpdateDataUnit(string collectionId, DataUnit dataUnit);
        void DeleteDataUnit(string collectionId, long dataUnitId);
    }
}