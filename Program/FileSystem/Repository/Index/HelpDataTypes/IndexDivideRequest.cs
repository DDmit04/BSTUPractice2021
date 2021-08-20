using System.Collections.Generic;
using System.Linq;

namespace Program
{
    public class IndexDivideRequest
    {
        public string FromFilename { get; }
        public Dictionary<string, SortedSet<long>> ToFilenamesIds { get; }
        public bool NeedDeleteFromFile
        {
            get => !ToFilenamesIds.Keys.Contains(FromFilename);
        }

        public IndexDivideRequest(string fromFilename, Dictionary<string, SortedSet<long>> toFilenamesIds)
        {
            FromFilename = fromFilename;
            ToFilenamesIds = toFilenamesIds;
        }
    }
}