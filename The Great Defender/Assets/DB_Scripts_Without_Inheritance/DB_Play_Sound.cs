using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB_Play_Sound : MonoBehaviour
{
    public AudioSource SpawnSound;

	// Use this for initialization
	void Start ()
    {
        SpawnSound = GetComponent<AudioSource>();
	}

    private void OnBecameVisible()
    {
        SpawnSound.Play();
    }
}
