// Autora: Raquel Mas

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
	Script del menú del juego de ritmo. Permite cargar las diferentes
	dificultades del juego y volver al menú principal. Al pulsar el botón
	de cada dificultad aparece un pop-up que muestra el nombre de la canción,
	el record actual y los botones para poder jugar o cerrar el pop-up.
*/
public class RhythmMenu : MonoBehaviour {

	public AudioSource audioSource; //reproductor de audio
	public AudioClip click, bclick; //sonidos de los botones
	public GameObject popUp, menu; //objeto del pop-up y botones del menú
	public Text mode, songName, record, info; //textos del pop-up y de información

	/*
		Se ejecuta al pulsar el botón de dificultad fácil.
		Muestra un pop-up con el nombre de la canción del modo fácil, el record actual
		y los botones para poder jugar o cerrar el pop-up
	*/
	public void Easy(){
		audioSource.clip = click; //asigna al reproductor el sonido del botón
		audioSource.Play (); //reproduce el sonido del botón
		popUp.SetActive (true); //muestra el pop-up
		//Ponemos los datos correspondientes en el pop-up
		mode.text = "FÁCIL";
		songName.text = "Canción: You could've been my queen\n Artista: Lowe Loveday";
		record.text = "Record: " + PlayerPrefs.GetInt ("SongHighscoreEasy");
	}

	/*
		Se ejecuta al pulsar el botón de dificultad media.
		Muestra un pop-up con el nombre de la canción del modo medio, el record actual
		y los botones para poder jugar o cerrar el pop-up
	*/
	public void Medium(){
		audioSource.clip = click; //asigna al reproductor el sonido del botón
		audioSource.Play (); //reproduce el sonido del botón
		popUp.SetActive (true); //muestra el pop-up
		//Ponemos los datos correspondientes en el pop-up
		mode.text = "MEDIA";
		songName.text = "Canción: Slash\n Artista: Other Noises";
		record.text = "Record: " + PlayerPrefs.GetInt ("SongHighscoreMedium");
	}

	/*
		Se ejecuta al pulsar el botón de dificultad difícil.
		Muestra un pop-up con el nombre de la canción del modo difícil, el record actual
		y los botones para poder jugar o cerrar el pop-up
	*/
	public void Hard(){
		audioSource.clip = click; //asigna al reproductor el sonido del botón
		audioSource.Play (); //reproduce el sonido del botón
		popUp.SetActive (true); //muestra el pop-up
		//Ponemos los datos correspondientes en el pop-up
		mode.text = "DIFÍCIL";
		songName.text = "Canción: Peyote\n Artista: Kinematic";
		record.text = "Record: " + PlayerPrefs.GetInt ("SongHighscoreHard");
	}

	//Se ejecuta cuando se pulsa el botón de volver, lleva al menú principal
	public void Back(){
		audioSource.clip = bclick; //asigna al reproductor el sonido del botón
		audioSource.Play (); //reproduce el sonido del botón
		SceneManager.LoadScene ("menuPrincipal"); //carga la pantalla
	}

	//Se ejecuta cuando se pulsa el botón de cerrar el pop-up
	public void Close(){
		audioSource.clip = bclick; //asigna al reproductor el sonido del botón
		audioSource.Play (); //reproduce el sonido del botón
		popUp.gameObject.SetActive (false); //dejamos de mostrar el pop-up
	}
		
	//Se ejecuta cuando se pulsa el botón de jugar del pop-up y carga el juego de ritmo
	public void Play(){
		audioSource.clip = click; //asigna al reproductor el sonido del botón
		audioSource.Play (); //reproduce el sonido del botón

		/*
		  Comprobamos el modo indicado en el texto del pop-up
		  y lo almacenamos para que el juego de ritmo sepa
		  cuál es la dificultad seleccionada.
		*/
		if (mode.text.Equals ("FÁCIL")) {
			PlayerPrefs.SetString("SongMode","Fácil");
		} else if (mode.text.Equals ("MEDIA")) {
			PlayerPrefs.SetString("SongMode","Media");
		} else {
			PlayerPrefs.SetString("SongMode","Difícil");
		}

		menu.gameObject.SetActive (false); //ocultamos los botones
		popUp.gameObject.SetActive (false); //ocultamos el pop-up

		/*
		 Indicamos en la información que el juego se está cargando
		 para que no parezca que se ha quedado la pantalla congelada
		 ya que el juego tarda un poco en cargar.
		*/
		info.text = "Cargando"; 
		SceneManager.LoadScene ("ritmo"); //cargamos la pantalla

	}
}
