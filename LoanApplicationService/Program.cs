using LoanApplicationService.Models;
using LoanApplicationService.RabbitMq;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<LoanApplicationDbContext>(optionsAction: options => options.UseSqlite("Data Source=loanApplicationService.db"));
builder.Services.AddSingleton<IRabbitMqUtil, RabbitMqUtil>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<LoanApplicationDbContext>();
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();

