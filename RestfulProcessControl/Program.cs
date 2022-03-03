namespace RestfulProcessControl;

public static class Program {

	public static void Main(string[] args) {
		var builder = CreateBuilder(args);
		AddServices(ref builder);
		BuildSwagger(ref builder);
		var app = CreateApp(ref builder);
		CreateRequestPipeline(ref app);
		RunApp(ref app);
	}

	private static WebApplicationBuilder CreateBuilder(string[] args) => WebApplication.CreateBuilder(args);

	private static void AddServices(ref WebApplicationBuilder builder)
	{
		builder.Services.AddCors(options =>
		{
			options.AddPolicy("AllowAll",
				policyBuilder => policyBuilder.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowCredentials());
		});
		builder.Services.AddControllers();
		builder.Services.AddAuthentication(options =>
		{
			options.DefaultChallengeScheme = "forbidscheme";
			options.DefaultForbidScheme = "forbidscheme";
			options.AddScheme<ForbiddenSchemeHandler>("forbidscheme", "Forbidden Handler");
		});
	}

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

	private static WebApplication CreateApp(ref WebApplicationBuilder builder) => builder.Build();

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

	private static void RunApp(ref WebApplication app) => app.Run();
}