#pragma strict

var spherePlayer : GameObject;

function Start () {
	spherePlayer = GameObject.Find("SpherePlayer");
}

function Update () {
	transform.position.x = spherePlayer.transform.position.x;
	transform.position.z = spherePlayer.transform.position.z;
}