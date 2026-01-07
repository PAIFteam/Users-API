var builder = WebApplication.CreateBuilder(args);

// Add services to the container 
builder.Services.AddCarter();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions(); // ajuda a performar melhor as operações do CRUD

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapCarter();
app.Run();
