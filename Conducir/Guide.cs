// Autora: Raquel Mas

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
	Representa una guia, un objeto auxiliar en el juego que no es visible
	para el jugador. Se utilizan unicamente para tomar los indicadores del
	jugador, cuando el coche pasa por ellas se calcula lo centrado que va
	por la carretera.
*/
public class Guide : MonoBehaviour {
	public CarGameManager manager; //referencia al controlador del juego

	/*
		Se ejecuta cuando el coche atraviesa el objeto. Calcula la distancia
		a su punto central, que se encuetra en el centro de la carrera, para
		ver lo centrado que va el coche por la carretera.
	*/
	void OnTriggerEnter(Collider col){
		//Comprobamos que el objeto que la atravisa es el coche y no
		//los trigger que hay a los lados de la carretera que controlan
		//si el coche se sale de ella.
		if (col.tag == "Car") {
			Vector3 carPos = col.gameObject.transform.position; //obtenemos la posición del coche
			//Calculamos la distancia al centro de la guia con sus vectores de posición:
			//Restamos la posición del coche a la de la guía y calculamos su módulo.
			float distance = (new Vector2 (carPos.x - transform.position.x, carPos.z - transform.position.z)).magnitude;
			//enviamos la distancia y la posición del acelerómetro al controlador para que las guarde en el CSV
			manager.AddIndicator (distance, Input.acceleration); 
		}
	}
}
