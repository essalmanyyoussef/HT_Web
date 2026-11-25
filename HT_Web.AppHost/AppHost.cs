var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.HT_Web>("ht-web");
builder.AddProject<Projects.ET_Training>("et-training");

builder.Build().Run();