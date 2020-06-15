using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Management : MonoBehaviour
{
    public enum VideoType 
    {
        AVI,
        MP4,
        MKV
    }
    private FFmpegTools tool;

    private string firstVideoFile;
    private string mideVideoFile;
    private string lastVideoFile;
    private string soundPath;

    private string transFormFile;

    private string srcVideoFile;
    private string outFileFolder;

    private string textPath;

    private List<string> combineList;
    public VideoType videoFormart = VideoType.AVI;
    public string bitRate = "16000k";
    private void OnEnable()
    {
        firstVideoFile = System.Environment.CurrentDirectory + "/Data/InputVideos/First."+ videoFormart.ToString().ToLower();
        mideVideoFile = System.Environment.CurrentDirectory + "/Data/InputVideos/Mid." + videoFormart.ToString().ToLower();
        lastVideoFile = System.Environment.CurrentDirectory + "/Data/InputVideos/Last." + videoFormart.ToString().ToLower();
        soundPath = System.Environment.CurrentDirectory + "/Data/InputVideos/background.mp3";
        srcVideoFile = System.Environment.CurrentDirectory + "/Data/InputVideos//Src." + videoFormart.ToString().ToLower();
        textPath = System.Environment.CurrentDirectory + "/Data/InputVideos/concat.txt";
        transFormFile = System.Environment.CurrentDirectory + "/Data/InputVideos/TransForm." + videoFormart.ToString().ToLower();
        outFileFolder = System.Environment.CurrentDirectory + "/Data/OutPutVideos/";

        combineList = new List<string>() { firstVideoFile, mideVideoFile, lastVideoFile };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnTrim("00:00:01", "6", () =>
            {
                Debug.Log("Trim Over");
            });
        }


        if (Input.GetKeyDown(KeyCode.W))
        {
            CustomConcatVideos(() =>
            {
                Debug.Log("Concate Over");
            });
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            CustomAddSound("time.mp4", path =>
            {
                Debug.Log(path);
            });

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            CustomConvert( () =>
            {
                Debug.Log("Convert Over");
            });
        }
    }



   

    public void OnTrim(string fromTime, string durationSec, UnityAction trim)
    {
        tool.TrimVideo(mideVideoFile, srcVideoFile, fromTime, durationSec,"10000k", trim);
    }

    public void OnAppend( UnityAction appand)
    {

        tool.AppendVideo(combineList, transFormFile, bitRate, appand);
    }



    public void OnAddSound(string outFile, UnityAction<string> appand)
    {
        string outFullPath = outFileFolder + outFile+"."+ videoFormart.ToString().ToLower();
        tool.AddSoundToVideo(outFullPath, soundPath, transFormFile, appand,bitRate);
    }


    public void CustomConvert(UnityAction convert)
    {
        //-i E:\Project\Project - 2020\LegoTakeVideo\Data\InputVideos\Src.mp4 - q:v 6 - b 16000k - r 25 E:\Project\Project - 2020\LegoTakeVideo\Data\InputVideos\road_1.avi - y
        var src = System.Environment.CurrentDirectory + "/Data/InputVideos//Src.mp4";
        string command = string.Format("-i {0} -q:v 6 -b 10000k -r 25 {1} -y",src, srcVideoFile);
        tool.CustomConvert(command, convert);
    }

    public void CustomConcatVideos(UnityAction concat)
    {
        textPath = textPath.Replace('/', '\\');
        string command = string.Format("-f concat -safe 0 -i {0} -c copy -b 5000k {1} -y", textPath, transFormFile);
        tool.CustomConcatVideos(command, concat);
    }


    public void CustomAddSound(string outFile, UnityAction<string> addSound)
    {
        string outFullPath = outFileFolder + outFile;
        //string command = string.Format("-i {0} -i {1} -codec copy -b 5000k {2} -y", transFormFile, soundPath, outFullPath);
        //待定
        string command = string.Format("-i {0} -i {1}  -b 5000k {2} -y", transFormFile, soundPath, outFullPath);
        tool.CustomAddSound(command, outFullPath, addSound);
    }

    private void Start()
    {
        tool = GameObject.FindObjectOfType<FFmpegTools>();
    }


    private void OnDisable()
    {

    }
}
