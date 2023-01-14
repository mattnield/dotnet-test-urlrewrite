using System.Text.Json;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
var app = builder.Build();

using (StreamReader rulesReader = File.OpenText("urlrewrite.xml"))
{
    var options = new RewriteOptions()
        //.AddRedirectToHttpsPermanent()
        //.AddRedirect("(.*)/$", "$1", 301)
        .AddIISUrlRewrite(rulesReader);

    app.UseRewriter(options);
}

app.Use(async (HttpContext ctx, Func<Task> _) =>
{
  var data = new Dictionary<string, string>();
  data.Add("Path", ctx.Request.Path);
  data.Add("Date", DateTime.Now.ToString());
  data.Add("Method", ctx.Request.Method);
  data.Add("QueryString", ctx.Request.QueryString.ToString());
  data.Add("Scheme", ctx.Request.Scheme);
  data.Add("Host", ctx.Request.Host.ToString());

  var message = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
  ctx.Response.Headers.ContentType = "application/json";
  await ctx.Response.WriteAsync(message);
});

app.Run();
