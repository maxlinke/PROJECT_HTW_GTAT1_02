using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour {

	[SerializeField] UnityEvent[] eventsToExecute;

	void OnTriggerEnter2D(Collider2D otherCollider){
		if(otherCollider.tag.Equals("Player")){
			for(int i=0; i<eventsToExecute.Length; i++){
				eventsToExecute[i].Invoke();
			}
		}
	}

}
