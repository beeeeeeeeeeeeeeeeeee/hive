using Hive.Email.Extensions;
using Hive.Webhooks.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddEmail();
builder.Services.AddWebhooks();

var host = builder.Build();
host.Run();
