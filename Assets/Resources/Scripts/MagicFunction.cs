using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicFunction : MonoBehaviour {

    // element idx
    public int idx;

    // describe function
    int ele_count = 0;
    int[] function_list;

    GameObject spin_center;
    float angle = 0;

    // selected present
    bool[] is_selected = { false, false };
    GameObject[] selected_ring = { null, null };   

    // Use this for initialization
    void Start()
    {
        function_list = new int[Consts.ele_spin_max_elements];
        for (int i = 0; i < Consts.ele_spin_max_elements; i++) {
            function_list[i] = -1;
        }
        spin_center = transform.FindChild("Ring").gameObject;
    }

    // Update is called once per frame
    void Update () {

        spin_element_lists();
        /*
        if (Input.GetKeyDown(KeyCode.Space)) {
            select();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            unselect();
        }
        */

        if (Input.GetKeyDown(KeyCode.C))
        {
            extend_function(MagicElements.instance.valid_elements[0]);
        }
    }

    // Operation
    public void extend_function(MagicElement ele) {
        if (ele_count >= Consts.ele_spin_max_elements) {
            Debug.Log("Too many elements in one function");
            return;
        }
        if (!MagicElements.instance.valid_elements[idx].is_function) {
            Debug.Log("You can only edit magic elements");
            return;
        }

        function_list[ele_count] = ele.idx;
        add_spin_elements(ele.spin_prefab);
        ele_count += 1;
    }

    public void clean_function() {
        clear_spin_elements();
        for (int i = 0; i < Consts.ele_spin_max_elements; i++) {
            function_list[i] = -1;
        }
        ele_count = 0;
    }
    
    // Select Ring
    public void select(int hand_idx) {
        if (is_selected[hand_idx]) {
            Debug.Log("Reselect:" + this.name);
            return ;
        }

        selected_ring[hand_idx] = Instantiate(MagicElements.instance.selected[hand_idx]) as GameObject;
        selected_ring[hand_idx].transform.parent = transform;
        selected_ring[hand_idx].transform.localPosition = Vector3.zero;
        selected_ring[hand_idx].transform.localScale = Vector3.one * 0.5f;
        is_selected[hand_idx] = true;
    }

    public void unselect(int hand_idx) {
        if (!is_selected[hand_idx]) {
            Debug.Log("unselect not select:" + this.name);
            return;
        }

        Destroy(selected_ring[hand_idx]);
        is_selected[hand_idx] = false;
    }

    // Spin Elements
    void spin_element_lists() {
        angle += Time.deltaTime * Consts.ele_spin_speed;
        if (angle > 360)
        {
            angle -= 360;
        }

        spin_center.transform.localRotation = Quaternion.Euler(new Vector3(0, angle, 0));
    }

    void add_spin_elements(GameObject new_ele_prefab) {
        GameObject new_ele = Instantiate(new_ele_prefab) as GameObject;
        new_ele.transform.parent = spin_center.transform;
        int l = spin_center.transform.childCount;
        float tmp_angle = l * Consts.ele_spin_angle_delta / 180 * Mathf.PI;
        float x = Consts.ele_spin_radius * Mathf.Cos(tmp_angle);
        float z = Consts.ele_spin_radius * Mathf.Sin(tmp_angle);
        new_ele.transform.localPosition = (new Vector3(x, 0, z)) * 0.1f * (1 / transform.localScale.x);
    }

    void clear_spin_elements() {
        while (spin_center.transform.childCount != 0) {
            GameObject obj = spin_center.transform.GetChild(0).gameObject;
            obj.transform.parent = null;
            Destroy(obj);
        }
    }


    // get set
    public int[] get_function_list() {
        return function_list;
    }
}
