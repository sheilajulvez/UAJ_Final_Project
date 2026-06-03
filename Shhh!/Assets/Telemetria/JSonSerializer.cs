using System.Collections.Generic;
using Newtonsoft.Json;

namespace UAJ.Telemetry
{
    public class JsonSerializer : ISerializer
    {
        public string Serialize(Dictionary<string, object> data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}
