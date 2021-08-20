using System.Collections.Generic;

namespace Program.userInterface
{
    public interface IDataUnitDataSource
    {
        void DivideIndexDataByTwo(string collectionFilepath, IndexDivideRequest indexDivideRequest);
        List<DataUnit> LoadDataUnitsFromFile(string filepath);
        DataUnit SaveDataUnit(string filepath, DataUnit dataUnit);
        bool DeleteDataUnit(string filepath, long dataUnitId);
    }
}