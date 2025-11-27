namespace Memento.Aspire.Core.Api;

using System.Text.Json.Serialization;

/// <summary>
/// Implements a generic standard result.
/// </summary>
public sealed record StandardResult<T> : StandardResult where T : class
{
	#region [Properties]
	/// <summary>
	/// The data.
	/// </summary>
	[JsonPropertyOrder(5)]
	public required T? Data { get; init; }
	#endregion

	#region [Constructor]
	/// <summary>
	/// Initializes a new instance of the <see cref="Result{T}"/> class.
	/// </summary>
	public StandardResult()
	{
		// Intentionally Empty.
	}
	#endregion
}

/// <summary>
/// Implements a non-generic standard result.
/// </summary>
public record StandardResult
{
	#region [Properties]
	/// <summary>
	/// Whether the operation was successful.
	/// </summary>
	[JsonPropertyOrder(0)]
	public required bool Success { get; init; }

	/// <summary>
	/// The status code.
	/// </summary>
	[JsonPropertyOrder(1)]
	public required int StatusCode { get; init; }

	/// <summary>
	/// The message.
	/// </summary>
	[JsonPropertyOrder(2)]
	public required string Message { get; init; }

	/// <summary>
	/// The errors.
	/// </summary>
	[JsonPropertyOrder(4)]
	public required IEnumerable<string> Errors { get; init; }
	#endregion

	#region [Constructor]
	/// <summary>
	/// Initializes a new instance of the <see cref="StandardResult"/> class.
	/// </summary>
	public StandardResult()
	{
		// Intentionally Empty.
	}
	#endregion
}
