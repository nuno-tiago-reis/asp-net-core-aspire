namespace Memento.Aspire.Domain.Service.Configuration;

using Memento.Aspire.Core.Localization;
using Memento.Aspire.Core.Logging;
using Memento.Aspire.Core.Messaging;

/// <summary>
/// Implements the 'ApplicationSettings' section.
/// </summary>
public sealed record ApplicationSettings
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the database options.
	/// </summary>
	public required DatabaseOptions Database { get; init; }

	/// <summary>
	/// Gets or sets the message bus options.
	/// </summary>
	public required MessageBusOptions MessageBus { get; init; }

	/// <summary>
	/// Gets or sets the localizer options.
	/// </summary>
	public required LocalizerOptions Localizer { get; init; }

	/// <summary>
	/// Gets or sets the logging options.
	/// </summary>
	public required LoggingOptions Logging { get; init; }

	#endregion
}

/// <summary>
/// Implements the 'DatabaseOptions' section.
/// </summary>
public sealed record DatabaseOptions
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the connection string.
	/// </summary>
	public required string ConnectionString { get; init; }
	#endregion
}
