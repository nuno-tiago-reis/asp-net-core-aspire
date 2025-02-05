namespace Memento.Aspire.Hosting;

/// <summary>
/// Implements the hosting program.
/// </summary>
internal sealed class Program
{
	/// <summary>
	/// Bootstraps the application.
	/// </summary>
	///
	/// <param name="arguments">The arguments.</param>
	internal static void Main(string[] arguments)
	{
		#region [Builder]
		// Builder
		var builder = DistributedApplication.CreateBuilder(arguments);

		// Builder (Cache)
		var cache = builder.AddRedis("Cache")
			.WithLifetime(ContainerLifetime.Persistent)
			.WithRedisInsight();

		// Builder (Database)
		var database = builder.AddSqlServer("DatabaseServer")
			.WithLifetime(ContainerLifetime.Persistent)
			.AddDatabase("Database");

			//.WithBindMount("VolumeMount.SqlServer", "/var/opt/mssql");
			//.WithDataBindMount(@"C:\Aspire\SqlServer");
			//.WithDataBindMount(@"C:\Users\Tiago Reis\Documents\Aspire\SqlServer");

		// Builder (Logger)
		var logger = builder.AddSeq("Logger")
			.WithLifetime(ContainerLifetime.Persistent);

			//.WithBindMount("VolumeMount.Seq", "/var/opt/seq");
			//.WithDataBindMount(@"C:\Aspire\Seq");
			//.WithDataBindMount(@"C:\Users\Tiago Reis\Documents\Aspire\Seq");

		// Builder (MessageBus)
		var messageBus = builder.AddRabbitMQ("MessageBus")
			.WithLifetime(ContainerLifetime.Persistent)
			.WithManagementPlugin();

			//.WithBindMount("VolumeMount.RabbitMQ", "/var/lib/rabbitmq");
			//.WithDataBindMount(@"C:\Aspire\RabbitMQ");
			//.WithDataVolume(@"C:\Users\Tiago Reis\Documents\Aspire\RabbitMQ");

		// Builder (Projects)
		builder.AddProject<Projects.Memento_Aspire_Domain_Api>("Api", "Default")
			.WithEndpoint("https", (endpoint) => endpoint.IsProxied = false)
			.WithExternalHttpEndpoints()
			.WithReference(cache)
			.WaitFor(cache)
			.WithEnvironment("Cache:ConnectionString", cache)
			.WithReference(database)
			.WaitFor(database)
			.WithEnvironment("Database:ConnectionString", database)
			.WithReference(logger)
			.WaitFor(logger)
			.WithEnvironment("Serilog:WriteTo__0__Args:configure__1__Args:serverUrl", logger)
			.WithReference(messageBus)
			.WaitFor(messageBus)
			.WithEnvironment("MessageBus:ConnectionString", messageBus);

		builder.AddProject<Projects.Memento_Aspire_Domain_Service>("Service", "Default")
			.WithEndpoint("https", (endpoint) => endpoint.IsProxied = false)
			.WithExternalHttpEndpoints()
			.WithReference(cache)
			.WaitFor(cache)
			.WithEnvironment("Cache:ConnectionString", cache)
			.WithReference(database)
			.WaitFor(database)
			.WithEnvironment("Database:ConnectionString", database)
			.WithReference(logger)
			.WaitFor(logger)
			.WithEnvironment("Serilog:WriteTo__0__Args:configure__1__Args:serverUrl", logger)
			.WithReference(messageBus)
			.WaitFor(messageBus)
			.WithEnvironment("MessageBus:ConnectionString", messageBus);
		#endregion

		#region [Application]
		// Application
		var application = builder.Build();
		#endregion

		#region [Runner]
		// Runner
		application.Run();
		#endregion
	}
}
