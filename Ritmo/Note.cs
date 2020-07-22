// Autora: Raquel Mas 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	Representa una nota musical en el juego. 
	Bajan desde la parte superior de la pantalla a una velocidad constante.
	Hay cuatro carriles por los que pueden bajar las notas.
	Las notas tienen diferente color dependiendo de su carril. 
	Cuando la nota llegue al Activador el jugador deberá pulsarlo.
	Además las notas pueden ser cortas o largas, aunque su comportamiento es el mismo.
*/
public class Note : MonoBehaviour {

	Rigidbody2D rg; //permite modificar la velocidad de la nota
	float speed = 6; //velocidad constante

	void Awake(){
		rg = GetComponent<Rigidbody2D> ();
	}

	void Start () {
		rg.velocity = new Vector2 (0,-speed);  //asignamos la velocidad a la nota
	}

}
