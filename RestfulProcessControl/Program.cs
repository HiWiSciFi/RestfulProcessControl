using RestfulProcessControl.Util;

namespace RestfulProcessControl;

public static class Program
{

	public static void Main(string[] args)
	{
		var builder = CreateBuilder(args);
		AddServices(ref builder);
		BuildSwagger(ref builder);
		var app = CreateApp(ref builder);
		CreateRequestPipeline(ref app);
		InitializeProgram();
		RunApp(ref app);
	}

	/// <summary>
	/// Creates the builder for the web application
	/// </summary>
	/// <param name="args">The input arguments of the application</param>
	/// <returns>The created WebApplicationBuilder</returns>
	private static WebApplicationBuilder CreateBuilder(string[] args) => WebApplication.CreateBuilder(args);

	/// <summary>
	/// Adds Services to the WebApplicationBuilder
	/// </summary>
	/// <param name="builder">The WebApplicationBuilder to add services to</param>
	private static void AddServices(ref WebApplicationBuilder builder)
	{
		builder.Services.AddControllers();
		builder.Services.AddAuthentication(options =>
		{
			options.DefaultChallengeScheme = "forbidscheme";
			options.DefaultForbidScheme = "forbidscheme";
			options.AddScheme<ForbiddenSchemeHandler>("forbidscheme", "Forbidden Handler");
		});
		builder.Services.AddCors(options =>
		{
			options.AddPolicy("AllowAll",
				policyBuilder => policyBuilder.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader());
		});
		builder.Services.AddLogging(options =>
		{
			options.AddConsole();
			options.AddSimpleConsole();
		});
	}

	/// <summary>
	/// Builds the swagger page
	/// </summary>
	/// <param name="builder">The WebApplicationBuilder</param>
	private static void BuildSwagger(ref WebApplicationBuilder builder)
	{
		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
			{
				Version = "v1",
				Title = "Process Control API Docs",
				Description = "API for controlling server processes"
			});
		});
	}

	/// <summary>
	/// Created the WebApplication from the WebApplicationBuilder
	/// </summary>
	/// <param name="builder">The builder to create the WebApplication from</param>
	/// <returns>The created WebApplication</returns>
	private static WebApplication CreateApp(ref WebApplicationBuilder builder) => builder.Build();

	/// <summary>
	/// Creates the Web request pipeline for the RESTful API
	/// </summary>
	/// <param name="app">The WebApplication to create the pipeline for</param>
	private static void CreateRequestPipeline(ref WebApplication app)
	{
		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
				c.RoutePrefix = "docs";
			});
		}

		app.UseCors("AllowAll");

		app.UseHttpsRedirection();
		app.UseAuthorization();

		app.MapControllers();
	}

	/// <summary>
	/// Initializes all classes that require further initialization
	/// </summary>
	private static void InitializeProgram()
	{
		if (!Globals.Reload()) Logger.LogWarning("Reloading Globals failed!");
		Logger.LogInformation("Database ConnectionString: \"{0}\"", Globals.ConnectionString);
		ApplicationManager.LoadAll();
	}

	/// <summary>
	/// Runs the WebApplication
	/// </summary>
	/// <param name="app">The WebApplication to run</param>
	private static void RunApp(ref WebApplication app) => app.Run();
}