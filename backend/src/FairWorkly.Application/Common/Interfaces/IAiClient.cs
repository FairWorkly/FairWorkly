namespace FairWorkly.Application.Common.Interfaces;

public interface IAiClient
{
    // Define a generic Post method for an HTTP POST call
    // TRequest: type of data sent to the AI
    // TResponse: type of data returned by the AI
    Task<TResponse> PostAsync<TRequest, TResponse>(
        string route,
        TRequest request,
        CancellationToken cancellationToken = default
    );
}
