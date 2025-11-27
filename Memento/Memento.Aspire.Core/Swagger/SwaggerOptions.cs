namespace Memento.Aspire.Core.Swagger;

/// <summary>
/// Implements the <see cref="SwaggerOptions"/>.
/// </summary>
public sealed record SwaggerOptions
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the title.
	/// </summary>
	public required string? Title { get; init; }

	/// <summary>
	/// Gets or sets the version.
	/// </summary>
	public required string? Version { get; init; }

	/// <summary>
	/// Gets or sets the endpoint.
	/// </summary>
	public required string? Endpoint { get; init; }

	/// <summary>
	/// Gets or sets the security options.
	/// </summary>
	public required SwaggerSecurityOptions? Security { get; init; }
	#endregion
}

/// <summary>
/// Implements the <see cref="SwaggerSecurityOptions"/>.
/// </summary>
public sealed record SwaggerSecurityOptions
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the openid connect options.
	/// </summary>
	public required SwaggerSecurityOpenIdConnectOptions OpenIdConnect { get; init; }

	/// <summary>
	/// Gets or sets the oauth options.
	/// </summary>
	public required SwaggerSecurityOAuthOptions OAuth { get; init; }
	#endregion
}

/// <summary>
/// Implements the <see cref="SwaggerSecurityOpenIdConnectOptions"/>.
/// </summary>
public sealed record SwaggerSecurityOpenIdConnectOptions
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the audience.
	/// </summary>
	public required string Audience { get; init; }

	/// <summary>
	/// Gets or sets the authority.
	/// </summary>
	public required string Authority { get; init; }

	/// <summary>
	/// Gets or sets the discovery url.
	/// </summary>
	public required string DiscoveryUrl { get; init; }

	/// <summary>
	/// Gets or sets the client identifier.
	/// </summary>
	public required string ClientId { get; init; }

	/// <summary>
	/// Gets or sets the client secret.
	/// </summary>
	public required string ClientSecret { get; init; }

	/// <summary>
	/// Gets or sets the scopes (key-Value pairs composed of the scope name and description).
	/// </summary>
	public required List<SwaggerSecurityScopeOptions> Scopes { get; init; } = [];
	#endregion
}

/// <summary>
/// Implements the <see cref="SwaggerSecurityOAuthOptions"/>.
/// </summary>
public sealed record SwaggerSecurityOAuthOptions
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the audience.
	/// </summary>
	public required string Audience { get; init; }

	/// <summary>
	/// Gets or sets the authority.
	/// </summary>
	public required string Authority { get; init; }

	/// <summary>
	/// Gets or sets the authorization url.
	/// </summary>
	public required string AuthorizationUrl { get; init; }

	/// <summary>
	/// Gets or sets the token url.
	/// </summary>
	public required string TokenUrl { get; init; }

	/// <summary>
	/// Gets or sets the client identifier.
	/// </summary>
	public required string ClientId { get; init; }

	/// <summary>
	/// Gets or sets the client secret.
	/// </summary>
	public required string ClientSecret { get; init; }

	/// <summary>
	/// Gets or sets the scopes (key-Value pairs composed of the scope name and description).
	/// </summary>
	public required List<SwaggerSecurityScopeOptions> Scopes { get; init; } = [];
	#endregion
}

/// <summary>
/// Implements the <see cref="SwaggerSecurityScopeOptions"/>.
/// </summary>
public sealed record SwaggerSecurityScopeOptions
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the name.
	/// </summary>
	public required string Name { get; init; }

	/// <summary>
	/// Gets or sets the description.
	/// </summary>
	public required string Description { get; init; }
	#endregion
}
