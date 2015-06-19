using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AndroidCamera : MonoBehaviour {

	public GameObject camera;
	public GameObject icosaedron;
	public GameObject player;

	private float radius;

	enum CameraMode { ORIGINAL, FREE, FREE_ORBIT, ORBIT };

	CameraMode cameraMode = CameraMode.ORBIT;//CameraMode.FREE;

	Vector3 currentRotation = new Vector3(0, 0, 0);
	Vector3 position = Vector3.zero;
	Quaternion rotation = Quaternion.identity;

	RaycastHit hit = new RaycastHit();
    Ray ray;

    ArrowsUI moveArrows;
    ArrowsUI lookArrows;

    static Text logTextField;
    static Logger Log;
	
	GameObject moveArrowsPanel;
    GameObject lookArrowsPanel;

    class Logger {
    	Text textField;

    	int length;
    	string[] messages;

		int index = 0;

    	public Logger(Text field) {
			this.length = 7; //because i can
			this.messages = new string[this.length];
    		this.textField = field;
    	}

    	public void print(string message) {
			this.index++;

    		for (int i = 1; i < length; i++) {
    			messages[i] = messages[i - 1];
    		}
    		messages[0] = message;

    		this.textField.text = formOutput();
		}

		string formOutput() {
			string res = "";

			for (int i = 0; i < length; i++) {
				if (messages[i] != "") {
					res += (this.index - i) + ") " + messages[i] + "\n";
				} else {
					res += "";
				}
			}

			return res;
		}
    }

    class MyOwnTouch {
    	public int fingerId;

    	public Vector3 startPos;
    	public Vector3 currentPos;

    	public bool up    = false;
    	public bool right = false;
    	public bool down  = false;
    	public bool left  = false;

		public string moveType = "";

		public MyOwnTouch(Touch touch, GameObject moveAP, GameObject moveLP) {
    		this.fingerId = touch.fingerId;

    		this.startPos   = touch.position;
    		this.currentPos = touch.position;

    		this.moveType = "none";

			if (isIn(touch.position, moveAP)) {
				this.moveType = "move";
			}

			if (isIn(touch.position, moveLP)) {
				this.moveType = "look";
			}
    	}

    	//only for debug
    	public MyOwnTouch(Vector3 pos, GameObject moveAP, GameObject moveLP) {
			this.fingerId = 10;

    		this.startPos   = pos;
    		this.currentPos = pos;

    		this.moveType = "none";

			if (isIn(pos, moveAP)) {
				this.moveType = "move";
			}

			if (isIn(pos, moveLP)) {
				this.moveType = "look";
			}

			Log.print("AndroidCamera | MyOwnTouch(debug) | moveType : " + this.moveType);
    	}

    	public void move(Vector3 newPos) {
    		this.currentPos = newPos;

    		float distance = Vector3.Distance(this.startPos, this.currentPos);

    		if (distance > 1.0f) {
    			this.up    = (this.currentPos.y - this.startPos.y   > 15.0f);
    			this.down  = (this.startPos.y   - this.currentPos.y > 15.0f);
    			this.left  = (this.startPos.x   - this.currentPos.x > 15.0f);
    			this.right = (this.currentPos.x - this.startPos.x   > 15.0f);
    		} else {
    			this.up    = false;
    			this.down  = false;
    			this.left  = false;
    			this.right = false;
    		}
    	}

    	private bool isIn(Vector3 dot, GameObject polygon) {
    		bool res = false;

    		Vector3 polyPos  = polygon.transform.position;
			Rect polySize = polygon.GetComponent<RectTransform>().rect;

			if ( dot.x > polyPos.x && dot.x < (polyPos.x + polySize.width ) &&
			     dot.y > polyPos.y && dot.y < (polyPos.y + polySize.height) ) {
				res = true;
			}

    		return res;
    	}
    }

    class ArrowsUI {
    	public Image up    = null;
    	public Image right = null;
    	public Image down  = null;
    	public Image left  = null;

    	public ArrowsUI(string type) {
    		this.up    = GameObject.Find(type + "_t_arrow").GetComponent<Image>();
    		this.right = GameObject.Find(type + "_r_arrow").GetComponent<Image>();
    		this.down  = GameObject.Find(type + "_b_arrow").GetComponent<Image>();
    		this.left  = GameObject.Find(type + "_l_arrow").GetComponent<Image>();
    	}

    	public void setDirection(bool up, bool right, bool down, bool left) {
			this.up.color    = up    ? Color.red : Color.black;
			this.right.color = right ? Color.red : Color.black;
			this.down.color  = down  ? Color.red : Color.black;
			this.left.color  = left  ? Color.red : Color.black;
    	}
    }

    List<MyOwnTouch> touchList = new List<MyOwnTouch>();
    List<MyOwnTouch> newTouchList;

    Quaternion plusOneDegree, minusOneDegree;
    float cameraOrbitAngle = Mathf.PI / 90.0f;

	void moveCamera() {
		bool moveUp    = false;
		bool moveRight = false;
		bool moveDown  = false;
		bool moveLeft  = false;

		bool lookUp    = false;
		bool lookRight = false;
		bool lookDown  = false;
		bool lookLeft  = false;

		foreach (MyOwnTouch ownTouch in touchList) {
			if (ownTouch.moveType == "move") {
				moveUp    = moveUp    || ownTouch.up;
				moveRight = moveRight || ownTouch.right;
				moveDown  = moveDown  || ownTouch.down;
				moveLeft  = moveLeft  || ownTouch.left;
			} else if (ownTouch.moveType == "look") {
				lookUp    = lookUp    || ownTouch.up;
				lookRight = lookRight || ownTouch.right;
				lookDown  = lookDown  || ownTouch.down;
				lookLeft  = lookLeft  || ownTouch.left;
			}
		}
		
		if (moveLeft) {
			camera.transform.position = rotateAroundVector(player.transform.position,  cameraOrbitAngle) * camera.transform.position;
			rotation.SetLookRotation(player.transform.position - camera.transform.position, Vector3.zero - camera.transform.position);
		}
		if (moveRight) {
			camera.transform.position = rotateAroundVector(player.transform.position,  -cameraOrbitAngle) * camera.transform.position; 
			rotation.SetLookRotation(player.transform.position - camera.transform.position, Vector3.zero - camera.transform.position);
		}

		if (moveUp) {
			Vector3 normal = Vector3.Cross(Vector3.zero - player.transform.position, Vector3.zero - camera.transform.position);
			Quaternion newRot = rotateAroundVector(normal, -Mathf.PI / 180.0f);
			player.transform.position = newRot * player.transform.position;
			camera.transform.position = newRot * camera.transform.position;

			rotation.SetLookRotation(player.transform.position - camera.transform.position, Vector3.zero - camera.transform.position);
		}

		if (moveDown) {
			Vector3 normal = Vector3.Cross(Vector3.zero - player.transform.position, Vector3.zero - camera.transform.position);
			Quaternion newRot = rotateAroundVector(normal, Mathf.PI / 180.0f);
			player.transform.position = newRot * player.transform.position;
			camera.transform.position = newRot * camera.transform.position;

			rotation.SetLookRotation(player.transform.position - camera.transform.position, Vector3.zero - camera.transform.position);
		}

		float lookX = 0;
		float lookY = 0;

		if (lookUp)    lookY += 5.0f;
		if (lookRight) lookX += 5.0f;
		if (lookDown)  lookY -= 5.0f;
		if (lookLeft)  lookX -= 5.0f;

		rotation *= Quaternion.Euler(lookY, lookX, 0);

		/*
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
		*/

		camera.transform.rotation = rotation;
		moveArrows.setDirection(moveUp, moveRight, moveDown, moveLeft);
		moveArrows.setDirection(lookUp, lookRight, lookDown, lookLeft);
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

		moveArrowsPanel = GameObject.Find("move_arrows");
    	lookArrowsPanel = GameObject.Find("look_arrows");

		moveArrows = new ArrowsUI("move");
		lookArrows = new ArrowsUI("look");

		logTextField = GameObject.Find("log_text").GetComponent<Text>();
		Log = new Logger(logTextField);
		
		Log.print("AndroidCamera | Start");
	}
	
	// Update is called once per frame
	void Update () {
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) {
				touchList.Add(new MyOwnTouch(touch, moveArrowsPanel, lookArrowsPanel));
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

		//only for debug
		if (Input.GetMouseButton(0)) {
			//Log.print("AndroidCamera | Update | Touch added in : " + Input.mousePosition);
			touchList.Add(new MyOwnTouch(Input.mousePosition, moveArrowsPanel, lookArrowsPanel));
		}

		moveCamera();
	}
}
