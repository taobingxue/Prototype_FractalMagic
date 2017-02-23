using UnityEngine;
using System.Collections;

public class Fractal : MonoBehaviour {

	private Vector3[] childDirections = {
		// Vector3.up,
		// Vector3.right,
		// Vector3.left,
		// Vector3.forward,
		// Vector3.back,
		// Vector3.down
	};

	private static Vector3[] normalChildDirections = {
		Vector3.up,
		Vector3.right,
		Vector3.left,
		Vector3.forward,
		Vector3.back,
		Vector3.down
	};

	private static Quaternion[] childOrientations = {
		Quaternion.identity,
		Quaternion.Euler(0f, 0f, -90f),
		Quaternion.Euler(0f, 0f, 90f),
		Quaternion.Euler(90f, 0f, 0f),
		Quaternion.Euler(-90f, 0f, 0f),
		Quaternion.identity
	};

	private static Quaternion[] childTetrahedronOrientations = {
		Quaternion.Euler(60f, 0f, -60f),
		Quaternion.Euler(-60f, 0f, 60f),
		Quaternion.Euler(-60f, 0f, -60f),
		Quaternion.Euler(0f, 180f, 0f),
		Quaternion.identity
	};

	public Mesh[] meshes;
	public Material material;
	public int maxDepth;
	public float childScale;
	public float spawnProbability;
	public int[] shapeIndices;

	private int depth;
	private Material[,] materials;

	private void InitializeMaterials () {
		materials = new Material[maxDepth + 1, 2];
		for (int i = 0; i <= maxDepth; i++) {
			float t = i / (maxDepth - 1f);
			t *= t;
			materials[i, 0] = new Material(material);
			materials[i, 0].color = Color.Lerp(Color.white, Color.yellow, t);
			materials[i, 1] = new Material(material);
			materials[i, 1].color = Color.Lerp(Color.white, Color.cyan, t);
		}
		materials[maxDepth, 0].color = Color.magenta;
		materials[maxDepth, 1].color = Color.red;
	}
	
	private void Start () {
		if (materials == null) {
			InitializeMaterials();
		}

		if (depth != 0) {
			generateFractalsFromIndices(shapeIndices);
		}
		// decide on mesh based on level
		// int[] test = {0, 0, 1};
		// generateFractalsFromIndices(test);
		//int meshIndex = Random.Range(0, meshes.Length);

		// int meshIndex = depth % meshes.Length;
		// gameObject.AddComponent<MeshFilter>().mesh =
		// 	meshes[meshIndex];
		// gameObject.AddComponent<MeshRenderer>().material =
		// 	materials[depth, Random.Range(0, 2)];

		// if (meshIndex == 0) {
		// 	childDirections = meshes[meshIndex].normals;
		// } else {
		// 	childDirections = normalChildDirections;
		// }

		// for (int i = 0; i < childDirections.Length; i++) {
		// 	Debug.Log(childDirections[i]);
		// 	//Debug.DrawLine(Vector3.zero, childDirections[i], Color.red, 100);
		// }
		// if (depth < maxDepth) {
		// 	StartCoroutine(CreateChildren());
		// }
	}

	private IEnumerator CreateChildren () {
		for (int i = 0; i < childDirections.Length; i++) {
			if (Random.value < spawnProbability) {
				yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
				GameObject FractalChild = new GameObject("Fractal Child");
				FractalChild.tag = "FractalChild";
				FractalChild.AddComponent<Fractal>().
					Initialize(this, i);
			}
		}
	}

	private void Initialize (Fractal parent, int childIndex) {
		// if (childIndex > childDirections.Length-1)
		// 	return;
		shapeIndices = parent.shapeIndices;
		meshes = parent.meshes;
		materials = parent.materials;
		maxDepth = parent.maxDepth;
		depth = parent.depth + 1;
		childScale = parent.childScale;
		spawnProbability = parent.spawnProbability;
		transform.parent = parent.transform;
		transform.localScale = Vector3.one * childScale;
		transform.localPosition =
			parent.getChildDirections()[childIndex] * (0.3f + 0.5f * childScale);
		//transform.localRotation = childOrientations[childIndex];
		// Vector3 relativePos = parent.getChildDirections()[childIndex] - transform.position;
  //       Quaternion rotation = Quaternion.LookRotation(parent.getChildDirections()[childIndex]);
  //       transform.localRotation = rotation;
	}

	public Vector3[] getChildDirections() {
		return childDirections;
	}

	public void cleanUpFractals() {
		//get all fractals and delete
		Destroy(gameObject.GetComponent<MeshFilter>());
		Destroy(gameObject.GetComponent<MeshRenderer>());
		GameObject[] FractalsToDelete = GameObject.FindGameObjectsWithTag("FractalChild");

		for (int i = 0; i < FractalsToDelete.Length; i++) {
			Destroy(FractalsToDelete[i]);
		}
	}

	public void generateFractalsFromIndices(int[] shapeIndices) {
		if (depth == 0) {
			this.shapeIndices = shapeIndices;
		}

		if (depth >= shapeIndices.Length) {
			Debug.Log("Either too deep or shapeIndices Length error");
			return;
		}

		if (shapeIndices[depth] > meshes.Length - 1 || shapeIndices[depth] < 0) {
			Debug.Log("No such index in meshes..");
			return;
		}
		int meshIndex = shapeIndices[depth];

		// if (meshes[meshIndex] == null) {
		// 	Debug.Log("Cannot get this mesh");
		// 	return;
		// }


		gameObject.AddComponent<MeshFilter>().mesh =
			meshes[meshIndex];
		gameObject.AddComponent<MeshRenderer>().material =
			materials[depth, Random.Range(0, 2)];

		if (meshIndex == 0) {
			childDirections = meshes[meshIndex].normals;
		} else {
			childDirections = normalChildDirections;
		}

		for (int i = 0; i < childDirections.Length; i++) {
			//Debug.Log(childDirections[i]);
			//Debug.DrawLine(Vector3.zero, childDirections[i], Color.red, 100);
		}
		if (depth < maxDepth) {
			StartCoroutine(CreateChildren());
		}
	}

	public void setFractalStartPosition(Vector3 position) {
		transform.position = position;
		return;
	}
}