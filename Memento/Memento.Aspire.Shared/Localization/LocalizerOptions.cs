namespace Memento.Aspire.Shared.Localization;

/// <summary>
/// Implements the 'Localizer' options.
/// </summary>
public sealed record LocalizerOptions
{
	#region [Constants]
	/// <summary>
	/// The default culture (English).
	/// </summary>
	public static readonly string DefaultDefaulCulture = "EN";

	/// <summary>
	/// The default culture (English, Portuguese).
	/// </summary>
	public static readonly string[] DefaultSupportedCultures = [ "EN", "PT" ];
	#endregion

	#region [Properties]
	/// <summary>
	/// Gets or sets the default culture.
	/// </summary>
	public required string DefaultCulture { get; init; }

	/// <summary>
	/// Gets or sets the supported cultures.
	/// </summary>
	public required string[] SupportedCultures { get; init; }
	#endregion
}
