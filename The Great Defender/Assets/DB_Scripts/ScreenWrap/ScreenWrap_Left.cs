using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWrap_Left : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.transform.position = new Vector3(70f, other.gameObject.transform.position.y, 0);
        }
    }
}
