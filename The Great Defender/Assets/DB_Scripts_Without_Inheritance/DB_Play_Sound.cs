using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class is only used and applied to the spawning Particle Element so the audio source knows when the Particle Element is seen you can play the Sound Clip
/// </summary>
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
