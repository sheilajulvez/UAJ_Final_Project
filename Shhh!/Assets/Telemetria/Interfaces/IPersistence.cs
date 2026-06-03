using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace UAJ.Telemetry
{
    public interface IPersistence
    {
        void Enqueue(TrackerEvent evt);
        void StartProcessing();
        void StopProcessing();
    }
}
