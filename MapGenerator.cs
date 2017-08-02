using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
	public Texture2D dataRegion;

	public DataNode[,] pixels;
	
	MeshGenerator meshGen;
	HeightGenerator heightGen;

	void Start () {
		meshGen = GetComponent<MeshGenerator> ();
		if (meshGen == null)
			Debug.LogError ("Mesh Generator component could not be found.");
		heightGen = GetComponent<HeightGenerator> ();
		if (heightGen == null)
			Debug.LogError ("Height Generator component could not be found.");
		GeneratePixels ();
		meshGen.GenerateChunks (Chunk.Chunkify (pixels, 100));
		//meshGen.GenerateGrid (pixels, 1.0f);
		//heightGen.GenerateHeight (meshGen.hexGrid);
		//meshGen.GenerateChunks ();
	}

	void Update () {
		
	}

	void GeneratePixels () {
		pixels = new DataNode [dataRegion.width, dataRegion.height];
		for (int x = 0; x < dataRegion.width; x++) {
			for (int y = 0; y < dataRegion.height; y++) {
				pixels [x, y] = new DataNode (dataRegion.GetPixel (x, y));
			}
		}
	}
}

public class DataNode {
	public Color color;
	public int hex;

	public float height;

	public DataNode (Color col, float _height = 0) {
		this.color = col;
		this.hex = HexHelper.Color2Hex (col);

		this.height = _height;
	}

}