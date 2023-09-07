using System;
using System.Collections;// useful?
using System.Collections.Generic; // useful?
using System.IO.Ports;// useful?

using UnityEngine;
using UnityEngine.Rendering; // useful?

using DG.Tweening;

public class SensingController : MonoBehaviour
{
    public GameObject indexJoint2;
    public GameObject hand;

    public SerialPort sp;


    void Start() {
        sp = new SerialPort("/dev/ttyACM0", 115200);
        sp.Open();
        sp.ReadTimeout = 100;
    }


    // Update is once per frame
    void Update() {
        if (sp.IsOpen) {
            try {
                if (sp.BytesToRead > 0) {
                    // touch, mx,my,mz, ax,ay,az
                    sp.ReadLine(); // drop a packet (MCU is too fast)
                    sp.ReadLine(); // drop a packet (MCU is too fast)
                    string[] data = sp.ReadLine().Split(' ');

                    float touchVal = float.Parse(data[0]);
                    bool isTouching = (touchVal > 5.0);

                    // use touch to change color:
                    SkinnedMeshRenderer handSkin = hand.transform.GetComponentInChildren<SkinnedMeshRenderer>();
                    if (isTouching)
                        handSkin.material.SetColor( "_BaseColor", Color.red);
                    else
                        handSkin.material.SetColor( "_BaseColor", Color.black);

                    // Bend index finger using the accelerometer
                    float bending = float.Parse(data[5]); // temporary test: accelerometer y
                    float index_angle = map(bending, -20,-70, -10,-70);
                    indexJoint2.transform.DOLocalRotate( new Vector3(0, 0, index_angle), 0.6f);

                    // Rotate hand (heading) using the magnetometer with atan2(y,x):
                    float heading = (float)Math.Atan2(double.Parse(data[2]), double.Parse(data[1]));
                    heading = (heading * 180.0f) / (float)Math.PI; // => [-180 ; +180]
                    float hand_angle = map(heading, 0,50, 130,190);
                    hand.transform.DOLocalRotate( new Vector3(0, hand_angle, 0), 0.8f);

                    //print(bending + "\t  " + heading);
                }
            } catch (System.Exception) {
                print("Error");
            }
        }
    }


    private float map(float input , float in_min,  float in_max,
                                    float out_min, float out_max) {

        float output = ((input - in_min) / (in_max - in_min))
                     * (out_max - out_min) + out_min;

        return output;
    }
}
