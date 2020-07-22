// Autora: Raquel Mas

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
	Script de la pantalla de puntuación del juego de conducción.
	Muestra la puntuación obtenida y el nombre del modo que se ha jugado.
	Si se supera el record de puntos de su modo se muestra por pantalla
	y se almacena como nuevo record. Permite volver a jugar al mismo modo
	o volver al menú del juego de conducción.
*/
public class CarScore : MonoBehaviour {

	public AudioSource audioSource; //reproductor de audio
	public AudioClip click, bclick; //sonidos de los botones
	public Text mode; //texto donde se muestra el modo de juego
	public Image newRecord; //imagen que se muestra si se supera un record
	string modeString; //auxiliar para almacenar el modo de juego
	int score; //puntuación

	void Start () {
		PlayerPrefs.SetInt ("GamesPlayed",1); //indicamos que se ha jugado
		modeString = PlayerPrefs.GetString ("DriveMode"); //obtenemos el modo de juego
		mode.text = modeString.ToUpper(); //Mostramos el modo de juego por pantalla
		
		score = PlayerPrefs.GetInt ("CarScore"); //obtenemos la puntuación

		//Comprobamos si se ha superado la mejor puntuación almacenada de su modo
		if (modeString.Equals ("Tercera Persona")) {
			if(PlayerPrefs.GetInt("CarHighscoreArcade") < score){ //en caso afirmativo
				newRecord.gameObject.SetActive (true); //informamos al jugador
				PlayerPrefs.SetInt ("CarHighscoreArcade", score); //guardamos el nuevo record
			}
		} else {
			if(PlayerPrefs.GetInt("CarHighscoreFP") < score){ //en caso afirmativo
				newRecord.gameObject.SetActive (true); //informamos al jugador
				PlayerPrefs.SetInt ("CarHighscoreFP", score); //guardamos el nuevo record
			}
		}
	}

	//Se ejecuta cuando se pulsa el botón de volver
	public void Back () {
		audioSource.clip = bclick; //asignamos el sonido correspondiente
		audioSource.Play (); //reproducimos el sonido
		SceneManager.LoadScene ("menuConducir"); //cargamos el menú del juego
	}

	//Se ejecuta cuando se pulsa el botón de jugar de nuevo
	public void TryAgain(){
		audioSource.clip = click; //asignamos el sonido correspondiente
		audioSource.Play (); //reproducimos el sonido

		//Dependiendo del modo de juego seleccionando se carga el tipo de juego
		if (modeString.Equals ("Tercera Persona")) 
			SceneManager.LoadScene ("conducir"); //cargamos la pantalla del modo arcade
		else 	
			SceneManager.LoadScene ("conducirFP"); //cargamos la pantalla de modo primera persona
	}
}
