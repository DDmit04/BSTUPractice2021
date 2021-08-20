using System;

namespace Program.FileSystem.Exceptions.dataUnit
{
    public class DataUnitIdCollisionException : Exception
    {
        public DataUnitIdCollisionException(string? message) : base(message)
        {
        }

        public static DataUnitIdCollisionException GenerateException()
        {
            return new("Calculation of new data unit ID is uses too many iterations because of collisions of ID's!");
        }
    }
}