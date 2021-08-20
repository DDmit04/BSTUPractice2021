using System;
using System.Collections.Generic;
using System.IO;

namespace Program
{
    [Serializable]
    public abstract class DataUnitProp : IComparable
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public DateTime CreationTime { get; }
        public DataType Type { get; }

        public bool IsValid
        {
            get => !string.IsNullOrEmpty(Name.Trim()) && Value != null;
        }

        public DataUnitProp(string name, object value, DataType type)
        {
            Name = name;
            Value = value;
            Type = type;
            CreationTime = DateTime.Now;
        }

        public DataUnitProp(FileStream fileStream, DataType type)
        {
            Name = SerializeUtils.ReadNextString(fileStream);
            CreationTime = SerializeUtils.ReadNextDateTime(fileStream);
            Type = type;
            Value = DeserializeValue(fileStream);
        }

        public List<byte> Serialize()
        {
            var bytes = new List<byte>();
            bytes.Add(SerializeUtils.IntToByte((int)Type));
            bytes.AddRange(SerializeUtils.StringToBytes(Name));
            bytes.AddRange(SerializeUtils.DateTimeToByte(CreationTime));
            bytes.AddRange(SerializeValue());
            return bytes;
        }
        protected abstract List<byte> SerializeValue();
        protected abstract object DeserializeValue(FileStream fileStream);

        protected bool Equals(DataUnitProp other)
        {
            return Name == other.Name && Equals(Value, other.Value) && CreationTime.Equals(other.CreationTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DataUnitProp) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Value, CreationTime);
        }

        public int CompareTo(object? obj)
        {
            if (obj != null)
            {
                var val = (DataUnitProp) obj;
                var compareRes = String.Compare(Name, val.Name, StringComparison.Ordinal);
                if(compareRes == 0)
                {
                    compareRes = CreationTime.CompareTo(val.CreationTime);
                }
                return compareRes;
            }
            else
            {
                return -1;
            }
        }
        public override string ToString()
        {
            return $"{Name} : {Value}";
        }

    }
}