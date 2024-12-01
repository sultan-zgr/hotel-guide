using CacheManager.Core;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Ocelot için yapýlandýrma
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// CacheManager ekleniyor
builder.Services.AddCacheManager<object>(settings => settings.WithDictionaryHandle());

// Ocelot ekleniyor
builder.Services.AddOcelot();

var app = builder.Build();

// Ocelot Middleware'i ekleyin
app.UseHttpsRedirection();
await app.UseOcelot();

app.Run();
