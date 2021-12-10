using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using TMPro;
using System.Threading;


public class TestController : MonoBehaviour
{
 

    public TextMeshPro Text;
    //public TextMeshPro ImuTextIndicator;

    public int myvalriable;

    ReseachModeSensors myHololens;

    Texture2D tex;
    Rect rect;
    // Start is called before the first frame update
    void Start()
    {
        myHololens = new ReseachModeSensors(true, true, false);

        myHololens.StartSensorStream();

        tex = new Texture2D(myHololens.depthW, myHololens.depthH, TextureFormat.R16, false);
        rect = new Rect(0, 0, tex.width, tex.height);

    }

    // Update is called once per frame
    void Update()
    {
        Text.text = "Stop button\n";

        Text.text = "accel {" + myHololens.outputImuAccX + ", " + myHololens.outputImuAccY + ", " + myHololens.outputImuAccX +
            "}\n gyro {" + myHololens.outputImuGyroX + ", " + myHololens.outputImuGyroY + ", " + myHololens.outputImuGyroX +
            "}\n mag {" + myHololens.outputImuMagX + ", " + myHololens.outputImuMagY + ", " + myHololens.outputImuMagX + '}';
        byte[] binaryUint16arr = new byte[myHololens.depthW * myHololens.depthH * 2];

        for (int i=0; i< myHololens.outputDepth.Length; i++)
        {
            UInt16 d;
            d = myHololens.outputDepth[i];

            binaryUint16arr[2 * i + 1] = (byte)(d >> 8);
            binaryUint16arr[2 * i] = (byte)(d);

        }
        tex.LoadRawTextureData(binaryUint16arr);
        tex.Apply();
        gameObject.GetComponent<Image>().sprite = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f));
    }

    public void ChangeValue()
    {
        myHololens.StopAllSensorStream();

    }
    public void OnDestroy()
    {
        myHololens.StopAllSensorStream();
    }
    public void ApplicationQuit()
    {
        Application.Quit();
    }
}


public class ReseachModeSensors
{
    [DllImport("dll_Test_UWP")]
    public static extern void GetSensorThread(ref int stopSign, int mode,
     ref float pImuAccX, ref float pImuAccY, ref float pImuAccZ,
     ref float pImuGyroX, ref float pImuGyroY, ref float pImuGyroZ,
     ref float pImuMagX, ref float pImuMagY, ref float pImuMagZ,
     UInt16[] pDepthImg, UInt16[] pAbImg);

    public float outputImuAccX, outputImuAccY, outputImuAccZ,
        outputImuGyroX, outputImuGyroY, outputImuGyroZ,
        outputImuMagX, outputImuMagY, outputImuMagZ;
    public UInt16[] outputDepth, outputAb;

    public int depthW, depthH;

    bool getImu = false;
    bool getDLT = false;
    bool getAHAT = false;

    Thread hololensThread;
    int stopSign;
    int mode;

    public ReseachModeSensors(bool setImu, bool setDLT, bool setAHAT)
    {
        if (setDLT && setAHAT)
            throw new Exception("Note that concurrent access to AHAT and Long Throw is currently not supported");
        getImu = setImu;
        getDLT = setDLT;
        getAHAT = setAHAT;

        if (getDLT)
        {
            depthW = 320;
            depthH = 288;
        }
        if (getAHAT)
        {
            depthW = 512;
            depthH = 512;
        }

        if (getDLT || getAHAT)
        {
            outputDepth = new UInt16[depthW * depthH * 1];
            outputAb = new UInt16[depthW * depthH * 1];
        }

        return;
    }

    ~ReseachModeSensors()
    {
        if (stopSign == 0)
            StopAllSensorStream();
    }

    public void StartSensorStream()
    {
        stopSign = 0;

        int mode = 0b0;
        if (getImu) mode += 0b100;
        if (getDLT) mode += 0b10;
        if (getAHAT) mode += 0b1;

        hololensThread = new Thread(() => GetSensorThread(ref stopSign, mode,
            ref outputImuAccX, ref outputImuAccY, ref outputImuAccZ,
        ref outputImuGyroX, ref outputImuGyroY, ref outputImuGyroZ,
        ref outputImuMagX, ref outputImuMagY, ref outputImuMagZ,
        outputDepth, outputAb));
        

        hololensThread.Start();
    }
    public void StopAllSensorStream()
    {
        stopSign = 1;
        hololensThread.Join();
    }
}