using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HeightGenerator : MonoBehaviour {
	public Texture2D heightImage;

	public float upperLimit;
	public float lowerLimit;

	void Start () {

	}

	void Update () {

	}

	public void GenerateHeight (DataNode[,] nodes) {
		float floor = 0;
		float ceiling = 0;
		for (int x = 0; x < heightImage.width; x++) {
			for (int y = 0; y < heightImage.height; y++) {
				int color = HexHelper.Color2Hex (heightImage.GetPixel (x, y));
				float avg = Hex2Height (color);
				if (avg > ceiling)
					ceiling = avg;
				if (avg < floor)
					floor = avg;
			}
		}
	}

	public static float Hex2Height (int hex) {
		float r = hex & 0xff0000 / 0x010000;
		float g = hex & 0xff00 / 0x0100;
		float b = hex & 0xff;
		return (r + g + b) / 3;
	}
}

//This neeeds a do-over.
/* public class HeightGenerator : MonoBehaviour {
	public Texture2D heightImg;

	public float heightMod = 0.4f;

	[Range (-128f, 128f)]
	public float heightOffset = -51f;

	public bool shouldSmooth = true;

	[Range (0, 6)]
	public int smoothIterations = 2;

	[Range (0f, 100f)]
	public float smoothDiff = 0.5f;

	void Start () {
		
	}

	void Update () {

	}

	public void GenerateHeight (HexGrid grid) { // (out HexGrid grid) {
		float[,] hXY = new float [heightImg.width, heightImg.height];
		for (int x = 0; x < heightImg.width; x++) {
			for (int y = 0; y < heightImg.height; y++) {
				int color = HexHelper.Color2Hex (heightImg.GetPixel (x, y));
				hXY [x, y] = HeightFromColor (color);
			}
		}
		if (grid.grid.GetLength (0) != hXY.GetLength (0) || grid.grid.GetLength (1) != hXY.GetLength (1))
			Debug.LogError ("Length of hexgrid not equal to height map's length!");
		for (int x = 0; x < grid.grid.GetLength (0); x++) {
			for (int y = 0; y < grid.grid.GetLength (1); y++) {
				Hex hex = grid.grid [x, y];
				float height = hXY [x, y];
				hex.height = height;
				hex.topLeft.height = height;
				hex.topMid.height = height;
				hex.topRight.height = height;
				hex.bottomRight.height = height;
				hex.bottomMid.height = height;
				hex.bottomLeft.height = height;
				hex.center.height = height;
			}
		}
		if (shouldSmooth) {
			for (int i = 0; i < smoothIterations; i++) {
				SmoothMap (grid);
			}
		}
	}

	//TODO: Make this work.
	//Smoothing from bottom left to top right, only using topmid, topright, and bottomright nodes

	public void SmoothMap (HexGrid grid) {
		if (grid.grid.GetLength (0) <= 2 || grid.grid.GetLength (1) <= 2)
			Debug.LogError ("Grid lengths do not allow for smoothing.");
		
		for (int x = 1; x < grid.grid.GetLength (0) - 2; x++) {
			for (int y = 1; y < grid.grid.GetLength (1) - 1; y++) {
				float ctr = grid.grid [x, y].center.height;
				float ctrTop, ctrRight, ctrBott = 0;
				if (x % 2 == 0) {
					ctrTop = (grid.grid [x - 1, y].center.height + grid.grid [x + 1, y].center.height) / 2;
					ctrRight = (grid.grid [x + 1, y].center.height + grid.grid [x + 2, y].center.height) / 2;
					ctrBott = (grid.grid [x + 1, y - 1].center.height + grid.grid [x + 2, y].center.height) / 2;
				} else {
					ctrTop = (grid.grid [x - 1, y + 1].center.height + grid.grid [x + 1, y].center.height) / 2;
					ctrRight = (grid.grid [x + 1, y + 1].center.height + grid.grid [x + 2, y].center.height) / 2;
					ctrBott = (grid.grid [x + 1, y].center.height + grid.grid [x + 2, y].center.height) / 2;
				}
				grid.grid [x, y].topMid.height = ctr + ctrTop / 2;
				grid.grid [x, y].topRight.height = ctr + ctrRight / 2;
				grid.grid [x, y].bottomRight.height = ctr + ctrBott / 2;
			}
		}
	}
	public void SmoothMap (HexGrid grid) { // (out HexGrid grid) {
		if (grid.hexMap.GetLength (0) <= 1 || grid.hexMap.GetLength (1) <= 1)
			Debug.LogError ("Grid lengths do not allow for smoothing.");
		for (int x = 1; x < grid.hexMap.GetLength (0) - 1; x++) {
			for (int y = 1; y < grid.hexMap.GetLength (1) - 1; y++) {
				Hex hex = grid.hexMap [x, y];
				if (x % 2 == 0) {
					hex.topLeft.height = (hex.topMid.height + hex.bottomLeft.height + grid.hexMap [x - 1, y].bottomLeft.height) / 3;
					hex.topMid.height = (hex.topLeft.height + hex.topRight.height + grid.hexMap [x - 1, y].topRight.height) / 3;
					hex.topRight.height = (hex.bottomRight.height + hex.topMid.height + grid.hexMap [x + 1, y].bottomRight.height) / 3;
					hex.bottomRight.height = (hex.bottomMid.height + hex.topRight.height + grid.hexMap [x + 1, y - 1].height) / 3;
					hex.bottomMid.height = (hex.bottomRight.height + hex.bottomLeft.height + grid.hexMap [x - 1, y - 1].height) / 3;
					hex.bottomLeft.height = (hex.bottomMid.height + hex.topLeft.height + grid.hexMap [x - 1, y - 1].height) / 3;
					hex.center.height = (hex.topLeft.height + hex.topMid.height + hex.topRight.height
						+ hex.bottomRight.height + hex.bottomMid.height + hex.bottomLeft.height) / 6;
				} else {
					hex.topLeft.height = (hex.bottomLeft.height + hex.topMid.height + grid.hexMap [x - 1, y + 1].bottomLeft.height) / 3;
					hex.topMid.height = (hex.topLeft.height + hex.topRight.height + grid.hexMap [x - 1, y + 1].topRight.height) / 3;
					hex.topRight.height = (hex.bottomRight.height + hex.topMid.height + grid.hexMap [x + 1, y + 1].bottomRight.height) / 3;
					hex.bottomRight.height = (hex.topRight.height + hex.bottomMid.height + grid.hexMap [x + 1, y].topRight.height) / 3;
					hex.bottomMid.height = (hex.bottomRight.height + hex.bottomLeft.height + grid.hexMap [x + 1, y].bottomLeft.height) / 3;
					hex.bottomLeft.height = (hex.bottomMid.height + hex.topLeft.height + grid.hexMap [x - 1, y].topLeft.height) / 3;
					hex.center.height = (hex.topLeft.height + hex.topMid.height + hex.topRight.height + hex.bottomRight.height
						+ hex.bottomMid.height + hex.bottomLeft.height) / 6;
				}
			}
		}
	}

	public float HeightFromColor (int hex) {
		float r = hex & 0xff0000 / 0x10000;
		float g = hex & 0xff00 / 0x100;
		float b = hex & 0xff;
		return heightOffset - ((r + g + b) / 3) * heightMod;
	 }
} */
