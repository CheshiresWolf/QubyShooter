using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class AndroidCamera : MonoBehaviour {

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

    class MyOwnTouch {
    	public int fingerId;

    	public Vector3 startPos;
    	public Vector3 currentPos;

    	public bool up    = false;
    	public bool right = false;
    	public bool down  = false;
    	public bool left  = false;

    	public MyOwnTouch(Touch touch) {
    		this.fingerId = touch.fingerId;

    		this.startPos   = touch.position;
    		this.currentPos = touch.position;
    	}

    	public void move(Vector3 newPos) {
    		this.currentPos = newPos;

    		float distance = Vector3.Distance(this.startPos, this.currentPos);

    		if (distance > 1.0f) {
    			this.up    = (this.currentPos.z - this.startPos.z   > 5.0f);
    			this.down  = (this.startPos.z   - this.currentPos.z > 5.0f);
    			this.left  = (this.startPos.x   - this.currentPos.x > 5.0f);
    			this.right = (this.currentPos.x - this.startPos.x   > 5.0f);
    		} else {
    			this.up    = false;
    			this.down  = false;
    			this.left  = false;
    			this.right = false;
    		}
    	}
    }

    List<MyOwnTouch> touchList = new List<MyOwnTouch>();
    List<MyOwnTouch> newTouchList;

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
		bool moveUp    = false;
		bool moveRight = false;
		bool moveDown  = false;
		bool moveLeft  = false;

		foreach (MyOwnTouch ownTouch in touchList) {
			moveUp    = moveUp    || ownTouch.up;
			moveRight = moveRight || ownTouch.right;
			moveDown  = moveDown  || ownTouch.down;
			moveLeft  = moveLeft  || ownTouch.left;
		}
		/*
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
		*/
		if (moveLeft) {//Input.GetKey(keyMap.left)) {
			camera.transform.position = rotateAroundVector(player.transform.position,  cameraOrbitAngle) * camera.transform.position;
			rotation.SetLookRotation(player.transform.position - camera.transform.position, Vector3.zero - camera.transform.position);
		}
		if (moveRight) {//Input.GetKey(keyMap.right)) {
			camera.transform.position = rotateAroundVector(player.transform.position,  -cameraOrbitAngle) * camera.transform.position; 
			rotation.SetLookRotation(player.transform.position - camera.transform.position, Vector3.zero - camera.transform.position);
		}

		if (moveUp) {//Input.GetKey(keyMap.forward)) {
			Vector3 normal = Vector3.Cross(Vector3.zero - player.transform.position, Vector3.zero - camera.transform.position);
			Quaternion newRot = rotateAroundVector(normal, -Mathf.PI / 180.0f);
			player.transform.position = newRot * player.transform.position;
			camera.transform.position = newRot * camera.transform.position;

			rotation.SetLookRotation(player.transform.position - camera.transform.position, Vector3.zero - camera.transform.position);
		}

		if (moveDown) {//Input.GetKey(keyMap.back)) {
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

		rotation = Quaternion.LookRotation(
			player.transform.position - camera.transform.position,
			Vector3.zero - camera.transform.position
		);
	}
	
	// Update is called once per frame
	void Update () {
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) {
				touchList.Add(new MyOwnTouch(touch));
			}
			if (touch.phase == TouchPhase.Moved) {
				foreach (MyOwnTouch ownTouch in touchList) {
					if (ownTouch.fingerId == touch.fingerId) {
						ownTouch.move(touch.position);
						break;
					}
				}
			}
			if (touch.phase == TouchPhase.Ended) {
				newTouchList = new List<MyOwnTouch>();

				foreach (MyOwnTouch ownTouch in touchList) {
					if (ownTouch.fingerId != touch.fingerId) {
						newTouchList.Add(ownTouch);
					}
				}

				touchList = newTouchList;
			}
		}

		moveCamera();
	}
}
