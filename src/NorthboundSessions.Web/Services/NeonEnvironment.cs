using System.Net;
using Npgsql;

namespace NorthboundSessions.Web.Services;

public static class NeonEnvironment
{
    public static void LoadDotEnv()
    {
        var paths = new[]
        {
            Path.Combine(Directory.GetCurrentDirectory(), ".env.local"),
            FindFileFrom(AppContext.BaseDirectory, ".env.local")
        };

        foreach (var path in paths.Distinct(StringComparer.Ordinal))
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                continue;
            }

            foreach (var line in File.ReadLines(path))
            {
                var trimmed = line.Trim();
                if (trimmed.Length == 0 || trimmed.StartsWith('#'))
                {
                    continue;
                }

                var separator = trimmed.IndexOf('=');
                if (separator <= 0)
                {
                    continue;
                }

                var key = trimmed[..separator].Trim();
                if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key)))
                {
                    continue;
                }

                Environment.SetEnvironmentVariable(key, Unquote(trimmed[(separator + 1)..].Trim()));
            }

            return;
        }
    }

    public static string? Get(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        return string.IsNullOrWhiteSpace(value) ? null : Unquote(value.Trim());
    }

    public static string? GetNpgsql(string key, bool pooled = true)
    {
        var value = Get(key);
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri)
            || (uri.Scheme != "postgres" && uri.Scheme != "postgresql"))
        {
            return value;
        }

        var userInfo = uri.UserInfo.Split(':', 2);
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')),
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty,
            SslMode = SslMode.Require,
            Pooling = pooled
        };

        foreach (var pair in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = pair.Split('=', 2);
            var name = WebUtility.UrlDecode(parts[0]);
            var parameterValue = parts.Length > 1 ? WebUtility.UrlDecode(parts[1]) : string.Empty;

            if (name.Equals("channel_binding", StringComparison.OrdinalIgnoreCase)
                && parameterValue.Equals("require", StringComparison.OrdinalIgnoreCase))
            {
                builder.ChannelBinding = ChannelBinding.Require;
            }
        }

        return builder.ConnectionString;
    }

    private static string Unquote(string value)
    {
        if (value.Length >= 2
            && ((value[0] == '"' && value[^1] == '"') || (value[0] == '\'' && value[^1] == '\'')))
        {
            return value[1..^1];
        }

        return value;
    }

    private static string? FindFileFrom(string startDirectory, string fileName)
    {
        var directory = new DirectoryInfo(startDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
