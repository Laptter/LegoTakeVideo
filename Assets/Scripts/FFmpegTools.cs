using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFmpeg;
using UnityEngine.Events;
using System;

public class FFmpegTools : MonoBehaviour, IFFmpegHandler
{
    //CustomExucuteCommand customConvert;
    //CustomExucuteCommand customConnect;
    //CustomExucuteCommand customTrim;
    //CustomExucuteCommand customAddAudio;


    private void OnEnable()
    {
        //customConvert = new CustomExucuteCommand(OnFinish, OnStart, OnFailure, OnProgress, OnSuccess);
        //customConnect = new CustomExucuteCommand(OnFinish, OnStart, OnFailure, OnProgress, OnSuccess);
        //customTrim = new CustomExucuteCommand(OnFinish, OnStart, OnFailure, OnProgress, OnSuccess);
        //customAddAudio = new CustomExucuteCommand(OnFinish, OnStart, OnFailure, OnProgress, OnSuccess);
    }





    public CustomProgress progressView;
    FFmpegHandler defaultHandler = new FFmpegHandler();

    UnityAction appandCallback;
    UnityAction trimCallback;
    UnityAction<string> addSoundCallback;
    UnityAction convertCallback;
    string outPath = string.Empty;
    public void TrimVideo(TrimData config, UnityAction trim)
    {
        this.appandCallback = null;
        this.trimCallback = null;
        this.addSoundCallback = null;
        this.convertCallback = null;


        this.trimCallback = trim;
        FFmpegCommands.Trim(config);
    }

    public void TrimVideo(string outputPath,string inputPath,string fromTime,
        string durationSec, string bitRate, UnityAction trim)
    {
        TrimData config = new TrimData();
        config.outputPath = outputPath;
        config.inputPath = inputPath;
        config.fromTime = fromTime;
        config.durationSec = int.Parse(durationSec);
        config.bitRate = bitRate;
        TrimVideo(config, trim);
    }

    public void DirectExcuteCommand(string command, UnityAction callback)
    {
        this.appandCallback = null;
        this.trimCallback = null;
        this.addSoundCallback = null;
        this.convertCallback = null;

        this.appandCallback = callback;
        FFmpegCommands.DirectInput(command);
    }
   

    public void CustomConcatVideos(string command, UnityAction concat)
    {
        this.appandCallback = null;
        this.trimCallback = null;
        this.addSoundCallback = null;
        this.convertCallback = null;

        this.appandCallback = concat;
        FFmpegCommands.DirectInput(command);
    }

    public void AppendVideo(AppendData config, UnityAction appand, bool fastMode = false)
    {
        this.appandCallback = null;
        this.trimCallback = null;
        this.addSoundCallback = null;
        this.convertCallback = null;

        this.appandCallback = appand;

        if (fastMode)
            FFmpegCommands.AppendFast(config);
        else
            FFmpegCommands.AppendFull(config);
    }

    public void AppendVideo(List<string>inputPathes,string outputPath, string bitRate, UnityAction appand, bool fastMode=false)
    {
        AppendData config = new AppendData();
        config.inputPaths = inputPathes;
        config.outputPath = outputPath;
        config.bitRate = bitRate;
        AppendVideo(config, appand, fastMode);
    }


    public void CustomAddSound(string command, string outFullPath, UnityAction<string> addSound)
    {
        this.appandCallback = null;
        this.trimCallback = null;
        this.addSoundCallback = null;
        this.convertCallback = null;
        this.outPath = outFullPath;
        this.addSoundCallback = addSound;
        FFmpegCommands.DirectInput(command);
    }

    public void AddSoundToVideo(SoundData config, UnityAction<string> addSound, bool fastMode = false)
    {
        this.appandCallback = null;
        this.trimCallback = null;
        this.addSoundCallback = null;
        this.convertCallback = null;

        this.addSoundCallback = addSound;

        if (fastMode)
            FFmpegCommands.AddSoundFast(config);
        else
            FFmpegCommands.AddSoundFull(config);
    }

    public void AddSoundToVideo(string outputPath, string soundPath,string inputPath, UnityAction<string> addSound, string bitRate, bool fastMode=false)
    {
        SoundData config = new SoundData();
        config.outputPath = outputPath;
        config.soundPath = soundPath;
        config.inputPath = inputPath;
        config.bitRate = bitRate;
        AddSoundToVideo(config, addSound, fastMode);
    }

    public void CustomConvert(string command, UnityAction convert)
    {
        this.appandCallback = null;
        this.trimCallback = null;
        this.addSoundCallback = null;
        this.convertCallback = null;

        this.convertCallback = convert;
        FFmpegCommands.DirectInput(command);
    }

    void Awake()
    {
        FFmpegParser.Handler = this;
    }


    public void OnFailure(string msg)
    {
        defaultHandler.OnFailure(msg);
        progressView.OnFailure(msg);
    }

    public void OnFinish()
    {
        defaultHandler.OnFinish();
        progressView.OnFinish();
        //Finish
       

        appandCallback?.Invoke();
        trimCallback?.Invoke();
        addSoundCallback?.Invoke(outPath);
        convertCallback?.Invoke();
    }


    public void OnProgress(string msg)
    {
        defaultHandler.OnProgress(msg);
        progressView.OnProgress(msg);
    }

    public void OnStart()
    {
        defaultHandler.OnStart();
        progressView.OnStart();
    }

    public void OnSuccess(string msg)
    {
       
    }
}


public class CustomExucuteCommand : IFFmpegHandler
{
    UnityAction <string>failureCallback,progressCallback,successCallback;
    UnityAction finishCallback, startCallback;
    public CustomExucuteCommand(UnityAction finishCallback,UnityAction startCallback,
        UnityAction<string> failureCallback, UnityAction<string> progressCallback, 
        UnityAction<string> successCallback)
    {
        this.finishCallback = finishCallback;
        this.startCallback = startCallback;
        this.failureCallback = failureCallback;
        this.progressCallback = progressCallback;
        this.successCallback = successCallback;
    }
    public void OnFailure(string msg)
    {
        failureCallback?.Invoke(msg);
    }

    public void OnFinish()
    {
        finishCallback?.Invoke();
    }

    public void OnProgress(string msg)
    {
        //通知进度
        progressCallback?.Invoke(msg);
    }

    public void OnStart()
    {
        startCallback?.Invoke();
    }

    public void OnSuccess(string msg)
    {
        successCallback?.Invoke(msg);
    }
}

