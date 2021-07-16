using System;
using System.Collections.Generic;
using System.IO;

namespace Program
{
    public class CollectionDefinition
    {
        public string Id { get; }
        public string Name { get; set; }
        public DateTime CreationDate { get; }

        public CollectionDefinition(string name)
        {
            Id = null;
            Name = name;
        }

        public CollectionDefinition(string id, string name)
        {
            Id = id;
            Name = name;
            CreationDate = DateTime.Now;
        }

        public CollectionDefinition(string id, string name, DateTime creationDate)
        {
            Id = id;
            Name = name;
            CreationDate = creationDate;
        }

        public List<byte> Serialize()
        {
            var bytes = new List<byte>();
            bytes.AddRange(SerializeUtils.StringToBytes(Id));
            bytes.AddRange(SerializeUtils.StringToBytes(Name));
            bytes.AddRange(SerializeUtils.DateTimeToByte(CreationDate));
            return bytes;
        }

        public static CollectionDefinition Deserialize(FileStream fileStream)
        {
            var id = SerializeUtils.ReadNextString(fileStream);
            var name = SerializeUtils.ReadNextString(fileStream);
            var creationDate = SerializeUtils.ReadNextDateTime(fileStream);
            return new CollectionDefinition(id, name, creationDate);
        }
    }
}