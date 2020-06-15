using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;  
using ZXing.QrCode; 
public class QRCardBehaviour : MonoBehaviour {
	Texture2D encoded;  
	public RawImage image;
	void OnEnable()
	{
		encoded = new Texture2D(256, 256);  
		
	}

	 /// <summary>  
    /// 生成二维码  
    /// </summary>  
    public void Btn_CreatQr(string inputMessage)   
    {
        if (inputMessage.Length > 1)  
        {  
            //二维码写入图片    
            var color32 = Encode(inputMessage, encoded.width, encoded.height);  
            encoded.SetPixels32(color32);  
            encoded.Apply();  
            //生成的二维码图片附给RawImage    
            image.texture = encoded; 
			image.SetNativeSize(); 
        }  
        else   
        {  
            //GameObject.Find("InputField").GetComponent<InputField>().text = "没有生成信息";  
        }  
    } 

	  //定义方法生成二维码    
    private static Color32[] Encode(string textForEncoding, int width, int height)  
    {  
        var writer = new BarcodeWriter  
        {  
            Format = BarcodeFormat.QR_CODE,  
            Options = new QrCodeEncodingOptions  
            {  
                Height = height,  
                Width = width  
            }  
        };  
        return writer.Write(textForEncoding);  
    }  

	void OnDisable()
	{

	}
}
