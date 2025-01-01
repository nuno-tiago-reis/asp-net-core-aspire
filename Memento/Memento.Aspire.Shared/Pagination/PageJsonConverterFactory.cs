namespace Memento.Aspire.Shared.Pagination;

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Implements the <see cref="Page{T}"/> json converter factory.
/// </summary>
public sealed class PageJsonConverterFactory : JsonConverterFactory
{
	#region [Methods]
	/// <inheritdoc />
	public override bool CanConvert(Type typeToConvert)
	{
		if (!typeToConvert.IsGenericType)
		{
			return false;
		}

		if (typeToConvert.GetGenericTypeDefinition() != typeof(Page<>))
		{
			return false;
		}

		return true;
	}

	/// <inheritdoc />
	public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
	{
		const BindingFlags converterFlags = BindingFlags.Instance | BindingFlags.Public;

		var genericType = typeof(PageJsonConverter<>).MakeGenericType(type.GetGenericArguments()[0]);

		var converter = (JsonConverter)Activator.CreateInstance(genericType, converterFlags, null, [options], null)!;

		return converter;
	}
	#endregion

	/// <summary>
	/// Implements the <see cref="Page{T}"/> json converter.
	/// </summary>
	///
	/// <typeparam name="T">The type.</typeparam>
	private sealed class PageJsonConverter<T> : JsonConverter<Page<T>>
	{
		#region [Properties]
		/// <summary>
		/// The type.
		/// </summary>
		private readonly Type ConverterType;

		/// <summary>
		/// The type converter.
		/// </summary>
		private readonly JsonConverter<IList<T>> Converter;
		#endregion

		#region [Constructors]
		/// <summary>
		/// Initializes a new instance of the <see cref="PageJsonConverter{T}"/> class.
		/// </summary>
		///
		/// <param name="options">The options.</param>
		public PageJsonConverter(JsonSerializerOptions options)
		{
			// Cache the types
			this.ConverterType = typeof(IList<T>);

			// For performance, use the existing converter if available
			this.Converter = (JsonConverter<IList<T>>)options.GetConverter(ConverterType);
		}
		#endregion

		#region [Methods] JsonConverter
		/// <inheritdoc />
		public override Page<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartObject)
			{
				throw new JsonException();
			}

			var pageNumber = 0;
			var pageSize = 0;
			var totalPages = 0;
			var totalItems = 0;
			var orderBy = default(string);
			var orderDirection = default(string);
			var items = new List<T>();

			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.EndObject)
				{
					return new Page<T>(items, totalItems, totalPages, pageNumber, pageSize, orderBy, orderDirection);
				}

				if (reader.TokenType != JsonTokenType.PropertyName)
				{
					throw new JsonException();
				}

				var propertyName = reader.GetString()!;

				// PageSize
				if (PropertyNameMatches(propertyName, nameof(Page<T>.PageSize), options))
				{
					pageSize = JsonSerializer.Deserialize<int>(ref reader, options);
				}

				// PageNumber
				if (PropertyNameMatches(propertyName, nameof(Page<T>.PageNumber), options))
				{
					pageNumber = JsonSerializer.Deserialize<int>(ref reader, options);
				}

				// TotalPages
				if (PropertyNameMatches(propertyName, nameof(Page<T>.TotalPages), options))
				{
					totalPages = JsonSerializer.Deserialize<int>(ref reader, options);
				}

				// TotalItems
				if (PropertyNameMatches(propertyName, nameof(Page<T>.TotalItems), options))
				{
					totalItems = JsonSerializer.Deserialize<int>(ref reader, options);
				}

				// OrderBy
				if (PropertyNameMatches(propertyName, nameof(Page<T>.OrderBy), options))
				{
					orderBy = JsonSerializer.Deserialize<string>(ref reader, options);
				}

				// OrderDirection
				if (PropertyNameMatches(propertyName, nameof(Page<T>.OrderDirection), options))
				{
					orderDirection = JsonSerializer.Deserialize<string>(ref reader, options);
				}

				// Items
				if (PropertyNameMatches(propertyName, nameof(Page<T>.Items), options))
				{
					if (this.Converter is not null)
					{
						reader.Read();
						items.AddRange(this.Converter.Read(ref reader, ConverterType, options)!);
					}
					else
					{
						items.AddRange(JsonSerializer.Deserialize<IList<T>>(ref reader, options)!);
					}
				}
			}

			throw new JsonException();
		}

		/// <inheritdoc />
		public override void Write(Utf8JsonWriter writer, Page<T> page, JsonSerializerOptions options)
		{
			writer.WriteStartObject();

			// PageSize
			writer.WritePropertyName(ConvertPropertyName(nameof(page.PageSize), options));
			JsonSerializer.Serialize(writer, page.PageSize, options);

			// PageNumber
			writer.WritePropertyName(ConvertPropertyName(nameof(page.PageNumber), options));
			JsonSerializer.Serialize(writer, page.PageNumber, options);

			// TotalPages
			writer.WritePropertyName(ConvertPropertyName(nameof(page.TotalPages), options));
			JsonSerializer.Serialize(writer, page.TotalPages, options);

			// TotalItems
			writer.WritePropertyName(ConvertPropertyName(nameof(page.TotalItems), options));
			JsonSerializer.Serialize(writer, page.TotalItems, options);

			// OrderBy
			writer.WritePropertyName(ConvertPropertyName(nameof(page.OrderBy), options));
			JsonSerializer.Serialize(writer, page.OrderBy, options);

			// OrderDirection
			writer.WritePropertyName(ConvertPropertyName(nameof(page.OrderDirection), options));
			JsonSerializer.Serialize(writer, page.OrderDirection, options);

			// Items
			writer.WritePropertyName(ConvertPropertyName(nameof(page.Items), options));
			if (Converter is not null)
			{
				Converter.Write(writer, page.Items, options);
			}
			else
			{
				JsonSerializer.Serialize(writer, page.Items, options);
			}

			writer.WriteEndObject();
		}
		#endregion

		#region [Methods] Helpers
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
