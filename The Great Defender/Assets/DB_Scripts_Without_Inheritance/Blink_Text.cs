using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script is applied to a TextMesh GameObject. in which will flicker. This is used for when an enemy dies a int value of points will be shown on the GameObject. 
/// </summary>
public class Blink_Text : MonoBehaviour
{
    // Variables
    public float animSpeedInSec = 1f;           // how fast the gameObject alpha value will change 
    bool keepAnimating = false;                 // Boolean flag to say when to animate

    public void Update()
    {
        startTextMeshAnimation();       // Call Function
    }
    private IEnumerator anim()
    {
        var textMesh = GetComponent<TextMesh>();        // Make a variable to find the TextMesh On this gameObject
        Color currentColor = textMesh.color;            // Variable for the color element on a TextMesh
        Color invisibleColor = textMesh.color;          // Another color variable to declare when not seen
        invisibleColor.a = 0;                           // Alpha = 0 Invisible variable used when the text isnt seen

        float oldAnimSpeedInSec = animSpeedInSec;       // Variable for the last Animation speed which will equal to public inspector variable speed
        float counter = 0;                              // Counter variable used in while loop
        while (keepAnimating)                           // When Animating gameObject is true 
        {
            while (counter < oldAnimSpeedInSec)         // Slowly lower the alpha to hide text
            {
                if (!keepAnimating)                     // When the Boolean flag is false
                { 
                    yield break;                        // return
                }

                //Reset counter when Speed is changed
                if (oldAnimSpeedInSec != animSpeedInSec)
                {
                    counter = 0;
                    oldAnimSpeedInSec = animSpeedInSec;
                }
                counter += Time.deltaTime;              // Counter number variable adds over time
                textMesh.color = Color.Lerp(currentColor, invisibleColor, counter / oldAnimSpeedInSec);     // The current color of the gameObjects TextMesh is currentColor invisiblecolor and counter divided by the old animation speed 
                yield return null;                  // Return as 0

            }
            yield return null;

            while (counter > 0)                     //Show TextMesh text Slowly
            {
                if (!keepAnimating)
                {
                    yield break;
                }
                //Reset counter when Speed is changed
                if (oldAnimSpeedInSec != animSpeedInSec)
                {
                    counter = 0;
                    oldAnimSpeedInSec = animSpeedInSec;
                }
                counter -= Time.deltaTime;          // Counter decrease with time
                textMesh.color = Color.Lerp(currentColor, invisibleColor, counter / oldAnimSpeedInSec);
                yield return null;
            }
        }
    }

    //Call to Start animation
    void startTextMeshAnimation()
    {
        if (keepAnimating)
        {
            return;
        }
        keepAnimating = true;
        StartCoroutine(anim());
    }

    //Call to Change animation Speed
    void changeTextMeshAnimationSpeed(float animSpeed)
    {
        animSpeedInSec = animSpeed;
    }

    //Call to Stop animation
    void stopTextMeshAnimation()
    { 
        keepAnimating = false;
    }
}
