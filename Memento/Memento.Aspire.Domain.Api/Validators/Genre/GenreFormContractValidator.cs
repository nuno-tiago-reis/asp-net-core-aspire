namespace Memento.Aspire.Domain.Api.Validators.Genre;

using FluentValidation;
using Memento.Aspire.Domain.Service.Contracts.Genre;

/// <summary>
/// Implements the 'Genre' form contract validator.
/// </summary>
internal sealed class GenreFormContractValidator : AbstractValidator<GenreFormContract>
{
	#region [Constructor]
	/// <summary>
	/// Initializes a new instance of the <see cref="GenreFormContractValidator"/> class.
	/// </summary>
	public GenreFormContractValidator()
	{
		this.RuleFor((contract) => contract.Name).Length(0, 200);
	}
	#endregion
}
