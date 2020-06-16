using NatCorder.Examples;
using RenderHeads.Media.AVProVideo;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProcessController : MonoBehaviour
{
    ///录频
    public ReplayCam replayCam;
    //转码工具
    public Management management;
    //视频采集
    public AVProLiveCamera avProLiveCamera;
    //视频播放
    public DisplayUGUI displayUGUI;
    //背景
    public GameObject backGround;
    //采集界面
    public GameObject proLiveUgui;
    //拍摄界面
    public GameObject editorPanel;
    //开始拍摄按钮
    public Button startRecord;
    //确认按钮
    public Button confirm;
    //进度条
    public GameObject processBar;
    //取消按钮
    public Button cancle;
    //二维码
    public Button qRcard;
    //采样进度条
    public Slider seekSlider;

    public Text trimText;
    public float SeekTime = 0f;
    private bool bSeeking = true;

    public CustomProgress customProgress;

    public void OnStartButtonClick()
    {
        StartPanel2EditorPanel();
    }

    void StartPanel2EditorPanel()
    {
        backGround.SetActive(false);
        editorPanel.SetActive(true);
        avProLiveCamera.enabled = true;
        proLiveUgui.SetActive(true);
        startRecord.gameObject.SetActive(true);
        confirm.gameObject.SetActive(false);
        cancle.gameObject.SetActive(false);
        seekSlider.gameObject.SetActive(false);
        displayUGUI.gameObject.SetActive(false);
        processBar.SetActive(false);
        qRcard.gameObject.SetActive(false);
        if (displayUGUI._mediaPlayer.Control != null)
        {
            displayUGUI._mediaPlayer.Control.CloseVideo();
        }
        bSeeking = true;
        SeekTime = 0f;
    }


    private void OnEnable()
    {
        Init();
        AddUIElementEvent();
    }

    void Init()
    {
        backGround.SetActive(true);
        editorPanel.SetActive(false);
        avProLiveCamera.enabled = false;
        displayUGUI._mediaPlayer.Events.AddListener(OnMediaPlayerEvent);
        trimText.text = string.Format("start trim:{0}", 0);
    }

    public void OnMediaPlayerEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.FinishedPlaying:
                if (bSeeking)
                {
                    Seek(SeekTime);
                }
                else
                {
                    mp.Rewind(false);
                }
                //Debug.Log("FinishedPlaying");
                break;
        }
    }

    void AddUIElementEvent()
    {
        startRecord.onClick.AddListener(StartRecordCallback);
        confirm.onClick.AddListener(ConfirmCallback);
        cancle.onClick.AddListener(CancleCallback);
        seekSlider.onValueChanged.AddListener(SeekCallBack);
        qRcard.onClick.AddListener(CancleCallback);
    }
    private void SeekCallBack(float value)
    {
        SeekTime = value;
        Seek(value);
    }
    private void CancleCallback()
    {
        StartPanel2EditorPanel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ConfirmCallback();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            var path = "E:\\Project\\Project-2020\\LegoTakeVideo\\Data\\OutPutVideos\\time.mp4";
            StartCoroutine(UploadVideo(path));
        }
    }

    private void ConfirmCallback()
    {
        //视频转码-->avi
        //视频裁剪-->seekTime
        //management.OnTrim()
        //视频连接
        //添加音效
        //上传服务器->返回二维码
        //生成二维码
        processBar.SetActive(true);
        seekSlider.gameObject.SetActive(false);
        confirm.gameObject.SetActive(false);
        cancle.gameObject.SetActive(false);

        management.CustomConvert(() => {
            management.OnTrim(string.Format("00:00:0{0}", SeekTime.ToString()), "6", () =>{
                management.CustomConcatVideos(() =>{
                    string finalVideo = string.Format("recording_{0}.mp4", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff"));
                    management.CustomAddSound(finalVideo, path =>
                    {
                        path = path.Replace('\\', '/');
                        StartCoroutine(UploadVideo(path));
                    });
                });
            });
        });
    }


    


    IEnumerator UploadVideo(string videoPath)
    {
        customProgress.BeforeUpLoad();
        WWWForm form = new WWWForm();
        try
        {
            byte[] bytes = File.ReadAllBytes(videoPath);
            int lastIndex = videoPath.LastIndexOf('/');
            var name = videoPath.Substring(lastIndex);
            form.AddBinaryData("files", bytes, name);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        //using (UnityWebRequest webRequest = UnityWebRequest.Post("http://47.105.54.177:8082/api/upload/multi?token=laogehendiao", form))
        using (UnityWebRequest webRequest = UnityWebRequest.Post("http://47.102.125.102:8082/api/upload/multi?token=laogehendiao", form))
        {
            webRequest.timeout = 120;
            webRequest.SendWebRequest();

            while (!webRequest.isDone)
            {
                yield return null;
                //Debug.Log("Uploading . Progress " + (int)(webRequest.uploadProgress * 100f) + "%");
                customProgress.OnUpLoadProgres(webRequest.uploadProgress);
            }

            if (webRequest.isNetworkError || webRequest.isHttpError )
            {
                Debug.Log(webRequest.error);
                //提供返回界面
            }
            else
            {
                //Debug.Log("Form upload complete! " + webRequest.downloadHandler.text);
                //int index = www.downloadHandler.text.LastIndexOf('/');
                //string videoName = www.downloadHandler.text.Substring(index+1);
                //GameObject.FindObjectOfType<QRCardBehaviour>().Btn_CreatQr(www.downloadHandler.text);
                ShowFinalInterface(webRequest.downloadHandler.text,videoPath);
            }
        }
    }


    public void ShowFinalInterface(string qRcardUrl,string videoPath)
    {
        bSeeking = false;
        GameObject.FindObjectOfType<QRCardBehaviour>().Btn_CreatQr(qRcardUrl);
        displayUGUI.gameObject.SetActive(true);
        displayUGUI._mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, videoPath);
        qRcard.gameObject.SetActive(true);
    }


    void StartRecordCallback()
    {
        startRecord.gameObject.SetActive(false);
        replayCam.StartRecording();
        Invoke("StopRecord", 8f);
    }

    public void Seek(float time)
    {
        trimText.text = string.Format("start trim:{0:f}", time);
        displayUGUI._mediaPlayer.Control.SeekFast(time*1000);
    }

    void StopRecord()
    {
        replayCam.StopRecording();
    }

    public void RecordingFinish(string path)
    {
        avProLiveCamera.enabled = false;
        confirm.gameObject.SetActive(true);
        cancle.gameObject.SetActive(true);
        proLiveUgui.SetActive(false);
        seekSlider.gameObject.SetActive(true);
        displayUGUI.gameObject.SetActive(true);
        displayUGUI._mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, path);
        displayUGUI._mediaPlayer.Control.Play();
    }

    private void OnDisable()
    {
        startRecord.onClick.RemoveAllListeners();
        confirm.onClick.RemoveAllListeners();
        cancle.onClick.RemoveAllListeners();
        seekSlider.onValueChanged.RemoveAllListeners();
        qRcard.onClick.RemoveAllListeners();
    }
}
