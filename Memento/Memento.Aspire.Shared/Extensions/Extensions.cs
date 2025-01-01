namespace Memento.Aspire.Shared.Extensions;

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

/// <summary>
/// Implements several extension methods.
/// </summary>
public static class Extensions
{
	#region [Extensions] AppDomain
	/// <summary>
	/// Gets the loaded assemblies matching the <see cref="Memento"/> prefix.
	/// </summary>
	public static Assembly[] GetMementoAssemblies(this AppDomain appDomain)
	{
		var assemblies = appDomain
			.GetAssemblies()
			.Where(assembly => assembly.GetTypes().Any(type => type.FullName?.StartsWith(nameof(Memento)) == true))
			.ToArray();

		return assemblies;
	}
	#endregion

	#region [Extensions] Enum
	/// <summary>
	/// Returns a message using the localized display name of the expression field.
	/// </summary>
	///
	/// <param name="enum">The enum.</param>
	/// <param name="message">The message.</param>
	public static string GetLocalizedMessage(this Enum @enum, string message)
	{
		var member = @enum.GetType().GetMember(@enum.ToString()).First();
		var memberAttribute = member.GetCustomAttribute<DisplayAttribute>();

		return string.Format(message, memberAttribute?.GetName() ?? member.Name.SpacesFromCamel());
	}

	/// <summary>
	/// Returns a message using the localized display name of the enum Value.
	/// </summary>
	///
	/// <param name="enum">The enum.</param>
	public static string GetLocalizedName(this Enum @enum)
	{
		var member = @enum.GetType().GetMember(@enum.ToString()).First();
		var memberAttribute = member.GetCustomAttribute<DisplayAttribute>();

		return memberAttribute?.GetName() ?? member.Name.SpacesFromCamel();
	}
	#endregion

	#region [Extensions] Generic
	/// <summary>
	/// Returns a message using the localized display name of the expression field.
	/// </summary>
	///
	/// <param name="_">The object.</param>
	/// <param name="expression">The expression.</param>
	/// <param name="message">The message.</param>
	public static string GetLocalizedMessage<TObject, TProperty>(this TObject _, Expression<Func<TObject, TProperty>> expression, string message)
	{
		var property = ((MemberExpression)expression.Body).Member;
		var propertyDisplayName = property?.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;

		return string.Format(message, propertyDisplayName?.GetName() ?? property?.Name.SpacesFromCamel() ?? string.Empty);
	}

	/// <summary>
	/// Returns the localized display name of the expression field.
	/// </summary>
	///
	/// <param name="_">The object.</param>
	/// <param name="expression">The expression.</param>
	public static string GetLocalizedName<TObject, TProperty>(this TObject _, Expression<Func<TObject, TProperty>> expression)
	{
		var property = ((MemberExpression)expression.Body).Member;
		var propertyDisplayName = property?.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;

		return propertyDisplayName?.GetName() ?? property?.Name.SpacesFromCamel() ?? string.Empty;
	}
	#endregion

	#region [Extensions] HttpContext
	/// <summary>
	/// Gets the users identifier.
	/// </summary>
	///
	/// <param name="httpContext">The http context.</param>
	public static Guid GetUserId(this HttpContext httpContext)
	{
		var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
		byte[] hashedUserId = MD5.HashData(Encoding.UTF8.GetBytes(userId));

		return new Guid(hashedUserId);
	}

	/// <summary>
	/// Gets the users name.
	/// </summary>
	///
	/// <param name="httpContext">The http context.</param>
	public static Guid GetUserName(this HttpContext httpContext)
	{
		return Guid.Parse(httpContext.User.FindFirstValue(ClaimTypes.Name) ?? string.Empty);
	}

	/// <summary>
	/// Gets the users email.
	/// </summary>
	///
	/// <param name="httpContext">The http context.</param>
	public static Guid GetUserEmail(this HttpContext httpContext)
	{
		return Guid.Parse(httpContext.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty);
	}
	#endregion

	#region [Extensions] JsonOptions
	/// <summary>
	/// Configures the default options for the JsonSerializerOptions.
	/// </summary>
	///
	/// <param name="options">The options.</param>
	public static void ConfigureDefaultOptions(this JsonSerializerOptions options)
	{
		// convert enums to strings
		options.Converters.Add(new JsonStringEnumConverter());
		// don't ignore null values
		options.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
		// don't convert dictionary keys
		options.DictionaryKeyPolicy = null;
		// ignore casing when deserializing
		options.PropertyNameCaseInsensitive = true;
		// convert properties to camel case
		options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		// ignore comments
		options.ReadCommentHandling = JsonCommentHandling.Skip;
	}
	#endregion

	#region [Extensions] Expression
	/// <summary>
	/// Returns a message using the name of the expression memberExpression.
	/// </summary>
	///
	/// <param name="expression">The expression.</param>
	public static string? GetName(this Expression expression)
	{
		if (expression is MemberExpression memberExpression)
		{
			var property = memberExpression.Member;

			return property?.Name;
		}

		if (expression is LambdaExpression lambdaExpression)
		{
			if (lambdaExpression.Body is MemberExpression lambdaMemberExpression)
			{
				var property = lambdaMemberExpression.Member;

				return property?.Name;
			}
			else if (lambdaExpression.Body is UnaryExpression lambdaUnaryExpression && lambdaUnaryExpression.Operand is MemberExpression lambdaOperandExpression)
			{
				var property = lambdaOperandExpression.Member;

				return property?.Name;
			}
		}

		return string.Empty;
	}

	/// <summary>
	/// Returns a message using the localized display name of the expression memberExpression.
	/// </summary>
	///
	/// <param name="expression">The expression.</param>
	public static string? GetDisplayName(this Expression expression)
	{
		if (expression is MemberExpression memberExpression)
		{
			var property = memberExpression.Member;
			var propertyDisplayName = property?.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;

			return propertyDisplayName?.GetName() ?? property?.Name.SpacesFromCamel();
		}

		if (expression is LambdaExpression lambdaExpression)
		{
			if (lambdaExpression.Body is MemberExpression lambdaMemberExpression)
			{
				var property = lambdaMemberExpression.Member;
				var propertyDisplayName = property?.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;

				return propertyDisplayName?.GetName() ?? property?.Name.SpacesFromCamel();
			}
			else if(lambdaExpression.Body is UnaryExpression lambdaUnaryExpression && lambdaUnaryExpression.Operand is MemberExpression lambdaOperandExpression)
			{
				var property = lambdaOperandExpression.Member;
				var propertyDisplayName = property?.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;

				return propertyDisplayName?.GetName() ?? property?.Name.SpacesFromCamel();
			}
		}

		return null;
	}
	#endregion

	#region [Extensions] MemberExpression
	/// <summary>
	/// Returns a message using the name of the expression memberExpression.
	/// </summary>
	///
	/// <param name="expression">The expression.</param>
	public static string? GetName(this MemberExpression expression)
	{
		var property = expression.Member;

		return property.Name;
	}

	/// <summary>
	/// Returns a message using the localized display name of the expression memberExpression.
	/// </summary>
	///
	/// <param name="expression">The expression.</param>
	public static string? GetDisplayName(this MemberExpression expression)
	{
		var property = expression.Member;
		var propertyDisplayName = property?.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;

		return propertyDisplayName?.GetName() ?? property?.Name.SpacesFromCamel();
	}
	#endregion

	#region [Extensions] MemberInfo
	/// <summary>
	/// Returns a message using the name of the memberExpression.
	/// </summary>
	///
	/// <param name="member">The memberExpression.</param>
	public static string? GetName(this MemberInfo member)
	{
		return member.Name;
	}

	/// <summary>
	/// Returns a message using the localized display name of the memberExpression.
	/// </summary>
	///
	/// <param name="member">The memberExpression.</param>
	public static string? GetDisplayName(this MemberInfo member)
	{
		var propertyDisplayName = member?.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;

		return propertyDisplayName?.GetName() ?? member?.Name.SpacesFromCamel();
	}
	#endregion

	#region [Extensions] String
	/// <summary>
	/// Spaces a string according to its camel case.
	/// </summary>
	///
	/// <param name="string">The string.</param>
	public static string SpacesFromCamel(this string @string)
	{
		if (@string.Length <= 0)
			return @string;

		var result = new List<char>();
		var array = @string.ToCharArray();

		foreach (var item in array)
		{
			if (char.IsUpper(item))
			{
				result.Add(' ');
			}
			result.Add(item);
		}

		return new string(result.ToArray()).Trim();
	}

	/// <summary>
	/// Converts a string to camel case.
	/// </summary>
	///
	/// <param name="string">The string.</param>
	public static string ToCamelCase(this string @string)
	{
		if (@string.Length < 2)
		{
			return @string.ToLower();
		}

		// Start with the first character
		var result = @string.Substring(0, 1).ToLower();

		// Remove extra whitespaces
		result += Regex.Replace(@string.Substring(1), " +([a-zA-Z])", "$1".ToUpper());

		return result;
	}

	/// <summary>
	/// Converts a string to pascal case.
	/// </summary>
	///
	/// <param name="string">The string.</param>
	public static string ToPascalCase(this string @string)
	{
		if (@string.Length < 2)
		{
			return @string.ToUpper();
		}

		// Start with the first character
		var result = @string.Substring(0, 1).ToUpper();

		// Remove extra whitespaces
		result += Regex.Replace(@string.Substring(1), " +([a-zA-Z])", "$1".ToUpper());

		return result;
	}

	/// <summary>
	/// Converts a string to proper case.
	/// </summary>
	///
	/// <param name="string">The string.</param>
	public static string ToProperCase(this string @string)
	{
		if (@string.Length < 2)
		{
			return @string.ToUpper();
		}

		// Start with the first character
		var result = @string.Substring(0, 1).ToUpper();

		// Remove extra whitespaces
		result += Regex.Replace(@string.Substring(1), " +([a-zA-Z])", " $1".ToUpper());

		// Insert missing whitespaces
		result = Regex.Replace(result, "([a-z])([A-Z])", "$1 $2");

		return result;
	}

	/// <summary>
	/// Compares two strings with the invariant culture and ignoring case.
	/// </summary>
	///
	/// <param name="string">The string.</param>
	/// <param name="value">The string to compare to.</param>
	public static bool EqualsNormalized(this string @string, string value)
	{
		return @string.Equals(value, StringComparison.InvariantCultureIgnoreCase);
	}

	/// <summary>
	/// Returns whether the string is null or white space.
	/// </summary>
	///
	/// <param name="string">The string.</param>
	public static bool IsNullOrWhiteSpace(this string @string)
	{
		return string.IsNullOrWhiteSpace(@string);
	}
	#endregion
}
