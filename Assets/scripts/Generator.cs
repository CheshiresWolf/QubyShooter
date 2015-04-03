using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

public class Generator : MonoBehaviour {
	//Set data from editor
	public GameObject parent;
	public GameObject masterPrefab;
	public Mesh masterMesh;
	public float radius;
	public float perspFactor;

	public bool showSpheres = true;

	public Mesh[] meshes;

	public GameObject player;
	
	//Local data
	Triangles[] faces;
	Positioner positioner;

	int currentMeshIndex = 0;

	//Subdivision degree
	public int subDegree = 1;

	//Custom types
	class Triangles {
		public Vector3 v1;
		public Vector3 v2;
		public Vector3 v3;

		public int meshIndex = 0;

		public PositionerResult positioner;
		public GameObject body;

		public Triangles(Vector3 v1, Vector3 v2, Vector3 v3) {
			this.v1 = v1;
			this.v2 = v2;
			this.v3 = v3;
		}
	};

	class PositionerResult {
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 scale;
		public Matrix4x4 matrix;

	    public PositionerResult(Vector3 p, Quaternion r, float s, Matrix4x4 m) {
	        position = p;
	        rotation = r;
	        scale    = new Vector3(s, s, s);
	        matrix   = m;
	    }
	};

	class Positioner {
	    float N;
	    float O;
	    float P;

	    float LP;
	    float NJ;
	    float OK;
	    float PH;
	    float PD;
	    float GOLPPHOK;
	    float LPOCPDOK;
	    float LPNFPHNJ;
	    float BNLPPDNJGOLPPHOKLPOCPDOKLPNFPHNJ;

	    public Positioner() {}

	    public Positioner(Vector3 bv1, Vector3 bv2, Vector3 bv3, Vector3 v4) {
	        setSource(bv1, bv2, bv3, v4);
	    }

	    public void setSource(Vector3 bv1, Vector3 bv2, Vector3 bv3, Vector3 v4) {
	        N = v4.x;
	        O = v4.y;
	        P = v4.z;
	        LP = bv3.z - P;
	        NJ = N - bv3.x;
	        OK = O - bv3.y;
	        PH = P - bv2.z;
	        PD = P - bv1.z;
	        GOLPPHOK = (bv2.y - O) * LP - PH * OK;
	        LPOCPDOK = LP * (O - bv1.y) + PD * OK;
	        LPNFPHNJ = LP * (N - bv2.x) + PH * NJ;
	        BNLPPDNJGOLPPHOKLPOCPDOKLPNFPHNJ = (
	            ( (bv1.x - N) * LP - PD * NJ ) * GOLPPHOK - LPOCPDOK * LPNFPHNJ
	        );
	    }


	    public PositionerResult getMatrixForDestination(Vector3 bv1, Vector3 bv2, Vector3 bv3) {
	        Vector3 center = (bv1 + bv2 + bv3) / 3.0f;

	        Quaternion rotation = new Quaternion();
	        rotation.SetLookRotation(
	            bv2 - center,
	            Vector3.Cross(bv3 - bv2, bv1 - bv2)
	        );

	        float scale = (
	            Vector3.Distance(bv1, bv2) +
	            Vector3.Distance(bv2, bv3) +
	            Vector3.Distance(bv3, bv1)
	        ) / 3.0f;

	        Matrix4x4 nm = Matrix4x4.TRS(center, rotation, scale * Vector3.one).inverse;

            float perspFactor = 1 / (1 - 1 / center.magnitude);

			//nm = Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(offsetAngle, Vector3.up), Vector3.one) * nm;

	        Vector3 v1 = nm.MultiplyPoint3x4(bv1);
	        Vector3 v2 = nm.MultiplyPoint3x4(bv2);
	        Vector3 v3 = nm.MultiplyPoint3x4(bv3);
	        Vector3 v4 = nm.MultiplyPoint3x4(
	            center - center.normalized * perspFactor
	        );
	        // todo - mul by scale?
	        //Vector4 v4 = new Vector4(0.0f, 1.0f, 0.0f, 1.0f) / (1.0f - 1.0f / center.magnitude);

	        Matrix4x4 matrix = new Matrix4x4();

	        matrix.SetRow(0, getRow( v1.x, v2.x, v3.x, v4.x        ));
	        matrix.SetRow(1, getRow( v1.y, v2.y, v3.y, v4.y        ));
	        matrix.SetRow(2, getRow( v1.z, v2.z, v3.z, v4.z        ));
	        matrix.SetRow(3, getRow( 1.0f, 1.0f, 1.0f, perspFactor ));

	        return new PositionerResult(center, rotation, scale, matrix);
	    }

	    Vector4 getRow(float A, float E, float I, float M) {

	        float IM = I - M;
	        float LPEMPHIM = LP * (E - M) + PH * IM;

	        float x = (
	            ( LP * (A - M) + PD * IM ) * GOLPPHOK + LPOCPDOK * LPEMPHIM
	        ) / BNLPPDNJGOLPPHOKLPOCPDOKLPNFPHNJ;

	        float y = (LPEMPHIM + x * LPNFPHNJ) / GOLPPHOK;
	        float z = (IM + x * NJ + y * OK) / LP;
	        float w = M - N * x - O * y - P * z;

	        return new Vector4(x, y, z, w);
	    }
	};

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
		float s33 = Mathf.Sqrt(3.0f) / 3.0f;

	    positioner = new Positioner(
	        new Vector3( -0.5f, 0.0f, -s33 / 2.0f ), // -0.288675
	        new Vector3(  0.0f, 0.0f,  s33        ), //  0.577350
	        new Vector3(  0.5f, 0.0f, -s33 / 2.0f ), // -0.288675
	        new Vector3(  0.0f, 1.0f,  0.0f       )
	    );
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
		//setColor(drawTriangle(faces[0], matrixes[0]), Color.green);
		//setColor(drawTriangle(faces[0]), Color.green);
		GameObject[] triangles = new GameObject[faces.Length];

		for (int i = 0; i < faces.Length; i++) {
			//setColor(drawTriangle(faces[i], matrixes[i]), Color.blue);
			//setColor(
			triangles[i] = drawTriangle(faces[i]);
			triangles[i].name = "triangle_" + i;
			//, Color.blue);
		}

		sendTriangles(triangles);
	}

	//GameObject drawTriangle(Triangles face, Matrix4x4 matrix) {
	GameObject drawTriangle(Triangles face) {
		face.body = Instantiate(masterPrefab) as GameObject;
		face.body.transform.parent = parent.transform;

		face.positioner = positioner.getMatrixForDestination(
	        face.v2,
	        face.v1,
	        face.v3
	    );

	    face.body.transform.position   = face.positioner.position;
	    face.body.transform.rotation   = face.positioner.rotation;
	    face.body.transform.localScale = face.positioner.scale;

		return changeMesh(face, masterMesh);
	}

	GameObject changeMesh(Triangles face, Mesh mesh) {
	    Matrix4x4 matrix = face.positioner.matrix;

	    Matrix4x4 invTran = matrix.inverse.transpose;

	    Vector3[] vertices = mesh.vertices;
	    Vector3[] normals  = mesh.normals;

	    for (int i = vertices.Length - 1; i >= 0; --i) {
	        vertices[i] = matrix.MultiplyPoint(vertices[i]);
	        normals[i]  = invTran.MultiplyPoint(normals[i]);
	    }

		MeshFilter mf = face.body.GetComponent("MeshFilter") as MeshFilter;
	    Mesh mfm = mf.mesh;

        mfm.Clear();
	    mfm.vertices  = vertices;
	    mfm.normals   = normals;
		mfm.triangles = mesh.triangles;
		mfm.uv        = mesh.uv;

		return face.body;
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
	
	public void changeMesh(int index) {
		Triangles face = faces[index] as Triangles;
		face.meshIndex = currentMeshIndex;

		changeMesh(face, meshes[currentMeshIndex]);
	}

	public void setMeshIndex(int index) {
		Debug.Log("Generator | setMeshIndex | index : " + index);
		currentMeshIndex = index;
	}

	public void sendTriangles(GameObject[] triangles) {
		player.GetComponent<WorldDestroyer>().setTriangles(triangles);

		Debug.Log("Generator | getTriangles | triangles.Length : " + triangles.Length);
	}

	//===================</Utils>=====================
	
	//====================<File>======================

	public void saveToFile(string name) {
		Debug.Log("Generator | saveToFile");
		string res = "";

		for (int i = 0; i < faces.Length; i++) {
			res += i + ":" + faces[i].meshIndex + ";";
		}
		using (StreamWriter sw = new StreamWriter("Assets/levels/" + name)) {
			sw.Write(res);
		}
	}

	public void loadFromFile(string name) {
		Debug.Log("Generator | loadFromFile");

		string allFile = "";

		try {
			string line;
			StreamReader theReader = new StreamReader("Assets/levels/" + name, Encoding.Default);

			using (theReader) {
				do {
					line = theReader.ReadLine();
					
					if (line != null) {
						allFile += line;
					}
				} while (line != null);

				theReader.Close();
			}
		} catch (Exception e) {
			Debug.Log("Generator | loadFromFile | error : " + e.Message);
		}

		string[] pairs = allFile.Split(';');

		Debug.Log("Generator | loadFromFile | last pair : " + pairs[pairs.Length - 1]);

		for (int i = 0; i < pairs.Length - 1; i++) {
			string[] data = pairs[i].Split(':');
			int faceIndex = int.Parse(data[0]);
			int meshIndex = int.Parse(data[1]);

			if ((faceIndex < faces.Length) && (meshIndex < meshes.Length)) {
				changeMesh(faces[faceIndex], meshes[meshIndex]);
			}
		}
	}
	
	//===================</File>======================
	
	// Use this for initialization
	void Start () {
		initIcosaedron();
		
		for (int i = 0; i < subDegree; i++) {
			subdivide();
		}
		
		calcMatrixes();
		
		if (showSpheres) drawSceleton();
		drawFaces();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
};