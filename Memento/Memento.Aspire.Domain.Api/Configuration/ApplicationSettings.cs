namespace Memento.Aspire.Domain.Api.Configuration;

using Memento.Aspire.Core.Cache;
using Memento.Aspire.Core.Localization;
using Memento.Aspire.Core.Logging;
using Memento.Aspire.Core.Messaging;
using Memento.Aspire.Core.Swagger;

/// <summary>
/// Implements the 'ApplicationSettings' section.
/// </summary>
public sealed record ApplicationSettings
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the cache options.
	/// </summary>
	public required CacheOptions Cache { get; init; }

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

	/// <summary>
	/// Gets or sets the swagger options.
	/// </summary>
	public required SwaggerOptions Swagger { get; init; }
	#endregion
}
