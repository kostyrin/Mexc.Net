namespace Mexc.Net.Objects.Core;

/// <summary>
/// API error
/// </summary>
public class MexcRestApiError : Error
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <param name="data"></param>
    public MexcRestApiError(int? code, string message, object? data) : base(code, message, data)
    {
    }
}