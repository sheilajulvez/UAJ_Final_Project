using System.Collections.Generic;
using UnityEngine;

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

        TrackEvent(new TrackerEvent(TelemetryEventType.SessionStart.ToString(), TelemetryTrackerEventType.ProgressionTracker.ToString(), data));
    }

    public void TrackSessionEndEvent(string sessionId)
    {
        var data = new Dictionary<string, object>
        {
            { "session_id", sessionId }
        };

        TrackEvent(new TrackerEvent(TelemetryEventType.SessionEnd.ToString(), TelemetryTrackerEventType.ProgressionTracker.ToString(), data));
    }

    public void TrackLevelStart(float levelId)
    {
        levelStart = Time.time;
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "level_id", levelId }
        };

        TrackEvent(new TrackerEvent(TelemetryEventType.LevelStart.ToString(), TelemetryTrackerEventType.ProgressionTracker.ToString(), data));
    }

    public void TrackLevelEnd(float levelId)
    {
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "level_id", levelId }
        };

        TrackEvent(new TrackerEvent(TelemetryEventType.LevelEnd.ToString(), TelemetryTrackerEventType.ProgressionTracker.ToString(), data));
    }

    public void TrackFallDeath(float positionX)
    {
        var data = new Dictionary<string, object>
        {
            { "position", positionX }
        };

        TrackEvent(new TrackerEvent(TelemetryEventType.FallDeath.ToString(), TelemetryTrackerEventType.ProgressionTracker.ToString(), data));
    }

    public void TrackSpikeDeath(float positionX)
    {
        var data = new Dictionary<string, object>
        {
            { "position", positionX }
        };

        TrackEvent(new TrackerEvent(TelemetryEventType.SpikeDeath.ToString(), TelemetryTrackerEventType.ProgressionTracker.ToString(), data));
    }

    public void TrackSlimeDeath(float positionX)
    {
        var data = new Dictionary<string, object>
        {
            { "position", positionX }
        };

        TrackEvent(new TrackerEvent(TelemetryEventType.SlimeDeath.ToString(), TelemetryTrackerEventType.ProgressionTracker.ToString(), data));
    }

    public void TrackPause(string levelId)
    {
        var data = new Dictionary<string, object>
        {
            { "session_id", sessionId },
            { "level_id", levelId }
        };

        TrackEvent(new TrackerEvent(TelemetryEventType.Pause.ToString(), TelemetryTrackerEventType.ResourceTracker.ToString(), data));
    }
}
