using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ApiGateway.Ocelot;

public static class OcelotRoutePatcher
{
    public static void Patch(
        ConfigurationManager configuration,
        IHostEnvironment env)
    {
        var publicRoutes =
            configuration.GetSection("Jwt:PublicRoutes").Get<string[]>()
            ?? Array.Empty<string>();

        var fileName = $"ocelot.{env.EnvironmentName}.json";
        var path = Path.Combine(env.ContentRootPath, fileName);

        if (!File.Exists(path))
            throw new FileNotFoundException($"Ocelot config not found: {fileName}");

        var json = File.ReadAllText(path);

        var root = JsonNode.Parse(
            json,
            nodeOptions: null,
            documentOptions: new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            })!.AsObject();

        var routes = root["Routes"]!.AsArray();

        var originalRoutes = routes
            .Select(r => r!.AsObject())
            .ToList();

        foreach (var baseRoute in originalRoutes)
        {
            var upstreamTemplate =
                baseRoute["UpstreamPathTemplate"]?.GetValue<string>();

            if (string.IsNullOrWhiteSpace(upstreamTemplate))
                continue;

            if (!IsCatchAll(upstreamTemplate))
                continue;

            var servicePrefix = GetPrefix(upstreamTemplate); // "/identity", "/mail"

            baseRoute["AuthenticationOptions"] = new JsonObject
            {
                ["AuthenticationProviderKey"] = "Bearer"
            };

            baseRoute["Priority"] = 0;

            var matchedPublicRoutes = publicRoutes
                .Where(p => p.StartsWith(servicePrefix, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            foreach (var p in matchedPublicRoutes)
            {
                var upstreamPublic = NormalizeToTemplate(p);

                if (routes.Any(r =>
                        string.Equals(
                            r!["UpstreamPathTemplate"]?.GetValue<string>(),
                            upstreamPublic,
                            StringComparison.OrdinalIgnoreCase)))
                    continue;

                // Clone base route để giữ downstream config
                var clone = JsonNode.Parse(baseRoute.ToJsonString())!.AsObject();

                clone["UpstreamPathTemplate"] = upstreamPublic;
                clone["DownstreamPathTemplate"] =
                    RemovePrefix(upstreamPublic, servicePrefix);

                clone.Remove("AuthenticationOptions");

                clone["Priority"] = 1;

                routes.Insert(0, clone);
            }
        }

        var patchedJson = root.ToJsonString(new JsonSerializerOptions
        {
            WriteIndented = false
        });

        using var ms = new MemoryStream(Encoding.UTF8.GetBytes(patchedJson));
        configuration.AddJsonStream(ms);
    }

    private static bool IsCatchAll(string upstreamTemplate)
        => upstreamTemplate.Contains("{everything}", StringComparison.OrdinalIgnoreCase);

    private static string GetPrefix(string upstreamTemplate)
    {
        var idx = upstreamTemplate.IndexOf("/{everything}", StringComparison.OrdinalIgnoreCase);
        return idx > 0 ? upstreamTemplate[..idx] : upstreamTemplate;
    }

    private static string NormalizeToTemplate(string p)
    {
        p = p.Trim();

        if (p.EndsWith("/*", StringComparison.Ordinal))
            return p[..^1] + "{everything}";

        return p;
    }

    private static string RemovePrefix(string fullPath, string prefix)
    {
        return fullPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? fullPath.Substring(prefix.Length)
            : fullPath;
    }
}
