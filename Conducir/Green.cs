// Autora: Raquel Mas

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
	Representa las zonas verdes situadas a los lados de la carretera.
	Cuando el coche entra en contacto con estas zonas se reproduce un 
	audio en bucle indicando al jugador de que se ha salido y tiene que 
	regresar a la carretera.
*/
public class Green : MonoBehaviour {

	public AudioSource audioSource; //reproductor de audio

	/*
		Estas son unas variables auxiliares para los tramos de carretera
		con curvas en los que estas zonas de los lados se componen de varios
		collider. Para evitar que el sonido empiece a reproducirse cada vez
		que pase de un collider a otro miramos si el sonido ya se está reproduciendo.
		Además contamos cada collider del que entra o sale para saber cuando sale
		definitivamente de la zona y poder dejar de reproducir el sonido.
	*/
	int aux = 0;
	bool isplaying = false;

	/*
		Se ejecuta cuando un objeto entra dentro de uno de los trigger
		de la zona. Comprueba si el objeto es el coche y si no se
		está reproduciendo ya el sonido lo reproduce.
	*/
	void OnTriggerEnter(Collider c){
		/*
		 	Comprobamos que el objeto es el coche ya que hay piedras y arboles
			de decoración sobre la zona. Además tambien hay triggers que son
			guias y puntos de control para el coche que atraviesan estas zonas. 
		*/
		if (c.tag == "Car" ) {
			aux++; //indicamos que se ha entrado en un trigger
			if (!isplaying) { //si el sonido NO se está reproduciendo
				isplaying = true; //marcamos que se está reproduciendo
				audioSource.Play (); //reproducimos el sonido
			}
		}
	}

	/*
		Se ejecuta cuando un objeto sale de uno de los trigger
		de la zona. Comprueba si el objeto es el coche y si no se
		está reproduciendo ya el sonido lo reproduce.
	*/
	void OnTriggerExit(Collider c){
		/*
		 	Comprobamos que el objeto es el coche ya hay que puntos de control 
		 	con triggers que atraviesan estas zonas y que desaparecen cuando 
		 	el coche pasa por ellos.			
		*/
		if (c.tag == "Car") {
			aux--; //indicamos que se ha salido de un trigger
			if (aux == 0) { //si el número de triggers es 0
				//significa que el coche ha salido completamente de la zona
				isplaying = false; //indicamos que el sonido no se está reproduciendo
				audioSource.Stop (); //dejamos de reproducir el sonido
			}
		}
	}
}
