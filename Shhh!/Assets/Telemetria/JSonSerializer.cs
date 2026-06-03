using System.Collections.Generic;
using Newtonsoft.Json;

public class JsonSerializer : ISerializer
{
    public string Serialize(Dictionary<string, object> data)
    {
        return JsonConvert.SerializeObject(data);
    }
}
