// Autora: Raquel Mas

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/*
	Script del menú principal. Permite ir a los menús de los
	juegos y volver a la pantalla de inicio. 
*/
public class MainMenu : MonoBehaviour {
	public AudioSource audioSource; //reproductor de audio
	public AudioClip bclick; //sonido del botón de volver
	public GameObject menu; //menú principal
	public GameObject popUp; //mensaje emergente

	//Carga el menú del juego de conducción
	public void DriveGame(){
		audioSource.Play (); //reproducimos el sonido del botón
		SceneManager.LoadScene ("menuConducir"); //cargamos la pantalla
	}

	//Carga el menú del juego de ritmo
	public void RhythmGame(){
		audioSource.Play (); //reproducimos el sonido del botón
		SceneManager.LoadScene ("menuRitmo"); //cargamos la pantalla
	}

	//Muestra la ventana emergente para comprobar si de verdad quiere volver al inicio
	public void ShowPopUp(){
		menu.gameObject.SetActive (false);
		popUp.gameObject.SetActive (true);
	}

	//Oculta la ventana emergente y vuelve a mostrar el menú
	public void HidePopUp(){
		popUp.gameObject.SetActive (false);
		menu.gameObject.SetActive (true);
	}

	/*
	 Permite volver a la pantalla de inicio. Antes lleva a la pantalla de 
	 los ejercicios de voz para recopilar datos después de haber jugado 
	 a los juegos.
	*/
	public void Back(){
		audioSource.clip = bclick; //asignamos el sonido del botón de volver
		audioSource.Play (); //reproducimos el sonido del botón

		/*
			Almacenamos en LastScene el nombre de esta pantalla
			para que al finalizar los ejercicios de voz se redireccione
			a la pantalla de inicio y no al menú principal de nuevo.
		*/
		PlayerPrefs.SetString ("LastScene", "MainMenu"); 
		if(PlayerPrefs.GetInt("GamesPlayed")==1) //si ya se ha jugado
			SceneManager.LoadScene ("facioquinesis"); //cargamos la pantalla de ejercicios de voz
		else //si no se ha jugado 
			SceneManager.LoadScene ("inicio"); //cargamos la pantalla de inicio
	}
}
