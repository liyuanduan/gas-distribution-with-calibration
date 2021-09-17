using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CloudCreate
{
    ParticleSystem[,] system = new ParticleSystem[10, 10];
    Texture[] textures = new Texture[11];
    //int change;
    //const int Increase = 1;
    //const int Decrease = -1;
    //int index = 0;
    Material particleMaterial;

    //smoke particle set

    public void Initial() 
    {
        // material with basic texture.
        textures[0] = Resources.Load<Texture2D>("Textures/Smoke-0");
        textures[1] = Resources.Load<Texture2D>("Textures/Smoke-10");
        textures[2] = Resources.Load<Texture2D>("Textures/Smoke-20");
        textures[3] = Resources.Load<Texture2D>("Textures/Smoke-30");
        textures[4] = Resources.Load<Texture2D>("Textures/Smoke-40");
        textures[5] = Resources.Load<Texture2D>("Textures/Smoke-50");
        textures[6] = Resources.Load<Texture2D>("Textures/Smoke-60");
        textures[7] = Resources.Load<Texture2D>("Textures/Smoke-70");
        textures[8] = Resources.Load<Texture2D>("Textures/Smoke-80");
        textures[9] = Resources.Load<Texture2D>("Textures/Smoke-90");
        textures[10] = Resources.Load<Texture2D>("Textures/Smoke-100");
        particleMaterial = new Material(Shader.Find("Legacy Shaders/Particles/Additive (Soft)"));
        particleMaterial.mainTexture = textures[0];
    }
    //Assets/Resources/Textures/Smoke-0.png

    public void Start(int x, int y, Vector3 position, GameObject parent)
    {
        if (system[x, y] == null)
        {
            var go = new GameObject("Particle System");
            go.transform.SetParent(parent.transform, false);
            go.transform.Rotate(-90, 0, 0); // Rotate so the system emits upwards.
            go.transform.localPosition = new Vector3(position.x, 0, position.z); // position of cloud
            system[x, y] = go.AddComponent<ParticleSystem>();
            system[x, y].Stop();
            go.GetComponent<ParticleSystemRenderer>().material = particleMaterial;
        }

       
        //basic setting
        var mainModule = system[x, y].main;
        mainModule.duration = 1;
        mainModule.loop = true;
        mainModule.startDelay = 0;
        mainModule.startLifetime = 1;
        mainModule.startColor = Color.white;
        mainModule.startSpeed = 1;
        mainModule.startSize = 1;
        mainModule.scalingMode = ParticleSystemScalingMode.Shape;
        mainModule.cullingMode = ParticleSystemCullingMode.PauseAndCatchup;
        mainModule.maxParticles = 100;
        //emission
        var emission = system[x, y].emission;
        emission.rateOverTime = 100;
        //shape
        var shape = system[x, y].shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 3;
        shape.radius = 0.1f;
        shape.arcSpread = 1;
        //color over lifetime
        var col = system[x, y].colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
        col.color = grad;
        //size over lifetime
        var sol = system[x, y].sizeOverLifetime;
        sol.enabled = true;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0.0f, 0.0f);
        curve.AddKey(1.0f, 1.0f);
        sol.size = new ParticleSystem.MinMaxCurve(1.0f, curve);
        //size by speed
        var sbs = system[x, y].sizeBySpeed;
        sbs.enabled = true;
        AnimationCurve curve1 = new AnimationCurve();
        curve1.AddKey(0.0f, 0.0f);
        curve1.AddKey(1.0f, 1.0f);
        sbs.size = new ParticleSystem.MinMaxCurve(1.0f, curve1);
        sbs.range = new Vector2(0, 1);
        //rotation over lifetime
        var rol = system[x, y].rotationOverLifetime;
        rol.enabled = true;
        
        //system[x, y].GetComponent<Rigidbody>().maxAngularVelocity = 35;
        AnimationCurve minCurve = new AnimationCurve();
        minCurve.AddKey(0.0f, 35.0f);
        minCurve.AddKey(1.0f, 35.0f);

        AnimationCurve maxCurve = new AnimationCurve();
        maxCurve.AddKey(0.0f, 0.0f);
        maxCurve.AddKey(1.0f, 35.0f);
        rol.z = new ParticleSystem.MinMaxCurve(1.0f, minCurve, maxCurve);

        system[x, y].Play();
    }

    //update v2: change cloud by changing particle system start color
    public  void Update(float value, int x, int y)
    {
        float temp = value / 5000.0f;

        //change size and position
        float height = temp * 2.5f;
        system[x, y].transform.localScale = new Vector3(0.1f, height, 0.1f); // change height of bar
        //system[x, y].transform.localPosition = new Vector3(system[x, y].transform.localPosition.x, height / 2, system[x, y].transform.localPosition.z); // always let bar stands on plan

        var mainModule = system[x, y].main;
        if (value >= 5000)
        {
            mainModule.startColor = Color.red;
            var col = system[x, y].colorOverLifetime;
            col.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
            col.color = grad;
        }
        if (value < 5000)
        {
            Color32 change = new Color32(255, 0, 0, 255); //(r, g, b, a)
            change.a = change.r = 255;
            change.g = change.b = Convert.ToByte((1 - temp) * 255);
            Color c = change;
            mainModule.startColor = c;


            float gradiant = temp * 0.9f + 0.1f;
            var col = system[x, y].colorOverLifetime;
            col.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(new GradientColorKey[] { new GradientColorKey(c, 0.0f), new GradientColorKey(c, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, gradiant) });
            col.color = grad;
        }
    }

    public void Clear()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (system[i, j] != null)
                {
                    system[i, j].Stop();
                    // UnityEngine.Object.Destroy(system[i, j].gameObject);
                }
            }
        }
    }
    //change texture
    /*public void Update(int value, int x, int y)
    {
        float s_temp = (float)value / 5000.0f;
        //float size = s_temp * 2.5f;
        var main = system[x, y].GetComponent<ParticleSystem>().main;
        main.scalingMode = ParticleSystemScalingMode.Local;

        if (value > 5000) //limit = 5000
        {
            float size = s_temp * 2.5f;
            system[x, y].GetComponent<ParticleSystemRenderer>().material.mainTexture = textures[10];
            system[x, y].GetComponent<ParticleSystem>().transform.localScale = new Vector3(size, size, size);
            system[x, y].GetComponent<ParticleSystem>().transform.localPosition = new Vector3(system[x, y].GetComponent<ParticleSystem>().transform.localPosition.x, size / 2, system[x, y].GetComponent<ParticleSystem>().transform.localPosition.z);

        }
        else
        {
            int temp = (int)Math.Floor((float)value / 500.0f); // use 500 as index seperator
            system[x, y].GetComponent<ParticleSystemRenderer>().material.mainTexture = textures[temp];
            system[x, y].GetComponent<ParticleSystem>().transform.localScale = new Vector3(1, 1, 1);
            system[x, y].GetComponent<ParticleSystem>().transform.localPosition = new Vector3(system[x, y].GetComponent<ParticleSystem>().transform.localPosition.x, 1 / 2, system[x, y].GetComponent<ParticleSystem>().transform.localPosition.z);
        }
    }


    //fire ball particle set
    public void Initial()
    {
        // material with basic texture.
        textures[10] = Resources.Load<Texture2D>("Textures/fx_fire_ball_red_0");
        textures[9] = Resources.Load<Texture2D>("Textures/fx_fire_ball_red-10");
        textures[8] = Resources.Load<Texture2D>("Textures/fx_fire_ball_red-20");
        textures[7] = Resources.Load<Texture2D>("Textures/fx_fire_ball_red-30");
        textures[6] = Resources.Load<Texture2D>("Textures/fx_fire_ball_red-40");
        textures[5] = Resources.Load<Texture2D>("Textures/fx_fire_ball_red-50");
        textures[4] = Resources.Load<Texture2D>("Textures/fx_fire_ball_red-60");
        textures[3] = Resources.Load<Texture2D>("Textures/fx_fire_ball_red-70");
        textures[2] = Resources.Load<Texture2D>("Textures/fx_fire_ball_red-80");
        textures[1] = Resources.Load<Texture2D>("Textures/fx_fire_ball_red-90");
        textures[0] = Resources.Load<Texture2D>("Textures/fx_fire_ball_red-100");
        particleMaterial = new Material(Shader.Find("Legacy Shaders/Particles/Additive (Soft)"));
        particleMaterial.mainTexture = textures[0];
    }

    //Assets/Resources/Textures/fx_fire_ball_red-10.png

    public void Start(int x, int y, Vector3 position, GameObject parent)
    {

        // Create a green Particle System.
        var go = new GameObject("Particle System");
        go.transform.SetParent(parent.transform, false);
        go.transform.Rotate(-90, 0, 0); // Rotate so the system emits upwards.
        go.transform.localPosition = new Vector3(position.x, 1, position.z); // position of cloud
        system[x, y] = go.AddComponent<ParticleSystem>();
        system[x, y].Stop();
        go.GetComponent<ParticleSystemRenderer>().material = particleMaterial;
        var mainModule = system[x, y].main;
        //mainModule.startColor = Color.green;
        mainModule.startSize = 1; //change depends on import data
        mainModule.duration = 0.1f;
        mainModule.startLifetime = 0.5f;
        mainModule.startSpeed = 0;
        mainModule.scalingMode = ParticleSystemScalingMode.Shape;
        mainModule.cullingMode = ParticleSystemCullingMode.PauseAndCatchup;
        var emission = system[x, y].emission;
        emission.rateOverTime = 15;
        var shape = system[x, y].shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.01f;
        var TextureSheetAnimation = system[x, y].textureSheetAnimation;
        TextureSheetAnimation.enabled = true;
        TextureSheetAnimation.mode = ParticleSystemAnimationMode.Grid;
        TextureSheetAnimation.numTilesX = 4;
        TextureSheetAnimation.numTilesY = 4;

        system[x, y].Play();
    }*/

    //update v1: change cloud by changing texture
    /*public void Update(int value, int x, int y)
    {
        float s_temp = (float)value / 5000.0f;
        //float size = s_temp * 2.5f;
        var main = system[x, y].GetComponent<ParticleSystem>().main;
        main.scalingMode = ParticleSystemScalingMode.Local;
       
        if (value > 5000) //limit = 5000
        {
            float size = s_temp * 0.5f;
            system[x, y].GetComponent<ParticleSystemRenderer>().material.mainTexture = textures[10];
            system[x, y].GetComponent<ParticleSystem>().transform.localScale = new Vector3(0.1f, size, 0.1f);
            system[x, y].GetComponent<ParticleSystem>().transform.localPosition = new Vector3(system[x, y].GetComponent<ParticleSystem>().transform.localPosition.x, 0, system[x, y].GetComponent<ParticleSystem>().transform.localPosition.z);

        }
        else
        {
            int temp = (int)Math.Floor((float)value / 500.0f); // use 500 as index seperator
            system[x, y].GetComponent<ParticleSystemRenderer>().material.mainTexture = textures[temp];
            system[x, y].GetComponent<ParticleSystem>().transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            system[x, y].GetComponent<ParticleSystem>().transform.localPosition = new Vector3(system[x, y].GetComponent<ParticleSystem>().transform.localPosition.x, 0, system[x, y].GetComponent<ParticleSystem>().transform.localPosition.z);
        }

    }*/

    /*
    public void test_update()
    {
        if (index == 0)
        {
            change = Increase;
        }
        if (index == 10)
        {
            change = Decrease;
        }

        index += change;
        //system[x, y].GetComponent<ParticleSystemRenderer>().material.mainTexture = textures[index];
    }*/
}
