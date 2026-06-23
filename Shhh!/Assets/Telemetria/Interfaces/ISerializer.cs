using System.Collections.Generic;
using Newtonsoft.Json;

namespace UAJ.Telemetry
{
    public interface ISerializer
    {
        string Serialize(Dictionary<string, object> data);
    }
}
