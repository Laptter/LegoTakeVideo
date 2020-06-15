using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AVProLiveCameraSetting : MonoBehaviour
{
    public AVProLiveCamera aVProLiveCamera;
    public GameObject QuiceDevice;

    private void OnEnable()
    {
        aVProLiveCamera._deviceSelection = AVProLiveCamera.SelectDeviceBy.Index;
        aVProLiveCamera._modeSelection = AVProLiveCamera.SelectModeBy.Index;
        aVProLiveCamera._desiredDeviceIndex = 0;
        aVProLiveCamera._desiredModeIndex = 33;
        aVProLiveCamera.Begin();
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && aVProLiveCamera.Device != null && aVProLiveCamera.Device.IsRunning)
        {
            
        }
    }

    private void OnDisable()
    {
        
    }
}
