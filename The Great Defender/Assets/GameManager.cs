using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // Variables 
    public float backgroundSize;            // How far away is the next sprite 

    private Transform cameraTransform;      // Reference to keep track of the camera position
    private Transform[] layers;         // An array that gathers the Children attached to this GameObject
    private float viewZone = 10f;       // The field of view
    private int leftIndex;      // Reference to the number of sprites on the left
    private int rightIndex;     // reference to the number of sprotes on the right


    private void Start()
    {
        cameraTransform = Camera.main.transform;

        layers = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
            layers[i] = transform.GetChild(i);

        leftIndex = 0;
        rightIndex = layers.Length - 1;
    }

    private void Update()
    {
        if (cameraTransform.position.x < (layers[leftIndex].transform.position.x + viewZone))       // If the position of the camera is less than than the position of the left index + the view zone
            ScrollLeft();           // Call ScrollLeft Function

        if (cameraTransform.position.x > (layers[rightIndex].transform.position.x - viewZone))      // If the camera transform is Greater than the klayers in the right index int field - viewzone
            ScrollRight();          // Call the ScrollRight Function
    }

    private void ScrollLeft()
    {
        int lastRight = rightIndex;     // lastRight overrides rightIndex. lastRight keeps a constant count of what is next
        layers[rightIndex].position = Vector3.right * (layers[leftIndex].position.x - backgroundSize);
        leftIndex = rightIndex;
        rightIndex--;
        if (rightIndex < 0)
            rightIndex = layers.Length - 1;
    }

    private void ScrollRight()
    {
        int lastLeft = leftIndex;     // lastRight overrides rightIndex. lastRight keeps a constant count of what is next
        layers[leftIndex].position = Vector3.right * (layers[rightIndex].position.x + backgroundSize);
        rightIndex = leftIndex;
        leftIndex++;
        if (leftIndex == layers.Length)
            leftIndex = 0;
    }
}
