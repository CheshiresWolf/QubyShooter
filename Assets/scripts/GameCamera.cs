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

    Quaternion plusOneDegree, minusOneDegree;

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

			if (Input.GetMouseButton(0)) {
				//Debug.Log("GameCamera | moveCamera | mouse button 0");
				rotation *= Quaternion.Euler(
					-Input.GetAxis("Mouse Y"),
					Input.GetAxis("Mouse X"),
					0
				);
			}

			if (Input.GetMouseButton(1)) {
				Debug.Log("GameCamera | moveCamera | mouse button 1");
				rotation = Quaternion.LookRotation(player.transform.position - camera.transform.position);
				//camera.transform.LookAt(player.transform.position, Vector3.up);
			}

			if (Input.GetKey(keyMap.left)) {
				/*
				Vector3 pivot = getPivot(player.transform.position, 3.5355f);
				Debug.Log("GameCamera | moveCamera | keyMap.left | pivot : " + pivot);
				camera.transform.position = rotatePointAroundPivot(
					camera.transform.position,
					pivot,
					rotateAroundPoint(player.transform.position, 1.0f)
				);*/
				camera.transform.position = plusOneDegree * camera.transform.position;
			}
			if (Input.GetKey(keyMap.right)) {
				/*
				Vector3 pivot = getPivot(player.transform.position, 3.5355f);
				camera.transform.position = rotatePointAroundPivot(
					camera.transform.position,
					player.transform.position,
					rotateAroundPoint(player.transform.position, -1.0f)
				);
				*/
				camera.transform.position = minusOneDegree * camera.transform.position;
			}

			camera.transform.rotation = rotation;
		}

	}

	Quaternion rotateAroundPoint(Vector3 point, float angle) {
		float distance = Vector3.Distance(Vector3.zero, point);

		//normalize
		Vector3 nPoint = new Vector3(
			(point.x != 0) ? (point.x / distance) : 0,
			(point.y != 0) ? (point.y / distance) : 0,
			(point.z != 0) ? (point.z / distance) : 0
		);

		return new Quaternion(
			Mathf.Cos(angle / 2.0f),
			nPoint.x * Mathf.Sin(angle / 2.0f),
			nPoint.y * Mathf.Sin(angle / 2.0f),
			nPoint.z * Mathf.Sin(angle / 2.0f)
		);
	}

	Vector3 mulPosRot(Vector3 pos, Quaternion rot) {
		//Quaternion qPos = new Quaternion(0, pos.x, pos.y, pos.z);
		//Quaternion mul  = rot * pos;//qPos;

		return rot * pos;//new Vector3(mul.x, mul.y, mul.z);
	}

	Vector3 getPivot(Vector3 point, float distance) {
		float centerDistance = Vector3.Distance(Vector3.zero, point);

		Debug.Log("GameCamera | getPivot | centerDistance : " + centerDistance);

		return new Vector3(
			(point.x != 0) ? (centerDistance * distance / point.x) : 0,
			(point.y != 0) ? (centerDistance * distance / point.y) : 0,
			(point.z != 0) ? (centerDistance * distance / point.z) : 0
		);
	}

	Vector3 rotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle) {
		return angle * (point - pivot) + pivot;
	}

	// Use this for initialization
	void Start () {
		radius = icosaedron.GetComponent<Generator>().radius;

		player.transform.position = new Vector3( 0,        0, radius - 0.5f    );
		camera.transform.position = new Vector3( 0, -3.5355f, radius - 3.5355f );

		if (cameraMode == CameraMode.ORBIT) {
			rotation = Quaternion.LookRotation(player.transform.position - camera.transform.position);

			plusOneDegree  = rotateAroundPoint(player.transform.position,  Mathf.PI / 180.0f);
    		minusOneDegree = rotateAroundPoint(player.transform.position, -Mathf.PI / 180.0f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if (
			cameraMode == CameraMode.FREE ||
			cameraMode == CameraMode.FREE_ORBIT ||
			cameraMode == CameraMode.ORBIT
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
