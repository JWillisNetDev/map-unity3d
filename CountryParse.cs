using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class CountryParse : MonoBehaviour {
	public CountryCollection countryCollection;
	public Color tcol;
	public string targPath;
	
	void Start () {
		ParseCountries (targPath);
		Debug.Log (string.Format ("Color of france: {0}", countryCollection.GetByName ("france").color));
	}

	void Update () {

	}

	void OnDrawGizmos () {
		int rgb = HexHelper.Color2Hex (tcol);
		Gizmos.color = HexHelper.Hex2Color (rgb);
		Gizmos.DrawCube (Vector3.zero, Vector3.one * 5);
	}

	void ParseCountries (string path) {
		TextAsset file = Resources.Load (path) as TextAsset;
		var parsed = JSON.Parse (file.ToString ());
		countryCollection = new CountryCollection ();
		for (int i = 0; i < parsed.Count; i++) {
			string name = "";
			int id = 0;
			Color col = Color.white;
			List<int> regions = new List<int> ();

			if (parsed [i] ["name"] != null)
				name = parsed [i] ["name"];
			else
				Debug.LogError (string.Format ("Could not find country name of index {0}", i));
			
			if (parsed [i] ["id"] != null)
				id = HexHelper.Hex2Int (parsed [i] ["id"]);
			else
				Debug.LogError (string.Format ("Could not find country id of index {0}", i));
			
			if (parsed [i] ["color"] != null)
				col = HexHelper.Hex2Color (HexHelper.Hex2Int (parsed [i] ["color"]));
			else
				Debug.LogError (string.Format ("Could not find country color of index {0}", i));
			
			if (parsed [i] ["regions"] != null) {
				regions = new List<int> ();
				for (int j = 0; j < parsed [i] ["regions"].Count; j++) {
					if (parsed [i] ["regions"] [j] != null)
						regions.Add (HexHelper.Hex2Int (parsed [i] ["regions"] [j]));
				}
			} else
				Debug.LogError (string.Format ("Could not find country regions of index {0}", i));
			
			Country c = new Country (name, id, col, regions);
			countryCollection.AddCountry (c);
			//CountryCollection.AddCountry (c);
		}
	}
}

public class HexHelper  {
	public static int Hex2Int (string str) {
		int val = 0;
		if (str.StartsWith ("0x")) {
			str = str.Substring (2);
			str = str.ToLower ();
			for (int i = 0; i < str.Length; i++) {
				if (str [i] >= '0' && str [i] <= '9') {
					val = val * 0x10 + str [i] - '0';
				} else if (str [i] >= 'a' && str [i] <= 'f') {
					val = val * 0x10 + str [i] - 'a' + 10;
				} else {
					Debug.LogError ("Unexpected character encountered in hex code.");
					return 0;
				}
			}
			return val;
		} else {
			Debug.LogError ("Hex must begin with 0x!");
			return 0;
		}
	}

	public static Color Hex2Color (int hex) {
		int r = (hex & 0xff0000) / 0x010000;
		int g = (hex & 0xff00) / 0x0100;
		int b = hex & 0xff;
		//Debug.Log (string.Format ("R: {0:X} .. {0:D} .. {3:F}, G: {1:X} .. {1:D} ..{4:F}, B: {2:X} .. {2:D} .. {5:F}", r, g, b,
		//	r / 255f, g / 255f, b / 255f));
		Color col = new Color (r / 255f, g / 255f, b / 255f);
		return col;
	}

	public static int Color2Hex (Color col) {
		int r = (int)(col.r * 255);
		int g = (int)(col.g * 255);
		int b = (int)(col.b * 255);
		int ret = (r << 16) + (g << 8) + b;
		//Debug.Log (string.Format ("R bitshift: {0:X} - {0:D}. G bitshift: {1:X} - {1:D}. B bitshift: {2:X} - {2:D}",
		//	r << 16, g << 8, b));
		//Debug.Log (string.Format ("R: {0:X} .. {0:D}, G: {1:X} .. {1:D}, B: {2:X} .. {2:D}\nRGB: {3:X6} // {3:D}", r, g, b, ret));
		return ret;
	}
}

public class CountryCollection {
	public Dictionary<int, Country> countries;

	public CountryCollection () {
		countries = new Dictionary<int, Country> ();
	}

	public void AddCountry (Country c) {
		countries.Add (c.id, c);
	}

	public Country GetByID (int id) {
		return countries [id];
	}

	public Country GetByName (string name) {
		Country[] arr = new Country[countries.Values.Count];
		countries.Values.CopyTo (arr, 0);
		for (int i = 0; i < arr.Length; i++) {
			if (arr [i].name.ToLower () == name.ToLower ())
				return arr [i];
		}

		return null;
	}
}

public class Country {
	public string name;
	public Color color;
	public int id;

	public List<int> regions;

	public Country (string _name, int _id, Color _color, List<int> _regions) {
		this.name = _name;
		this.id = _id;
		this.color = _color;
		this.regions = _regions;
	}
}