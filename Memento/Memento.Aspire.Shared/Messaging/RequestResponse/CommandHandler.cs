namespace Memento.Aspire.Shared.Messaging.RequestResponse;

using Microsoft.Extensions.Logging;

/// <summary>
/// Implements a generic command handler.
/// Provides methods to handle commands and command results.
/// </summary>
public abstract class CommandHandler<TCommand, TCommandResult> : MessageHandler<TCommand, TCommandResult>
	where TCommand : Command<TCommandResult>
	where TCommandResult : CommandResult
{
	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="CommandHandler{TCommand, TCommandResult}"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	protected CommandHandler(ILogger<CommandHandler<TCommand, TCommandResult>> logger) : base(logger)
	{
		// Intentionally Empty.
	}
	#endregion
}
