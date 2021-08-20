using System;
using System.Collections.Generic;
using Program.Controller.interfaces;

namespace Program.userInterface
{
    public class UserInterface : IUserInterface
    {
        protected Comparison<DataUnit> DefaultDataUnitsSort = (unit, dataUnit) => unit.CreationTime.CompareTo(dataUnit.CreationTime);
        protected IMainRepo MainRepo { get; }

        public UserInterface(IMainRepo mainRepo)
        {
            MainRepo = mainRepo;
        }

        public List<CollectionDefinition> GetCollectionDefinitions()
        {
            var definitions = MainRepo.LoadCollectionDefinitions();
            definitions.Sort((firstDef, secDef) => firstDef.CreationDate.CompareTo(secDef.CreationDate));
            return definitions;
        }

        public CollectionDefinition CreateCollection(string collectionName)
        {
            return MainRepo.CreateCollection(collectionName);
        }

        public CollectionDefinition RenameCollection(string collectionId, string newName)
        {
            return MainRepo.RenameCollection(collectionId, newName);
        }

        public void DeleteCollection(string collectionId)
        {
            MainRepo.DeleteCollection(collectionId);
        }

        public List<DataUnit> GetCollectionData(string collectionId, Comparison<DataUnit> sortFunc = null)
        {
            sortFunc ??= DefaultDataUnitsSort;
            var dataUnits = MainRepo.LoadCollectionData(collectionId);
            dataUnits.Sort(sortFunc);
            return dataUnits;
        }

        public DataUnit AddDataUnit(string collectionId)
        {
            return MainRepo.CreateDataUnit(collectionId);
        }

        public DataUnit UpdateDataUnit(string collectionId, DataUnit dataUnit)
        {
            return MainRepo.UpdateDataUnit(collectionId, dataUnit);
        }

        public void DeleteDataUnit(string collectionId, long dataUnitId)
        {
            MainRepo.DeleteDataUnit(collectionId, dataUnitId);
        }

        public List<DataUnit> SearchDataUnits(string collectionId, List<DataUnitProp> searchFields, Comparison<DataUnit> sortFunc = null)
        {
            sortFunc ??= DefaultDataUnitsSort;
            var dataUnits = new List<DataUnit>();
            if (searchFields.Count == 0)
            {
                dataUnits = MainRepo.LoadCollectionData(collectionId);
            }
            else
            {
                dataUnits = MainRepo.GetDataUnitsByProps(collectionId, searchFields);
            }
            dataUnits.Sort(sortFunc);
            return dataUnits;
        }
        
        public List<DataUnit> SearchDataUnitsAllCollections(List<DataUnitProp> searchFields, Comparison<DataUnit> sortFunc = null)
        {
            sortFunc ??= DefaultDataUnitsSort;
            if (searchFields.Count == 0)
            {
                throw new Exception("Search fields are not defined!");
            }
            var dataUnits = MainRepo.GetDataUnitsByPropsAllCollections(searchFields);
            dataUnits.Sort(sortFunc);
            return dataUnits;
        }
    }
}