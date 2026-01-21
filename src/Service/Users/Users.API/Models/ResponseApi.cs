namespace Users.API.Models;

using System.Text.Json.Serialization;

public sealed class DomainNotification
{
    public DomainNotification(string key, string message)
    {
        Key = key;
        Message = message;
    }

    public string Key { get; }
    public string Message { get; }
}

public sealed class ResponseApi
{
    public ResponseApi(object? data = null)
    {
        Data = NormalizeData(data);
    }

    public object? Data { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<DomainNotification>? Notifications { get; private set; }

    public ResponseApi AddNotification(string key, string message)
    {
        Notifications ??= [];
        Notifications.Add(new DomainNotification(key, message));
        return this;
    }

    private static object? NormalizeData(object? data)
    {
        if (data is null)
            return null;

        if (data is Exception ex)
        {
            return new
            {
                Error = ex.GetType().Name,
                ex.Message
            };
        }

        return data;
    }
}
