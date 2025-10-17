namespace MyPinPad.Core.Helpers
{
    public class AutoIncrementIdGenerator
    {
        public static long _id = 0;

        public static long GetNextId() => Interlocked.Increment(ref _id);
    }
}
