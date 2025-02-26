using UnityEngine;
using System;
using System.Runtime.InteropServices;
using FMODUnity;
using System.Globalization;
using System.Collections.Generic;
using FMOD;

public class BeatManager : MonoBehaviour
{
    public EventReference eventToPlay;

    public enum BeatType { FixedBeat, UpBeat };

    public static bool isPlayingMusic = false;

    [Header("DEBUG STUFF:")]
    public bool doDebugSounds = false;
    public FMODUnity.StudioEventEmitter downBeatEvent;
    public FMODUnity.StudioEventEmitter upBeatEvent;

    [Header("OTHER OPTIONS:")]
    public float upBeatDivisor = 2f;

    private int masterSampleRate;
    private double currentSamples = 0;
    private static double currentTime = 0f;

    private ulong dspClock;
    private ulong parentDSP;

    private static double beatInterval = 0f;

    private static double dspDeltaTime = 0f;
    private static double lastDSPTime = 0f;

    private bool tempoWaitForNextBeat = false;


    private double tempoTrackDSPStartTime;
    private static string markerString = "";
    private static bool justHitMarker = false;
    private static int markerTime;

    // BEAT DELEGATES!!!
    public delegate void BeatEventDelegate();
    public static event BeatEventDelegate onFixedBeat;

    private static double lastFixedBeatTime = -2;
    private static double lastFixedBeatDSPTime = -2;

    public static event BeatEventDelegate onUpBeat;

    private double lastUpBeatTime = -2;

    public delegate void TempoUpdateDelegate(float beatInterval);
    public static event TempoUpdateDelegate onTempoChanged;

    public delegate void MarkerListenerDelegate();
    public static event MarkerListenerDelegate onMarkerUpdated;
    // END!!!

    public static BeatManager _instance;


    private FMOD.Studio.PLAYBACK_STATE musicPlayState;
    private FMOD.Studio.PLAYBACK_STATE lastMusicPlayState;

    [StructLayout(LayoutKind.Sequential)]
    public class TimelineInfo
    {
        public int currentBeat = 0;
        public int currentBar = 0;
        public int beatPosition = 0;
        public float currentTempo = 0;
        public float lastTempo = 0;
        public int currentPosition = 0;
        public int songLengthInMS = 0;
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    }

    public TimelineInfo timelineInfo = null;

    private GCHandle timelineHandle;

    private FMOD.Studio.EVENT_CALLBACK beatCallback;
    private FMOD.Studio.EventDescription descriptionCallback;

    private static FMOD.Studio.EventInstance currentMusicTrack;


    private void Awake()
    {
        _instance = this;
    }

    private void AssignMusicCallbacks()
    {
        timelineInfo = new TimelineInfo();
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);
        currentMusicTrack.setUserData(GCHandle.ToIntPtr(timelineHandle));
        currentMusicTrack.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        currentMusicTrack.getDescription(out descriptionCallback);
        descriptionCallback.getLength(out int length);

        timelineInfo.songLengthInMS = length;

        //Debug.Log("SONG LENGTH = " + length);


        FMOD.SPEAKERMODE speakerMode;
        int numRawSpeakers;

        //FMODUnity.RuntimeManager.CoreSystem.getMasterChannelGroup(out masterChannelGroup);

        FMODUnity.RuntimeManager.CoreSystem.getSoftwareFormat(out masterSampleRate, out speakerMode, out numRawSpeakers);
    }


    private static float currentPitch = 1f;

    public static void SetPitch(float newPitch)
    {
        currentMusicTrack.setPitch(newPitch);
        currentPitch = newPitch;
    }

    public static float GetPitch()
    {
        return currentPitch;
    }

    public static void SetMusicTrack(EventReference newTrack)
    {
        FMOD.Studio.EventDescription description;

        if (isPlayingMusic)
        {
            isPlayingMusic = false;

            currentMusicTrack.getDescription(out description);
            description.unloadSampleData();

            currentMusicTrack.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        currentMusicTrack = RuntimeManager.CreateInstance(newTrack);

        currentMusicTrack.getDescription(out description);

        description.loadSampleData();
    }
    public void playMetronome(bool b)
    {
        doDebugSounds = b;
    }
    public static void PlayMusicTrack()
    {
        isPlayingMusic = true;

        currentMusicTrack.start();

        _instance.AssignMusicCallbacks();
    }

    private void SetTrackStartInfo()
    {
        lastDSPTime = 0f;

        UpdateDSPClock();

        tempoTrackDSPStartTime = currentTime;
        lastFixedBeatTime = 0f;
        lastFixedBeatDSPTime = currentTime;
    }

    private void UpdateDSPClock()
    {
        //masterChannelGroup.getDSPClock(out dspClock, out parentDSP);

        currentMusicTrack.getChannelGroup(out FMOD.ChannelGroup newChanGrp);
        newChanGrp.getDSPClock(out dspClock, out parentDSP);

        currentSamples = dspClock;
        currentTime = currentSamples / masterSampleRate;

        dspDeltaTime = currentTime - lastDSPTime;

        lastDSPTime = currentTime;
    }

    public static void CheckHitSpecialMarker(string markerName)
    {
        markerName = markerName.ToLower();

        if (markerName.Contains("swing =") || markerName.Contains("swing="))
        {
            markerName = markerName.Split('=')[1];
            if (float.TryParse(markerName, NumberStyles.Any, CultureInfo.InvariantCulture, out float swingValue))
            {
                _instance.upBeatDivisor = swingValue;
            }

            return;
        }
    }

    public static float GetBeatInterval()
    {
        return (float)beatInterval;
    }

    public static float GetLastFixedBeatDSPTime()
    {
        return (float)lastFixedBeatDSPTime;
    }

    public static float GetCurrentTime()
    {
        return (float)currentTime;
    }

    public static int GetCurrentTimeInMilliseconds()
    {
        return _instance.timelineInfo.currentPosition;
    }

    public static int GetTrackLengthInMilliseconds()
    {
        return _instance.timelineInfo.songLengthInMS;
    }

    public static float GetDSPDeltaTime()
    {
        return (float)dspDeltaTime;
    }

    public void playSong()
    {
        SetMusicTrack(eventToPlay);
        PlayMusicTrack();
    }

    private void Update()
    {

        currentMusicTrack.getPlaybackState(out musicPlayState);

        if (lastMusicPlayState != FMOD.Studio.PLAYBACK_STATE.PLAYING && musicPlayState == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            SetTrackStartInfo();
        }

        lastMusicPlayState = musicPlayState;

        if (musicPlayState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            //isPlayingMusic = false;
            return;
        }


        currentMusicTrack.getTimelinePosition(out timelineInfo.currentPosition);

        UpdateDSPClock();

        CheckTempoMarkers();

        if (beatInterval == 0f)
        {
            return;
        }

        if (justHitMarker)
        {
            justHitMarker = false;

            if (lastFixedBeatDSPTime < currentTime - (beatInterval / 2f))
            {
                DoFixedBeat();
            }

            int currentTimelinePos;

            currentMusicTrack.getTimelinePosition(out currentTimelinePos);

            float offset = (currentTimelinePos - markerTime) / 1000f;

            tempoTrackDSPStartTime = currentTime - offset;
            lastFixedBeatTime = 0f;
            lastFixedBeatDSPTime = tempoTrackDSPStartTime;

            lastUpBeatTime = 0f;

            if (onMarkerUpdated != null)
            {
                onMarkerUpdated();
            }
        }

        CheckNextBeat();
    }

    public static float UpBeatPosition()
    {
        return ((float)beatInterval / _instance.upBeatDivisor);
    }

    private void CheckNextBeat()
    {
        float fixedSongPosition = (float)(currentTime - tempoTrackDSPStartTime);
        float upBeatSongPosition = fixedSongPosition + UpBeatPosition();

        // FIXED BEAT
        if (fixedSongPosition >= lastFixedBeatTime + beatInterval)
        {
            float r = Mathf.Repeat(fixedSongPosition, (float)beatInterval);

            DoFixedBeat();


            lastFixedBeatTime = (fixedSongPosition - r);
            lastFixedBeatDSPTime = (currentTime - r);

            if (tempoWaitForNextBeat)
            {
                SetTrackTempo();

                tempoWaitForNextBeat = false;
            }
        }

        // UP BEAT
        if (upBeatSongPosition >= lastUpBeatTime + beatInterval)
        {
            float r = Mathf.Repeat(upBeatSongPosition, (float)beatInterval);

            DoUpBeat();

            lastUpBeatTime = (upBeatSongPosition - r);
        }
    }

    private void DoFixedBeat()
    {
        if (onFixedBeat != null)
        {
            onFixedBeat();
        }

        if (doDebugSounds)
        {
            downBeatEvent.Play();
        }
    }

    private void DoUpBeat()
    {
        if (onUpBeat != null)
        {
            onUpBeat();
        }

        if (doDebugSounds)
        {
            upBeatEvent.Play();
        }
    }

    private bool CheckTempoMarkers()
    {
        if (timelineInfo.currentTempo != timelineInfo.lastTempo && !tempoWaitForNextBeat)
        {
            SetTrackTempo();
            return true;
        }

        return false;
    }

    private void SetTrackTempo()
    {
        int currentTimelinePos;

        currentMusicTrack.getTimelinePosition(out currentTimelinePos);

        float offset = (currentTimelinePos - timelineInfo.beatPosition) / 1000f; // divided into seconds

        tempoTrackDSPStartTime = currentTime - offset;

        lastFixedBeatTime = 0f;
        lastFixedBeatDSPTime = tempoTrackDSPStartTime;

        lastUpBeatTime = 0f;

        timelineInfo.lastTempo = timelineInfo.currentTempo;

        beatInterval = 60f / timelineInfo.currentTempo;

        if (onTempoChanged != null)
        {
            onTempoChanged((float)beatInterval);
        }
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        // Retrieve the user data
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
        if (result != FMOD.RESULT.OK)
        {
            UnityEngine.Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            // Get the object to store beat and marker details
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        //Debug.Log("RECEIVED BEAT CALLBACK!!!");

                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.currentBar = parameter.bar;
                        timelineInfo.currentBeat = parameter.beat;
                        timelineInfo.beatPosition = parameter.position;
                        timelineInfo.currentTempo = parameter.tempo;
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.lastMarker = parameter.name;
                        markerString = parameter.name;
                        markerTime = parameter.position;
                        CheckHitSpecialMarker(parameter.name);

                        justHitMarker = true;
                    }
                    break;
            }
        }
        return FMOD.RESULT.OK;
    }


    private float terribleThreshold = 0.4f, coolThreshold = 0.75f, perfectThreshold = 0.9f;
    //public float[] thresholds
    public double easyMultiplier = 0.1;
    public SCORE evaluateClick(double clickTime, float specialMult)
    {
        /*
CAMBIOS esto estaba roto. xd. comprobara siempre con el primero y le a�adimos un margen de tolerancia para comprobar un early.
           */

        SCORE res = SCORE.NONE;
        double lastBeat = lastFixedBeatDSPTime; double nextBeat = lastBeat + beatInterval;



        //comprobamos que no este pegadito por detras y que sea un early que deberia dar mas puntos
        double beatToCheck = lastBeat;
        if ((nextBeat - clickTime) / beatInterval <= 0.1)
        {
            beatToCheck = nextBeat;
        }

        double evaluacion = 1 - ((Math.Abs(clickTime - beatToCheck)) / beatInterval);

        if (evaluacion < 0) evaluacion = 0;
        double easyMode = 1 + easyMultiplier * (1 - evaluacion);

        if (evaluacion <= terribleThreshold) // mas cercano al centro, peor
        {
            res = SCORE.TERRIBLE;
        }
        else if (evaluacion <= coolThreshold)
        {
            res = SCORE.COOL;
        }
        else if (evaluacion <= perfectThreshold)
        {
            res = SCORE.PERFECT;
        }
        else
        {
            res = SCORE.HEAVY;
        }
        UnityEngine.Debug.Log($"{res}, {easyMode},  {evaluacion}. comparando con beat {beatToCheck}");
        UnityEngine.Debug.Log($" click: {clickTime},  lastbeat = {lastBeat}, nextBeat = {nextBeat}");

        LevelManager._instance.addPointsByScore(res, specialMult);
        return res;
    }
}

