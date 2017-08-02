using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {
	public HexCollection hexCollection;

	public Vector3 offset;

	[HideInInspector]
	public List<Vector3> vertices;
	[HideInInspector]
	public List<int> triangles;

	void Start () {

	}

	void Update () {

	}

	public static List<HexCollection> Chunkify (DataNode[,] nodes, int maxSize) {
		
		return new List<HexCollection> ();
	}

	public void GenerateHexCollection (DataNode[,] nodes, float size,
		bool shouldGenerateHeight = true, bool shouldSmoothHeight = true) {
		hexCollection.GenerateGrid (nodes, size, shouldGenerateHeight, shouldSmoothHeight);
	}
	
	#region Chunk and Mesh generation

	public void GenerateMesh () {
		vertices = new List<Vector3> ();
		triangles = new List<int> ();

		for (int x = 0; x < hexCollection.grid.GetLength (0); x++) {
			for (int y = 0; y < hexCollection.grid.GetLength (1); y++) {
				TriangulateHex (hexCollection.grid [x, y]);
			}
		}

		MeshFilter meshFilter = GetComponent<MeshFilter> ();
		if (meshFilter == null)
			Debug.LogError (string.Format ("Chunk instance {0} contains no MeshFilter component.", gameObject.GetInstanceID ()));

		if (GetComponent<MeshRenderer> () == null)
			Debug.LogWarning (string.Format ("Chunk instance {0} has a MeshFilter component but no MeshRenderer component.", gameObject.GetInstanceID ()));

		Mesh mesh = new Mesh ();
		mesh.vertices = vertices.ToArray ();
		mesh.triangles = triangles.ToArray ();
		meshFilter.mesh = mesh;
		mesh.RecalculateNormals ();
	}

	void AssignVertices (params Point[] points) {
		for (int i = 0; i < points.Length; i++) {
			if (points [i].vertexIndex == -1) {
				points [i].vertexIndex = vertices.Count;
				vertices.Add (points [i].position);
			}
		}
	}

	void CreateTriangle (Point a, Point b, Point c) {
		triangles.Add (a.vertexIndex);
		triangles.Add (b.vertexIndex);
		triangles.Add (c.vertexIndex);
	}

	void TriangulateHex (Hex h) {
		AssignVertices (h.center, h.topLeft, h.topMid, h.topRight, h.bottomRight, h.bottomMid, h.bottomLeft);
		CreateTriangle (h.topLeft, h.topMid, h.center);
		CreateTriangle (h.topMid, h.topRight, h.center);
		CreateTriangle (h.topRight, h.bottomRight, h.center);
		CreateTriangle (h.bottomRight, h.bottomMid, h.center);
		CreateTriangle (h.bottomMid, h.bottomLeft, h.center);
		CreateTriangle (h.bottomLeft, h.topLeft, h.center);
	}

	public int CaculateVertNumber () { // A CLEVER METHOD AND PROUD OF IT //
		int x = hexCollection.grid.GetLength (0);
		int y = hexCollection.grid.GetLength (1);
		return (x % 2 == 1) ? (x + (2 * y) * (x + 2) + (x * y)) : (2 * (x + 1) + (x + 2) * (2 * y - 1) + (x * y)); // See notebook for solution explanation
	}

	#endregion
}

#region Hex Data Objects

public class HexCollection {
	public Hex[,] grid;

	public int width;
	public int height;

	public HexCollection () {

	}

	public void GenerateGrid (DataNode[,] nodes, float size, bool shouldGenerateHeight, bool shouldSmoothMap) {
		this.width = nodes.GetLength (0);
		this.height = nodes.GetLength (1);
		this.grid = new Hex[width, height];

		float conX = size * Mathf.Cos (Mathf.PI / 6);
		float conY = size * Mathf.Sin (Mathf.PI / 6);

		for (int x = 0; x < width; x++) {
			float posX = x * size * conX;
			for (int y = 0; y < height; y++) {
				float posY = y * 2 * (1 + conY) + size * (x % 2) * (1 + conY);
				float posZ = nodes [x, y].height;
				Vector3 pos = new Vector3 (posX, posZ, posY);

				Point TL, TM, TR, BR, BM, BL;
				Point C = new Point (pos);

				//TODO: Get change all indexes which use the '+' operator so width or height can equal 1 in extreme special cases.
				TR = new Point (posX + conX * size, posZ, posY + conY * size);
				BR = new Point (posX + conX * size, posZ, posY - conY * size);

				if (x == 0) {
					TL = new Point (posX - conX * size, posZ, posY + conY * size);
					TM = new Point (posX, posZ, posY + size);
					BM = new Point (posX, posZ, posY - size);
					BL = new Point (posX - conX * size, posZ, posY - conY * size);
				} else if (x % 2 == 1 && y < height - 1) {
					TL = grid [x - 1, y + 1].bottomMid; //Rid this of y + 1?
					TM = grid [x - 1, y + 1].bottomRight; //Rid this of y + 1?
					BM = grid [x - 1, y].topRight;
					BL = grid [x - 1, y].topMid;
				} else if (x == 1 && y == height - 1) {
					TL = new Point (posX - conX * size, posZ, posY + conY * size);
					TM = new Point (posX, posZ, posY + size);
					BM = grid [x - 1, y].topRight;
					BL = grid [x - 1, y].topMid;
				} else if (x % 2 == 1 && y == height - 1) {
					TL = grid [x - 2, y].topRight;
					TM = new Point (posX, posZ, posY + size);
					BM = grid [x - 1, y].topRight;
					BL = grid [x - 1, y].topMid;
				} else if (x % 2 == 0 && y == 0) {
					TL = grid [x - 1, y].bottomMid;
					TM = grid [x - 1, y].bottomRight;
					BM = new Point (posX, posZ, posY - size);
					BL = grid [x - 2, y].bottomRight;
				} else {
					TL = grid [x - 1, y].bottomMid;
					TM = grid [x - 1, y].bottomRight;
					BM = grid [x - 1, y - 1].topRight;
					BL = grid [x - 1, y - 1].topMid;
				}

				grid [x, y] = new Hex (C, TL, TM, TR, BR, BM, BL, nodes[x, y]);
			}
		}
	}
}

public class Hex {
	public Point topLeft, topMid, topRight, bottomRight, bottomMid, bottomLeft, center;

	public DataNode dataNode;

	public Hex (Point _topLeft, Point _topMid, Point _topRight, Point _bottomRight, Point _bottomMid, Point _bottomLeft, Point _center,
		DataNode _dataNode) {
		this.topLeft = _topLeft;
		this.topMid = _topMid;
		this.topRight = _topRight;
		this.bottomRight = _bottomRight;
		this.bottomMid = _bottomMid;
		this.bottomLeft = _bottomLeft;
		this.center = _center;

		this.dataNode = _dataNode;
	}
}

public class Point {
	public Vector3 position;
	public int vertexIndex;

	public Point (Vector3 pos) {
		this.position = pos;
		this.vertexIndex = -1;
	}

	public Point (float x, float y, float z) {
		this.position = new Vector3 (x, y, z);
		this.vertexIndex = -1;
	}
}

#endregion