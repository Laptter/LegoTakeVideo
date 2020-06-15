using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFmpeg;
using UnityEngine.Events;
using System;

public class FFmpegTools : MonoBehaviour, IFFmpegHandler
{
    CustomExucuteCommand customConvert;
    CustomExucuteCommand customConnect;
    CustomExucuteCommand customTrim;
    CustomExucuteCommand customAddAudio;


    private void OnEnable()
    {
        
    }





    public CustomProgress progressView;
    FFmpegHandler defaultHandler = new FFmpegHandler();

    UnityAction appandCallback;
    UnityAction trimCallback;
    UnityAction addSoundCallback;
    UnityAction convertCallback;
    public void TrimVideo(TrimData config, UnityAction trim)
    {
        this.trimCallback = null; 
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
        this.appandCallback = callback;
        FFmpegCommands.DirectInput(command);
    }
   

    public void CustomConcatVideos(string command, UnityAction concat)
    {
        this.appandCallback = null;
        this.appandCallback = concat;
        FFmpegCommands.DirectInput(command);
    }

    public void AppendVideo(AppendData config, UnityAction appand, bool fastMode = false)
    {
        this.appandCallback = null;
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


    public void CustomAddSound(string command, UnityAction addSound)
    {
        this.addSoundCallback = null;
        this.addSoundCallback = addSound;
        FFmpegCommands.DirectInput(command);
    }

    public void AddSoundToVideo(SoundData config, UnityAction addSound, bool fastMode = false)
    {
        this.addSoundCallback = null;
        this.addSoundCallback = addSound;

        if (fastMode)
            FFmpegCommands.AddSoundFast(config);
        else
            FFmpegCommands.AddSoundFull(config);
    }

    public void AddSoundToVideo(string outputPath, string soundPath,string inputPath, UnityAction addSound, string bitRate, bool fastMode=false)
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
        addSoundCallback?.Invoke();
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
    public CustomExucuteCommand(UnityAction finishCallback)
    {
        this.finishCallback = finishCallback;
    }
    public void OnFailure(string msg)
    {
        
    }

    public void OnFinish()
    {
        
    }

    public void OnProgress(string msg)
    {
        //通知进度
    }

    public void OnStart()
    {
       
    }

    public void OnSuccess(string msg)
    {
        
    }
}

