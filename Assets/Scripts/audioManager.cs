using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    // References for each individual sound here, callable from other scripts
    /*  --Example--
    public AudioClip skidSound; // an audio clip is good for short sounds
    public AudioSource raceMusic; // an audio source would better play long tracks such as overworld music
    */
    public AudioSource audioSource; // all sound will come from this object, for ease
    public AudioSource raceMusic; // over world music
    public AudioClip skidSound1;
    public AudioClip skidSound2;
    public AudioClip drivingSound;
    public AudioClip honkSound1;
    public AudioClip honkSound2;



    // this function is to play a single audio clip, callable from another script
    // it passes in the desired sound to play and at which volume
    public void PlayAudio(AudioClip sound, float volume)
    {
        audioSource.clip = sound; // set the desired sound to play
        audioSource.pitch = Random.Range(0.75f, 1.0f); // shift pitch so sounds aren't always the same
        audioSource.volume = volume; // set volume
        audioSource.PlayOneShot(sound);  // this plays the sound ONCE
    }

    //Maybe another function for sustained sound?
    //for long skids, or horn presses ?

    //function to start the race music, callable from other scripts
    public void PlayRaceMusic()
    {
        raceMusic.Play();
    }

    //function to stop the race music, callable from other scripts
    public void StopRaceMusic()
    {
        raceMusic.Stop();
    }
}
