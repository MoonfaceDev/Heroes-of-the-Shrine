public interface IInterruptable
{
    delegate void OnStop();

    event OnStop onStop;
    void Stop();
}
