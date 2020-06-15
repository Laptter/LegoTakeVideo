using NatCorder.Examples;
using RenderHeads.Media.AVProVideo;
using System;
using UnityEngine;
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

    public Slider seekSlider;
    public float SeekTime = 0f;
    private bool bSeeking = true;

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
                Debug.Log("FinishedPlaying");
                break;
        }
    }

    void AddUIElementEvent()
    {
        startRecord.onClick.AddListener(StartRecordCallback);
        confirm.onClick.AddListener(ConfirmCallback);
        cancle.onClick.AddListener(CancleCallback);
        seekSlider.onValueChanged.AddListener(SeekCallBack);
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
        management.CustomConvert(() => {
            Debug.Log("---------------1------------------");
            management.OnTrim("00:00:01", "6", () =>
            {
                Debug.Log("---------------2------------------");
            });
        });
        return;
        management.CustomConvert(() => {
            management.OnTrim(string.Format("00:00:0{0}", SeekTime.ToString()), "6", () =>
            {
                Debug.Log("Trim Over");
                //management.CustomConcatVideos(() =>
                //{
                //    Debug.Log("concat over");
                //    string finalVideo = string.Format("recording_{0}.avi", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff"));
                //    management.CustomAddSound(finalVideo, () =>
                //    {
                //        Debug.Log("AddSoundCallback");
                //    });
                //});
            });
        });
        
    }

    void StartRecordCallback()
    {
        startRecord.gameObject.SetActive(false);
        replayCam.StartRecording();
        Invoke("StopRecord", 9f);
    }

    public void Seek(float time)
    {
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
        startRecord.onClick.RemoveListener(StartRecordCallback);
        confirm.onClick.RemoveListener(ConfirmCallback);
        cancle.onClick.RemoveListener(CancleCallback);
    }

}
