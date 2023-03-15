using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using UnityEngine.Rendering;
using DG.Tweening;

public class SensingController : MonoBehaviour
{
    public float inputIndexMin;
    public float inputIndexMax;
    public float inputThumbMin;
    public float inputThumbMax;
    public float outputMin = -95;
    public float outputMax = -21;
    public float outputScaleMin = 0.01f;
    public float outputScaleMax = 0.1f;
    public float thumbRotationMin;
    public float thumbRotationMax;

    public bool thumb = false;

    public GameObject indexJoint2;
    public GameObject thumbJoint3;
    public GameObject pressureBall;

    // change your serial port
    public SerialPort sp;

    private float _resistance = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!thumb)
        {
           sp = new SerialPort("COM8", 115200);
        } else
        {
           sp = new SerialPort("COM5", 115200);
        }

        sp.Open();
        sp.ReadTimeout = 100; // In my case, 100 was a good amount to allow quite smooth transition. 


    }

    // Update is called once per frame
    void Update()
    {
        if (sp.IsOpen)
        {
            try
            {
                if (sp.BytesToRead > 0)
                {
                    string resistanceString = sp.ReadLine().Trim();
                    print(resistanceString);
                    _resistance = float.Parse(resistanceString);
                    //print("WHAWHA");

                    if (thumb)
                    {
                        thumbJoint3.transform.localRotation = Quaternion.Euler(0, 0, MapThumbValue(_resistance));
                    } else
                    {
                        // Rotate index joint
                        //indexJoint2.transform.localRotation = Quaternion.Euler(0, 0, MapValue(_resistance));
                        indexJoint2.transform.DOLocalRotate(new Vector3(0, 0, MapValue(_resistance)), 0.2f);
                    }

                    

                    // Scale pressure ball
                    float newScale = MapScaleValue(_resistance);
                    pressureBall.transform.localScale = new Vector3(newScale, newScale, newScale);
                }
            }
            catch (System.Exception)
            {

            }


            
                
                
    

        }
    }

    private float MapValue(float input)
    {
        float output = ((input - inputIndexMin) / (inputIndexMax - inputIndexMin)) * (outputMax - outputMin) + outputMin;

        
        if (output < outputMin)
        {
            output = outputMin;
        }

        if (output > outputMax)
        {
            output = outputMax;
        }
        
        
        return output;
    }

    private float MapScaleValue(float input)
    {
        float output;
        if (!thumb)
        {
            output = ((input - inputIndexMin) / (inputIndexMax - inputIndexMin)) * (outputScaleMax - outputScaleMin) + outputScaleMin;
        } else
        {
            output = ((input - inputThumbMin) / (inputThumbMax - inputThumbMin)) * (outputScaleMax - outputScaleMin) + outputScaleMin;
        }
        

        if(output < outputScaleMin)
        {
            output = outputScaleMin;
        }

        if (output > outputScaleMax)
        {
            output = outputScaleMax;
        }

        return output;
    }

    private float MapThumbValue(float input)
    {
        float output = ((input - inputThumbMin) / (inputThumbMax - inputThumbMin)) * (thumbRotationMax - thumbRotationMin) + thumbRotationMin;

        if (output < thumbRotationMin)
        {
            output = thumbRotationMin;
        }

        if (output > thumbRotationMax)
        {
            output = thumbRotationMax;
        }

        return output;
    }

    public float ConvertStringToFloat(string stringValue)
    {
        float floatValue;
        if (float.TryParse(stringValue, out floatValue))
        {
            return floatValue;
        }
        else
        {
            Debug.LogError("Invalid string value: " + stringValue);
            return 0.0f; // or any other default value you want to use
        }
    }
}
