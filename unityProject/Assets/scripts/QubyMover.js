#pragma strict

var spherePlayer : GameObject;
var step : float;
var noAnimateRadius : float;

function Start () {
	spherePlayer = GameObject.Find("SpherePlayer");
	step = 0.1;
	noAnimateRadius = 5;
}

function Update () {
	var distanse = distanseToPlayer(1);
	var offset = (distanse > noAnimateRadius) ? (distanse - noAnimateRadius) / 2 : 0; 
	
	transform.position.y = getNewY(offset);
}

function distanseToPlayer(pow : int) : float {
	var n : float = Mathf.Pow(10.0, pow);
	
	return Mathf.Round( Mathf.Sqrt(
		Mathf.Pow(transform.position.x - spherePlayer.transform.position.x, 2) + Mathf.Pow(transform.position.z - spherePlayer.transform.position.z, 2)
	) * n ) / n;
}

function getNewY(offset : float) : float {
	var res : float = 0;
	
	if ( Mathf.Abs(offset + transform.position.y) > step) {
		res = ( -offset > transform.position.y) ? step : -step;
	} else {
		res = -1 * (offset + transform.position.y);
	}
	
	return transform.position.y + res;
}