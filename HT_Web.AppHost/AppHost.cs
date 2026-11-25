var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.HT_Web>("ht-web")
    .WithExternalHttpEndpoints();

builder.Build().Run();
