// Autora: Raquel Mas

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
	Script de la pantalla de puntuación del juego de ritmo.
	Se encarga de mostrar la puntuación, fallos y mejor combo realizados en la canción.
	Evalua la puntuación con letras (A,B,C o D). Si se ha superado un record de puntos
	en la dificultad correspondiente se muestra al jugador. Permite volver a jugar la 
	canción o volver al menú del juego de ritmo.
*/
public class SongScore : MonoBehaviour {

	public AudioSource audioSource; //reproductor de audio
	public AudioClip click, bclick; //sonidos de los botones 
	public Text songName, artist, mode, note, score; //datos de la canción, dificultad, nota y puntos
	public Image newRecord; //imagen que se muestra cuando se ha batido el record de puntos
	int scoreInt; //puntuación
	public Text info; //muestra "cargando" si el jugador pulsa el botón de jugar de nuevo
	public GameObject part; //auxiliar para ocultar los botones y la puntuación mientras carga la pantalla

	void Start () {
		PlayerPrefs.SetInt ("GamesPlayed",1); //indicamos que se ha jugado
		scoreInt = PlayerPrefs.GetInt ("SongScore"); //almacenamos la puntuación
		//Operación auxiliar para mostrar más centrada la puntuación por pantalla
		string scoreString = scoreInt+"";
		while (scoreString.Length < 5) {
			scoreString = "0" + scoreString;
		}
		score.text = scoreString; //mostramos la puntuación

		string song = PlayerPrefs.GetString ("SongName"); //nombre de la canción y artista
		songName.text = " "+song.Split(new char[] {'-'})[0]; //muestra el título de la canción
		artist.text = song.Split(new char[] {'-'})[1]+" "; //muestra el artista de la canción
		mode.text = PlayerPrefs.GetString("SongMode").ToUpper();

		Evaluate (); //llamamos a la función que calcula la puntuación con letra
	
		//Comprobamos si se ha superado la mejor puntuación almacenada de su dificultad
		string highscoreString;
		if (PlayerPrefs.GetString ("SongMode").Equals ("Fácil")) {
			highscoreString = "SongHighscoreEasy";
		} else if (PlayerPrefs.GetString ("SongMode").Equals ("Media")) {
			highscoreString = "SongHighscoreMedium";
		} else {
			highscoreString = "SongHighscoreHard";
		}
		if (PlayerPrefs.GetInt (highscoreString) < scoreInt) { //en caso afirmativo
			newRecord.gameObject.SetActive (true); //informamos al jugador
			PlayerPrefs.SetInt (highscoreString,scoreInt); //guardamos la nueva mejor puntuación
		}
	}

	/*
	 Calcula y muestra la puntuación con letra por pantalla.
	 La mayor a menor puntuación las letras son: A, B, C y D.
	 Además cada letra tiene diferente color para ser más visual.
	*/
	void Evaluate(){
		//puntuación máxima que se puede obtener en la canción
		int totalScore = PlayerPrefs.GetInt ("FullScore");
		int percent = scoreInt*100/totalScore; //calculamos la puntuación en porcentaje

		//Dependiendo del porcentaje asignamos la letra y el color correspondiente.
		if (percent < 25) {
			note.text = "D";
			note.color = Color.red;
		} else if (percent < 50) {
			note.text = "C";
			note.color = Color.yellow;
		} else if (percent < 75) {
			note.text = "B";
			note.color = Color.green;
		} else {
			note.text = "A";
			note.color = Color.magenta;
		}

	}

	//Vuelve al menú del juego de ritmo
	public void Back(){
		audioSource.clip = bclick; //asignamos el audio correspondiente
		audioSource.Play (); //reproduce el sonido del botón
		SceneManager.LoadScene ("menuRitmo"); //cargamos la escena
	}

	//Permite volver a jugar la misma canción
	public void TryAgain(){
		audioSource.clip = click; //asignamos el audio correspondiente
		audioSource.Play (); //reproduce el sonido del botón
		part.gameObject.SetActive(false); //ocultamos los botones y la puntuación mientras
		info.gameObject.SetActive(true); //mostramos que está cargando
		SceneManager.LoadScene ("ritmo"); //cargamos la escena
	}
}
