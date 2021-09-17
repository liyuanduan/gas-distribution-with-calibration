using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BarCreate
{
    GameObject[,] bar = new GameObject[10, 10];
    const int INCREASE = 1;
    const int DECREASE = -1;
    int flag;
    Color32 change = new Color32(255, 0, 0, 255); //(r, g, b, a)
    Color test = new Color32(0, 1, 1, 0);

    float center_y;

    // Start : 确定bar的位置x，y，起始z，颜色
    public void Start(int x, int y, Vector3 position, GameObject parent) //(Object o)
    {
        //create bar
        bar[x, y] = new GameObject();
        bar[x, y] = GameObject.CreatePrimitive(PrimitiveType.Cube);

        bar[x, y].transform.SetParent(parent.transform, false);
        
        bar[x, y].transform.localPosition = position;
        //center_y = position.y;
        var renderer = bar[x, y].GetComponent<MeshRenderer>();
        renderer.material.SetColor("_Color", Color.red);
        bar[x, y].transform.localScale = new Vector3(1, 2.5f, 1);
        bar[x, y].layer = 2;
    }

    // Update： 改变高度，颜色
    public void Update(float value, int x, int y)
    {
        //Debug.Log(value);

        float temp = value / 5000.0f;

        //change size and position
        float height = temp * 2.5f;
        bar[x, y].transform.localScale = new Vector3(1, height, 1); // change height of bar
        bar[x, y].transform.localPosition = new Vector3(bar[x, y].transform.localPosition.x, height / 2, bar[x, y].transform.localPosition.z); // always let bar stands on plan


        //change color
        //Color32 curcolor = bar.GetComponent<MeshRenderer>().material.color;

        if (value >= 5000)
        {
            bar[x, y].GetComponent<MeshRenderer>().material.color = Color.red;
        }

        if (value < 5000)
        {
            change.a = change.r = 255;
            change.g = change.b = Convert.ToByte((1 - temp) * 255);
            //Debug.Log(change);
            bar[x, y].GetComponent<MeshRenderer>().material.color = change;
            //bar.GetComponent<MeshRenderer>().material.SetColor("_Color", change);
            // Debug.Log(bar[x, y].GetComponent<MeshRenderer>().material.color);
        }

    }

    public void setParent(int x, int y, GameObject parent) {
        bar[x, y].transform.parent = parent.transform;
    }

    public void Clear()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (bar[i, j] != null)
                {
                    UnityEngine.Object.Destroy(bar[i, j].gameObject);
                }
            }
        }
    }




    // just for test
    public void test_update(int x, int y)
    {
        Color32 curcolor;
        curcolor = bar[x, y].GetComponent<MeshRenderer>().material.color;

        //Debug.Log("color.b is :" + curcolor.b);
        if (curcolor.b == 255)
        {
            flag = DECREASE;
        }
        if (curcolor.b == 0)
        {
            flag = INCREASE;
        }

        if (flag == INCREASE)
        {
            bar[x, y].GetComponent<MeshRenderer>().material.color = curcolor + test;
        }

        if (flag == DECREASE)
        {
            bar[x, y].GetComponent<MeshRenderer>().material.color = curcolor - test;
        }
    }
}


