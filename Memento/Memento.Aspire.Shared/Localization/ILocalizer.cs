namespace Memento.Aspire.Shared.Localization;

/// <summary>
/// Defines a generic interface for a localizer.
/// Provides methods to get localized strings using keys.
/// </summary>
public interface ILocalizer
{
	#region [Methods]
	/// <summary>
	/// Returns the localized string with the given key.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="arguments">The arguments.</param>
	string GetString(string key, params object[] arguments);

	/// <summary>
	/// Returns the localized string with the given key (assumes the context of <seealso cref="T"/>).
	/// </summary>
	///
	/// <param name="key">The key</param>
	/// <param name="arguments">The arguments.</param>
	string GetString<T>(string key, params object[] arguments) where T : class;

	/// <summary>
	/// Returns the localized string with the given key (assumes the context of <seealso cref="Type"/>).
	/// </summary>
	///
	/// <param name="type">The type</param>
	/// <param name="key">The key</param>
	/// <param name="arguments">The arguments.</param>
	string GetString(Type type, string key, params object[] arguments);
	#endregion
}
