using Fake.Detection.Telegram.Client.Api;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(x => x.UseStartup<Startup>());

builder.Build().Run();