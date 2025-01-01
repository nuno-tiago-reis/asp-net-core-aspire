namespace Memento.Aspire.Domain.Api.Modules;

using FluentValidation;
using Memento.Aspire.Domain.Api.Constants;
using Memento.Aspire.Domain.Service.Contracts.Author;
using Memento.Aspire.Domain.Service.Messaging.Author.Commands;
using Memento.Aspire.Domain.Service.Messaging.Author.Queries;
using Memento.Aspire.Shared.Api;
using Memento.Aspire.Shared.Cache;
using Memento.Aspire.Shared.Contracts;
using Memento.Aspire.Shared.Extensions;
using Memento.Aspire.Shared.Messaging;
using Memento.Aspire.Shared.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

/// <summary>
/// Implements the API module for the 'Author' entity that provides the base for the CRUD operations.
/// </summary>
///
/// <seealso cref="EntityModule" />
public sealed class AuthorModule : EntityModule
{
	#region [Constants]
	/// <summary>
	/// The default route.
	/// </summary>
	private const string BASE_ROUTE = "/api/minimal-apis/authors";
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="AuthorModule"/> class.
	/// </summary>
	public AuthorModule() : base(BASE_ROUTE)
	{
		// Intentionally Empty.
	}
	#endregion

	#region [Methods] Module
	/// <inheritdoc/>
	public override void AddRoutes(IEndpointRouteBuilder builder)
	{
		builder
			.MapPost($"", this.HandleCreateAsync)
			.Produces<StandardResult<AuthorDetailContract>>(StatusCodes.Status201Created)
			.Produces<StandardResult<AuthorDetailContract>>(StatusCodes.Status400BadRequest)
			.Produces<StandardResult<AuthorDetailContract>>(StatusCodes.Status500InternalServerError)
			.RequireCors()
			.RequireAuthorization()
			.WithOpenApi();

		builder
			.MapPut($"{{id}}", this.HandleUpdateAsync)
			.Produces<StandardResult>(StatusCodes.Status200OK)
			.Produces<StandardResult>(StatusCodes.Status400BadRequest)
			.Produces<StandardResult>(StatusCodes.Status500InternalServerError)
			.RequireCors()
			.RequireAuthorization()
			.WithOpenApi();

		builder
			.MapDelete($"{{id}}", this.HandleDeleteAsync)
			.Produces<StandardResult>(StatusCodes.Status200OK)
			.Produces<StandardResult>(StatusCodes.Status400BadRequest)
			.Produces<StandardResult>(StatusCodes.Status500InternalServerError)
			.RequireCors()
			.RequireAuthorization()
			.WithOpenApi();

		builder
			.MapGet($"{{id}}", this.HandleGetAsync)
			.Produces<StandardResult<AuthorDetailContract>>(StatusCodes.Status200OK)
			.Produces<StandardResult<AuthorDetailContract>>(StatusCodes.Status400BadRequest)
			.Produces<StandardResult<AuthorDetailContract>>(StatusCodes.Status500InternalServerError)
			.RequireCors()
			.RequireAuthorization()
			.WithOpenApi();

		builder
			.MapGet($"", this.HandleGetAllAsync)
			.Produces<StandardResult<Page<AuthorSummaryContract>>>(StatusCodes.Status200OK)
			.Produces<StandardResult<Page<AuthorSummaryContract>>>(StatusCodes.Status400BadRequest)
			.Produces<StandardResult<Page<AuthorSummaryContract>>>(StatusCodes.Status500InternalServerError)
			.RequireCors()
			.RequireAuthorization()
			.WithOpenApi();
	}
	#endregion

	#region [Methods] Handlers
	/// <summary>
	/// Creates an 'Author' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="contract">The contract.</param>
	/// <param name="httpContext">The http context.</param>
	public async Task<IResult> HandleCreateAsync([FromBody] AuthorFormContract contract, HttpContext httpContext)
	{
		// Get the dependencies
		var messageBus = httpContext.RequestServices.GetService<IMessageBus>()!;
		var cancellationToken = httpContext.RequestAborted;

		// Validate the parameters
		var validator = httpContext.RequestServices.GetService<IValidator<AuthorFormContract>>()!;
		var validationResult = await validator.ValidateAsync(contract);

		if (!validationResult.IsValid)
		{
			return this.BuildErrorResult<AuthorDetailContract>(httpContext, validationResult);
		}

		// Build and execute the command
		var command = new CreateAuthorCommand
		{
			AuthorContract = contract,
			CorrelationId = Guid.NewGuid(),
			IdempotencyId = Guid.NewGuid(),
			UserId = httpContext.GetUserId()
		};
		var commandResult = await messageBus.RequestResponseViaBusAsync<CreateAuthorCommand, CreateAuthorCommandResult>(command, cancellationToken);

		if (commandResult.Success)
		{
			return this.BuildCreateResult(httpContext, commandResult.AuthorContract!);
		}
		else
		{
			return this.BuildErrorResult<AuthorDetailContract>(httpContext, commandResult.Exception!);
		}
	}

	/// <summary>
	/// Updates an 'Author' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="id">The id.</param>
	/// <param name="contract">The contract.</param>
	/// <param name="httpContext">The http context.</param>
	public async Task<IResult> HandleUpdateAsync([FromRoute] Guid id, [FromBody] AuthorFormContract contract, HttpContext httpContext)
	{
		// Get the dependencies
		var messageBus = httpContext.RequestServices.GetService<IMessageBus>()!;
		var cancellationToken = httpContext.RequestAborted;

		// Validate the parameters
		var validator = httpContext.RequestServices.GetService<IValidator<AuthorFormContract>>()!;
		var validationResult = await validator.ValidateAsync(contract);

		if (!validationResult.IsValid)
		{
			return this.BuildErrorResult(httpContext, validationResult);
		}

		// Build and execute the command
		var command = new UpdateAuthorCommand
		{
			AuthorId = id,
			AuthorContract = contract,
			CorrelationId = Guid.NewGuid(),
			IdempotencyId = Guid.NewGuid(),
			UserId = httpContext.GetUserId()
		};
		var commandResult = await messageBus.RequestResponseViaBusAsync<UpdateAuthorCommand, UpdateAuthorCommandResult>(command, cancellationToken);

		if (commandResult.Success)
		{
			return this.BuildUpdateResult(httpContext);
		}
		else
		{
			return this.BuildErrorResult(httpContext, commandResult.Exception!);
		}
	}

	/// <summary>
	/// Deletes an 'Author' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="id">The id.</param>
	/// <param name="httpContext">The http context.</param>
	public async Task<IResult> HandleDeleteAsync([FromRoute] Guid id, HttpContext httpContext)
	{
		// Get the dependencies
		var messageBus = httpContext.RequestServices.GetService<IMessageBus>()!;
		var cancellationToken = httpContext.RequestAborted;

		// Build and execute the command
		var command = new DeleteAuthorCommand
		{
			AuthorId = id,
			CorrelationId = Guid.NewGuid(),
			IdempotencyId = Guid.NewGuid(),
			UserId = httpContext.GetUserId()
		};
		var commandResult = await messageBus.RequestResponseViaBusAsync<DeleteAuthorCommand, DeleteAuthorCommandResult>(command, cancellationToken);

		if (commandResult.Success)
		{
			return this.BuildDeleteResult(httpContext);
		}
		else
		{
			return this.BuildErrorResult(httpContext, commandResult.Exception!);
		}
	}

	/// <summary>
	/// Gets an 'Author' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="id">The id.</param>
	/// <param name="httpContext">The http context.</param>
	public async Task<IResult> HandleGetAsync([FromRoute] Guid id, HttpContext httpContext)
	{
		// Get the dependencies
		var cache = httpContext.RequestServices.GetService<ICache>()!;
		var messageBus = httpContext.RequestServices.GetService<IMessageBus>()!;
		var cancellationToken = httpContext.RequestAborted;

		// Try to get the author from the cache
		var cachedAuthorContract = await cache.TryGetAsync<AuthorDetailContract>(CacheEntries.GetAuthorCacheKey(id));

		// If it is there, we short circuit the flow and return straight away
		if (cachedAuthorContract is not null)
		{
			return this.BuildGetResult(httpContext, cachedAuthorContract);
		}

		// Build and execute the query
		var query = new GetAuthorQuery
		{
			AuthorId = id,
			CorrelationId = Guid.NewGuid(),
			UserId = httpContext.GetUserId()
		};
		var queryResult = await messageBus.RequestResponseViaBusAsync<GetAuthorQuery, GetAuthorQueryResult>(query, cancellationToken);

		if (queryResult.Success)
		{
			return this.BuildGetResult(httpContext, queryResult.AuthorContract!);
		}
		else
		{
			return this.BuildErrorResult<AuthorDetailContract>(httpContext, queryResult.Exception!);
		}
	}

	/// <summary>
	/// Gets multiple 'Authors' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="filter">The filter.</param>
	/// <param name="httpContext">The http context.</param>
	public async Task<IResult> HandleGetAllAsync([AsParameters] AuthorFilterContract filter, HttpContext httpContext)
	{
		// Get the dependencies
		var messageBus = httpContext.RequestServices.GetService<IMessageBus>()!;
		var cancellationToken = httpContext.RequestAborted;

		// Build and execute the query
		var query = new GetAuthorsQuery
		{
			AuthorFilterContract = filter,
			CorrelationId = Guid.NewGuid(),
			UserId = httpContext.GetUserId()
		};
		var queryResult = await messageBus.RequestResponseViaBusAsync<GetAuthorsQuery, GetAuthorsQueryResult>(query, cancellationToken);

		if (queryResult.Success)
		{
			return this.BuildGetAllResult(httpContext, queryResult.AuthorContracts!);
		}
		else
		{
			return this.BuildErrorResult<Page<AuthorSummaryContract>>(httpContext, queryResult.Exception!);
		}
	}
	#endregion
}
