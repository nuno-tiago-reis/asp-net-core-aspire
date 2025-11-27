namespace Memento.Aspire.Core.Api;

using Microsoft.AspNetCore.Routing;
using System.Globalization;
using System.Text.RegularExpressions;

/// <summary>
/// Implements an <seealso cref="IOutboundParameterTransformer" /> that separates words by using slashes.
/// </summary>
public sealed partial class SlashParameterTransformer : IOutboundParameterTransformer
{
	#region [Methods]
	/// <summary>
	/// Regular expression for matching route parts.
	/// </summary>
	[GeneratedRegex(@"(?<=[A-Z])(?=[A-Z][a-z]) | (?<=[^A-Z])(?=[A-Z]) | (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace)]
	private static partial Regex RouteRegularExpression();
	#endregion

	#region [Methods]
	/// <summary>
	/// Transforms the outbound parameter values by splitting whenever a capital letter occurs using a slash.
	/// </summary>
	///
	/// <param name="value">The Value.</param>
	public string? TransformOutbound(object? value)
	{
		if (value is null)
		{
			return null;
		}

		var parameter = string.Empty;

		var regex = RouteRegularExpression();
		var regexTokens = regex.Split(value.ToString() ?? string.Empty);

		foreach (var regexToken in regexTokens)
		{
			if (regexToken.EndsWith('y') || regexToken.EndsWith('Y'))
			{
				parameter += $"{regexToken[ ..^1 ]}ies/";
			}
			else
			{
				parameter += $"{regexToken}s/";
			}
		}

		return parameter.ToLower(CultureInfo.InvariantCulture).TrimEnd('/');
	}
	#endregion
}
