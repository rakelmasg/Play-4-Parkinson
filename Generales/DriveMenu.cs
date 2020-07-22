// Autora: Raquel Mas

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
	Script del menú del juego de de conducción. Permite cargar las diferentes
	dificultades del juego y volver al menú principal. Al pulsar el botón
	de cada dificultad aparece un pop-up que muestra el record actual y 
	los botones para poder jugar o cerrar el pop-up.
*/
public class DriveMenu : MonoBehaviour {

	public AudioSource audioSource; //reproductor de audio
	public AudioClip click, bclick; //sonidos de los botones
	public GameObject popUp; //objeto del pop-up
	public Text mode, record; //textos del pop-up

	/*
		Se ejecuta al pulsar el botón del modo Primera Persona.
		Muestra un pop-up con el record actual y los botones 
		para poder jugar o cerrar el pop-up
	*/
	public void FP(){
		audioSource.clip = click; //asigna al reproductor el sonido del botón
		audioSource.Play (); //reproduce el sonido del botón
		popUp.gameObject.SetActive (true); //muestra el pop-up
		//Ponemos los datos correspondientes en el pop-up
		mode.text = "PRIMERA PERSONA";
		record.text = "Record: " + PlayerPrefs.GetInt ("CarHighscoreFP");
	}

	/*
		Se ejecuta al pulsar el botón del modo Arcade.
		Muestra un pop-up con el record actual y los botones 
		para poder jugar o cerrar el pop-up
	*/
	public void Arcade(){
		audioSource.clip = click; //asigna al reproductor el sonido del botón
		audioSource.Play (); //reproduce el sonido del botón
		popUp.gameObject.SetActive (true); //muestra el pop-up
		//Ponemos los datos correspondientes en el pop-up
		mode.text = "TERCERA PERSONA";
		record.text = "Record: " + PlayerPrefs.GetInt ("CarHighscoreArcade");
	}

	//Se ejecuta cuando se pulsa el botón de volver, lleva al menú principal
	public void Back(){
		audioSource.clip = bclick; //asigna al reproductor el sonido del botón
		audioSource.Play (); //reproduce el sonido del botón
		SceneManager.LoadScene ("menuPrincipal"); //carga la pantalla
	}

	//Se ejecuta cuando se pulsa el botón jugar del pop-up
	//Carga el juego de conducir según el modo seleccionado
	public void Play(){
		audioSource.clip = click; //asigna al reproductor el sonido del botón
		audioSource.Play (); //reproduce el sonido del botón

		//Comprobamos el modo seleccionado y cargamos el juego con el correspondiente.
		//También almacenamos el modo para poder mostrarlo en la pantalla de puntuación.
		if (mode.text.Equals ("PRIMERA PERSONA")) {
			PlayerPrefs.SetString ("DriveMode", "Primera Persona"); //Almacenamos el modo
			SceneManager.LoadScene ("conducirFP"); //cargamos la pantalla
		} else {
			PlayerPrefs.SetString ("DriveMode","Tercera Persona"); //Almacenamos el modo
			SceneManager.LoadScene ("conducir"); //cargamos la pantalla
		}
	}

	//Se ejecuta cuando se pulsa el botón de cerrar el pop-up
	public void Close(){
		audioSource.clip = bclick; //asigna al reproductor el sonido del botón
		audioSource.Play (); //reproduce el sonido del botón
		popUp.gameObject.SetActive (false); //dejamos de mostrar el pop-up
	}
}
