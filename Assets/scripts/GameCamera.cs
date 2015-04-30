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
    float cameraOrbitAngle = Mathf.PI / 90.0f;

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
				rotation = Quaternion.LookRotation(player.transform.position - camera.transform.position, Vector3.zero - camera.transform.position);
			}

			if (Input.GetKey(keyMap.left)) {
				camera.transform.position = rotateAroundVector(player.transform.position,  cameraOrbitAngle) * camera.transform.position;
				rotation.SetLookRotation(player.transform.position - camera.transform.position, Vector3.zero - camera.transform.position);
			}
			if (Input.GetKey(keyMap.right)) {
				camera.transform.position = rotateAroundVector(player.transform.position,  -cameraOrbitAngle) * camera.transform.position; 
				rotation.SetLookRotation(player.transform.position - camera.transform.position, Vector3.zero - camera.transform.position);
			}

			if (Input.GetKey(keyMap.forward)) {
				Vector3 normal = Vector3.Cross(Vector3.zero - player.transform.position, Vector3.zero - camera.transform.position);
				Quaternion newRot = rotateAroundVector(normal, -Mathf.PI / 180.0f);
				player.transform.position = newRot * player.transform.position;
				camera.transform.position = newRot * camera.transform.position;

				rotation.SetLookRotation(player.transform.position - camera.transform.position, Vector3.zero - camera.transform.position);
			}

			if (Input.GetKey(keyMap.back)) {
				Vector3 normal = Vector3.Cross(Vector3.zero - player.transform.position, Vector3.zero - camera.transform.position);
				Quaternion newRot = rotateAroundVector(normal, Mathf.PI / 180.0f);
				player.transform.position = newRot * player.transform.position;
				camera.transform.position = newRot * camera.transform.position;

				rotation.SetLookRotation(player.transform.position - camera.transform.position, Vector3.zero - camera.transform.position);
			}

			if (Input.GetKey(keyMap.clockWise)) {
				Vector3 normal = Vector3.Cross(Vector3.zero - player.transform.position, Vector3.zero - camera.transform.position);
				Vector3 contrNormal = Vector3.Cross(Vector3.zero - normal, Vector3.zero - player.transform.position);

				Quaternion newRot = rotateAroundVector(contrNormal, -Mathf.PI / 180.0f);
				player.transform.position = newRot * player.transform.position;
				camera.transform.position = newRot * camera.transform.position;

				rotation.SetLookRotation(player.transform.position - camera.transform.position, Vector3.zero - camera.transform.position);
			}
			if (Input.GetKey(keyMap.counterClockWise)) {
				Vector3 normal = Vector3.Cross(Vector3.zero - player.transform.position, Vector3.zero - camera.transform.position);
				Vector3 contrNormal = Vector3.Cross(Vector3.zero - normal, Vector3.zero - player.transform.position);

				Quaternion newRot = rotateAroundVector(contrNormal, Mathf.PI / 180.0f);
				player.transform.position = newRot * player.transform.position;
				camera.transform.position = newRot * camera.transform.position;

				rotation.SetLookRotation(player.transform.position - camera.transform.position, Vector3.zero - camera.transform.position);
			}

			camera.transform.rotation = rotation;
		}

	}

	Quaternion rotateAroundVector(Vector3 vec, float angle) {
		float distance = Vector3.Distance(Vector3.zero, vec);

		//normalize
		Vector3 nPoint = new Vector3(
			(vec.x != 0) ? (vec.x / distance) : 0,
			(vec.y != 0) ? (vec.y / distance) : 0,
			(vec.z != 0) ? (vec.z / distance) : 0
		);

		return new Quaternion(
			nPoint.x * Mathf.Sin(angle / 2.0f),
			nPoint.y * Mathf.Sin(angle / 2.0f),
			nPoint.z * Mathf.Sin(angle / 2.0f),
			           Mathf.Cos(angle / 2.0f)
		);
	}

	Vector3 getPivot(Vector3 point, float distance) {
		float centerDistance = Vector3.Distance(Vector3.zero, point);

		Debug.Log("GameCamera | getPivot | centerDistance : " + centerDistance);

		return new Vector3(
			point.x - ( (point.x != 0) ? (point.x * distance / centerDistance) : 0 ),
			point.y - ( (point.y != 0) ? (point.y * distance / centerDistance) : 0 ),
			point.z - ( (point.z != 0) ? (point.z * distance / centerDistance) : 0 )
		);
	}

	Vector3 rotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle) {
		return angle * (point - pivot) + pivot;
	}

	Vector3 placeOnSphere(Vector3 point) {
		float centerDistance = Vector3.Distance(Vector3.zero, player.transform.position);

		return new Vector3(
			point.x * radius / centerDistance,
			point.y * radius / centerDistance,
			point.z * radius / centerDistance
		);
	}

	// Use this for initialization
	void Start () {
		radius = icosaedron.GetComponent<Generator>().radius;

		player.transform.position = new Vector3( 0,        0, radius - 0.5f    );
		camera.transform.position = new Vector3( 0, -3.5355f, radius - 3.5355f );

		if (cameraMode == CameraMode.ORBIT) {
			rotation = Quaternion.LookRotation(player.transform.position - camera.transform.position, Vector3.zero - camera.transform.position);
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
