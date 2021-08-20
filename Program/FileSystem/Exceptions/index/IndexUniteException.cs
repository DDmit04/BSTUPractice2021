namespace Program.Exceptions
{
    public class IndexUniteException : IndexException
    {
        public IndexUniteException(string? message) : base(message)
        {
        }

        public static IndexUniteException GenerateException(IdIndex indexToUnite)
        {
            return new($"Can't unite index [{indexToUnite.MinBorder}..{indexToUnite.MaxBorder}]!");
        }
        
        public static IndexUniteException GenerateIndexMergeException()
        {
            return new($"Can't merge indexes with non zero child count!");
        }
    }
}