namespace dotnet_test_urlrewrite;

public class VanityRedirectMiddleware
{
    private readonly RequestDelegate _next;

    public VanityRedirectMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        Console.WriteLine("> Path: {0}", context.Request.Path);
        if (context.Request.Path.Equals("/vanity", StringComparison.InvariantCultureIgnoreCase))
        {
            var newUrl = "/en-gb/vanity-target";
            var redirectQuery = "utm=yup&id=123";

            if (context.Request.Query.Any())
            {
                var existingQuery = context.Request.Query
                    .ToDictionary(entry => entry.Key, entry => entry.Value);
                var newParamEntries = redirectQuery.Split('&')
                    .ToDictionary(entry => entry.Split('=')[0], entry => entry.Split('=')[1]);

                foreach (var newParam in newParamEntries)
                {
                    if (existingQuery.ContainsKey(newParam.Key)) existingQuery[newParam.Key] = newParam.Value;
                    else existingQuery.Add(newParam.Key, newParam.Value);
                }

                newUrl = string.Join('?', newUrl, string.Join('&',
                    existingQuery.Select(entry => string.Join('=', entry.Key, entry.Value))));
            }
            else
            {
                newUrl = string.Join('?', (new[] {newUrl, redirectQuery}).Where(s => !string.IsNullOrEmpty(s)));
            }

            context.Response.Redirect(newUrl, true);
            context.Response.Headers["X-Redirect"] = "redirected";

            return;
        }

        await _next(context);
    }
}