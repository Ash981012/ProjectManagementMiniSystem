namespace ProjectManagement.Application.Abstractions;

public interface IAppCache
{
    Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan absoluteExpirationRelativeToNow,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken);

    void Remove(string key);
}
