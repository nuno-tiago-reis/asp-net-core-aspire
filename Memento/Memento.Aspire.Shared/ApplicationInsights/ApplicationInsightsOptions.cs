namespace Memento.Aspire.Shared.ApplicationInsights;

using Microsoft.ApplicationInsights.AspNetCore.Extensions;

/// <summary>
/// Implements the 'ApplicationInsights' options.
/// </summary>
public sealed class ApplicationInsightsOptions : ApplicationInsightsServiceOptions
{
	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="ApplicationInsightsOptions"/> class.
	/// </summary>
	public ApplicationInsightsOptions()
	{
		// Metrics
		this.AddAutoCollectedMetricExtractor = true;

		// Modules
		this.EnableAppServicesHeartbeatTelemetryModule = true;
		this.EnableAzureInstanceMetadataTelemetryModule = true;
		this.EnableDependencyTrackingTelemetryModule = true;
		this.EnableEventCounterCollectionModule = true;
		this.EnablePerformanceCounterCollectionModule = true;
		this.EnableRequestTrackingTelemetryModule = true;

		// Dependencies
		this.DependencyCollectionOptions.EnableLegacyCorrelationHeadersInjection = true;

		// Requests
		this.RequestCollectionOptions.InjectResponseHeaders = true;
		this.RequestCollectionOptions.TrackExceptions = true;

		// Miscellaneous
		this.EnableAdaptiveSampling = true;
		this.EnableAuthenticationTrackingJavaScript = false;
		this.EnableDebugLogger = false;
		this.EnableHeartbeat = true;
		this.EnableQuickPulseMetricStream = true;
	}
	#endregion
}
