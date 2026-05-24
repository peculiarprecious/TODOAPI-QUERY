using Microsoft.EntityFrameworkCore;
using TODOAPI_QUERY.Data;
using TODOAPI_QUERY.DatabaseContext;
using TODOAPI_QUERY.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value != null &&
                            e.Value.Errors.Count > 0)
                .ToDictionary(
                    e => e.Key,
                    e => e.Value!.Errors
                              .Select(x => x.ErrorMessage)
                              .ToArray()
                );
            var errorResponse = new TODOAPI_QUERY.Responses.ErrorResponse
            {
                StatusCode = 400,
                Message = "Validation failed",
                Errors = errors,
                Timestamp = DateTime.Now
            };
            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(errorResponse);
        };
    });

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TodoDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ToDoDbContextConnection"));
});

builder.Services.AddScoped<ITodoService, TodoServices>();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

    // Automatically applies migrations and creates todo.db if it doesn't exist
    context.Database.EnsureCreated();

    // Executes your 10-item data dump
    DatabaseSeeder.SeedData(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
