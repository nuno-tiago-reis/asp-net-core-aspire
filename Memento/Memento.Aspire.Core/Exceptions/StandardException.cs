namespace Memento.Aspire.Core.Exceptions;

using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

/// <summary>
/// Implements a custom exception that allows the correlation of exceptions with specific http status codes.
/// </summary>
///
/// <seealso cref="Exception" />
[JsonConverter(typeof(StandardExceptionJsonConverterFactory))]
public sealed class StandardException : Exception
{
	#region [Properties]
	/// <summary>
	/// The message.
	/// </summary>
	public override string Message
	{
		get
		{
			return this.Messages.Aggregate((i, j) => i + Environment.NewLine + j);
		}
	}

	/// <Value>
	/// The messages.
	/// </Value>
	public string[] Messages { get; }

	/// <summary>
	/// The type.
	/// </summary>
	public StandardExceptionType Type { get; }
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="StandardException"/> class.
	/// </summary>
	///
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="type">The corresponding status code enum Value.</param>
	public StandardException(string message, StandardExceptionType? type)
		: this([message], null, type)
	{
		// Intentionally Empty.
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="StandardException"/> class.
	/// </summary>
	///
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="exception">The inner exception that originated this exception.</param>
	/// <param name="type">The corresponding status code enum Value.</param>
	public StandardException(string message, Exception? exception, StandardExceptionType? type)
		: this([message], exception, type)
	{
		// Intentionally Empty.
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="StandardException"/> class.
	/// </summary>
	///
	/// <param name="messages">The error messages that explain the reasons for the exception.</param>
	/// <param name="type">The corresponding status code enum Value.</param>
	public StandardException(IEnumerable<string> messages, StandardExceptionType? type)
		: this(messages, null, type)
	{
		// Intentionally Empty.
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="StandardException"/> class.
	/// </summary>
	///
	/// <param name="messages">The error messages that explain the reasons for the exception.</param>
	/// <param name="exception">The inner exception that originated this exception.</param>
	/// <param name="type">The corresponding status code enum Value.</param>
	public StandardException(IEnumerable<string>? messages, Exception? exception, StandardExceptionType? type)
		: base(null, exception)
	{
		this.Messages = messages?.ToArray() ?? [];
		this.Type = type ?? StandardExceptionType.InternalServerError;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="StandardException"/> class.
	/// </summary>
	public StandardException()
	{
		this.Messages = [];
		this.Type = StandardExceptionType.InternalServerError;
	}
	#endregion

	#region [Methods]
	/// <summary>
	/// Gets the status code that corresponds with the type.
	/// </summary>
	public int GetStatusCode()
	{
		switch (this.Type)
		{
			case StandardExceptionType.BadRequest:
			{
				return StatusCodes.Status400BadRequest;
			}
			case StandardExceptionType.Unauthorized:
			{
				return StatusCodes.Status401Unauthorized;
			}
			case StandardExceptionType.Forbidden:
			{
				return StatusCodes.Status403Forbidden;
			}
			case StandardExceptionType.NotFound:
			{
				return StatusCodes.Status404NotFound;
			}
			case StandardExceptionType.InternalServerError:
			{
				return StatusCodes.Status500InternalServerError;
			}
			default:
			{
				return StatusCodes.Status500InternalServerError;
			}
		}
	}
	#endregion
}

/// <summary>
/// Defines the available exception types.
/// These exception types are then read by an exception handler
/// in order to return the corresponding http status code instead of a generic 500 error.
/// </summary>
public enum StandardExceptionType
{
	/// <summary>
	/// Maps to the 400 status code.
	/// </summary>
	BadRequest,
	/// <summary>
	/// Maps to the 401 status code.
	/// </summary>
	Unauthorized,
	/// <summary>
	/// Maps to the 403 status code.
	/// </summary>
	Forbidden,
	/// <summary>
	/// Maps to the 404 status code.
	/// </summary>
	NotFound,
	/// <summary>
	/// Maps to the 500 status code.
	/// </summary>
	InternalServerError
}
