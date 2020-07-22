// Autora: Raquel Mas

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
	Script de la pantalla de créditos
*/
public class Credits : MonoBehaviour {

	public AudioSource audioSource; //reproductor de audio

	//Se ejecuta al pulsar el botón de volver, lleva a la pantalla de inicio
	public void Back () {
		audioSource.Play (); //reproduce el sonido del botón
		SceneManager.LoadScene ("inicio"); //cargamos la pantalla
	}
}
