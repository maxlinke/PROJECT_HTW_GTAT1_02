using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitUI : MonoBehaviour {

	[SerializeField] Image backgroundImage;
	[SerializeField] Text textField;

	[SerializeField] KeyCode key_exit;

	[SerializeField] float promptDuration;

	float promptEnd;

	void Start () {
		SetVisualsActive(false);
		textField.text = "Press " + key_exit.ToString() + " again to quit";
	}
	
	void Update () {
		if(Input.GetKeyDown(key_exit)){
			if(textField.gameObject.activeSelf){
				Application.Quit();
			}else{
				SetVisualsActive(true);
				promptEnd = Time.time + promptDuration;
			}
		}
		if(textField.gameObject.activeSelf){
			float inverseProgress = (promptEnd - Time.time) / promptDuration;
			if(inverseProgress > 0f){
				backgroundImage.rectTransform.localScale = new Vector3(inverseProgress, 1f, 1f);
			}else{
				SetVisualsActive(false);
			}
		}
	}

	void SetVisualsActive(bool value){
		backgroundImage.gameObject.SetActive(value);
		textField.gameObject.SetActive(value);
	}
}
