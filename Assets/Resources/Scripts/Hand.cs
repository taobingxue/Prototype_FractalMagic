using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

    public GameObject[] magic_element_objs;
    bool last_status = false;
    GameObject active_element = null;
    public GameObject fractal_generator;
    GameObject grabed = null;
    int grabed_idx = -1;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        update_active_element();
        update_status();
        /*
        Debug.Log("A" + OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger));
        Debug.Log("B" + OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger));
        Debug.Log("C" + OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger));
        Debug.Log("D" + OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger));
        Debug.Log("E" + OVRInput.Get(OVRInput.Button.SecondaryHandTrigger));
        Debug.Log("F" + OVRInput.Get(OVRInput.Button.PrimaryHandTrigger));
        */
    }

    // Status Maintain and Operation Trigger
    void update_status() {
        bool status = check_grab();

        trigger_grab(status);
        trigger_release(status);
        last_status = status;
    }

    bool check_grab() {
        return OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
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
        }
    }

    void trigger_release(bool status) {
        if (last_status && status == false && grabed != null) {
            if (grabed_idx == MagicElements.instance.main_function_idx) {
                // release main
                int[] magic_spell = MagicElements.instance.get_spell();

                string a = "";
                for (int i = 0; i < 10; ++i) {
                    a += " " + magic_spell[i];
                }
                Debug.Log("magic_spell" + a);
                // TODO: call bloom
                // need to cleanup former bloom
                fractal_generator.GetComponent<Fractal>().setFractalStartPosition(transform.position);
                fractal_generator.GetComponent<Fractal>().cleanUpFractals();
                fractal_generator.GetComponent<Fractal>().generateFractalsFromIndices(magic_spell);

            } else if (active_element == null) {
                // release to the sky
                Debug.Log("You release it to nothing!");
                // TODO: call destroy partical
            } else {
                // release to a function
                MagicFunction active_element_function = active_element.GetComponent<MagicFunction>();
                active_element_function.extend_function(MagicElements.instance.valid_elements[grabed_idx]);
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
