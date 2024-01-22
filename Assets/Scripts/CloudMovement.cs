using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CloudMovement : MonoBehaviour
{
    public float horizontal_oscilate_speed, horizontalspeed, phase_y, phase_x, horizontal_oscilate_amount;
    private float initx, inity, offset;
    private Transform cloud;

    private void Start()
    {
        cloud = gameObject.transform;
        initx = cloud.position.x;
        inity = cloud.position.y;
        offset = Random.Range(-1f, 1f);

    }

    void Update()
    {
        if (cloud.position.x <= -15)
        {
            cloud.position = new Vector3(15, cloud.position.y, cloud.position.z);
        }
        else
        {

            //MOVING STRAIGHT LEFT PORTION
            float leftmove = -horizontalspeed * Time.deltaTime;

            //OSCILATE LEFT AND RIGHT PORTION
            float horizontal_oscilation = horizontal_oscilate_amount* Mathf.Sin((Time.time+offset) * horizontal_oscilate_speed + phase_x) -0.01f;

            //OSCILATE UP AND DOWN PORTION
            float vertical_oscilation = Mathf.Sin((Time.time+offset) * horizontal_oscilate_speed + phase_y) - 0.01f;

            //TOTAL HORIZONTAL TRANSLATION
            float totalhorizontal = leftmove + horizontal_oscilation;

            //APPLY TO CLOUD
            cloud.position = new Vector3(cloud.position.x, inity + vertical_oscilation, cloud.position.z);
            cloud.Translate(totalhorizontal, 0, 0);

        }
    }
}
