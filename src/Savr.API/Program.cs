using Savr.Application;
using Savr.Persistence;
using Savr.Identity;
using Savr.Presentation.Configuration;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Savr.Persistence.Data;
using Savr.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Serilog.Sinks.PostgreSQL;
using Savr.API;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(configure =>
{
    configure.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.TryAdd("traceId", context.HttpContext.TraceIdentifier);
    };
});

Log.Logger = new LoggerConfiguration()
    //.MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.PostgreSQL(
        connectionString: builder.Configuration.GetConnectionString("postgres"),
        tableName: "logs",
        needAutoCreateTable: true,
        columnOptions: new Dictionary<string, ColumnWriterBase>
        {
            { "message", new RenderedMessageColumnWriter() },
            { "message_template", new MessageTemplateColumnWriter() },
            { "level", new LevelColumnWriter() },
            { "timestamp", new TimestampColumnWriter() },
            { "exception", new ExceptionColumnWriter() },
            { "properties", new PropertiesColumnWriter() },
            { "log_event", new LogEventSerializedColumnWriter() }
        })
    .CreateLogger();


builder.Host.UseSerilog();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddControllers();
builder.Services.AddApplicationDependencies()
                .AddPersistenceDependencies(builder.Configuration)
                .AddIdentityServices(builder.Configuration)
                .AddSwagger()
                .AddHttpConetxt()
                ;
                //.AddStackExchangeRedisCache(options =>
                //{
                //    options.Configuration = builder.Configuration.GetConnectionString("Redis");
                //    options.InstanceName = "Session";
                //}).Configure<LoggerFilterOptions>(options => options.AddFilter("StackExchange.Redis", LogLevel.Trace)); ;

builder.Services.AddResponseCompression(options => 
{
        options.Providers.Add<GzipCompressionProvider>();
        //options.EnableForHttps = true;  
});

builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level = System.IO.Compression.CompressionLevel.SmallestSize);
builder.Services.AddCors(options =>
{
    options.AddPolicy("default", builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();

    });
});
var app = builder.Build();

app.UseRouting();
app.UseSession();


if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwaggerDocumentation();
    
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    using (var context = scope.ServiceProvider.GetService<ApplicationDbContext>())
    {
        context!.Database.Migrate();
        context!.Database.EnsureCreated();
    }

    using (var context = scope.ServiceProvider.GetService<UserDbContext>())
    {
        context!.Database.Migrate();
        context!.Database.EnsureCreated();
    }

    //using (var context = scope.ServiceProvider.GetService<LogDbContext>())
    //{
    //    context!.Database.Migrate();
    //    context!.Database.EnsureCreated();
    //}
}

app.UseIdentity();
app.UseHttpsRedirection();
app.UseSwaggerDocumentation();
//app.UseCors(policy => policy
//    .WithOrigins("https://localhost:5173") // your frontend origin
//    .AllowAnyHeader()
//    .AllowAnyMethod()
//    .AllowCredentials()); // required for cookies/session-based auth

app.UseCors();
app.UseExceptionHandler();
app.MapControllers();


//app.Use(async (context, next) =>
//{

//    await next(context);

//    // Log response details
//    Log.Information($"Response: {context.Response.StatusCode}");
//});


try
{
    Log.Information("Starting app {Time}", DateTime.UtcNow);
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "App crashed.");
}
finally
{
    Log.CloseAndFlush();
}




