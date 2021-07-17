using System.Collections.Generic;

namespace Program.userInterface
{
    public interface IDataUnitDataSource
    {
        void DivideIndexDataByTwo(string oldIndexFilepath, string newLeftIndexPath, string newRightIndexPath,
            long midId);
        List<DataUnit> LoadDataUnitsFromFile(string filepath);
        DataUnit SaveDataUnit(string filepath, DataUnit dataUnit);
        bool DeleteDataUnit(string filepath, long dataUnitId);
        public void UniteDataIndex(string parentIndexFilepath, string leftIndexPath, string rightIndexPath);
    }
}