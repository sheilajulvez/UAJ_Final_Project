using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using UnityEngine;

public class FilePersistence : IPersistence
{
    private BlockingCollection<TrackerEvent> eventQueue;
    private string filename;
    private string path;
    private ISerializer serializer;

    private Thread processingThread;
    private volatile bool isRunning;
    private volatile bool hasFailed;

    private FileStream fileStream;
    private StreamWriter writer;

    public FilePersistence(string filename, ISerializer serializer)
    {
        this.filename = filename;
        this.serializer = serializer;

        eventQueue = new BlockingCollection<TrackerEvent>();

        path = Path.Combine(Application.dataPath, "Telemetria", filename);

        try
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            Debug.Log("Ruta de telemetria: " + path);
        }
        catch (Exception ex)
        {
            hasFailed = true;
            Debug.LogError($"[Telemetry] Error al preparar la carpeta de telemetria: {ex.Message}");
        }
    }

    public void Enqueue(TrackerEvent evt)
    {
        if (evt == null || hasFailed || !isRunning || eventQueue.IsAddingCompleted)
        {
            return;
        }

        try
        {
            eventQueue.Add(evt);
        }
        catch (InvalidOperationException)
        {
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Telemetry] Error al encolar evento: {ex.Message}");
        }
    }

    public void StartProcessing()
    {
        if (hasFailed || isRunning)
        {
            return;
        }

        try
        {
            fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
            writer = new StreamWriter(fileStream);

            isRunning = true;

            processingThread = new Thread(ProcessEvents);
            processingThread.IsBackground = true;
            processingThread.Name = "TelemetryThread";
            processingThread.Start();
        }
        catch (Exception ex)
        {
            hasFailed = true;
            isRunning = false;
            Debug.LogError($"[Telemetry] Error al iniciar la persistencia en fichero: {ex.Message}");
            SafeCloseWriter();
        }
    }

    public void StopProcessing()
    {
        if (!isRunning)
        {
            return;
        }

        isRunning = false;

        try
        {
            eventQueue.CompleteAdding();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[Telemetry] Error al cerrar la cola de eventos: {ex.Message}");
        }

        try
        {
            if (processingThread != null && processingThread.IsAlive)
            {
                processingThread.Join(2000);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[Telemetry] Error al detener el hilo de telemetria: {ex.Message}");
        }
        finally
        {
            SafeCloseWriter();
        }
    }

    private void ProcessEvents()
    {
        try
        {
            foreach (TrackerEvent evt in eventQueue.GetConsumingEnumerable())
            {
                if (hasFailed)
                {
                    break;
                }

                try
                {
                    string serializedEvent = serializer.Serialize(evt.parameters);
                    writer.WriteLine(serializedEvent);
                    writer.Flush();
                }
                catch (Exception ex)
                {
                    hasFailed = true;
                    Debug.LogError($"[Telemetry] Error al serializar o escribir un evento: {ex.Message}");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            hasFailed = true;
            Debug.LogError($"[Telemetry] Error en el hilo de persistencia: {ex.Message}");
        }
        finally
        {
            SafeCloseWriter();
        }
    }

    private void SafeCloseWriter()
    {
        try
        {
            writer?.Flush();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[Telemetry] Error al hacer flush del writer: {ex.Message}");
        }

        try
        {
            writer?.Dispose();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[Telemetry] Error al cerrar el writer: {ex.Message}");
        }
        finally
        {
            writer = null;
        }

        try
        {
            fileStream?.Dispose();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[Telemetry] Error al cerrar el file stream: {ex.Message}");
        }
        finally
        {
            fileStream = null;
        }
    }

    public void Dispose()
    {
        StopProcessing();
        eventQueue?.Dispose();
    }
}
