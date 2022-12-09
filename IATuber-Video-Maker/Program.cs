using Domain.Interfaces;
using Infra.ExternalServices.Image;
using Infra.ExternalServices.Music;
using Infra.ExternalServices.Storage;
using Infra.ExternalServices.TextGenerator;
using Infra.ExternalServices.TextService;
using Infra.ExternalServices.Translate;
using Infra.ExternalServices.VideoEditor;
using Infra.ExternalServices.VoiceGenerator;
using Infra.MySQL;
using Infra.MySQL.Services;
using Infra.RabbitMQ;
using Infra.RabbitMQ.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Polly;
using Polly.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<DataContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});
builder.Services.AddSingleton<IVideoService, VideoService>();
builder.Services.AddSingleton<IVoiceService, VoiceServiceAzure>();
builder.Services.AddSingleton<ITranslateService, TranslateService>();
builder.Services.AddSingleton<IImageService, ImageService>();
builder.Services.AddSingleton<IImageDbService, ImageDbService>();
builder.Services.AddSingleton<IMusicService, MusicService>();
builder.Services.AddSingleton<IVideoEditor, VideoEditor>();
builder.Services.AddSingleton<ICloudStorage, GoogleCloudStorage>();

//Rabbit config
var configSection = builder.Configuration.GetSection("RabbitMQ");
var settings = new RabbitMQSettings();
configSection.Bind(settings);
builder.Services.AddSingleton(settings);

builder.Services.AddSingleton<IConnectionFactory>(x => new ConnectionFactory
{
    HostName = settings.HostName,
    Port = settings.port,
    UserName = settings.UserName,
    Password = settings.Password
});
builder.Services.AddSingleton<RabbitModelFactory>();
builder.Services.AddSingleton(x => x.GetRequiredService<RabbitModelFactory>().CreateChannel());
builder.Services.AddSingleton<IMessageQueue, UploadVideoMessage>();
//Rabbit config

builder.Services.AddHttpClient("General").AddPolicyHandler(GetRetryPolicy());

builder.Services.AddSingleton<ITextService, TextService>();

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


static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => !msg.IsSuccessStatusCode)
        .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}
