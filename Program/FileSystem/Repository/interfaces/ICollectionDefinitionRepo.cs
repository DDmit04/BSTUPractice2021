using System.Collections.Generic;

namespace Program.Controller.interfaces
{
    public interface ICollectionDefinitionRepo
    {
        public List<CollectionDefinition> LoadCollectionDefinitions();
        public CollectionDefinition SaveCollection(CollectionDefinition collectionDefinition);
        public CollectionDefinition CreateCollection(string collectionName);
        public CollectionDefinition DeleteCollection(string collectionId);
    }
}