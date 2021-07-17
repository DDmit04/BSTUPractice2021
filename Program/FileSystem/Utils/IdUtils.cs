using System;
using System.Numerics;

namespace Program.FileSystem.Utils
{
    public class IdUtils
    {
        public static long GenerateDataUnitId()
        {
            var rnd = new Random();
            var res =((long) (uint) rnd.Next(int.MinValue, int.MaxValue)) << 32 | (uint) rnd.Next(int.MinValue, int.MaxValue);
            if (res < 0)
            {
                res *= -1;
            }
            return res;
        }
        public static string GenerateId()
        {
            var idChars = new char[32];
            var rnd = new Random();
            for (var i = 0; i < 32; i++)
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

        public static string GetMinObjId()
        {
            var idChars = new char[32];
            for (var i = 0; i < 32; i++)
            {
                var nextChar = i switch
                {
                    < 8 => 'A',
                    < 16 => 'a',
                    < 24 => '0',
                    _ => 'A'
                };
                idChars[i] = nextChar;
            }
            return new string(idChars);
        }

        public static string GetMaxObjId()
        {
            var idChars = new char[32];
            for (var i = 0; i < 32; i++)
            {
                var nextChar = i switch
                {
                    < 8 => 'Z',
                    < 16 => 'z',
                    < 24 => '9',
                    _ => 'Z'
                };
                idChars[i] = nextChar;
            }
            return new string(idChars);
        }
        public static long GetMidLong(long first, long sec)
        {
            var bigFirst = new BigInteger(first);
            var bigSec = new BigInteger(sec);
            var res = (bigFirst + bigSec) / 2;
            return (long) Math.Round((double) res);
        }
    }
}