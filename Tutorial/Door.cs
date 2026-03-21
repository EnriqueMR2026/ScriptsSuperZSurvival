using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
	public Animator anim;
	private bool estaAbierta = false;

	// Mantenemos el Start intacto
	void Start () {
		anim = GetComponent<Animator> ();
	}

	// Quitamos los OnTrigger y ponemos la función que tu botón sabe usar
	public void AlternarPuerta() 
	{
		estaAbierta = !estaAbierta;

		if (estaAbierta) 
		{
			anim.SetBool ("DoorOpen", true);
			anim.SetBool ("DoorClose", false);
		}
		else 
		{
			anim.SetBool ("DoorOpen", false);
			anim.SetBool ("DoorClose", true);
		}
	}
}