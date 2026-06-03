using System.Collections.Generic;
using UnityEngine;

namespace UAJ.Telemetry
{
public class Tracker
{
    public static Tracker Instance { get; private set; } = new Tracker();

    public ISerializer serializer;
    public IPersistence persistence;
    private float startTime;
    private float levelStart;

    Dictionary<string, bool> trackerMap = new Dictionary<string, bool>();
    private string sessionId;

    public string GetSessionId() => sessionId;

    public void Initialize(ISerializer serializer, IPersistence persistence, Dictionary<string, bool> trackerMap)
    {
        this.serializer = serializer;
        this.persistence = persistence;
        this.trackerMap = trackerMap;
        sessionId = System.Guid.NewGuid().ToString();
    }

    public void Stop()
    {
        TrackSessionEndEvent(GetSessionId());
        persistence.StopProcessing();
    }

    public void Start()
    {
        Debug.Log("START");
        persistence.StartProcessing();
    }

    public void TrackEvent(TrackerEvent eventToTrack)
    {
        if (trackerMap[eventToTrack.trackerName])
        {
            persistence.Enqueue(eventToTrack);
        }
    }

    public void TrackSessionStartEvent(string sessionId)
    {
        startTime = Time.time;
        var data = new Dictionary<string, object>
        {
            { "session_id", sessionId }
        };

        TrackEvent(new TrackerEvent(EventType.SessionStart.ToString(), TrackerEventType.ProgressionTracker.ToString(), data));
    }

    public void TrackSessionEndEvent(string sessionId)
    {
        var data = new Dictionary<string, object>
        {
            { "session_id", sessionId }
        };

        TrackEvent(new TrackerEvent(EventType.SessionEnd.ToString(), TrackerEventType.ProgressionTracker.ToString(), data));
    }

    public void TrackLevelStart(float levelId)
    {
        levelStart = Time.time;
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "level_id", levelId }
        };

        TrackEvent(new TrackerEvent(EventType.LevelStart.ToString(), TrackerEventType.ProgressionTracker.ToString(), data));
    }

    public void TrackLevelEnd(float levelId)
    {
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "level_id", levelId }
        };

        TrackEvent(new TrackerEvent(EventType.LevelEnd.ToString(), TrackerEventType.ProgressionTracker.ToString(), data));
    }

    public void TrackFallDeath(float positionX)
    {
        var data = new Dictionary<string, object>
        {
            { "position", positionX }
        };

        TrackEvent(new TrackerEvent(EventType.FallDeath.ToString(), TrackerEventType.ProgressionTracker.ToString(), data));
    }

    public void TrackSpikeDeath(float positionX)
    {
        var data = new Dictionary<string, object>
        {
            { "position", positionX }
        };

        TrackEvent(new TrackerEvent(EventType.SpikeDeath.ToString(), TrackerEventType.ProgressionTracker.ToString(), data));
    }

    public void TrackSlimeDeath(float positionX)
    {
        var data = new Dictionary<string, object>
        {
            { "position", positionX }
        };

        TrackEvent(new TrackerEvent(EventType.SlimeDeath.ToString(), TrackerEventType.ProgressionTracker.ToString(), data));
    }

    public void TrackPause(string levelId)
    {
        var data = new Dictionary<string, object>
        {
            { "session_id", sessionId },
            { "level_id", levelId }
        };

        TrackEvent(new TrackerEvent(EventType.Pause.ToString(), TrackerEventType.ResourceTracker.ToString(), data));
    }
}
}
