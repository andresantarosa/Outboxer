using Microsoft.EntityFrameworkCore;
using Outboxer;
using Outboxer.FakeEnqueuer;
using Outboxer.Test.API.Context;
using Outboxer.Test.API.Repository;
using Outboxer.Test.API.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<StudentsContext>(options =>
{
    options.UseSqlServer("Server=localhost; Database=Students; User Id=sa; Password=yourStrong(!)Password; Trusted_Connection=false; MultipleActiveResultSets=true;");
    options.EnableSensitiveDataLogging();
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddOutboxer<StudentsContext>();
builder.Services.UseFakeEnqueuer();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

public partial class Program { }