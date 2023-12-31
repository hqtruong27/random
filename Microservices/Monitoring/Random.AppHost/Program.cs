var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.GenshinImpact_Api>("genshinimpact.api");

builder.AddProject<Projects.Hoyolab_Api>("hoyolab.api");

builder.AddProject<Projects.Spending_Api>("spending.api");

builder.Build().Run();
