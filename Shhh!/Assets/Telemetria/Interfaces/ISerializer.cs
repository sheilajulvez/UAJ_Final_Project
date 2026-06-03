using System.Collections.Generic;
using Newtonsoft.Json;

public interface ISerializer
{
    string Serialize(Dictionary<string, object> data);
}
