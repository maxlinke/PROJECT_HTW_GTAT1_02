using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEditor;

public class LevelEndTrigger : MonoBehaviour {

	//[SerializeField] SceneAsset nextLevel;
	[SerializeField] string nextLevel;

	void Start () {
		
	}
	
	void Update () {
		
	}

	public void LoadNextLevel(){
//		SceneManager.LoadScene(nextLevel.name);
		SceneManager.LoadScene(nextLevel);
	}
}
