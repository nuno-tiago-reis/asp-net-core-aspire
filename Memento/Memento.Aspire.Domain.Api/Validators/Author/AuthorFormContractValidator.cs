namespace Memento.Aspire.Domain.Api.Validators.Author;

using FluentValidation;
using Memento.Aspire.Domain.Service.Contracts.Author;

/// <summary>
/// Implements the 'Author' form contract validator.
/// </summary>
internal sealed class AuthorFormContractValidator : AbstractValidator<AuthorFormContract>
{
	#region [Constructor]
	/// <summary>
	/// Initializes a new instance of the <see cref="AuthorFormContractValidator"/> class.
	/// </summary>
	public AuthorFormContractValidator()
	{
		this.RuleFor((contract) => contract.Name).Length(0, 200);
		this.RuleFor((contract) => contract.BirthDate).NotNull();
	}
	#endregion
}
