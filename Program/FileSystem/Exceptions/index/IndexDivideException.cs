namespace Program.Exceptions
{
    public class IndexDivideException : IndexException
    {
        public IndexDivideException(string? message) : base(message)
        {
        }

        public static IndexDivideException GenerateException(IdIndex indexToDivide)
        {
            return new($"Can't divide index [{indexToDivide.MinBorder}..{indexToDivide.MaxBorder}] no parent and this index isn't leaf!");
        }
    }
}