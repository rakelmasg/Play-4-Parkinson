// Autora: Raquel Mas

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;


/*
	Script de la pantalla de inicio. Permite ir al menú principal
	y a los créditos del juego. También se encarga de llamar a la
	función de enviar mail para mandar los datos recopilados del día
	anterior.
*/
public class Init : MonoBehaviour {
	public Mail mail; //script para mandar el mail
	public AudioSource audioSource; //reproductor de audio
	public Text aux;

	//Se ejecuta cuando se carga la pantalla
	void Awake(){
		//guardamos el día actual
		int date = int.Parse (DateTime.Now.ToString ("yyyyMMdd"));

		//si es la primera vez que se ejecuta la aplicación no mandamos el mail
		if(PlayerPrefs.GetInt("LastMail")==0){
			PlayerPrefs.SetInt ("LastMail",date); //guardamos el día actual
		//si no es la primera vez y tampoco se ha abierto hoy la aplicación
		}else if(PlayerPrefs.GetInt("LastMail")<date){
			if (mail.SendMail())  //mandamos el mail con los datos del día anterior
				PlayerPrefs.SetInt ("LastMail",date); //guardamos el día actual
		}
	}

	//Se ejecuta cuando se pulsa el botón de jugar y carga la
	//pantalla de formulario correspondiente.
	public void StartApp(){
		audioSource.Play (); //reproduce el sonido del botón

		//Si es la primera vez que se ejecuta la aplicación cargamos
		//la pantalla del formulario de registro.
		if (PlayerPrefs.GetInt ("FirstTime") == 0)
			SceneManager.LoadScene ("primerCuestionario");
		else  //si no cargamos la pantalla del formulario diario
			SceneManager.LoadScene ("cuestionarioDiario");
	}

	//Se ejecuta cuando se pulsa el botón de créditos y carga esa pantalla.
	public void Credits(){
		audioSource.Play (); //reproduce el sonido del botón
		SceneManager.LoadScene ("creditos"); //carga la pantalla
	}

	//Se ejecuta cuando se pulsa el botón de salir, cierra la aplicación
	public void Exit(){
		audioSource.Play (); //reproduce el sonido del botón
		Application.Quit (); //cerramos la aplicación
	}

	//SOLO DEBUG
	public void mandarMail(){
		audioSource.Play ();
		if (mail.SendMail())
			aux.text = "¡Mensaje enviado!";
		else
			aux.text = "ERROR";
	}
}
