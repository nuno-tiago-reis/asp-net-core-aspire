namespace Memento.Aspire.Shared.Binding;

/// <summary>
/// Implements a parameter binder designed to be a workaround a Minimal API limitation that prevents case insensitive parsing.
/// </summary>
public sealed class ParameterBinder<T> where T : Enum
{
	#region [Properties]
	/// <summary>
	/// The value.
	/// </summary>
	public T? Value { get; init; }
	#endregion

	#region [Methods]
	/// <summary>
	/// Returns a string representation of the parameter.
	/// </summary>
	public override string? ToString()
	{
		return this.Value?.ToString();
	}

	/// <summary>
	/// Attempts to parse the given value.
	/// </summary>
	///
	/// <param name="value">The value.</param>
	/// <param name="parameter">The parameter.</param>
	public static bool TryParse(string? value, out ParameterBinder<T> parameter)
	{
		var success = TryParseParameter(value, out var parsedValue);
		if (!success)
		{
			parameter = new ParameterBinder<T>
			{
				Value = default
			};
		}
		else
		{
			parameter = new ParameterBinder<T>
			{
				Value = parsedValue
			};
		}

		return true;
	}

	/// <summary>
	/// Attempts to parse the given value.
	/// </summary>
	///
	/// <param name="value">The value.</param>
	/// <param name="formatProvider">The format formatProvider.</param>
	/// <param name="parameter">The parameter.</param>
	private static bool TryParseParameter(string? value, out T? result)
	{
		var success = false;

		result = default;

		if (typeof(T).IsEnum)
		{
			success = Enum.TryParse(typeof(T), value, true, out var parsedValue);
			success = success && Enum.IsDefined(typeof(T), parsedValue!);

			if (success)
			{
				result = (T?)parsedValue;
			}

			return success;
		}

		return success;
	}

	/// <summary>
	/// Returns the value of the parameter.
	/// </summary>
	public static implicit operator T?(ParameterBinder<T> parameter)
	{
		if (Enum.IsDefined(typeof(T), parameter.Value!))
		{
			return parameter.Value;
		}

		return default;
	}

	/// <summary>
	/// Returns the value of the parameter.
	/// </summary>
	public static implicit operator ParameterBinder<T>?(T parameter)
	{
		if (TryParse(parameter.ToString(), out var parameterBinder))
		{
			return parameterBinder;
		}

		return null;
	}
	#endregion
}
