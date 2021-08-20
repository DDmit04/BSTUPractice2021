using System.Collections.Generic;

namespace Program.userInterface
{
    public interface IDataUnitIndexDataSource
    {
        void UpdateIndexFile(string collectionId, IdIndex index);
        public Dictionary<string, IdIndex> LoadRootIndexes(List<CollectionDefinition> colDefs);
        public void SaveIndexToFile(string filepath, IdIndex index);
        IdIndex LoadRootIndexFromFile(string filepath, string collectionId);

    }
}