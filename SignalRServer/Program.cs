using SignalRServer.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddSingleton<IDictionary<string, conexaoUsuario>>(opts => new Dictionary<string, conexaoUsuario>());

var app = builder.Build();

app.UseRouting();
app.UseCors();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ServicoConversacaotHub>("/chat");
});
app.Run();
