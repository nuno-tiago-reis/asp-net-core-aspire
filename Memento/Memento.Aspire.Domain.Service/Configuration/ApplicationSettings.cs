namespace Memento.Aspire.Domain.Service.Configuration;

using Memento.Aspire.Shared.Localization;
using Memento.Aspire.Shared.Logging;
using Memento.Aspire.Shared.Messaging;

/// <summary>
/// Implements the 'ApplicationSettings' section.
/// </summary>
public sealed record ApplicationSettings
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the database options.
	/// </summary>
	public required DatabaseOptions Database { get; set; }

	/// <summary>
	/// Gets or sets the message bus options.
	/// </summary>
	public required MessageBusOptions MessageBus { get; set; }

	/// <summary>
	/// Gets or sets the localizer options.
	/// </summary>
	public required LocalizerOptions Localizer { get; set; }

	/// <summary>
	/// Gets or sets the logging options.
	/// </summary>
	public required LoggingOptions Logging { get; set; }

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
	public required string ConnectionString { get; set; }
	#endregion
}
