namespace Memento.Aspire.Core.Localization;

using Microsoft.Extensions.Localization;
using System.Globalization;

/// <summary>
/// Implements the generic interface for a localizer.
/// Provides methods to get localized strings using keys.
/// </summary>
///
/// <typeparam name="TSharedResources">The shared resources type.</typeparam>
public sealed class Localizer<TSharedResources> : ILocalizer where TSharedResources : class
{
	#region [Properties]
	/// <summary>
	/// The string localizer.
	/// </summary>
	private readonly IStringLocalizer StringLocalizer;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="Localizer{TSharedResources}"/> class.
	/// </summary>
	///
	/// <param name="stringLocalizer">The string localizer.</param>
	public Localizer(IStringLocalizer<TSharedResources> stringLocalizer)
	{
		this.StringLocalizer = stringLocalizer;
	}
	#endregion

	#region [Methods]
	/// <summary>
	/// Returns the localized string with the given key.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="arguments">The arguments.</param>
	public string GetString(string key, params object[] arguments)
	{
		if (arguments is not null && arguments.Length > 0)
		{
			return this.StringLocalizer[string.Format(CultureInfo.InvariantCulture, key, arguments)];
		}
		else
		{
			return this.StringLocalizer[key];
		}
	}

	/// <summary>
	/// Returns the localized string with the given key (assumes the context of <seealso cref="T"/>).
	/// </summary>
	///
	/// <param name="key">The key</param>
	/// <param name="arguments">The arguments.</param>
	public string GetString<T>(string key, params object[] arguments) where T : class
	{
		var format = $"{typeof(T).FullName}.{key}";

		if (arguments is not null && arguments.Length > 0)
		{
			return this.StringLocalizer[string.Format(CultureInfo.InvariantCulture, format, arguments)];
		}
		else
		{
			return this.StringLocalizer[format];
		}
	}

	/// <summary>
	/// Returns the localized string with the given key (assumes the context of <seealso cref="Type"/>).
	/// </summary>
	///
	/// <param name="type">The type</param>
	/// <param name="key">The key</param>
	/// <param name="arguments">The arguments.</param>
	public string GetString(Type type, string key, params object[] arguments)
	{
		var format = $"{type.FullName}.{key}";

		if (arguments is not null && arguments.Length > 0)
		{
			return this.StringLocalizer[string.Format(CultureInfo.InvariantCulture, format, arguments)];
		}
		else
		{
			return this.StringLocalizer[format];
		}
	}
	#endregion
}
