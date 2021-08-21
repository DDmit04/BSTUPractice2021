namespace Program.Exceptions
{
    public class IndexUniteException : IndexException
    {
        public IndexUniteException(string? message) : base(message)
        {
        }

        public static IndexUniteException GenerateIndexMergeException()
        {
            return new($"Can't merge indexes with non zero child count!");
        }
    }
}