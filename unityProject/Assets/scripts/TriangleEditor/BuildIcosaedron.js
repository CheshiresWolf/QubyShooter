#pragma strict

var faces : TriangleIndices[];
var radius : float;
var masterCopy : GameObject;
var meshMasterCopy : Mesh;
var matrixes : Matrix4x4[];

var perspFactor : float;
var parent : GameObject;

function Start () {
	parent = GameObject.Find("Isocaedron");
	radius = 10.0;
	
	initFaces();
	
	subdivide();
	//subdivide();
	//subdivide();
	//subdivide();
	
	calcTransformMatrixes();
		
	drawFaces();
	
	placeTriangles();
	
	log(matrixes);
}

function Update () {

}

//==============================<Calc>==============================

function placeOnSphere(vectorIn : Vector3) : Vector3 {
	return placeOnSphere(vectorIn.x, vectorIn.y, vectorIn.z);
}

function placeOnSphere(x : float, y : float, z : float) : Vector3 {
	var length : float = Mathf.Sqrt(x * x + y * y + z * z);
	var pos : Vector3 = new Vector3(x * radius / length, y * radius / length, z * radius / length);
	
	return pos;
}

function placeTriangles() {
	var trianglePos;
	
	var firstTriangle = drawTriangle(faces[0], matrixes[0]);
	if (!firstTriangle) print(firstTriangle);
	setColor(firstTriangle, Color.green);

	for (var i : int = 1; i < faces.length; i++) {
		//trianglePos = getFaceCenter(i);
		var bufTriangle = drawTriangle(faces[i], matrixes[i]);//trianglePos);
		setColor(bufTriangle, Color.blue);
	}
}

function getFaceCenter(face : TriangleIndices, mp : Vector3) : Vector3 {
	return new Vector3(
		mp.x + (face.v3.x - mp.x) / 3,
		mp.y + (face.v3.y - mp.y) / 3,
		mp.z + (face.v3.z - mp.z) / 3
	);
}

function getMiddlePoint(p1 : Vector3, p2 : Vector3) : Vector3 {		
	return new Vector3(
		(p1.x + p2.x) / 2.0,
		(p1.y + p2.y) / 2.0,
		(p1.z + p2.z) / 2.0
	);
}

function radiansToDegrees(rad : float) : float {
	return (rad * 180) / Mathf.PI;
}

//==============================<Draw>==============================

function setColor(body : GameObject, color : Color) {
	var gameObjectRenderer : MeshRenderer = body.GetComponent("MeshRenderer");
	gameObjectRenderer.material.color = color;
}

function drawSphere(pos : Vector3) : GameObject {
	var body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			
	body.transform.localScale = Vector3(0.1, 0.1, 0.1);
	body.transform.position = pos;
	body.transform.parent = parent.transform;
	
	return body;
}

function drawTriangle(face : TriangleIndices, matrix : Matrix4x4) : GameObject {
	var mp : Vector3 = getMiddlePoint(face.v1, face.v2);
	var cp : Vector3 = getFaceCenter(face, mp);
	
	var initQuaternion = Quaternion.LookRotation( cp, face.v1 - cp );
	
	var body = Instantiate(
		masterCopy
	) as GameObject;
	
	body.transform.parent = parent.transform;
	
	var child : GameObject = body;

	var mesh : Mesh = new Mesh();

	mesh.vertices  = meshMasterCopy.vertices;
	mesh.triangles = meshMasterCopy.triangles;
	mesh.uv        = meshMasterCopy.uv;
	mesh.normals   = meshMasterCopy.normals;
          
	var vertices : Vector3[] = mesh.vertices;
	var normals  : Vector3[] = mesh.normals;
	
	for (var i = 0; i < vertices.length; i++) {
		vertices[i] = matrix.MultiplyPoint(vertices[i]);
		normals[i]  = matrix.inverse.transpose.MultiplyPoint(normals[i]) * -1;
	}
	mesh.vertices = vertices;
	mesh.normals  = normals;
	child.GetComponent(MeshFilter).mesh = mesh;
	
	Debug.Log("BuildIcosaedron | drawTriangle | mesh size : " + child.GetComponent(MeshFilter).mesh.vertices.length);

	return child;
}

var alreadyDraw = new Array();

//LAST RETURN IS VERY BAD IDEA.
//DELETE IT AFTER DEBUG
function checkBeforeDrawSphere(vec : Vector3) {
	for (var i : int = 0; i < alreadyDraw.length; i++) {
		if ( alreadyDraw[i] == vec ) return;
	}

	alreadyDraw.Push(vec);
	return drawSphere(vec);
};

function drawFaces() {
	
	setColor(checkBeforeDrawSphere(faces[0].v1), Color.yellow);
	setColor(checkBeforeDrawSphere(faces[0].v2), Color.yellow);
	setColor(checkBeforeDrawSphere(faces[0].v3), Color.yellow);

	for (var i : int = 1; i < faces.length; i++) {
		checkBeforeDrawSphere(faces[i].v1);
		checkBeforeDrawSphere(faces[i].v2);
		checkBeforeDrawSphere(faces[i].v3);
	}
}

function log(array : Matrix4x4[]) {
	for (var i = 0; i < array.length; i++) {
		Debug.Log("BuildIcosaedron | log | array[" + i + "] : " + array[i]);
	}
}

function getMatrix(p1: Piramide, p2: Piramide) {

    var result : Matrix4x4 = new Matrix4x4();

    result.SetRow(
        0, getRow(
            p1.v1, p1.v2, p1.v3, p1.v4,
            p2.v1.x, p2.v2.x, p2.v3.x, p2.v4.x
        )
    );

    result.SetRow(
        1, getRow(
            p1.v1, p1.v2, p1.v3, p1.v4,
            p2.v1.y, p2.v2.y, p2.v3.y, p2.v4.y
        )
    );

    result.SetRow(
        2, getRow(
            p1.v1, p1.v2, p1.v3, p1.v4,
            p2.v1.z, p2.v2.z, p2.v3.z, p2.v4.z
        )
    );

    result.SetRow(
        3, getRow(
            p1.v1, p1.v2, p1.v3, p1.v4,
            1, 1, 1, perspFactor
        )
    );

    return result;
}

function getRow(v1 : Vector3, v2 : Vector3, v3 : Vector3, v4 : Vector3, c1 : float, c2 : float, c3 : float, c4 : float) {

    var A = c1;
    var B = v1.x;
    var C = v1.y;
    var D = v1.z;
    var E = c2;
    var F = v2.x;
    var G = v2.y;
    var H = v2.z;
    var I = c3;
    var J = v3.x;
    var K = v3.y;
    var L = v3.z;
    var M = c4;
    var N = v4.x;
    var O = v4.y;
    var P = v4.z;

    var x = (
        (
            (L - P) * (A - M) + (P - D) * (I - M)
        ) * (
            (G - O) * (L - P) - (P - H) * (O - K)
        ) + (
            (L - P) * (O - C) + (P - D) * (O - K)
        ) * (
            (L - P) * (E - M) + (P - H) * (I - M)
        )
    ) / (
        (
            (B - N) * (L - P) - (P - D) * (N - J)
        ) * (
            (G - O) * (L - P) - (P - H) * (O - K)
        ) - (
            (L - P) * (O - C) + (P - D) * (O - K)
        ) * (
            (L - P) * (N - F) + (P - H) * (N - J)
        )
    );

    var y = (
        (L - P) * (E - M) + (P - H) * (I - M) +
        x * ( (L - P) * (N - F) + (P - H) * (N - J) )
    ) / (
        (G - O) * (L - P) - (P - H) * (O - K)
    );

    var z = ( I - M + x * (N - J) + y * (O - K) ) / (L - P);

    var w = M - N * x - O * y - P * z;

    return Vector4(x, y, z, w);
}

function getV4(face : TriangleIndices) {
	var res = getFaceCenter(face, getMiddlePoint(face.v1, face.v2));
	res = new Vector3(
		res.x - (res.x / radius) / 8,
		res.y - (res.y / radius) / 8,
		res.z - (res.z / radius) / 8
	);

	drawSphere(res);

	return res;
}

//==============================<init>==============================

function initFaces() {
	var t = (1.0 + Mathf.Sqrt(5.0)) / 2.0;

	var points = new Vector3[12];
	
	points[0]  = placeOnSphere(-1,  t,  0);
	points[1]  = placeOnSphere( 1,  t,  0);
	points[2]  = placeOnSphere(-1, -t,  0);
	points[3]  = placeOnSphere( 1, -t,  0);
	points[4]  = placeOnSphere( 0, -1,  t);
	points[5]  = placeOnSphere( 0,  1,  t);
	points[6]  = placeOnSphere( 0, -1, -t);
	points[7]  = placeOnSphere( 0,  1, -t);
	points[8]  = placeOnSphere( t,  0, -1);
	points[9]  = placeOnSphere( t,  0,  1);
	points[10] = placeOnSphere(-t,  0, -1);
	points[11] = placeOnSphere(-t,  0,  1);
	
	faces = new TriangleIndices[20];

	// 5 faces around point 0
	faces[0] = new TriangleIndices(0, 11, 5 , points);
	faces[1] = new TriangleIndices(0, 5,  1 , points);
	faces[2] = new TriangleIndices(0, 1,  7 , points);
	faces[3] = new TriangleIndices(0, 7,  10, points);
	faces[4] = new TriangleIndices(0, 10, 11, points);

	// 5 adjacent faces 
	faces[5] = new TriangleIndices(1,  5,  9, points);
	faces[6] = new TriangleIndices(5,  11, 4, points);
	faces[7] = new TriangleIndices(11, 10, 2, points);
	faces[8] = new TriangleIndices(10, 7,  6, points);
	faces[9] = new TriangleIndices(7,  1,  8, points);

	// 5 faces around point 3
	faces[10] = new TriangleIndices(3, 9, 4, points);
	faces[11] = new TriangleIndices(3, 4, 2, points);
	faces[12] = new TriangleIndices(3, 2, 6, points);
	faces[13] = new TriangleIndices(3, 6, 8, points);
	faces[14] = new TriangleIndices(3, 8, 9, points);

	// 5 adjacent faces 
	faces[15] = new TriangleIndices(4, 9, 5 , points);
	faces[16] = new TriangleIndices(2, 4, 11, points);
	faces[17] = new TriangleIndices(6, 2, 10, points);
	faces[18] = new TriangleIndices(8, 6, 7 , points);
	faces[19] = new TriangleIndices(9, 8, 1 , points);
}

function subdivide() {
    var faces2 = new TriangleIndices[faces.length * 4];
    
    for (var i : int = 0; i < faces.length; i++) {
        var a : Vector3 = placeOnSphere(getMiddlePoint(faces[i].v1, faces[i].v2));
        var b : Vector3 = placeOnSphere(getMiddlePoint(faces[i].v2, faces[i].v3));
        var c : Vector3 = placeOnSphere(getMiddlePoint(faces[i].v3, faces[i].v1));

        // replace triangle by 4 triangles
        faces2[i * 4 + 0] = new TriangleIndices(faces[i].v1, a, c);	
        faces2[i * 4 + 1] = new TriangleIndices(faces[i].v2, b, a);
        faces2[i * 4 + 2] = new TriangleIndices(faces[i].v3, c, b);
        faces2[i * 4 + 3] = new TriangleIndices(		  a, b, c);
    }
    
    faces = faces2;    
}

function calcTransformMatrixes() {
	matrixes = new Matrix4x4[faces.length];

	for (var i : int = 0; i < faces.length; i++) {
		var p1 = new Piramide();

		p1.v1 = new Vector3(-0.5, 0, -Mathf.Sqrt(3) / 6 );
		p1.v2 = new Vector3(0.5,  0, -Mathf.Sqrt(3) / 6 );
		p1.v3 = new Vector3(0,    0, Mathf.Sqrt(3) / 3  );
		p1.v4 = new Vector3(0,    1, 0                  );

		var p2 = new Piramide();

		p2.v1 = faces[i].v1;
		p2.v2 = faces[i].v2;
		p2.v3 = faces[i].v3;
		p2.v4 = getV4(faces[i]);

		/*
		var transformMatrix = new Matrix4x4();
		var rotation = Quaternion.identity;
		// Assign a rotation 30 degrees around the y axis
		rotation.eulerAngles = Vector3(90, 0, 0);
		transformMatrix.SetTRS(Vector3.zero, rotation, Vector3.one);
		*/
		matrixes[i] = getMatrix(p1, p2);// * transformMatrix;
	}
}

//==============================<Class>==============================

public class TriangleIndices {
	public var v1 : Vector3;
	public var v2 : Vector3;
	public var v3 : Vector3;

	public function TriangleIndices(v1 : Vector3, v2 : Vector3, v3 : Vector3) {
	    this.v1 = v1;
	    this.v2 = v2;
	    this.v3 = v3;
	}
	
	public function TriangleIndices(v1 : int, v2 : int, v3 : int, points : Vector3[]) {
	    this.v1 = points[v1];
	    this.v2 = points[v2];
	    this.v3 = points[v3];
	}
}

public class TriangleOptions {
	public var pos : Vector3;
	public var rot : Quaternion;

	public function TriangleOptions(pos : Vector3, rot : Quaternion) {
		this.pos = pos;
		this.rot = rot;
	}
}

public class Piramide {
	public var v1 : Vector3;
	public var v2 : Vector3;
	public var v3 : Vector3;
	public var v4 : Vector3;
}
