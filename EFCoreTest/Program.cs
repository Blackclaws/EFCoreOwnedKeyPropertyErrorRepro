// See https://aka.ms/new-console-template for more information

using System.Xml.XPath;
using EFCoreTest;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddLogging();

var connectionString = "DataSource=:memory:;cache=shared";
var connection = new SqliteConnection(connectionString);
connection.Open();
builder.Services.AddDbContext<AppDbContext>(optionsAction =>
{
    optionsAction.EnableSensitiveDataLogging();
    optionsAction.UseSqlite(connection);
});


var app = builder.Build();

{
    var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

var testOwnedOne = new TestOwned("TestValueOne", "TestValueTwo");

{
    var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var entity = new TestEntity();


    entity.OwnedInternal.Add(testOwnedOne);

    await context.Set<TestEntity>().AddAsync(entity);

    await context.SaveChangesAsync();
}

{
    var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // This throws
    var firstOrDefault = context.Set<TestEntity>().FirstOrDefault(x => x.OwnedInternal.Any(y => y == testOwnedOne));
}