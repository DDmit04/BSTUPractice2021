using System;

namespace Program.FileSystem.Utils
{
    public class IdUtils
    {
        public static long GenerateDataUnitId()
        {
            var max = DbConfig.MAX_ID;
            var min = DbConfig.MIN_ID;
            Random rd = new Random();
            if (max <= min)
            {
                throw new ArgumentOutOfRangeException("max", "max must be > min!");
            }
            var uRange = (ulong)(max - min);
            ulong ulongRand;
            do
            {
                var buf = new byte[8];
                rd.NextBytes(buf);
                ulongRand = (ulong)BitConverter.ToInt64(buf, 0);
            } while (ulongRand > ulong.MaxValue - ((ulong.MaxValue % uRange) + 1) % uRange);

            return (long)(ulongRand % uRange) + min;
        }
        public static string GenerateCollectionId(int length = 32)
        {
            var idChars = new char[length];
            var rnd = new Random();
            for (var i = 0; i < length; i++)
            {
                var nextChar = i switch
                {
                    < 8 => (char) rnd.Next('A', 'Z' + 1),
                    < 16 => (char) rnd.Next('a', 'z' + 1),
                    < 24 => (char) rnd.Next('0', '9' + 1),
                    _ => (char) rnd.Next('A', 'Z' + 1)
                };
                idChars[i] = nextChar;
            }
            return new string(idChars);
        }
    }
}