namespace Memento.Aspire.Core.Cache;

/// <summary>
/// Implements the 'Cache' options.
/// </summary>
public sealed record CacheOptions
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the connection string.
	/// </summary>
	public required string? ConnectionString { get; init; }
	#endregion
}
