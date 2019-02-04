namespace WebStats.Interfaces
{
    public interface IRequestProcessor
    {
        void ProcessRequestEnd();
        void ProcessRequestStart();
    }
}