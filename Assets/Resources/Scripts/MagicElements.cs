using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MagicElement {
    public GameObject model_prefab, spin_prefab, instance;
    public int idx;
    public bool is_function;

    // color, scale, ...

    public MagicElement() {
        model_prefab = null;
        spin_prefab = null;
        instance = null;
        idx = -1;
        is_function = false;
    }
}

public class MagicElements : MonoBehaviour
{
    public MagicElement[] valid_elements;
    public int main_function_idx;

    public GameObject[] selected;
    public static MagicElements instance;
    public bool debugging;

    public int max_expand;

    int count;

    public void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    int[] recurrsive_expand(int[] res, int expand_idx) {
        if (debugging) {
            string m = "";
            for (int i = 0; i < 10; ++i) { m += " " + res[i]; }
            Debug.Log("res = " + m);
            Debug.Log("expand_idx = " + expand_idx);
            Debug.Log("count = " + count);
        }
        count += 1;
        if (count >= max_expand) {
            return res;
        }
        if (!valid_elements[expand_idx].is_function) {
            res[10] += 1;
            res[res[10]] = expand_idx;
        } else {
            int[] expand_list = valid_elements[expand_idx].instance.GetComponent<MagicFunction>().get_function_list();
            for (int i = 0; i < Consts.ele_spin_max_elements; i++) {
                if (expand_list[i] < 0) {
                    break;
                }
                res = recurrsive_expand(res, expand_list[i]);
                if (res[10] == 9) break;
            }
        }
        return res;
    }

    public int[] get_spell() {
        int[] res = new int[11];
        for (int i = 0; i < 10; ++i) {
            res[i] = -1;
        }
        res[10] = -1;

        count = 0;
        return recurrsive_expand(res, main_function_idx);
    }
}


