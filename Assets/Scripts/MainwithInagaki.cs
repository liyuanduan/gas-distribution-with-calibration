using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MainwithInagaki : MonoBehaviour
{

    Vector3 O_lmm_pos;
    Vector3 calibrator_pos;
    Vector3 O_plane_pos;

    bool buttonClicked = false;

    string button = "bar"; //default model is bar
    string lastButton = "bar";

    string lastGrid = "-10-10";
    string currentGrid = "-10-10";

    List<Vector3> offset_l = new List<Vector3>();
    List<float> offset_r_l = new List<float>();

    Vector3 offset = new Vector3(0, 0, 0); //position
    Vector3 offset_r = new Vector3(0, 0, 0); // rotation

    GameObject TargetPlane;
    GameObject ulmm;
    GameObject olmm;
    GameObject calibrator;
    GameObject ocalibrator;
    GameObject buttons;
    GameObject timeCounting;
    //GameObject Laser_Point; //GameObject Laser;

    BarCreate bars = new BarCreate();
    BallCreate balls = new BallCreate();
    CloudCreate clouds = new CloudCreate();

    public int flag = 5; //define by user,  times of calibration
    int time = 0;

    int csvtime = 0;
    string[] csvline = new string[9];

    List<int> Consentrations = new List<int>();

    private SensorUdp lmm = new SensorUdp();
    private Grid[,] gridArray = new Grid[10, 10];

    string NameToCheck = "Sphere"; // create ball as laser point

    Vector3 targetPlace;

    struct Line
    {
        public Vector3 P1;
        public Vector3 P2;
        public Double Length;
    }

    //bool calibrating = true;
    // Start is called before the first frame update
    void Awake()
    {
        ulmm = GameObject.Find("U_lmm");
        buttons = GameObject.Find("MixedRealityPlayspace/Buttons");

        calibrator = GameObject.Find("calibrator");
        calibrator_pos = calibrator.transform.position;

        olmm = GameObject.Find("Main/O_lmm");

        ocalibrator = GameObject.Find("Main/O_calibrator");

        timeCounting = GameObject.Find("MixedRealityPlayspace/Main Camera/TimeCounter");
        timeCounting.GetComponentInChildren<TimeCounter>().Initial();
        //timeCounting.GetComponentInChildren<TimeCounter>(); get time counter script

        lmm.Start();

        clouds.Initial();

        ulmm.SetActive(false);

        buttons.SetActive(false);

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                gridArray[i, j] = new Grid();
            }
        }



    }

    // Update is called once per frame
    void Update()
    {
        //calibration part
        if (time < flag)
        {
            
            if (calibrator.GetComponent<SpeechManager>().get_calibration())
            {
                if (time == 1 || time == 5)
                {
                    show_offset(time);
                }

                Vector3 temp_p = calibrator_pos - ocalibrator.transform.position;
                Debug.Log("temp pos offset: " + temp_p);
                Vector3 temp_r = calibrator.transform.eulerAngles - ocalibrator.transform.eulerAngles;
                if (temp_r.y <= -300)
                {
                    temp_r = new Vector3(temp_r.x, temp_r.y + 180, temp_r.z);
                }
                Debug.Log("temp rot offset: " + temp_r);

                offset_l.Add(temp_p);
                offset_r_l.Add(temp_r.y);

                calibrator.GetComponent<SpeechManager>().new_pos();
                calibrator.GetComponent<SpeechManager>().set_calibration(false);

                time += 1;
            }
        }
        else //start tracking
        {
            //create target plane, laser etc.
            if (!TargetPlane)
            {
                GameObject arrow = GameObject.Find("MixedRealityPlayspace/Chevron");
                arrow.SetActive(false);
                calibrator.SetActive(false);


                (offset_r_l, offset_l) = del_noise(offset_r_l, offset_l);

                //get average of offset_pos
                offset = new Vector3(offset_l.Average(x => x.x), offset_l.Average(x => x.y), offset_l.Average(x => x.z));
                //Debug.Log("offset pos: " + offset);

                //get average of offset_rot
                offset_r = new Vector3(0, offset_r_l.Average() + 180, 0);

                show_offset(flag);

                GameObject O_plane = GameObject.Find("O_plane");
                //Debug.Log("optitrack's plane rotation: " + O_plane.transform.eulerAngles);
                O_plane_pos = O_plane.transform.position;

                targetPlace = pos_transfer(O_plane_pos);
                Debug.Log("target place start pos: " + (O_plane_pos + offset));


                TargetPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                TargetPlane.transform.position = targetPlace;
                TargetPlane.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                TargetPlane.GetComponent<Renderer>().material.color = new Color32(177, 226, 163, 110); //new Color32(5, 39, 4, 255); //new Color32(177, 226, 163, 110);
                //TargetPlane.transform.rotation = Quaternion.identity;
                TargetPlane.transform.eulerAngles = offset_r;
                TargetPlane.tag = "TargetPlane";
                TargetPlane.name = "TargetPlane";

                Debug.Log("target place rotated pos: " + targetPlace);
                Debug.Log("target plane's rotation: " + TargetPlane.transform.eulerAngles);

                ulmm.SetActive(true);
                buttons.SetActive(true);
            }

            //gas distribute detection start
            else
            {
                //get gas concentration value
                var instance = lmm.GetValue();
                Consentrations = instance;
                //Debug.Log("consentrations: " + Consentrations.Count());
                lmm.Update();

                //let unity lmm follows optitrack lmm
                O_lmm_pos = olmm.transform.position;

                Vector3 rot = olmm.transform.eulerAngles;

                Vector3 lmm_new_pos = pos_transfer(O_lmm_pos);
                ulmm.transform.position = lmm_new_pos;
                ulmm.transform.eulerAngles = rot + offset_r;

                //Debug.Log("button clicked: " + buttonClicked);
                //Debug.Log("last button is: " + lastButton);
                //Debug.Log("now button is: " + button);


                //if button clicked

                if (buttonClicked)
                {
                    Debug.Log("last button is: " + lastButton);
                    Debug.Log("now button is: " + button);

                    buttonClicked = false;

                    if (lastButton != button)
                    {
                        Debug.Log("model change to: " + button);

                        //clear last models
                        switch (lastButton)
                        {
                            case "bar":
                                bars.Clear();
                                break;
                            case "ball":
                                balls.Clear();
                                break;
                            case "cloud":
                                clouds.Clear();
                                break;
                        }

                        if (button == "clear")
                        {
                            button = lastButton;
                        }

                        else //initial new models 
                        {
                            switch (button)
                            {
                                case "bar":
                                    for (int i = 0; i < 10; i++)
                                    {
                                        for (int j = 0; j < 10; j++)
                                        {
                                            if (gridArray[i, j].tracked)
                                            {
                                                bars.Start(i, j, gridArray[i, j].center, TargetPlane);
                                                bars.Update(Convert.ToInt32(gridArray[i, j].value.Average()), i, j);
                                            }
                                        }
                                    }
                                    break;
                                case "ball":
                                    for (int i = 0; i < 10; i++)
                                    {
                                        for (int j = 0; j < 10; j++)
                                        {
                                            if (gridArray[i, j].tracked)
                                            {
                                                balls.Start(i, j, gridArray[i, j].center, TargetPlane);
                                                balls.Update(Convert.ToInt32(gridArray[i, j].value.Average()), i, j);
                                            }
                                        }
                                    }
                                    break;
                                case "cloud":
                                    for (int i = 0; i < 10; i++)
                                    {
                                        for (int j = 0; j < 10; j++)
                                        {
                                            if (gridArray[i, j].tracked)
                                            {
                                                clouds.Start(i, j, gridArray[i, j].center, TargetPlane);
                                                clouds.Update(Convert.ToInt32(gridArray[i, j].value.Average()), i, j);
                                            }
                                        }
                                    }
                                    break;
                            }

                            lastButton = button;
                        }
                        
                        
                    }
                }

                //check detector points on target plane, and change the concentrate value on that position
                if (ulmm.GetComponentInChildren<Laser>().IsHitted())
                {
                    Vector3 pos = ulmm.GetComponentInChildren<Laser>().HitPos();
                    //Debug.Log("hitted pos: " + pos);
                    pos = PosOnPlane(pos); //find local position value on plane
                                           //Debug.Log("hitted pos on plane: " + pos);

                    float distance = ulmm.GetComponentInChildren<Laser>().getDistance();

                    ChangeGrid(pos, distance);

                }

                else timeCounting.GetComponentInChildren<TimeCounter>().Clear(); 
            }


        }

    }

    Vector3 pos_transfer(Vector3 p)
    {
        Vector3 result = p + offset;
        result = rotation_matrix(offset_r.y, result);
        
        /*
        Vector3 result = rotation_matrix(offset_r.y, p); 
        result = result + offset;*/
        return result;
    }

    List<Vector3> findPoints()
    {
        //Debug.Log("finding points");

        GameObject Oplane = GameObject.Find("Main/O_lmm");
        Vector3 Oplane_pos = Oplane.transform.position;

        List<Vector3> result = new List<Vector3>();

        List<Vector3> points = new List<Vector3>();
        //List<GameObject> spheres = new List<GameObject>();
        List<Line> order_l = new List<Line>();
        foreach (GameObject s in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (s.name == NameToCheck)
            {
                //spheres.Add(s);
                //points.Add(CoTrans(s.transform.position)); //position (x, y, z)
                points.Add(s.transform.position);
            }
        }

        if (points.Count() > 2)
        {
            foreach (Vector3 point in points)
            {
                Line l = new Line();
                l.P1 = Oplane_pos;
                l.P2 = point;
                l.Length = getLength(l.P1, l.P2);
                order_l.Add(l);
            }

            List<Line> sorted = order_l.OrderBy(o => o.Length).ToList();

            for (int i = 0; i < 3; i++)
            {
                result.Add(sorted[i].P2);
            }
        }

        return result;

    }

    double getLength(Vector3 p1, Vector3 p2)
    {
        return Math.Sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y) + (p1.z - p2.z) * (p1.z - p2.z));
    }


    /*---------------------------------------------------
     * Find the higher point in lmm's 3 points
     * Input: a sorted list of 3 lines, higher point is common point in 2 shortest lines
     * Output: higher point's vector 3 position
     ----------------------------------------------------*/
    Vector3 FindHigh(List<Line> sorted)
    {
        List<Vector3> check_duplicate = new List<Vector3>();
        List<Vector3> source = new List<Vector3>() { sorted[1].P1, sorted[1].P2, sorted[0].P1, sorted[0].P2 };
        Vector3 high = new Vector3(0, 0, 0);

        foreach (Vector3 p in source)
        {
            if (check_duplicate.Contains(p))
            {
                high = p;
                break;
            }
            else
            {
                check_duplicate.Add(p);
            }
        }
        return high;
    }


    /*----------------------------------------------------
     * Find the lower point in lmm's 3 points
     * Input: the higher point's vector 3 position, the line connects higher and lower point
     * Output: the lower point's vector 3 position
     -----------------------------------------------------*/
    Vector3 FindLow(Vector3 high, Line line)
    {
        if (line.P1 != high)
        {
            return line.P1;
        }
        else
        {
            return line.P2;
        }
    }

    /*------------------------------------------------------
     * Let a vector3 pos rotate around y axis with theta degree
     * Input: the double degree needs will rotate, the vector3 pos which will be ratoted around y axis
     * Output: vector 3 postion after rotation
     -------------------------------------------------------*/
    Vector3 rotation_matrix(Double theta, Vector3 pos)
    {
        Vector3 result;

        theta = Math.PI * theta / 180.0;

        Double x = Math.Cos(theta) * pos.x + Math.Sin(theta) * pos.z;
        Double y = pos.y;
        Double z = Math.Cos(theta) * pos.z - Math.Sin(theta) * pos.x;

        result = new Vector3(Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(z));
        return result;
    }

    void show_offset(int time)
    {
        Vector3 temp_p = new Vector3(offset_l.Average(x => x.x), offset_l.Average(x => x.y), offset_l.Average(x => x.z));
        Vector3 temp_r = new Vector3(0, offset_r_l.Average() + 180, 0);
        Debug.Log("calibrate times: " + time + "with offset_p: " + temp_p + ", and offset_r: " + temp_r);
    }

    /*------------------------------------------------------
     * Find out the error rotation in list with range from mean - std to mean + std
     * Input: list of rotation y 
     * Output: list of rotation y
     -------------------------------------------------------*/
    private (List<float> r, List<Vector3> p) del_noise(List<float> r, List<Vector3> p)
    {
        List<float> result_r = new List<float>();
        List<Vector3> result_p = new List<Vector3>();

        double mean = r.Average();
        double sum_std = r.Sum(d => (d - mean) * (d - mean));
        double std = Math.Sqrt(sum_std / r.Count());


        for (int i = 0; i < flag; i++)
        {
            if ((r[i] <= mean + std) && (r[i] >= mean - std))
            {
                result_r.Add(r[i]);
                result_p.Add(p[i]);
            }
        }

        return (result_r, result_p);
    }

    /*------------------------------------------------------
     * Find where lmm is pointing on real (optitrack's) plane
     * Input: higher point's vector 3 position, lower point's vector3 postion
     * Output: vector 3 postion of laser point
     -------------------------------------------------------*/
    Vector3 opti_LaserPosition(Vector3 high, Vector3 low)
    {
        Vector3 result = new Vector3(0, 0, 0);

        result.y = O_plane_pos.y;
        result.x = (result.y - low.y) * (high.x - low.x) / (high.y - low.y) + low.x;
        result.z = (result.y - low.y) * (high.z - low.z) / (high.y - low.y) + low.z;

        return result;
    }


    /*-------------------------------------------------------
     * Check lmm is pointing on real (optitrak's) plane
     * Input: laser point's vector 3 position
     * Output: True for is on plane
     --------------------------------------------------------*/
    bool IsonPlane(Vector3 pos)
    {
        if ((pos.x >= O_plane_pos.x - 1) & (pos.x <= O_plane_pos.x + 1) & (pos.z >= O_plane_pos.z - 1) & (pos.z <= O_plane_pos.z + 1))
        {
            return true;
        }
        return false;
    }

    Vector3 PosOnPlane(Vector3 pos)
    {
        return TargetPlane.transform.InverseTransformPoint(pos);
    }

    /*----------------------------------------------------
     * Get latest density value from lmm
     * Output: return latest density value
     ------------------------------------------------------*/
    int getValue()
    {
        int value = 0;
        //Debug.Log("# of consentrations " + Consentrations.Count);
        if (Consentrations.Count >= 1)
        {
            value = Consentrations[Consentrations.Count - 1];
        }

        return value;
    }

    /*-------------------------------------------------------
     * Change the density value of pointed grid, create or update this grid's bar model
     * Input: laser point's vector3 position
     --------------------------------------------------------*/
    void ChangeGrid(Vector3 pos, float distance)
    {
        if (pos.y == 0)
        {
            //Debug.Log("grid changing!1");
            var value = getValue();
            float ppm = value / distance;
            //Debug.Log("lmm's value: " + value);

            if (ppm > 0) 
            {
                //---------csv part start
                csvline[0] = csvtime.ToString();
                csvtime += 1;
                csvline[1] = pos.x.ToString();
                csvline[2] = pos.y.ToString();
                csvline[3] = pos.z.ToString();
                csvline[4] = ulmm.transform.eulerAngles.x.ToString();
                csvline[5] = ulmm.transform.eulerAngles.y.ToString();
                csvline[6] = ulmm.transform.eulerAngles.z.ToString();
                csvline[7] = ppm.ToString();
                csvline[8] = distance.ToString();


                //----------csv part end

                var v1 = FindGridIndex(pos.x);
                var v2 = FindGridIndex(pos.z);

                currentGrid = v1.ToString() + v2.ToString();
                //Debug.Log("current grid: " + currentGrid);

                if (currentGrid != lastGrid)
                {
                    lastGrid = currentGrid;
                    timeCounting.GetComponentInChildren<TimeCounter>().Start();
                }
                timeCounting.GetComponentInChildren<TimeCounter>().Counting();

                //Debug.Log("v1: " + v1 + "; v2: " + v2);
                gridArray[v1, v2].value.Add(ppm);

                switch (button)
                {
                    case "bar":
                        if (gridArray[v1, v2].tracked == false)
                        {
                            gridArray[v1, v2].tracked = true;
                            gridArray[v1, v2].center = GetCenter(v1, v2);
                            gridArray[v1, v2].changePlane(TargetPlane);
                            bars.Start(v1, v2, gridArray[v1, v2].center, TargetPlane);
                        }

                        bars.Update(ppm, v1, v2);
                        
                        break;
                    case "ball":
                        if (gridArray[v1, v2].tracked == false)
                        {
                            gridArray[v1, v2].tracked = true;
                            gridArray[v1, v2].center = GetCenter(v1, v2);
                            gridArray[v1, v2].changePlane(TargetPlane);
                            balls.Start(v1, v2, gridArray[v1, v2].center, TargetPlane);
                        }

                        balls.Update(ppm, v1, v2);
                        
                        break;
                    case "cloud":
                        if (gridArray[v1, v2].tracked == false)
                        {
                            gridArray[v1, v2].tracked = true;
                            gridArray[v1, v2].center = GetCenter(v1, v2);
                            gridArray[v1, v2].changePlane(TargetPlane);
                            clouds.Start(v1, v2, gridArray[v1, v2].center, TargetPlane);
                        }

                        clouds.Update(ppm, v1, v2);
                        break;
                }



                

            }
        }
        
    }

    /*----------------------------------------------------------
     * Find the index of grid this value should be (child's position on parent)
     * Input: float value (could be child position's x or z), center of parent is (0,0)
     * Output: int index value
     -----------------------------------------------------------*/
    int FindGridIndex(float v)
    {
        var i = (int)Math.Floor(v);
        if (i >= 0)
        {
            return i;
        }
        return 4 - i;
    }

    /*-----------------------------------------------------------
     * Get grid's center point
     * Intput: grid's index for x axis, grid's index for z axis
     * Output: center's vector 3 position
     ------------------------------------------------------------*/
    Vector3 GetCenter(int v1, int v2)
    {
        float x = GetSide(v1);
        float z = GetSide(v2);

        return new Vector3(x, 0, z);
    }


    /*-------------------------------------------------------------
     * Get this grid's start position
     * Input: grid's index v, center of target place's x or z axis
     * Output: float position number on axis
     --------------------------------------------------------------*/
    float GetSide(int v)
    {
        if (v <= 4)
        {
            return v + 0.5f;
        }

        return 4.5f - v;
    }
    /*
    int grid_value(int initial, List<int>values)
    {


        List<int> result_r = new List<int>();
        double mean = values.Average();
        double sum_std = r.Sum(d => (d - mean) * (d - mean));
        double std = Math.Sqrt(sum_std / r.Count());


        for (int i = 0; i < flag; i++)
        {
            if ((r[i] <= mean + std) && (r[i] >= mean - std))
            {
                result_r.Add(r[i]);
            }
        }
        return 0; 
    }
    */

    public void Cloud_Button_click()
    {

        buttonClicked = true;
        button = "cloud";

        Debug.Log("cloud clicked");
    }

    public void Ball_Button_click()
    {

        buttonClicked = true;
        button = "ball";

        Debug.Log("ball clicked");
    }

    public void Bar_Button_click()
    {

        buttonClicked = true;
        button = "bar";

        Debug.Log("bar clicked");
    }

    public void Clear_Button_click()
    {

        buttonClicked = true;
        button = "clear";

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (gridArray[i, j].tracked)
                {
                    gridArray[i, j].clear();
                }
            }
        }

        Debug.Log("clear clicked");
    }

}

