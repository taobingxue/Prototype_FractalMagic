using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAudio : MonoBehaviour {

    public static SoundAudio instance;
    AudioSource source;

    public AudioClip[] clips;
    public AudioClip grab, release, remove;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
    // Use this for initialization
    void Start () {
        source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void playSyllable(int idx = 0) {
        if (idx >= clips.Length) {
            return;
        }
        play(clips[idx]);
    }

    public void playGrab() {
        play(grab);
    }
    public void playRelease() {
        play(release);
    }
    public void playRemove() {
        play(remove);
    }

    void play(AudioClip clip) {
        source.Stop();
        source.clip = clip;
        source.Play();
    }
}
