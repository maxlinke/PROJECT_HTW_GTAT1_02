using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamParallax : MonoBehaviour {

	[SerializeField] float distanceFactor;

	//Parallax Objects MUST have a parent, to which they will move relatively
	List<GameObject> parallaxObjects;

	void Start () {
		parallaxObjects = new List<GameObject>();
		parallaxObjects.AddRange(GameObject.FindGameObjectsWithTag("Parallax"));
	}
	
	void LateUpdate () {
		if(parallaxObjects.Count < 1) parallaxObjects.AddRange(GameObject.FindGameObjectsWithTag("Parallax"));
		Vector3 cPos = this.transform.position;
//		Vector3 camOffset = new Vector3(cPos.x, cPos.y, 0);
		for(int i=0; i<parallaxObjects.Count; i++){
			GameObject obj = parallaxObjects[i];
			if(obj == null){
				parallaxObjects.RemoveAt(i);
				i--;
			}else{
				Vector3 parentPos = obj.transform.parent.position;
				Vector3 camOffset = new Vector3(cPos.x - parentPos.x, cPos.y - parentPos.y, 0f);
				float z = parentPos.z;
				float factor = z * distanceFactor;
				obj.transform.localPosition = new Vector3(camOffset.x * factor, camOffset.y * factor, obj.transform.localPosition.z);
			}
		}
	}
}
