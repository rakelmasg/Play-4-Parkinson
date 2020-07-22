// Autora: Raquel Mas 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
	Script auxiliar para mostrar en tiempo real los valores de datos
	almacenados en las PlayerPrefs.
*/
public class PPText : MonoBehaviour {

	public string n; //nombre del dato que se quiere mostrar


	void Update () {
		//Muestra en un Text el valor del dato que se quiere mostrar
		GetComponent<Text> ().text = PlayerPrefs.GetInt (n) + "";

		//Para que no se muestre siempre el 0 del combo en el juego de ritmo
		if (n.Equals ("Combo") && PlayerPrefs.GetInt (n) == 0)
			GetComponent<Text> ().text = "";
	}
}
