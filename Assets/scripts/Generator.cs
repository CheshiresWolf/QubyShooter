using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Generator : MonoBehaviour {
	//Set data from editor
	public GameObject parent;
	public GameObject masterPrefab;
	public Mesh masterMesh;
	public float radius;
	public float perspFactor;

	//Local data
	Matrix4x4[] matrixes;
	Triangles[] faces;

	//Subdivision degree
	public int subDegree = 1;

	//Custom types
	class Triangles {
		public Vector3 v1;
		public Vector3 v2;
		public Vector3 v3;

		public Triangles(Vector3 v1, Vector3 v2, Vector3 v3) {
			this.v1 = v1;
			this.v2 = v2;
			this.v3 = v3;
		}
	}

	class Piramide {
		public Vector3 v1;
		public Vector3 v2;
		public Vector3 v3;
		public Vector3 v4;

		public Piramide(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
			this.v1 = v1;
			this.v2 = v2;
			this.v3 = v3;
			this.v4 = v4;
		}
	}

	//=====================<Init>=====================

	void initIcosaedron() {
		float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;
		
		Vector3[] points = new Vector3[12];
		
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
		
		faces = new Triangles[20];
		
		// 5 faces around point 0
		faces[0] = new Triangles( points[0], points[11], points[5]  );
		faces[1] = new Triangles( points[0], points[5],  points[1]  );
		faces[2] = new Triangles( points[0], points[1],  points[7]  );
		faces[3] = new Triangles( points[0], points[7],  points[10] );
		faces[4] = new Triangles( points[0], points[10], points[11] );
		
		// 5 adjacent faces 
		faces[5] = new Triangles( points[1],  points[5],  points[9] );
		faces[6] = new Triangles( points[5],  points[11], points[4] );
		faces[7] = new Triangles( points[11], points[10], points[2] );
		faces[8] = new Triangles( points[10], points[7],  points[6] );
		faces[9] = new Triangles( points[7],  points[1],  points[8] );
		
		// 5 faces around point 3
		faces[10] = new Triangles( points[3], points[9], points[4] );
		faces[11] = new Triangles( points[3], points[4], points[2] );
		faces[12] = new Triangles( points[3], points[2], points[6] );
		faces[13] = new Triangles( points[3], points[6], points[8] );
		faces[14] = new Triangles( points[3], points[8], points[9] );
		
		// 5 adjacent faces 
		faces[15] = new Triangles( points[4], points[9], points[5]  );
		faces[16] = new Triangles( points[2], points[4], points[11] );
		faces[17] = new Triangles( points[6], points[2], points[10] );
		faces[18] = new Triangles( points[8], points[6], points[7]  );
		faces[19] = new Triangles( points[9], points[8], points[1]  );
	}

	void subdivide() {
		Triangles[] newFaces = new Triangles[faces.Length * 4];

		for (int i = 0; i < faces.Length; i++) {
			Vector3 a = placeOnSphere(getMiddlePoint(faces[i].v1, faces[i].v2));
			Vector3 b = placeOnSphere(getMiddlePoint(faces[i].v2, faces[i].v3));
			Vector3 c = placeOnSphere(getMiddlePoint(faces[i].v3, faces[i].v1));
			
			// replace triangle by 4 triangles
			newFaces[i * 4 + 0] = new Triangles(faces[i].v1, a, c);	
			newFaces[i * 4 + 1] = new Triangles(faces[i].v2, b, a);
			newFaces[i * 4 + 2] = new Triangles(faces[i].v3, c, b);
			newFaces[i * 4 + 3] = new Triangles(		  a, b, c);
		}

		faces = newFaces;
	}

	//====================</Init>=====================

	//===================<Matrixes>===================

	void calcMatrixes() {
		matrixes = new Matrix4x4[faces.Length];

		Piramide p1 = new Piramide(
			new Vector3(-0.5f, 0.0f, -Mathf.Sqrt(3.0f) / 6.0f ),
			new Vector3( 0.5f, 0.0f, -Mathf.Sqrt(3.0f) / 6.0f ),
			new Vector3( 0.0f, 0.0f,  Mathf.Sqrt(3.0f) / 3.0f ),
			new Vector3( 0.0f, 1.0f,                     0.0f )
		);

		for (int i = 0; i < faces.Length; i++) {
			Piramide p2 = new Piramide(
				faces[i].v1,
				faces[i].v2,
				faces[i].v3,
				getV4(faces[i])
			);

			matrixes[i] = getMatrix(p1, p2);
		}
	}

	Matrix4x4 getMatrix(Piramide p1, Piramide p2) {
		
		Matrix4x4 result = new Matrix4x4();
		
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

	Vector4 getRow(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float c1, float c2, float c3, float c4) {
		
		float A = c1;
		float B = v1.x;
		float C = v1.y;
		float D = v1.z;
		float E = c2;
		float F = v2.x;
		float G = v2.y;
		float H = v2.z;
		float I = c3;
		float J = v3.x;
		float K = v3.y;
		float L = v3.z;
		float M = c4;
		float N = v4.x;
		float O = v4.y;
		float P = v4.z;
		
		float x = (
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
		
		float y = (
			(L - P) * (E - M) + (P - H) * (I - M) +
			x * ( (L - P) * (N - F) + (P - H) * (N - J) )
			) / (
			(G - O) * (L - P) - (P - H) * (O - K)
		);
		
		float z = ( I - M + x * (N - J) + y * (O - K) ) / (L - P);

		float w = M - N * x - O * y - P * z;
		
		return new Vector4(x, y, z, w);
	}

	//==================</Matrixes>===================	

	//=====================<Draw>=====================

	void drawSceleton() {
		
		//setColor(checkSphere(faces[0].v1), Color.yellow);
		//setColor(checkSphere(faces[0].v2), Color.yellow);
		//setColor(checkSphere(faces[0].v3), Color.yellow);
		
		for (int i = 0; i < faces.Length; i++) {
			checkSphere(faces[i].v1);
			checkSphere(faces[i].v2);
			checkSphere(faces[i].v3);
		}
	}

	void drawSphere(Vector3 pos) {
		GameObject body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		
		body.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		body.transform.position = pos;
		body.transform.parent = parent.transform;
	}

	void drawFaces() {
		setColor(drawTriangle(faces[0], matrixes[0]), Color.green);
		
		for (int i = 1; i < faces.Length; i++) {
			setColor(drawTriangle(faces[i], matrixes[i]), Color.blue);
		}
	}

	GameObject drawTriangle(Triangles face, Matrix4x4 matrix) {
		GameObject body = Instantiate(masterPrefab) as GameObject;
		
		body.transform.parent = parent.transform;

		Mesh mesh = new Mesh();
		
		mesh.vertices  = masterMesh.vertices;
		mesh.triangles = masterMesh.triangles;
		mesh.uv        = masterMesh.uv;
		mesh.normals   = masterMesh.normals;
		
		Vector3[] vertices = mesh.vertices;
		Vector3[] normals  = mesh.normals;
		
		for (int i = 0; i < vertices.Length; i++) {
			vertices[i] = matrix.MultiplyPoint(vertices[i]);
			normals[i]  = matrix.inverse.transpose.MultiplyPoint(normals[i]) * -1;
		}
		mesh.vertices = vertices;
		mesh.normals  = normals;

		MeshFilter meshFilter = body.GetComponent("MeshFilter") as MeshFilter;
		meshFilter.mesh = mesh;
		
		Debug.Log("BuildIcosaedron | drawTriangle | mesh size : " + meshFilter.mesh.vertices.Length);
		
		return body;
	}

	//====================</Draw>=====================

	
	//====================<Utils>=====================

	Vector3 placeOnSphere(Vector3 vector) {
		return placeOnSphere(vector.x, vector.y, vector.z);
	}

	Vector3 placeOnSphere(float x, float y, float z) {
		//center of sphere in (0, 0, 0)
		float localRadius = Mathf.Sqrt(x * x + y * y + z * z);

		return new Vector3(
			x * radius / localRadius,
			y * radius / localRadius,
			z * radius / localRadius
		);
	}

	Vector3 getMiddlePoint(Vector3 p1, Vector3 p2) {		
		return new Vector3(
			(p1.x + p2.x) / 2.0f,
			(p1.y + p2.y) / 2.0f,
			(p1.z + p2.z) / 2.0f
		);
	}

	Vector3 getV4(Triangles face) {
		Vector3 res = getFaceCenter(face, getMiddlePoint(face.v1, face.v2));

		return new Vector3(
			res.x - (res.x / radius),
			res.y - (res.y / radius),
			res.z - (res.z / radius)
		);
	}

	Vector3 getFaceCenter(Triangles face, Vector3 mp) {
		return new Vector3(
			mp.x + (face.v3.x - mp.x) / 3.0f,
			mp.y + (face.v3.y - mp.y) / 3.0f,
			mp.z + (face.v3.z - mp.z) / 3.0f
		);
	}

	List<Vector3> alreadyDrawSphere = new List<Vector3>();
	void checkSphere(Vector3 vec) {
		bool existsFlag = false;

		for (int i = 0; i < alreadyDrawSphere.Count; i++) {
			if (alreadyDrawSphere[i] == vec) {
				existsFlag = true;
				break;
			}
		}

		if (!existsFlag) {
			alreadyDrawSphere.Add(vec);
			drawSphere(vec);
		}
	}

	void setColor(GameObject body, Color color) {
		MeshRenderer gameObjectRenderer = body.GetComponent("MeshRenderer") as MeshRenderer;
		gameObjectRenderer.material.color = color;
	}

	//===================</Utils>=====================

	// Use this for initialization
	void Start () {
		initIcosaedron();

		for (int i = 0; i < subDegree; i++) {
			subdivide();
		}

		calcMatrixes();

		drawSceleton();
		drawFaces();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
