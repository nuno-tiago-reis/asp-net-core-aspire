namespace Memento.Aspire.Shared.Exceptions;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Implements the <see cref="StandardException"/> json converter factory.
/// </summary>
public sealed class StandardExceptionJsonConverterFactory : JsonConverterFactory
{
	#region [Methods]
	/// <inheritdoc />
	public override bool CanConvert(Type typeToConvert)
	{
		if (typeToConvert != typeof(StandardException))
		{
			return false;
		}

		return true;
	}

	/// <inheritdoc />
	public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	{
		var converter = new StandardExceptionJsonConverter();

		return converter;
	}
	#endregion

	/// <summary>
	/// Implements the <see cref="StandardException{T}"/> json converter.
	/// </summary>
	///
	/// <typeparam name="T">The typeToConvert.</typeparam>
	private sealed class StandardExceptionJsonConverter : JsonConverter<StandardException>
	{
		#region [Methods] JsonConverter
		/// <inheritdoc />
		public override StandardException Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartObject)
			{
				throw new JsonException();
			}

			// Data
			var messages = Array.Empty<string>();
			var source = default(string);
			var type = default(StandardExceptionType);

			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.EndObject)
				{
					return new StandardException(messages ?? [], type)
					{
						Source = source
					};
				}

				if (reader.TokenType != JsonTokenType.PropertyName)
				{
					throw new JsonException();
				}

				var propertyName = reader.GetString()!;

				// Messages
				if (PropertyNameMatches(propertyName, nameof(StandardException.Messages), options))
				{
					messages = JsonSerializer.Deserialize<string[]>(ref reader, options);
				}

				// Source
				if (PropertyNameMatches(propertyName, nameof(StandardException.Source), options))
				{
					source = JsonSerializer.Deserialize<string>(ref reader, options);
				}

				// ConverterType
				if (PropertyNameMatches(propertyName, nameof(StandardException.Type), options))
				{
					type = JsonSerializer.Deserialize<StandardExceptionType>(ref reader, options);
				}
			}

			throw new JsonException();
		}

		/// <inheritdoc />
		public override void Write(Utf8JsonWriter writer, StandardException exception, JsonSerializerOptions options)
		{
			writer.WriteStartObject();

			// Messages
			writer.WritePropertyName(ConvertPropertyName(nameof(exception.Messages), options));
			JsonSerializer.Serialize(writer, exception.Messages, options);

			// Source
			writer.WritePropertyName(ConvertPropertyName(nameof(exception.Source), options));
			JsonSerializer.Serialize(writer, exception.Source, options);

			// ConverterType
			writer.WritePropertyName(ConvertPropertyName(nameof(exception.Type), options));
			JsonSerializer.Serialize(writer, exception.Type, options);

			writer.WriteEndObject();
		}
		#endregion

		#region [Methods] Utility
		/// <summary>
		/// Converts a property name according to the given options.
		/// </summary>
		///
		/// <param name="sourceName">The source name.</param>
		/// <param name="options">The options.</param>
		private static string ConvertPropertyName(string sourceName, JsonSerializerOptions options)
		{
			if (options?.PropertyNamingPolicy is not null)
			{
				return options.PropertyNamingPolicy.ConvertName(sourceName);
			}

			return sourceName;
		}

		/// <summary>
		/// Checks if a property name matches according to the given options.
		/// </summary>
		///
		/// <param name="sourceName">The source name.</param>
		/// <param name="targetName">The target name.</param>
		/// <param name="options">The options.</param>
		private static bool PropertyNameMatches(string sourceName, string targetName, JsonSerializerOptions options)
		{
			if (options.PropertyNameCaseInsensitive == false && sourceName == targetName)
			{
				return true;
			}

			if (options.PropertyNameCaseInsensitive == true && sourceName.Equals(targetName, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}

			return false;
		}
		#endregion
	}
}
