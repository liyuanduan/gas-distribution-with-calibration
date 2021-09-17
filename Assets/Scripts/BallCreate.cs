using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BallCreate
{
    const int INCREASE = 1;
    const int DECREASE = -1;
    int flag;
    Color32 change = new Color32(255, 0, 0, 255); //(r, g, b, a)


    GameObject[,] ball = new GameObject[10, 10];

    // Start is called before the first frame update
    public void Start(int x, int y, Vector3 position, GameObject parent) //(Object o)
    {
        //create bar
        ball[x, y] = new GameObject();
        ball[x, y] = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        ball[x, y].transform.SetParent(parent.transform, false);

        ball[x, y].transform.localPosition = position;
        //center_y = position.y;
        var renderer = ball[x, y].GetComponent<MeshRenderer>();
        renderer.material.SetColor("_Color", Color.red);
        ball[x, y].transform.localScale = new Vector3(1, 1, 1);
        ball[x, y].layer = 2;
    }

    // Update is called once per frame
    public void Update(float value, int x, int y)
    {
        //Debug.Log(value);

        float temp = value / 5000.0f;

        //change size and position
        float height = temp * 2.5f;
        ball[x, y].transform.localScale = new Vector3(height, height, height); // change height of bar
        ball[x, y].transform.localPosition = new Vector3(ball[x, y].transform.localPosition.x, height / 2, ball[x, y].transform.localPosition.z); // always let bar stands on plan


        //change color
        //Color32 curcolor = bar.GetComponent<MeshRenderer>().material.color;

        if (value >= 5000)
        {
            ball[x, y].GetComponent<MeshRenderer>().material.color = Color.red;
        }

        if (value < 5000)
        {
            change.a = change.r = 255;
            change.g = change.b = Convert.ToByte((1 - temp) * 255);
            //Debug.Log(change);
            ball[x, y].GetComponent<MeshRenderer>().material.color = change;
            //bar.GetComponent<MeshRenderer>().material.SetColor("_Color", change);
            //Debug.Log(ball[x, y].GetComponent<MeshRenderer>().material.color);
        }

    }

    public void Clear()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (ball[i, j] != null)
                {
                    UnityEngine.Object.Destroy(ball[i, j].gameObject);
                }
            }
        }
    }




    /*public void Start()
    {
        //create ball
        ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.transform.position = new Vector3(0.2f, 0.1f, 0);
        ball.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        ball.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }*/


    /*public void test_update()
    {
        Color32 curcolor;
        curcolor = ball.GetComponent<MeshRenderer>().material.color;

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
            ball.GetComponent<MeshRenderer>().material.color = curcolor + change;
        }

        if (flag == DECREASE)
        {
            ball.GetComponent<MeshRenderer>().material.color = curcolor - change;
        }
    }*/
}
