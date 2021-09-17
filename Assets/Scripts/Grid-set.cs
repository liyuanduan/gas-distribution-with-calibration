using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public Vector3 center;
    public List<float> value;
    public bool tracked;
    public int initial;
    private GameObject plane;

    public Grid() 
    {
        this.center = new Vector3(0, 0, 0);
        this.value = new List<float>();
        this.tracked = false;
        this.initial = 0;
    }

    public void changePlane(GameObject parent)
    {
        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.SetParent(parent.transform, false);
        plane.transform.localPosition = new Vector3(this.center.x, 1/5100, this.center.z);
        plane.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        plane.GetComponent<Renderer>().material.color = new Color32(5, 39, 4, 255);
        plane.layer = 2;
    }

    public void clear()
    {
        this.center = new Vector3(0, 0, 0);
        this.value = new List<float>();
        this.tracked = false;
        this.initial = 0;
        Object.Destroy(this.plane);
    }
}
