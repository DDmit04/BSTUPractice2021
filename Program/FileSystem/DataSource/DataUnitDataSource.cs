using System.Collections.Generic;
using System.IO;
using System.Linq;
using Program.userInterface;
using Program.Utils;

namespace Program
{
    public class DataUnitDataSource : IDataUnitDataSource
    {
        public void DivideIndexDataByTwo(string collectionFilepath, IndexDivideRequest indexDivideRequest)
        {
            var fromFilepath = collectionFilepath + indexDivideRequest.FromFilename;
            var dataUnits = LoadDataUnitsFromFile(fromFilepath);
            foreach (var newIdsLocation in indexDivideRequest.ToFilenamesIds)
            {
                var filepath = collectionFilepath + newIdsLocation.Key;
                var ids = newIdsLocation.Value;
                var units = dataUnits.Where(unit => ids.Contains(unit.Id));
                RewriteDataUnitsToFile(filepath, new List<DataUnit>(units));
            }
            if (indexDivideRequest.NeedDeleteFromFile)
            {
                DirUtils.DeleteFile(fromFilepath);
            }
        }

        public List<DataUnit> LoadDataUnitsFromFile(string filepath)
        {
            var fileExists = File.Exists(filepath);
            if (fileExists)
            {
                var dataList = new List<DataUnit>();
                using (var stream = new FileStream(filepath, FileMode.Open))
                {
                    var unitsCount = SerializeUtils.ReadNextInt(stream);
                    for (var i = 0; i < unitsCount; i++)
                    {
                        var dataUnit = DataUnit.Deserialize(stream);
                        dataList.Add(dataUnit);
                    }
                }
                return dataList;
            }
            throw new FileNotFoundException($"No file {filepath} found for loading data units!");
        }

        public DataUnit SaveDataUnit(string filepath, DataUnit dataUnit)
        {
            var fileExists = File.Exists(filepath);
            if (fileExists)
            {
                var dataUnits = LoadDataUnitsFromFile(filepath);
                var dataUnitToSave = dataUnits.Find(unit => unit.Id == dataUnit.Id);
                if (dataUnitToSave != null)
                {
                    dataUnits.Remove(dataUnitToSave);
                    dataUnits.Add(dataUnit);
                    RewriteDataUnitsToFile(filepath, dataUnits);
                }
                else
                {
                    AppendDataUnitToFile(filepath, dataUnit);
                }
            }
            else
            {
                RewriteDataUnitsToFile(filepath, new List<DataUnit>(){dataUnit});
            }
            return dataUnit;
        }

        public bool DeleteDataUnit(string filepath, long dataUnitId)
        {
            var dataUnits = LoadDataUnitsFromFile(filepath);
            var dataUnitToDelete = dataUnits.Find(unit => unit.Id == dataUnitId);
            if (dataUnitToDelete != null)
            {
                dataUnits.Remove(dataUnitToDelete);
                if (dataUnits.Count == 0)
                {
                    DirUtils.DeleteFile(filepath);
                }
                else
                {
                    RewriteDataUnitsToFile(filepath, dataUnits);
                }
                return true;
            }
            return false;
        }
        protected void AppendDataUnitToFile(string filepath, DataUnit dataUnit)
        {
            using (var fileStream = new FileStream(filepath, FileMode.Open))
            {
                var bytes = new List<byte>();
                bytes.AddRange(dataUnit.Serialize());
                var unitsCount = SerializeUtils.ReadNextInt(fileStream);
                if (unitsCount == -1)
                {
                    unitsCount = 0;
                }
                unitsCount++;
                fileStream.Seek(0, SeekOrigin.Begin);
                fileStream.WriteByte(SerializeUtils.IntToByte(unitsCount));
                fileStream.Seek(0, SeekOrigin.End);
                fileStream.Write(bytes.ToArray(), 0, bytes.Count);
            }
        }

        protected void RewriteDataUnitsToFile(string filepath, List<DataUnit> dataUnits)
        {
            var unicUnits = dataUnits.GroupBy(x => x)
                .Where(x => x.Count() == 1)
                .Select(x => x.Key)
                .ToList();
            var fileExists = File.Exists(filepath);
            if (!fileExists)
            {
                DirUtils.CreateDirsForFile(filepath);
            }
            using (var stream = new FileStream(filepath, FileMode.Create))
            {
                var bytes = new List<byte>();
                bytes.Add(SerializeUtils.IntToByte(unicUnits.Count));
                foreach (var dataUnit in unicUnits)
                {
                    bytes.AddRange(dataUnit.Serialize());
                }
                stream.Write(bytes.ToArray(), 0, bytes.Count);
            }
        }
    }
}