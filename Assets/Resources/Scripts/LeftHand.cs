using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHand : MonoBehaviour
{

    public GameObject[] magic_element_objs;
    GameObject active_element = null;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        update_active_element();
        if (check_shoot())
        {
            shoot();
        }
    }

    bool check_shoot()
    {
        return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger);
    }

    // Active Element Maintain
    GameObject find_active()
    {
        int l = magic_element_objs.Length;
        for (int i = 0; i < l; ++i)
        {
            Vector2 vec = new Vector2(magic_element_objs[i].transform.position.x - transform.position.x,
                                      magic_element_objs[i].transform.position.z - transform.position.z);
            if (vec.magnitude < Consts.grab_distance)
            {
                return magic_element_objs[i];
            }
        }
        return null;
    }

    void update_active_element()
    {
        GameObject target = find_active();
        if (active_element != target)
        {
            if (active_element != null)
            {
                active_element.GetComponent<MagicFunction>().unselect(1);
            }
            active_element = target;
            if (active_element != null)
            {
                active_element.GetComponent<MagicFunction>().select(1);
            }
        }
    }

    // Shoot function to clean up function
    void shoot()
    {
        if (active_element == null) return;
        MagicFunction active_element_function = active_element.GetComponent<MagicFunction>();
        if (!MagicElements.instance.valid_elements[active_element_function.idx].is_function)
        {
            Debug.Log("You are shooting an element not a function!");
            return;
        }
        active_element_function.clean_function();
    }
}
