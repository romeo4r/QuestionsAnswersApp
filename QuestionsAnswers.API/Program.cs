using Microsoft.EntityFrameworkCore;
using QuestionsAnswers.API.Data;
using QuestionsAnswers.API.Services;

var builder = WebApplication.CreateBuilder(args);


// Register DbContext with the connection string from the appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBQuestionsAnswersConnection")));

// Register Question sAnwers(QA) Services
builder.Services.AddScoped<UserQAService>();
builder.Services.AddScoped<QuestionService>();
builder.Services.AddScoped<AnswerService>();

// Add services to the container.
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
