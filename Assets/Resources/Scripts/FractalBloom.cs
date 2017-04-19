using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalObj {
    public Vector3[] vecs;
    public float r;
	public int ty;

    public FractalObj() {
        vecs = new Vector3[0];
        r = 0;
		ty = -1;
    }

    public FractalObj(Vector3[] _vecs, float _r, int _ty) {
        vecs = _vecs;
        r = _r;
		ty = _ty;
    }
}

public class FractalBloom : MonoBehaviour {
    public float R;
    public float ratio;
    public int level;
    public float delay;

    public int TRIANGLE, CUBE;
	
	public Material mt;

    public Color[] colors;
    int lc;

    GameObject last_obj;

	// Use this for initialization
	void Start () {
        lc = colors.Length;
        last_obj = null;
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void clean_up() {
        if (last_obj != null) {
            Destroy(last_obj);
        }
		last_obj = null;
    }

    public GameObject bloom(Vector3 pos, int[] spell) {
        clean_up();
        GameObject main_body = new GameObject("new bloom");
        last_obj = main_body;
        int l = spell.Length;
        if (l == 0 || spell[0] == -1)
        {
            Debug.Log("Fractal: spell nothing !");
            return main_body;
        }
        StartCoroutine(co_bloom(main_body, pos, spell));
        return main_body;
    }

    IEnumerator co_bloom(GameObject obj, Vector3 pos, int[] spell) {

        int l = spell.Length;
        FractalObj[] q = null;
        int last_color_idx = Random.Range(1, lc);
        if (spell[0] == TRIANGLE) {
			FractalObj tmp = create_triangle(pos, R);
            q = grow_triangle(tmp, obj, colors[last_color_idx]);
        } else if (spell[0] == CUBE) {
			FractalObj tmp = create_rectangle(pos, R);
			q = grow_cube(tmp, obj, colors[last_color_idx]);
		}
		
        for (int i = 1; i < level; ++i) {
            yield return new WaitForSeconds(delay);
            if (spell[i] == -1) break;

            int new_idx = Random.Range(1, lc);
            while (new_idx == last_color_idx) {
                new_idx = Random.Range(1, lc);
            }
            last_color_idx = new_idx;

            q = create_layer(q, spell[i], obj, colors[last_color_idx]);
        }
    }

    FractalObj create_triangle(Vector3 pos, float r) {
		Vector3[] vecs = new Vector3[3];
		vecs[0] = pos + (new Vector3(0, 0, r * 0.57735f));
		vecs[1] = pos - (new Vector3(0, 0, r * 0.28867513459f)) + (new Vector3(r * 0.5f ,0 , 0));
		vecs[2] = pos - (new Vector3(0, 0, r * 0.28867513459f)) + (new Vector3(-r * 0.5f ,0 , 0));
        return new FractalObj(vecs, r, TRIANGLE);
    }
	
	FractalObj create_rectangle(Vector3 pos, float r) {
		Vector3[] vecs = new Vector3[4];
		vecs[0] = pos + (new Vector3(r * 0.5f, 0, r * 0.5f));
		vecs[3] = pos + (new Vector3(-r * 0.5f, 0, r * 0.5f));
		vecs[2] = pos + (new Vector3(-r * 0.5f, 0, -r * 0.5f));
		vecs[1] = pos + (new Vector3(r * 0.5f, 0, -r * 0.5f));
        return new FractalObj(vecs, r, CUBE);
    }
	
	FractalObj[] grow_triangle(FractalObj patch, GameObject obj_p, Color c) {
        if (patch.ty != TRIANGLE)
        {
            Debug.Log("hehe");
            return null;
        }

        GameObject obj = new GameObject("tria");
		obj.transform.parent = obj_p.transform;
        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();


        Vector3[] vertices = new Vector3[12];
        int[] triangles = new int[12];

        Vector3 norm = Vector3.Cross(patch.vecs[1] - patch.vecs[0], patch.vecs[2] - patch.vecs[1]);
        Vector3 h_norm = norm.normalized * patch.r * 0.816496580927726f; // sqrt(6)/3

        //under
        vertices[0] = patch.vecs[2];
        vertices[1] = patch.vecs[1];
        vertices[2] = patch.vecs[0];
        //0
        vertices[3] = (patch.vecs[0] + patch.vecs[2] + patch.vecs[1]) * (1 / 3.0f) + h_norm;
        vertices[4] = vertices[1];
        vertices[5] = vertices[0];
        //1
        vertices[6] = vertices[3];
        vertices[7] = vertices[0];
        vertices[8] = vertices[2];
        //2
        vertices[9] = vertices[3];//  + Vector3.one * 0.1f;
        vertices[10] = vertices[2];//  + Vector3.one * 0.1f;
        vertices[11] = vertices[1];//  + Vector3.one * 0.1f;


        for (int i = 0; i < 12; i++)
        {
            triangles[i] = i;
        }

        mf.mesh.vertices = vertices;
        mf.mesh.triangles = triangles;
        mf.mesh.RecalculateNormals();

        Material new_material = new Material(mt);
        mr.materials[0] = new_material;
        obj.GetComponent<Renderer>().material.color = c;		
		
		FractalObj[] fracs = new FractalObj[3];
		for (int i = 1; i < 4; ++i) {
			Vector3[] tmp = new Vector3[3];
			tmp[0] = vertices[i * 3];
			tmp[1] = vertices[i * 3 + 1];
			tmp[2] = vertices[i * 3 + 2];
			fracs[i - 1] = new FractalObj(tmp, patch.r, TRIANGLE);
		}
		return fracs;
	}
	
	FractalObj[] grow_cube(FractalObj patch, GameObject obj_p, Color c) {
		if (patch.ty != CUBE) {
            Debug.Log("hehe");
            return null;
        }

        GameObject obj = new GameObject("rect");
		obj.transform.parent = obj_p.transform;
        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();

        Vector3[] vertices = new Vector3[24];
        int[] triangles = new int[36];

        Vector3 norm = Vector3.Cross(patch.vecs[1] - patch.vecs[0], patch.vecs[2] - patch.vecs[1]);
        Vector3 h_norm = norm.normalized * patch.r;

        //forward
        vertices[0] = patch.vecs[0];
        vertices[1] = patch.vecs[1];
        vertices[2] = patch.vecs[3];
        vertices[3] = patch.vecs[2];
        //back
        vertices[4] = vertices[2] + h_norm;
        vertices[5] = vertices[3] + h_norm;
        vertices[6] = vertices[0] + h_norm;
        vertices[7] = vertices[1] + h_norm;
        //up
        vertices[8] = vertices[2];
        vertices[9] = vertices[3];
        vertices[10] = vertices[4];
        vertices[11] = vertices[5];
        //down
        vertices[12] = vertices[6];
        vertices[13] = vertices[7];
        vertices[14] = vertices[0];
        vertices[15] = vertices[1];
        //right
        vertices[16] = vertices[6];
        vertices[17] = vertices[0];
        vertices[18] = vertices[4];
        vertices[19] = vertices[2];
        //left
        vertices[20] = vertices[5];
        vertices[21] = vertices[3];
        vertices[22] = vertices[7];
        vertices[23] = vertices[1];

        int currentCount = 0;
        for (int i = 0; i < 24; i = i + 4)
        {
            triangles[currentCount++] = i;
            triangles[currentCount++] = i + 3;
            triangles[currentCount++] = i + 1;

            triangles[currentCount++] = i;
            triangles[currentCount++] = i + 2;
            triangles[currentCount++] = i + 3;

        }

        mf.mesh.vertices = vertices;
        mf.mesh.triangles = triangles;
        mf.mesh.RecalculateNormals();

        Material new_material = new Material(mt);
        mr.materials[0] = new_material;
        obj.GetComponent<Renderer>().material.color = c;
		
		FractalObj[] fracs = new FractalObj[5];
		for (int i = 1; i < 6; ++i) {
			Vector3[] tmp = new Vector3[4];
			tmp[0] = vertices[i * 4];
			tmp[1] = vertices[i * 4 + 2];
			tmp[2] = vertices[i * 4 + 3];
			tmp[3] = vertices[i * 4 + 1];
			fracs[i - 1] = new FractalObj(tmp, patch.r, CUBE);
		}
		return fracs;
	}
	
	FractalObj create_triangle_from_triangle(FractalObj patch) {
		if (patch.vecs.Length != 3) {
			Debug.Log("Wrong XD !!!!!!!!!!!!!!!");
			return null;
		}
		Vector3 mid = (patch.vecs[0] + patch.vecs[1] + patch.vecs[2]) * (1 / 3.0f);
		float r_tmp = patch.r * ratio;
		
		Vector3[] vecs = new Vector3[3];
		vecs[0] = mid + (patch.vecs[0] - mid).normalized * r_tmp * 0.57735f;
		vecs[1] = mid + (patch.vecs[1] - mid).normalized * r_tmp * 0.57735f;
		vecs[2] = mid + (patch.vecs[2] - mid).normalized * r_tmp * 0.57735f;
		return new FractalObj(vecs, r_tmp, TRIANGLE);
	}
	
	FractalObj create_triangle_from_rectangle(FractalObj patch) {
		if (patch.vecs.Length != 4) {
			Debug.Log("Wrong XD !!!!!!!!!!!!!!!");
			return null;
		}
		Vector3 mid = (patch.vecs[0] + patch.vecs[1] + patch.vecs[2] + patch.vecs[3]) * 0.25f;
		float r_tmp = patch.r * ratio;
		
		Vector3[] vecs = new Vector3[3];
		vecs[0] = mid + (patch.vecs[0] - mid).normalized * r_tmp;
		vecs[1] = mid + (patch.vecs[1] - patch.vecs[3]).normalized * r_tmp * 0.866f + (patch.vecs[2] - mid).normalized * r_tmp * 0.5f;
		vecs[2] = mid + (patch.vecs[3] - patch.vecs[1]).normalized * r_tmp * 0.866f + (patch.vecs[2] - mid).normalized * r_tmp * 0.5f;
		return new FractalObj(vecs, r_tmp, TRIANGLE);
	}
	
	FractalObj create_rectangle_from_triangle(FractalObj patch) {
		if (patch.vecs.Length != 3) {
			Debug.Log("Wrong XD !!!!!!!!!!!!!!!");
			return null;
		}
		Vector3 mid = (patch.vecs[0] + patch.vecs[1] + patch.vecs[2]) * (1 / 3.0f);
		float r_tmp = patch.r * ratio;
		Vector3 l1 = (patch.vecs[0] - mid).normalized * r_tmp * 0.4f;
		Vector3 l2 = (patch.vecs[2] - patch.vecs[1]).normalized * r_tmp * 0.4f;
		
		Vector3[] vecs = new Vector3[4];
		vecs[0] = mid + l1 + l2;
		vecs[1] = mid + l1 - l2;
		vecs[2] = mid - l1 - l2;
		vecs[3] = mid - l1 + l2;
		return new FractalObj(vecs, r_tmp * 0.8f, CUBE);
	}
	
	FractalObj create_rectangle_from_rectangle(FractalObj patch) {
		if (patch.vecs.Length != 4) {
			Debug.Log("Wrong XD !!!!!!!!!!!!!!!");
			return null;
		}
		Vector3 mid = (patch.vecs[0] + patch.vecs[1] + patch.vecs[2] + patch.vecs[3]) * 0.25f;
		float r_tmp = patch.r * ratio;
		
		Vector3[] vecs = new Vector3[4];
		vecs[0] = mid + (patch.vecs[0] - mid).normalized * r_tmp * 0.7071f;
		vecs[1] = mid + (patch.vecs[1] - mid).normalized * r_tmp * 0.7071f;
		vecs[2] = mid + (patch.vecs[2] - mid).normalized * r_tmp * 0.7071f;
		vecs[3] = mid + (patch.vecs[3] - mid).normalized * r_tmp * 0.7071f;
		return new FractalObj(vecs, r_tmp, CUBE);		
	}
	
	FractalObj[] create_from(FractalObj fractal, int ty, GameObject obj, Color c) {
		if (ty == TRIANGLE) {
			if (fractal.ty == TRIANGLE) {
				FractalObj tmp = create_triangle_from_triangle(fractal);
				return grow_triangle(tmp, obj, c);
			} else if (fractal.ty == CUBE) {
				FractalObj tmp = create_triangle_from_rectangle(fractal);
				return grow_triangle(tmp, obj, c);
			}
		} else if (ty == CUBE) {
			if (fractal.ty == TRIANGLE) {
				FractalObj tmp = create_rectangle_from_triangle(fractal);
				return grow_cube(tmp, obj, c);
			} else if (fractal.ty == CUBE) {
				FractalObj tmp = create_rectangle_from_rectangle(fractal);
				return grow_cube(tmp, obj, c);
			}			
		}
		return null;
	}

    FractalObj[] create_layer(FractalObj[] objs, int ty, GameObject obj, Color c) {
        int l = objs.Length;
        int new_l = 0;
        new_l = l * 3;
		if (ty == CUBE) {
			new_l = l * 5;
		}

        FractalObj[] res = new FractalObj[new_l];
		FractalObj[] tmp = null;
		int p = 0;
		for (int i = 0; i < l; ++i) {
			tmp = create_from(objs[i], ty, obj, c);
			for (int k = 0; k < tmp.Length; ++k) {
				res[p] = tmp[k];
				p += 1;
			}
		}
        return res;
    }
}
