/// <summary>
/// API Response wrapper for citizen data.
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates if the operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response message.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Response data payload.
    /// </summary>
    public T? Data { get; set; }
}