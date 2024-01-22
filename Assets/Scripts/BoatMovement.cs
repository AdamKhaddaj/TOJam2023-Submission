using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    public float hoverspeed, rotationintensity;
    public Transform boat;

    public Sprite fullhealthboat, twohealthboat, onehealthboat;

    // Update is called once per frame
    void Update()
    {
        float hover = -0.85f + Mathf.Sin(Time.time * hoverspeed) * 0.4f;
        boat.position = new Vector3(boat.position.x, hover*1.3f, boat.position.z);
        float rotation = Mathf.Sin(Time.time) * rotationintensity;
        boat.rotation = Quaternion.Euler(boat.rotation.x, boat.rotation.y, boat.rotation.z + rotation);
    }

    public void UpdateShipSprite()
    {
        if(GameManager.instance.structure == 3)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = fullhealthboat;
        }
        if (GameManager.instance.structure == 2)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = twohealthboat;
        }
        if (GameManager.instance.structure == 1)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = onehealthboat;
        }
    }
}
