namespace Memento.Aspire.Core.Logging;

using Microsoft.AspNetCore.HttpLogging;

/// <summary>
/// Implements the <see cref="LoggingOptions"/>.
/// </summary>
public sealed record LoggingOptions
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the http logging options.
	/// </summary>
	public required HttpLoggingOptions? HttpLogging { get; init; }
	#endregion
}

/// <summary>
/// Implements the <see cref="HttpLoggingOptions"/>.
/// </summary>
public sealed record HttpLoggingOptions
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the fields.
	/// </summary>
	public required HttpLoggingFields? Fields { get; init; }
	#endregion
}
