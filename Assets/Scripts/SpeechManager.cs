using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

//namespace Microsoft.MixedReality.Toolkit.Input

public class SpeechManager : MonoBehaviour, IMixedRealitySpeechHandler
{

    private bool calibration = false;

    IMixedRealityInputSystem inputSystem;
    private void OnEnable()
    {
        Debug.Log("enabled!");
        inputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);
    }

    private void OnDisable()
    {
        //IMixedRealityInputSystem inputSystem;
        inputSystem?.UnregisterHandler<IMixedRealitySpeechHandler>(this);
        Debug.Log("speech input disabled");
    }

    void IMixedRealitySpeechHandler.OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        Debug.Log(eventData.Command.Keyword);


        if (eventData.Command.Keyword.ToLower() == "smaller")
        {
            transform.localScale *= 0.5f;
        }
        else if (eventData.Command.Keyword.ToLower() == "bigger")
        {
            transform.localScale *= 2.0f;
        }
        else if (eventData.Command.Keyword.ToLower() == "calibrate")
        {
            calibrate();
        }
    }

    public void calibrate()
    {
        calibration = true;
        Debug.Log("calibration!");
    }

    public void set_calibration(bool c)
    {
        calibration = c;
    }

    public bool get_calibration()
    {
        return calibration;
    }

    public void new_pos()
    {
        var pos = new Vector3(Random.Range(0.0f, 0.5f), 0, Random.Range(-0.9f, -1.2f));
        transform.position = pos;
    }
}



