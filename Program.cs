using System.Globalization;
using System.Text.Json;
using dotnet_test_urlrewrite;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Register our vanity URL redirect
app.UseMiddleware<VanityRedirectMiddleware>();

// Register the IIS URL rewrite maps
var rewriteOptions = new RewriteOptions();
rewriteOptions.AddIISUrlRewrite(builder.Environment.ContentRootFileProvider, "url-rewrite.xml");
app.UseRewriter(rewriteOptions);

// Handle all the requests with wholesome JSON
app.Use(async (HttpContext ctx, Func<Task> _) =>
{
  var data = new Dictionary<string, string>
  {
    {"Path", ctx.Request.Path},
    {"Date", DateTime.Now.ToString(CultureInfo.InvariantCulture)},
    {"Method", ctx.Request.Method},
    {"QueryString", ctx.Request.QueryString.ToString()},
    {"Scheme", ctx.Request.Scheme},
    {"Host", ctx.Request.Host.ToString()}
  };
  
  var message = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
  ctx.Response.Headers.ContentType = "application/json";
  await ctx.Response.WriteAsync(message);
});

app.Run();


