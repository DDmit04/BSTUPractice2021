using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Program
{
    [Serializable]
    public class DataUnit : IComparable
    {
        public long Id { get; }
        public List<DataUnitProp> Props { get; }
        
        public DateTime CreationTime { get; }

        public DataUnit()
        {
            Id = 0;
            Props = new List<DataUnitProp>();
            CreationTime = DateTime.Now;
        }

        public DataUnit(long id)
        {
            Id = id;
            Props = new List<DataUnitProp>();
            CreationTime = DateTime.Now;
        }

        protected DataUnit(long id, List<DataUnitProp> props, DateTime creationTime)
        {
            Id = id;
            Props = props;
            CreationTime = creationTime;
        }

        public void ChangePropName(string oldName, string newName)
        {
            var oldProp = GetProperty(oldName);
            if (oldProp != null)
            {
                oldProp.Name = newName;
            }
        }
        public bool MatchWithProps(List<DataUnitProp> propsToMatch)
        {
            var matches = propsToMatch.All(searchField =>
            {
                var matchingProp = GetProperty(searchField.Name);
                if (matchingProp != null)
                {
                    return matchingProp.Value.Equals(searchField.Value);
                }
                return false;
            });
            return Props.Count >= propsToMatch.Count & matches;
        }

        public bool RemoveProperty(string propName)
        {
            var removedProps = Props.RemoveAll(prop => prop.Name == propName);
            return removedProps > 0;
        }
        public void Update(List<DataUnitProp> updatedProps)
        {
            foreach (var prop in updatedProps)
            {
                if (!Props.Contains(prop))
                {
                    AddProperty(prop);
                }
                else
                {
                    UpdateProperty(prop);
                }
            }
        }

        public DataUnitProp GetProperty(string name)
        {
            return Props.FirstOrDefault(prop => prop.Name == name);
        }

        public void AddProperty(DataUnitProp dataUnitProp)
        {
            Props.Add(dataUnitProp);
        }
        public void UpdateProperty(DataUnitProp dataUnitProp)
        {
            SetProperty(dataUnitProp.Name, dataUnitProp);
        }
        public void SetProperty(string oldName, DataUnitProp dataUnitProp)
        {
            var propToUpdate = GetProperty(oldName);
            if (propToUpdate != null && dataUnitProp.Name != null)
            {
                Props.Remove(propToUpdate);
                Props.Add(dataUnitProp);
            }
            else
            {
                AddProperty(dataUnitProp);
            }
        }

        public List<byte> Serialize()
        {
            var bytes = new List<byte>();
            bytes.AddRange(SerializeUtils.LongToBytes(Id));
            bytes.AddRange(SerializeUtils.DateTimeToByte(CreationTime));
            var propsCount = Props.Count;
            bytes.Add(SerializeUtils.IntToByte(propsCount));
            foreach (var prop in Props)
            {
                bytes.AddRange(prop.Serialize());
            }
            return bytes;
        }

        public static DataUnit Deserialize(FileStream fileStream)
        {
            var id = SerializeUtils.ReadNextLong(fileStream);
            var creationTime = SerializeUtils.ReadNextDateTime(fileStream);
            var propsCount = SerializeUtils.ReadNextInt(fileStream);
            var props = new List<DataUnitProp>();
            for (var i = 0; i < propsCount; i++)
            {
                var prop = DataUnitPropFactory.DeserializeDataUnit(fileStream);
                props.Add(prop);
            }
            return new DataUnit(id, props, creationTime);
        }
        protected bool Equals(DataUnit other)
        {
            return Id == other.Id && CreationTime == other.CreationTime;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DataUnit) obj);
        }
        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }

        public int CompareTo(object? obj)
        {
            if (obj != null)
            {
                var val = (DataUnit) obj;
                return Id.CompareTo(val.Id);
            }
            else
            {
                return -1;
            }
        }

        public bool DeepEquals(DataUnit other)
        {
            var otherProps = new HashSet<DataUnitProp>(other.Props);
            var resProps = new HashSet<DataUnitProp>(Props);
            resProps.ExceptWith(otherProps);
            return Equals(other) && resProps.Count == 0;
        }

        public override string ToString()
        {
            var resStr = "{\n\t";
            resStr += Id.ToString();
            if (Props.Count > 0)
            {
                resStr += ",\n\t";
                DataUnitProp prop;
                for (var i = 0; i < Props.Count - 1; i++)
                {
                    prop = Props[i];
                    resStr += prop.ToString();
                    resStr += ",\n\t";
                }
                prop = Props[Props.Count - 1];
                resStr += prop.ToString();
            }
            resStr += "\n}";
            return resStr;
        }
    }
}