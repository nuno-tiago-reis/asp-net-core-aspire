namespace Memento.Aspire.Domain.Service.Persistence;

using Memento.Aspire.Domain.Service.Persistence.Entities.Author;
using Memento.Aspire.Domain.Service.Persistence.Entities.Book;
using Memento.Aspire.Domain.Service.Persistence.Entities.Genre;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

/// <summary>
/// Implements the domain entity seeder.
/// </summary>
///
/// <seealso cref="DomainContext"/>
public sealed class DomainSeeder
{
	#region [Constants]
	/// <summary>
	/// The filename with the seeding data for the 'Author' models.
	/// </summary>
	private const string AUTHORS_FILE_NAME = "Persistence/Seeding/Authors";

	/// <summary>
	/// The filename with the seeding data for the 'Book' models.
	/// </summary>
	private const string BOOKS_FILE_NAME = "Persistence/Seeding/Books";

	/// <summary>
	/// The filename with the seeding data for the 'Genre' models.
	/// </summary>
	private const string GENRES_FILE_NAME = "Persistence/Seeding/Genres";
	#endregion

	#region [Properties]
	/// <summary>
	/// The context.
	/// </summary>
	private readonly DomainContext Context;

	/// <summary>
	/// The hosting environment.
	/// </summary>
	private readonly IHostEnvironment Environment;

	/// <summary>
	/// The logger.
	/// </summary>
	private readonly ILogger Logger;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="DomainSeeder"/> class.
	/// </summary>
	///
	/// <param name="context">The context.</param>
	/// <param name="environment">The environment.</param>
	/// <param name="logger">The logger.</param>
	public DomainSeeder
	(
		DomainContext context,
		IHostEnvironment environment,
		ILogger<DomainSeeder> logger
	)
	{
		this.Context = context;
		this.Environment = environment;
		this.Logger = logger;
	}
	#endregion

	#region [Methods]
	/// <summary>
	/// Seeds the identity context models (as well as other necessary entities).
	/// </summary>
	public void Seed()
	{
		this.SeedAuthors();
		this.SeedGenres();
		this.SeedBooks();
	}

	/// <summary>
	/// Seeds the authors.
	/// </summary>
	private void SeedAuthors()
	{
		// Build the authors
		var authors = new List<Author>();

		try
		{
			// Read the authors from the global file
			string globalFile = $"{AUTHORS_FILE_NAME}.json";
			authors.AddRange(JsonSerializer.Deserialize<List<Author>>(File.ReadAllText(globalFile))!);
		}
		catch (DirectoryNotFoundException)
		{
			// Ignore if the file does not exist
		}
		catch (Exception exception)
		{
			this.Logger.LogError("{Message}: {Exception}", exception.Message, exception);
		}

		try
		{
			// Read the authors from the environment specific file
			string environmentFile = $"{AUTHORS_FILE_NAME}.{this.Environment.EnvironmentName}.json";
			authors.AddRange(JsonSerializer.Deserialize<List<Author>>(File.ReadAllText(environmentFile))!);
		}
		catch (DirectoryNotFoundException)
		{
			// Ignore if the file does not exist
		}
		catch (Exception exception)
		{
			this.Logger.LogError("{Message}: {Exception}", exception.Message, exception);
		}

		// Sort the authors
		authors.Sort((first, second) => string.Compare(first.Name, second.Name, StringComparison.Ordinal));

		// Update the context
		foreach (var author in authors)
		{
			// Check if it exists
			var contextAuthor = this.Context.Authors.FirstOrDefault((a) => a.Name == author.Name);
			if (contextAuthor == null)
			{
				// Add the author
				this.Context.Authors.Add(author);
				continue;
			}
		}

		// Save the context
		this.Context.SaveChanges();
	}

	/// <summary>
	/// Seeds the books.
	/// </summary>
	private void SeedBooks()
	{
		// Build the books
		var books = new List<Book>();

		try
		{
			// Read the books from the global file
			string globalFile = $"{BOOKS_FILE_NAME}.json";
			books.AddRange(JsonSerializer.Deserialize<List<Book>>(File.ReadAllText(globalFile))!);
		}
		catch (DirectoryNotFoundException)
		{
			// Ignore if the file does not exist
		}
		catch (Exception exception)
		{
			this.Logger.LogError("{Message}: {Exception}", exception.Message, exception);
		}

		try
		{
			// Read the books from the environment specific file
			string environmentFile = $"{BOOKS_FILE_NAME}.{this.Environment.EnvironmentName}.json";
			books.AddRange(JsonSerializer.Deserialize<List<Book>>(File.ReadAllText(environmentFile))!);
		}
		catch (DirectoryNotFoundException)
		{
			// Ignore if the file does not exist
		}
		catch (Exception exception)
		{
			this.Logger.LogError("{Message}: {Exception}", exception.Message, exception);
		}

		// Sort the books
		books.Sort((first, second) => string.Compare(first.Name, second.Name, StringComparison.Ordinal));

		// Update the context
		foreach (var book in books)
		{
			// Check if it exists
			var contextBook = this.Context.Books.FirstOrDefault((b) => b.Name == book.Name);
			if (contextBook == null)
			{
				// Add the book
				this.Context.Books.Add(book);
				continue;
			}
		}

		// Save the context
		this.Context.SaveChanges();
	}

	/// <summary>
	/// Seeds the genres.
	/// </summary>
	private void SeedGenres()
	{
		// Build the genres
		var genres = new List<Genre>();

		try
		{
			// Read the genres from the global file
			string globalFile = $"{GENRES_FILE_NAME}.json";
			genres.AddRange(JsonSerializer.Deserialize<List<Genre>>(File.ReadAllText(globalFile))!);
		}
		catch (DirectoryNotFoundException)
		{
			// Ignore if the file does not exist
		}
		catch (Exception exception)
		{
			this.Logger.LogError("{Message}: {Exception}", exception.Message, exception);
		}

		try
		{
			// Read the genres from the environment specific file
			string environmentFile = $"{GENRES_FILE_NAME}.{this.Environment.EnvironmentName}.json";
			genres.AddRange(JsonSerializer.Deserialize<List<Genre>>(File.ReadAllText(environmentFile))!);
		}
		catch (DirectoryNotFoundException)
		{
			// Ignore if the file does not exist
		}
		catch (Exception exception)
		{
			this.Logger.LogError("{Message}: {Exception}", exception.Message, exception);
		}

		// Sort the genres
		genres.Sort((first, second) => string.Compare(first.Name, second.Name, StringComparison.Ordinal));

		// Update the context
		foreach (var genre in genres)
		{
			// Check if it exists
			var contextGenre = this.Context.Genres.FirstOrDefault((g) => g.Name == genre.Name);
			if (contextGenre == null)
			{

				// Add the genre
				this.Context.Genres.Add(genre);
				continue;
			}
		}

		// Save the context
		this.Context.SaveChanges();
	}
	#endregion
}
