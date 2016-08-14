using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class MainMenuController : MonoBehaviour {

	public RawImage splashscreen;

	// Use this for initialization
	void Start () {
		LoadMainImage ();
	}

	void LoadMainImage() {
		// -- EXTRACTING --

		ZipUtil.Unzip (GameController.apk_location, Application.temporaryCachePath + "/apk_extracted");
		string filename = Application.temporaryCachePath + "/apk_extracted/res/drawable/gi_background.png";

		byte[] filedata = File.ReadAllBytes(filename);
		Texture2D tex = new Texture2D(2, 2);
		tex.LoadImage(filedata);

		splashscreen.texture = tex;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
