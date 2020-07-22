// Autora: Raquel Mas

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	Representa una moneda en el juego. Cuando el coche choca con ella
	incrementa la puntuación y desaparece. La moneda se presenta
	girando sobre si misma en el centro de la carretera, para marcar
	el camino al jugador.
*/
public class Coin : MonoBehaviour {

	void Update () {
		//Hace que la moneda gire sobre si misma
		transform.Rotate (new Vector3 (0f, 1f, 0f),Space.World);
	}
}
