using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public static string data_location = "/home/luca/Downloads/gangstar_miami/gameloft/games/Gangstar2";
	public static string apk_location = "/home/luca/Downloads/gangstar_miami/6_gangstar_miami_vindication.apk";

	public string soundname;

	private AudioSource audiosource;

	private int[] indices;

	void Start () {
		if (soundname != "") {
			AudioClip myAudioClip = new WWW ("file://" + data_location + "/" + soundname).audioClip;				
			audiosource = GetComponent<AudioSource> ();
			StartCoroutine(loadFile());
		}
	}
	
	IEnumerator loadFile() {
		WWW www = new WWW ("file://" + data_location + "/" + soundname);
		AudioClip myAudioClip = www.audioClip;

		while (!myAudioClip.isReadyToPlay)
			yield return www;

		AudioClip clip = www.GetAudioClip(false);
		audiosource.clip = clip;
		audiosource.Play ();
	}
}
