using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    private LineRenderer lr;
    private Vector3 pos;
    private bool hitted = false;
    private float distance;

    // Start is called before the first frame update
    void Start()
    {
        lr = gameObject.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.material.SetColor("_Color", Color.red);
        lr.widthMultiplier = 0.001f;
        lr.positionCount = 2;


    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0, transform.position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider.tag == "TargetPlane")
            {
                lr.SetPosition(1, hit.point);
                hitted = true;
                pos = hit.point;
                distance = hit.distance - 0.18f / 2;
            }
        }
        else
        {
            lr.SetPosition(1, transform.forward * 3 + transform.position);
            hitted = false;
        }
    }

    public bool IsHitted()
    {
        return hitted;
    }

    public Vector3 HitPos()
    {
        return pos;
    }

    public float getDistance()
    {
        return distance;
    }
}
