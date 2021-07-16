using System.Collections.Generic;
using Program.Controller.interfaces;
using Program.FileSystem.Utils;
using Program.userInterface;

namespace Program.Controller
{
    public class CollectionDefinitionRepository : ICollectionDefinitionRepo
    {
        public ICollectionDefDataSource CollectionDefDataSource { get; }

        public CollectionDefinitionRepository(ICollectionDefDataSource collectionDefDataSource)
        {
            CollectionDefDataSource = collectionDefDataSource;
        }

        public List<CollectionDefinition> LoadCollectionDefinitions()
        {
            return CollectionDefDataSource.LoadCollectionDefinitions();
        }

        public CollectionDefinition SaveCollection(CollectionDefinition collectionDefinition)
        {
            var oldDef = CollectionDefDataSource.LoadCollectionDefinitions()
                .Find(def => def.Id == collectionDefinition.Id);
            if (oldDef != null)
            {
                var updatedDef = new CollectionDefinition(oldDef.Id, collectionDefinition.Name, oldDef.CreationDate);
                CollectionDefDataSource.SaveCollectionDefinition(updatedDef);
            }
            else
            {
                CollectionDefDataSource.SaveCollectionDefinition(collectionDefinition);
            }
            return oldDef;
        }
        public CollectionDefinition CreateCollection(string collectionName)
        {
            var colId = IdUtils.GenerateId();
            var newColDef = new CollectionDefinition(colId, collectionName);
            CollectionDefDataSource.SaveCollectionDefinition(newColDef);
            return newColDef;
        }

        public CollectionDefinition DeleteCollection(string collectionId)
        {
            return CollectionDefDataSource.DeleteCollection(collectionId);
        }
    }
}