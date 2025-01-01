namespace Memento.Aspire.Shared.Swagger;

/// <summary>
/// Implements the <see cref="SwaggerOptions"/>.
/// </summary>
public sealed record SwaggerOptions
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the title.
	/// </summary>
	public required string? Title { get; set; }

	/// <summary>
	/// Gets or sets the version.
	/// </summary>
	public required string? Version { get; set; }

	/// <summary>
	/// Gets or sets the endpoint.
	/// </summary>
	public required string? Endpoint { get; set; }

	/// <summary>
	/// Gets or sets the security options.
	/// </summary>
	public required SwaggerSecurityOptions? Security { get; set; }
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
	public required SwaggerSecurityOpenIdConnectOptions OpenIdConnect { get; set; }

	/// <summary>
	/// Gets or sets the oauth options.
	/// </summary>
	public required SwaggerSecurityOAuthOptions OAuth { get; set; }
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
	public required string Audience { get; set; }

	/// <summary>
	/// Gets or sets the authority.
	/// </summary>
	public required string Authority { get; set; }

	/// <summary>
	/// Gets or sets the discovery url.
	/// </summary>
	public required string DiscoveryUrl { get; set; }

	/// <summary>
	/// Gets or sets the client identifier.
	/// </summary>
	public required string ClientId { get; set; }

	/// <summary>
	/// Gets or sets the client secret.
	/// </summary>
	public required string ClientSecret { get; set; }

	/// <summary>
	/// Gets or sets the scopes (key-Value pairs composed of the scope name and description).
	/// </summary>
	public required List<SwaggerSecurityScopeOptions> Scopes { get; set; } = [];
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
	public required string Audience { get; set; }

	/// <summary>
	/// Gets or sets the authority.
	/// </summary>
	public required string Authority { get; set; }

	/// <summary>
	/// Gets or sets the authorization url.
	/// </summary>
	public required string AuthorizationUrl { get; set; }

	/// <summary>
	/// Gets or sets the token url.
	/// </summary>
	public required string TokenUrl { get; set; }

	/// <summary>
	/// Gets or sets the client identifier.
	/// </summary>
	public required string ClientId { get; set; }

	/// <summary>
	/// Gets or sets the client secret.
	/// </summary>
	public required string ClientSecret { get; set; }

	/// <summary>
	/// Gets or sets the scopes (key-Value pairs composed of the scope name and description).
	/// </summary>
	public required List<SwaggerSecurityScopeOptions> Scopes { get; set; } = [];
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
	public required string Name { get; set; }

	/// <summary>
	/// Gets or sets the description.
	/// </summary>
	public required string Description { get; set; }
	#endregion
}
