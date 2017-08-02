using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Chunking!

public class MeshGenerator : MonoBehaviour {
	public GameObject chunkPrefab;

	[Range (1f, 10f)]
	public float size = 1.0f;

	[Range (1, 10000)]
	public int hexPerChunk = 10000;

	void Start () {
		
	}

	void Update () {

	}

	public void GenerateChunks (List<HexCollection> collections) {
		if (collections.Count == 1) {
			GameObject obj = Instantiate (chunkPrefab);
			Chunk c = chunkPrefab.GetComponent<Chunk> ();
			c.GenerateMesh ();
		} else {

		}
	}
}

/* public class HexGrid {
	public Hex[,] grid;

	public HexGrid (Pixel[,] pixels, float size) {
		GenerateHexGrid (pixels, size);
	}

	public HexGrid () {

	}

	public void GenerateHexGrid (Pixel[,] pixels, float size) {
		int width = pixels.GetLength (0);
		int height = pixels.GetLength (1);

		grid = new Hex[width, height];
		float conX = Mathf.Cos (Mathf.PI / 6);
		float conY = Mathf.Sin (Mathf.PI / 6);

		for (int x = 0; x < width; x++) {
			float posX = x * size * conX;
			for (int y = 0; y < height; y++) {
				float posY = y * 2 * (1 + conY) + size * (x % 2) * (1 + conY);
				Vector3 pos = new Vector3 (posX, 0, posY);

				Node TL, TM, TR, BR, BM, BL;
				Node C = new Node (pos);

				//TODO: Get change all indexes which use the '+' operator so width or height can equal 1 in extreme special cases.
				TR = new Node (posX + conX * size, 0, posY + conY * size);
				BR = new Node (posX + conX * size, 0, posY - conY * size);

				if (x == 0) {
					TL = new Node (posX - conX * size, 0, posY + conY * size);
					TM = new Node (posX, 0, posY + size);
					BM = new Node (posX, 0, posY - size);
					BL = new Node (posX - conX * size, 0, posY - conY * size);
				} else if (x % 2 == 1 && y < height - 1) {
					TL = grid [x - 1, y + 1].bottomMid; //Rid this of y + 1?
					TM = grid [x - 1, y + 1].bottomRight; //Rid this of y + 1?
					BM = grid [x - 1, y].topRight;
					BL = grid [x - 1, y].topMid;
				} else if (x == 1 && y == height - 1) {
					TL = new Node (posX - conX * size, 0, posY + conY * size);
					TM = new Node (posX, 0, posY + size);
					BM = grid [x - 1, y].topRight;
					BL = grid [x - 1, y].topMid;
				} else if (x % 2 == 1 && y == height - 1) {
					TL = grid [x - 2, y].topRight;
					TM = new Node (posX, 0, posY + size);
					BM = grid [x - 1, y].topRight;
					BL = grid [x - 1, y].topMid;
				} else if (x % 2 == 0 && y == 0) {
					TL = grid [x - 1, y].bottomMid;
					TM = grid [x - 1, y].bottomRight;
					BM = new Node (posX, 0, posY - size);
					BL = grid [x - 2, y].bottomRight;
				} else {
					TL = grid [x - 1, y].bottomMid;
					TM = grid [x - 1, y].bottomRight;
					BM = grid [x - 1, y - 1].topRight;
					BL = grid [x - 1, y - 1].topMid;
				}

				grid [x, y] = new Hex (C, TL, TM, TR, BR, BM, BL, pixels[x, y], 0);
			}
		}
	}
}

public class Hex {
	public Node topLeft, topMid, topRight, bottomRight, bottomMid, bottomLeft, center;

	public Pixel pixel;

	public float height;

	public Hex (Node _center, Node _topLeft, Node _topMid, Node _topRight, Node _bottomRight,
		Node _bottomMid, Node _bottomLeft, Pixel pix, float h) {
		this.center = _center;
		this.topLeft = _topLeft;
		this.topMid = _topMid;
		this.topRight = _topRight;
		this.bottomRight = _bottomRight;
		this.bottomMid = _bottomMid;
		this.bottomLeft = _bottomLeft;
		this.pixel = pix;
		this.height = h;
	}
}

public class Node {
	public Vector3 position;
	public float height;
	public int vertexIndex = -1;

	public Node (Vector3 pos, float _height = 0) {
		this.position = pos;
		this.height = _height;
	}

	public Node (float x, float y, float z) {
		this.position = new Vector3 (x, y, z);
		this.height = y;
	}

	public Vector3 GetPosition () {
		return this.position + Vector3.up * height;
	}
} */