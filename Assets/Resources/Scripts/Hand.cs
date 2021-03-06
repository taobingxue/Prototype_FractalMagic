﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

    public GameObject[] magic_element_objs;
    bool last_status = false;
    GameObject active_element = null;
    // public GameObject fractal_generator;
    GameObject grabed = null;
    int grabed_idx = -1;

    public FractalBloom fractal_generator;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        update_active_element();
        update_status();

		if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick) || OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick)) {
            SoundAudio.instance.playRemove(transform.position);
			fractal_generator.clean_up();
		}
    }

    // Status Maintain and Operation Trigger
    void update_status() {
        bool status = check_grab();

        trigger_grab(status);
        trigger_release(status);
        last_status = status;
    }

    bool check_grab() {
        return OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) || OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
    }

    // trigger operation
    void trigger_grab(bool status) {
        if (last_status == false && status && active_element != null) {
            MagicFunction active_element_function = active_element.GetComponent<MagicFunction>();
            GameObject new_obj = Instantiate(MagicElements.instance.valid_elements[active_element_function.idx].model_prefab) as GameObject;
            new_obj.transform.parent = transform;
            new_obj.transform.localPosition = Vector3.zero;
            grabed = new_obj;
            grabed_idx = active_element_function.idx;

            if (grabed_idx != MagicElements.instance.main_function_idx) {
                SoundAudio.instance.playGrab(transform.position);
            } else {
                SoundAudio.instance.playMagic(transform.position);
            }
        }
    }

    void trigger_release(bool status) {
        if (last_status && status == false && grabed != null) {
            if (grabed_idx == MagicElements.instance.main_function_idx) {
                // release main
                int[] magic_spell = MagicElements.instance.get_spell();

                string a = "";
                for (int i = 0; i < 10; ++i)
                {
                    a += " " + magic_spell[i];
                }
                Debug.Log("magic_spell" + a);

                /*
                 *  call more general but not exact fractal with fractal class instead of fractal bloom
                fractal_generator.GetComponent<Fractal>().setFractalStartPosition(transform.position);
                fractal_generator.GetComponent<Fractal>().cleanUpFractals();
                fractal_generator.GetComponent<Fractal>().generateFractalsFromIndices(magic_spell);
                */

                SoundAudio.instance.stopMagic();
                fractal_generator.bloom(transform.position, magic_spell);
            } else {
                SoundAudio.instance.playRelease(transform.position);
                if (active_element == null)
                {
                    // release to the sky
                    Debug.Log("You release it to nothing!");
                }
                else
                {
                    // release to a function
                    MagicFunction active_element_function = active_element.GetComponent<MagicFunction>();
                    active_element_function.extend_function(MagicElements.instance.valid_elements[grabed_idx]);
                }
            }

            Destroy(grabed);
            grabed = null;
        }
    }

    // Active Element Maintain
    GameObject find_active() {
        int l = magic_element_objs.Length;
        for (int i = 0; i < l; ++i) {
            Vector2 vec = new Vector2(magic_element_objs[i].transform.position.x - transform.position.x,
                                      magic_element_objs[i].transform.position.z - transform.position.z);
            if (vec.magnitude < Consts.grab_distance) {
                return magic_element_objs[i];
            }
        }
        return null;
    }

    void update_active_element() {
        GameObject target = find_active();
        if (active_element != target)
        {
            if (active_element != null)
            {
                active_element.GetComponent<MagicFunction>().unselect(0);
            }
            active_element = target;
            if (active_element != null)
            {
                active_element.GetComponent<MagicFunction>().select(0);
            }
        }
    }
}
