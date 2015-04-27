using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameCamera : MonoBehaviour {

	public GameObject camera;
	public GameObject icosaedron;
	public GameObject player;

	private float radius;

	enum CameraMode { ORIGINAL, FREE, FREE_ORBIT, ORBIT };

	CameraMode cameraMode = CameraMode.ORBIT;//CameraMode.FREE;

	EditorKeyMap keyMap = new EditorKeyMap();

	Vector3 currentRotation = new Vector3(0, 0, 0);
	Vector3 position = Vector3.zero;
	Quaternion rotation = Quaternion.identity;

	RaycastHit hit = new RaycastHit();
    Ray ray;

	class EditorKeyMap {

		public KeyCode toggle    = KeyCode.Mouse0;
		public KeyCode mouseLook = KeyCode.Mouse1;

		public KeyCode lookUp    = KeyCode.UpArrow;
		public KeyCode lookDown  = KeyCode.DownArrow;
		public KeyCode lookLeft  = KeyCode.LeftArrow;
		public KeyCode lookRight = KeyCode.RightArrow;

		public KeyCode clockWise        = KeyCode.E;
		public KeyCode counterClockWise = KeyCode.Q;
		
		public KeyCode forward = KeyCode.W;
		public KeyCode left    = KeyCode.A;
		public KeyCode back    = KeyCode.S;
		public KeyCode right   = KeyCode.D;
		public KeyCode up      = KeyCode.LeftControl;
		public KeyCode down    = KeyCode.Space;

		public KeyCode precise = KeyCode.LeftShift;

		public EditorKeyMap() {}

	};

	void moveCamera() {

		if (cameraMode == CameraMode.ORIGINAL) {

			if (Input.GetKey(KeyCode.W)) {
				if (currentRotation.z < 90) currentRotation.z += 1;
			}
			if (Input.GetKey(KeyCode.S)) {
				if (currentRotation.z > -90) currentRotation.z -= 1;
			}

			if (Input.GetKey(KeyCode.A)) currentRotation.y += 1;
			if (Input.GetKey(KeyCode.D)) currentRotation.y -= 1;

			Quaternion tempRotation = new Quaternion();

			tempRotation.eulerAngles = currentRotation;

			camera.transform.position = rotatePointAroundPivot(
				camera.transform.position, Vector3.zero, tempRotation
			);
			currentRotation.y = 0;
			currentRotation.z = 0;

			camera.transform.LookAt(Vector3.zero, Vector3.up);

		} else

		if (cameraMode == CameraMode.FREE) {

			float movSpeed = 0.1f;
			float rotSpeed = 1;

			if (Input.GetKey(keyMap.precise)) {
				movSpeed /= 10;
			}

			// ==== Rotation ====
			
				if (Input.GetMouseButton(1)) {
					rotation *= Quaternion.Euler(
						-Input.GetAxis("Mouse Y"),
						Input.GetAxis("Mouse X"),
						0
					);
				}

				if (Input.GetKey(keyMap.clockWise)) {
					rotation *= Quaternion.Euler(0, 0, -rotSpeed);
				}
				if (Input.GetKey(keyMap.counterClockWise)) {
					rotation *= Quaternion.Euler(0, 0, rotSpeed);
				}

				if (Input.GetKey(keyMap.lookUp)) {
					rotation *= Quaternion.Euler(rotSpeed, 0, 0);
				}
				if (Input.GetKey(keyMap.lookDown)) {
					rotation *= Quaternion.Euler(-rotSpeed, 0, 0);
				}

				if (Input.GetKey(keyMap.lookLeft)) {
					rotation *= Quaternion.Euler(0, -rotSpeed, 0);
				}
				if (Input.GetKey(keyMap.lookRight)) {
					rotation *= Quaternion.Euler(0, rotSpeed, 0);
				}
			
			// ==================

			// ==== Position ====

				if (Input.GetKey(keyMap.forward)) {
					position += rotation * (movSpeed * Vector3.forward);
				}
				if (Input.GetKey(keyMap.back)) {
					position += rotation * (-movSpeed * Vector3.forward);
				}

				if (Input.GetKey(keyMap.left)) {
					position += rotation * (-movSpeed * Vector3.right);
				}
				if (Input.GetKey(keyMap.right)) {
					position += rotation * (movSpeed * Vector3.right);
				}

				if (Input.GetKey(keyMap.down)) {
					position += rotation * (movSpeed * Vector3.up);
				}
				if (Input.GetKey(keyMap.up)) {
					position += rotation * (-movSpeed * Vector3.up);
				}

			// ==================

			camera.transform.position = position; // rotation * (radius * Vector3.back)
			camera.transform.rotation = rotation;

		} else

		if (cameraMode == CameraMode.FREE_ORBIT) {

		} else

		if (cameraMode == CameraMode.ORBIT) {

		}

	}

	Vector3 rotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle) {
		return angle * (point - pivot) + pivot;
	}

	// Use this for initialization
	void Start () {
		radius = icosaedron.GetComponent<Generator>().radius;

		//camera.transform.position = new Vector3(0, 0, (radius - player.transform.position.z));
		//camera.transform.LookAt(Vector3.zero, Vector3.up);
		player.transform.position = new Vector3(0, 0, radius - 0.5f);

		if (cameraMode == CameraMode.ORBIT) {
			//camera.transform.position = new Vector3(0.0f, 0.0f, 1.0f);
			//camera.transform.position.y -= 3.5355f;
			Vector3 pos = camera.transform.position;
			pos.y += 3.5355f;
			pos.z  = 3.5355f;
			camera.transform.position = pos;//1.4645f;

			camera.transform.LookAt(player.transform.position, Vector3.up);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if (
			cameraMode == CameraMode.FREE ||
			cameraMode == CameraMode.FREE_ORBIT
		) {
			if (Input.GetKeyDown(keyMap.mouseLook)) {
				Cursor.lockState = CursorLockMode.Locked; // .Locked .Confined
				Cursor.visible = false;
			}
			if (Input.GetKeyUp(keyMap.mouseLook)) {
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}

		moveCamera();
	}
}
