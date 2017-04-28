using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAudio : MonoBehaviour {

    public static SoundAudio instance;

    public AudioClip[] clips;
    public AudioClip grab, release, remove, magic;

    float t;
    GameObject magicObj;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }

        t = 0;
    }
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        t += Time.deltaTime;
        if (t > 5) {
            t = 0;
            for (int i = transform.childCount - 1; i >= 0; --i) {
                GameObject obj = transform.GetChild(i).gameObject;
                if (!obj.GetComponent<AudioSource>().isPlaying) {
                    Destroy(obj);
                }
            }
        }
	}

    public void playSyllable(int idx, Vector3 pos) {
        if (idx >= clips.Length) {
            idx %= clips.Length;
        }
        play(clips[idx], pos);
    }

    public void playGrab(Vector3 pos) {
        play(grab, pos);
    }
    public void playRelease(Vector3 pos) {
        play(release, pos);
    }
    public void playRemove(Vector3 pos) {
        play(remove, pos);
    }

    public void playMagic(Vector3 pos) {
        magicObj = play(magic, pos);
        magicObj.GetComponent<AudioSource>().loop = true;
    }
    public void stopMagic() {
        Destroy(magicObj);
    }

    GameObject play(AudioClip clip, Vector3 pos) {
        GameObject obj = new GameObject();
        obj.transform.position = pos;
        obj.AddComponent<AudioSource>();
        AudioSource source = obj.GetComponent<AudioSource>();

        source.Stop();
        source.clip = clip;
        source.Play();
        return obj;
    }
}
