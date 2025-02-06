namespace Memento.Aspire.Domain.Api.Validators.Book;

using FluentValidation;
using Memento.Aspire.Domain.Service.Contracts.Book;

/// <summary>
/// Implements the 'Book' form contract validator.
/// </summary>
internal sealed class BookFormContractValidator : AbstractValidator<BookFormContract>
{
	#region [Constructor]
	/// <summary>
	/// Initializes a new instance of the <see cref="BookFormContractValidator"/> class.
	/// </summary>
	public BookFormContractValidator()
	{
		this.RuleFor((contract) => contract.Name).Length(0, 200);
		this.RuleFor((contract) => contract.ReleaseDate).NotNull();
		this.RuleFor((contract) => contract.AuthorId).NotNull();
		this.RuleFor((contract) => contract.GenreId).NotNull();
	}
	#endregion
}
