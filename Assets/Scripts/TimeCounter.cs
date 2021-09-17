using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeCounter : MonoBehaviour
{

    public int second = 3;
    public bool word = true;
    public bool audio = true;
    private float startTime = 0;
    private TMPro.TextMeshProUGUI tmp;
    private AudioClip[] clips = new AudioClip[3];
    private AudioSource audio_source;
    List<int> check_audio = new List<int>();

    string[] texts = new string[3] {"3", "2", "1" }; 

    // Start is called before the first frame update
    public void Initial()
    {
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        // load audioclip Assets/Resources/Audio/Audio-1.mp3
        clips[0] = Resources.Load<AudioClip>("Audio/Audio-3");
        clips[1] = Resources.Load<AudioClip>("Audio/Audio-2");
        clips[2] = Resources.Load<AudioClip>("Audio/Audio-1");
        audio_source = GetComponent<AudioSource>();
    }

    public void Start()
    {
        startTime = Time.time;
        check_audio = new List<int>();
    }

    // Update is called once per frame
    public void Counting()
    {
        int delta = Mathf.FloorToInt(Time.time - startTime);
        if ((delta >= 0) && (delta <= 2))
        {
            //Debug.Log("delta: " + delta);
            //tmp.text = texts[delta];
            //Debug.Log("should be: " + texts[delta]);
            if (word)
            {
                tmp.text = texts[delta];
            }
            
            if (audio)
            {
                if (!check_audio.Contains(delta))
                {
                    //audio.clip = clips[delta];
                    audio_source.PlayOneShot(clips[delta], 1);
                    //Debug.Log("played: " + delta);
                    check_audio.Add(delta);
                }
            }
            
        }

        else
        {
            tmp.text = "";
        }
    }

    public void Clear()
    {
        tmp.text = "";
        check_audio = new List<int>();
    }
}
