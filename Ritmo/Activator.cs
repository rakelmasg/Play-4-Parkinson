// Autora: Raquel Mas 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
	Representa un botón en el juego.
	Hay cuatro activadores cada uno con el color correspondiente a su carril.
	El jugador deberá pulsar el activador cuando una nota lo toque para destruirla.
*/
public class Activator : MonoBehaviour{
	
	bool pressed = false; //indica si el activador está siendo pulsado
	GameObject note = null, noteL = null; //referencia a la nota (larga o corta) que está sobre el activador
	GameObject gm; //referencia al gameManager que se encarga de administrar todo el juego
	GameObject copy; //auxiliar para destruir las notas
	public string color; //auxiliar con el color del activador

	void Start () {
		gm = GameObject.Find ("SongGameManager"); 
	}

	//Se ejecuta cuando una nota entra en el activador
	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Note") { //si es corta
			note = col.gameObject; //la guardamos como nota corta
			//indicamos al gameManager que hay una nota en el activador
			gm.GetComponent<SongGameManager> ().AddActiveNote (color); 
		}
		if (col.gameObject.tag == "NoteL") { //si es larga
			noteL = col.gameObject; //la guardamos como nota larga
		}
	}

	/*
	 	Se ejecuta cuando una nota sale del activador, pero esto también ocurre cuando se destruye una nota.
		Para diferenciar si la nota ha sido destruida (acierto) o el jugador no ha pulsado a tiempo (fallo)
		comprobamos si la nota es distinta de null, ya que ponemos las notas a null antes de destruirlas 
		cuando el jugador acierta.
	*/
	void OnTriggerExit2D(Collider2D col){
		if (note != null) { //fallada
			gm.GetComponent<SongGameManager> ().Fail (); //indicamos al gameManager el fallo
			//indicamos al gameManager que no hay notas en el activador
			gm.GetComponent<SongGameManager> ().RemoveActiveNote (color); 
			note = null;
		}

		if (noteL != null) { //fallada
			gm.GetComponent<SongGameManager> ().Fail (); //indicamos al gameManager el fallo
			noteL = null;
		}
	}

	//Se ejecuta cuando el activador está siendo pulsado
	public void PressButton()
	{
		pressed = true; //pulsado
		if (note!=null) { //si hay una nota tocando el activador
			//destruimos la nota
			copy = note; 
			note = null; //la ponemos a null antes de destruirla para evitar que OnTriggerExit2D la cosidere fallada
			Destroy (copy);
			gm.GetComponent<SongGameManager> ().Success (); //indicamos al gameManager el acierto
			StartCoroutine ("RemoveActiveNote"); //indicamos que no hay notas en el activador
		}
	}

	//Se ejecuta cuando el activador se deja de pulsar
	public void ReleaseButton(){
		pressed = false; //no pulsado
	}

	//Se utiliza para las notas largas, en las que el activador se mantiene pulsado
	void Update(){
		//si hay una nota larga y el botón está siendo pulsado
		if (noteL !=null && pressed) {
			//destruimos la nota
			copy = noteL;
			noteL = null; //la ponemos a null antes de destruirla para evitar que OnTriggerExit2D la cosidere fallada
			Destroy (copy);
			gm.GetComponent<SongGameManager> ().Success (); //indicamos al gameManager el acierto
			for (int i = 0; i < Input.touchCount; ++i) { //por cada pulsación la pantalla mantenida 
				if (Input.touchCount > 0 && Input.GetTouch (i).phase == TouchPhase.Stationary) {
					//enviamos al gameManager la fuerza de presión ejercida por el dedo
					gm.GetComponent<SongGameManager> ().SavePressure (Input.GetTouch (i).pressure); 
				}
			}
		}
	}

	//Indica al gameManager que no hay notas en el activador
	IEnumerator RemoveActiveNote(){
		yield return new WaitForSeconds (0.05f);
		gm.GetComponent<SongGameManager> ().RemoveActiveNote (color);
	}
}
