#pragma strict
var defaultCamera : GameObject;
var spherePlayer  : GameObject;

var step : float;
var leadLength : float;
var cameraOffsetX : float;
var cameraOffsetZ : float;

var up    : boolean;
var down  : boolean;
var left  : boolean;
var right : boolean;

function Start () {
	step = 0.5;
	leadLength = 10.0;
	
	defaultCamera = GameObject.Find("Camera");
	spherePlayer  = GameObject.Find("SpherePlayer");
	
	cameraOffsetX = defaultCamera.transform.position.x - spherePlayer.transform.position.x;
	cameraOffsetZ = defaultCamera.transform.position.z - spherePlayer.transform.position.z;
}

var rotateFlag : boolean = false;
function Update () {
	var cameraPos : Vector3 = defaultCamera.transform.position;
	var playerPos : Vector3 = spherePlayer.transform.position;
	
	//Camera folow player
	
	if (!rotateFlag) {
		cameraPos.z = cameraOffsetZ + playerPos.z;
		cameraPos.x = cameraOffsetX + playerPos.x;
	} else {
		cameraOffsetX = cameraPos.x - playerPos.x;
		cameraOffsetZ = cameraPos.z - playerPos.z;
		
		rotateFlag = false;
	}
	
	//Apply camera controls transforms
	
	if (left || right || up || down) {
		var x = ( (up)   ? 1 : 0 ) + ( (down)  ? -1 : 0 );
		var z = ( (left) ? 1 : 0 ) + ( (right) ? -1 : 0 );
		
		cameraPos = rotateAroundPoint(cameraPos, playerPos, Quaternion.Euler(0, z, x));
		rotateFlag = true;
	}
	
	//Save changes
	
	defaultCamera.transform.position = cameraPos;
	
	//Look at player
	
	defaultCamera.transform.LookAt(playerPos);
}

private function rotateAroundPoint(point : Vector3, pivot : Vector3, angle : Quaternion) : Vector3 {
	return angle * ( point - pivot ) + pivot;
}


public function moveUp(flag : boolean) {
	up = flag;
}

public function moveDown(flag : boolean) {
	down = flag;
}

public function moveLeft(flag : boolean) {
	left = flag;
}

public function moveRight(flag : boolean) {
	right = flag;
}