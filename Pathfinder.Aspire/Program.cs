using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder( args );

builder.AddProject<Pathfinder_Web>( "Pathfinder.Web" );
builder.Build().Run();