using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System;

public class SensorUdp
{
    readonly int LOCA_LPORT = 52525;
    static UdpClient udp;
    Thread thread;

     
    List<int> Intensity = new List<int>();
    List<int> IntensityPercent = new List<int>();
    List<int> ErrorCode = new List<int>();
    List<int> Value = new List<int>();

    public void Start()
    {
        udp = new UdpClient(LOCA_LPORT);// open udp port
        //Debug.Log("udp started");
        udp.Client.ReceiveTimeout = 0;// set timeout for waiting sent data
        thread = new Thread(new ThreadStart(ThreadMethod));// make a thread(ThreadMethod) that receive sent datas via udp
        thread.Start();// start the thread
    }

    public void Update()
    {
        #region AppFinishTreat
        if (!Application.isPlaying)
        {
            thread.Abort();
            Debug.Log("aborted");
        }
        #endregion
    }

    #region FinishTreat
    void OnDestroy()
    {
        thread.Abort();
    }

    void OnApplicationQuit()
    {
        thread.Abort();
    }
    #endregion

    /// <summary>
    /// Receive sent datas via udp communication
    /// </summary>
    private void ThreadMethod()
    {
        //Debug.Log("test");
        while (true)
        {
            //Debug.Log("looping");
            try
            {
                //Debug.Log("start");
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] returnData = udp.Receive(ref remoteEP);// receive the data via udp communication
                //Debug.Log(Encoding.ASCII.GetString(returnData));
                //string text = Encoding.ASCII.GetString(Intensity);// encoding the data to string
                var returnString = Encoding.ASCII.GetString(returnData);
               // Debug.Log(returnString);
                if (returnString != "")
                {
                    DataStore(returnString.Split());
                }
                

            }
            catch (SocketException ex)
            {
                Debug.Log(ex.ToString());
            }

        }
    }

    void DataStore(string[] list)
    {
        //Debug.Log(list[0]);
        Intensity.Add(Int32.Parse(list[0]));
        //Debug.Log(list[1]);
        IntensityPercent.Add(Int32.Parse(list[1]));
        //Debug.Log(list[2]);
        ErrorCode.Add(Int32.Parse(list[2]));
        //Debug.Log(list[3]);
        Value.Add(Int32.Parse(list[3]));
    }


    public List<int> GetIntensity()
    {
        lock(Intensity)
            return Intensity;
    }

    public List<int> GetIntensityPercent()
    {
        lock(IntensityPercent)
            return IntensityPercent;
    }

    public List<int> GetErrorCode()
    {
        lock(ErrorCode)
            return ErrorCode;
    }

    public List<int> GetValue()
    {
        lock(Value)
            return Value;
    }
}

