using System.Linq;

namespace Program.Exceptions
{
    public class IndexRangeException : IndexException
    {
        
        public IndexRangeException(string? message) : base(message)
        {
        }

        public static IndexRangeException GenerateIdNotFoundException(IdIndex index, long id)
        {
            return new($"DataUnit with id - {id} isn't in index range! [{index.Borders.Min()}..{index.Borders.Max()}] ");
        }
        
        public static IndexRangeException GenerateNearestIndexNotFoundException(IdIndex from, IdIndex index)
        {
            return new($"Can't find nearest index for [{index.MinBorder}..{index.MaxBorder}] from [{from.MinBorder}..{from.MaxBorder}] ");
        }
    }
}