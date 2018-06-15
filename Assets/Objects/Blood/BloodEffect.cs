using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffect : MonoBehaviour {

	[SerializeField] Light light;
	[SerializeField] float timeUntilLightsOut;

	float startIntensity;
	float startTime;

	void Start () {
		startIntensity = light.intensity;
		startTime = Time.time;
	}
	
	void Update () {
		float progress = Mathf.Clamp01((Time.time - startTime) / timeUntilLightsOut);
		light.intensity = (1f - progress) * startIntensity;
		if(progress == 1f) DestroyYourselfM8();
	}

	public void DestroyYourselfM8(){
		Destroy(this.gameObject);
	}
}
