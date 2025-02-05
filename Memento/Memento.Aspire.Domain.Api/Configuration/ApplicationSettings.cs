namespace Memento.Aspire.Domain.Api.Configuration;

using Memento.Aspire.Shared.Cache;
using Memento.Aspire.Shared.Localization;
using Memento.Aspire.Shared.Logging;
using Memento.Aspire.Shared.Messaging;
using Memento.Aspire.Shared.Swagger;

/// <summary>
/// Implements the 'ApplicationSettings' section.
/// </summary>
public sealed record ApplicationSettings
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the cache options.
	/// </summary>
	public required CacheOptions Cache { get; set; }

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

	/// <summary>
	/// Gets or sets the swagger options.
	/// </summary>
	public required SwaggerOptions Swagger { get; set; }
	#endregion
}
