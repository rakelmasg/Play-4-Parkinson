// Autora: Raquel Mas

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/*
	Script de la pantalla del formulario diario. Guarda las respuestas
	en un fichero de texto para que luego sean enviadas por correo.
	Después carga la pantallad de los ejercicios de voz.
*/
public class DiaryForm : MonoBehaviour {
	
	public AudioSource audioSource; //reproductor de audio
	public HandleTextFile textWriter; //script auxiliar para escribir en un fichero
	public InputField eventualMed; //respuesta a la pregunta de medicación circunstancial
	public Dropdown PDMedTurn; //menú desplegable con las respuestas a la pregunta del turno

	//auxiliares para la respuesta del menú desplegable
	int turnoInt = 0;
	string turno;

	void Update(){
		//actualizamos la respuesta seleccionada en el menú desplegable
		turnoInt = PDMedTurn.value;
	}

	//Se ejecuta cuando se pulsa el botón de continuar. Guarda las respuestas en un fichero
	//y carga la pantalla de los ejercicios de voz
	public void Continue() {
		audioSource.Play (); //reproduce el sonido del botón

		//escribimos la respuesta en el fichero
		textWriter.WriteString ("\nMedicación circunstancial: "+eventualMed.text,"formulario.txt");

		//dependiendo de la respuesta seleccionada en el menú desplegable
		//guardamos la respuesta en el fichero de texto
		switch(turnoInt){
		case 0:
			turno = "Mañana";
			break;
		case 1:
			turno = "Tarde";
			break;
		default:
			turno = "Noche";
			break;
		}
		textWriter.WriteString ("\nÚltimo turno de la medicación para el Parkinson: "+turno,"formulario.txt");

		/*
			Almacenamos en LastScene el nombre de esta pantalla
			para que al finalizar los ejercicios de voz se redireccione
			al menú principal y no al la pantalla de inicio.
		*/
		PlayerPrefs.SetString ("LastScene", "DiaryForm");
		SceneManager.LoadScene ("facioquinesis"); //cargamos la pantalla
	}
}
