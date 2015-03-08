using UnityEngine;
using System.Collections;

public class EditorCamera : MonoBehaviour {

	public GameObject camera;
	public float radius;

	Keys pressedKeys;

	Quaternion rotation;
	Vector3 currentRotation = new Vector3(0, 0, 0);

	RaycastHit hit = new RaycastHit();
    Ray ray;

	class Keys {
		public bool up;
		public bool left;
		public bool right;
		public bool down;

		public Keys() {}

		public void print() {
			Debug.Log("EditorCamera | pressedKeys(wasd) : (" + up + "," + left + "," + right + "," + down + ")");
		}
	};

	void moveCamera() {
		if (pressedKeys.up)   {
			if (currentRotation.z < 90) currentRotation.z += 1;
		}
		if (pressedKeys.down) {
			if (currentRotation.z > -90) currentRotation.z -= 1;
		}

		if (pressedKeys.left)  currentRotation.y += 1;
		if (pressedKeys.right) currentRotation.y -= 1;

		rotation.eulerAngles = currentRotation;

		camera.transform.position = rotatePointAroundPivot(camera.transform.position, Vector3.zero, rotation);
		currentRotation.y = 0;
		currentRotation.z = 0;

		camera.transform.LookAt(Vector3.zero, Vector3.up);
	}

	Vector3 rotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle) {
		return angle * (point - pivot) + pivot;
	}

	void selection() {
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Debug.Log("EditorCamera | selection | ray : " + ray);


		if (Physics.Raycast(ray, out hit, 100.0f)) {
			Debug.Log("EditorCamera | selection | hit : " + hit);
			GameObject face = hit.transform.gameObject;

			setColor(face, Color.red);          
		}
	}

	void setColor(GameObject body, Color color) {
		MeshRenderer gameObjectRenderer = body.GetComponent("MeshRenderer") as MeshRenderer;
		gameObjectRenderer.material.color = color;
	}

	// Use this for initialization
	void Start () {
		camera.transform.position = new Vector3(radius, 0, 0);
		camera.transform.LookAt(Vector3.zero, Vector3.up);

		pressedKeys = new Keys();
		rotation = new Quaternion();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.W)) {
			pressedKeys.up    = true;
		}
		if (Input.GetKeyDown(KeyCode.S)) {
			pressedKeys.down  = true;
		}
		if (Input.GetKeyDown(KeyCode.A)) {
			pressedKeys.left  = true;
		}
		if (Input.GetKeyDown(KeyCode.D)) {
			pressedKeys.right = true;
		}

		if (Input.GetKeyUp(KeyCode.W)) {
			pressedKeys.up    = false;
		}
		if (Input.GetKeyUp(KeyCode.S)) {
			pressedKeys.down  = false;
		}
		if (Input.GetKeyUp(KeyCode.A)) {
			pressedKeys.left  = false;
		}
		if (Input.GetKeyUp(KeyCode.D)) {
			pressedKeys.right = false;
		}

		
		if (Input.GetKeyDown(KeyCode.Mouse0)) {
			selection();
		}

		//pressedKeys.print();

		moveCamera();
	}
}
