// Autora: Raquel Mas

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/*
	Script de la pantalla de ejercicios de voz. Realiza 3 ejercicios
	obligatorios y uno aleatorio entre 3. Muestra las instrucciones al
	jugador, cuando se pulsa el botón empieza a grabarse el audio y 
	se va mostrando el tiempo restante. Al finalizar los ejercicios
	lleva al menú principal si se ha llamado desde el formulario diario
	o a la pantalla de inicio si se ha llamado desde el menú principal.
*/
public class AudioRecorder : MonoBehaviour {

	public AudioSource audioSource; //reproductor de audio
	public Button recordBtn, continueBtn; //botón de grabar y de continuar
	public Text timer, info, subtitle; //contador, información, subtitulo

	AudioClip clip; //clip de audio donde se graba el audio del ejercicio
	string clipName; //nombre del archivo de wav donde se va a guardar el audio
	int min = 1; //duración del audio en minutos, varía según el ejercicio
	int sec = 00; //duración del audio en segundos, varía según el ejercicio
	int frecuency = 22050; //frecuencia de muestreo de audio

	//Array con los textos de los ejercicios obligatorios
	string[] exercices = new string[]{"diga en voz alta las 5 vocales de forma sostenida y cogiendo aire:",
		"repita varias veces en voz alta hasta que finalice el tiempo la siguiente locución: 'AY'",
		"repita varias veces en voz alta hasta que finalice el tiempo la siguiente locución: 'PATAKA'"};

	//Array con los textos de los ejercicios aleatorios
	string[] randomExercices = new string[]{"repita en voz alta hasta que finalice el tiempo la siguiente locución: 'PA'",
		"repita varias veces en voz alta hasta que finalice el tiempo la siguiente locución: 'TA'",
		"repita varias veces en voz alta hasta que finalice el tiempo la siguiente locución: 'KA'"};
	
	int currentExe = 0; //indice del ejercicio obligatorio actual
	bool randomExe = false; //indica si se ha realizado un ejercicio aleatorio
	int id; //indice actual del clip de audio, se lleva la cuenta de los que se van creando

	void Awake(){
		if (PlayerPrefs.GetString ("LastScene").Equals ("DiaryForm")) {
			PlayerPrefs.SetInt ("GamesPlayed",0); //indicamos que aún no se ha jugado
			subtitle.text += "Antes de Jugar";
		} else {
			subtitle.text += "Después de Jugar";
		}

		nextExercise ();//llamamos a la función que muestra las istrucciones del ejercicio
	}

	//Se ejecuta cuando se pulsa el botón de grabar. Graba un clip de audio y prepara el nombre que llevará
	//el audio cuando se pase a formato wav.
	public void Record(){
		id = PlayerPrefs.GetInt ("AudioClipID"); //recuperamos el indice que corresponde al clip de audio
		clipName = "audio" + id; //creamos el nombre del wav combinando "audio" a su indice
		recordBtn.gameObject.SetActive (false); //ocultamos el botón de grabar
		timer.gameObject.SetActive (true); //mostramos el contador
		StartCoroutine ("ShowTime"); //llamamos a la función que muestra el tiempo del audio

		//guaramos el clip de audio con el micrófono del dispositivo, la frecuencia indicada
		//y la duración del ejercicio.
		clip = Microphone.Start(Microphone.devices [0], false, (min*60+sec), frecuency);
		}

	/*
	 	Función recursiva que va cambiando el temporizador, mostrando el tiempo que queda de ejercicio.
		Cuando el tiempo se acaba llama a la función para guardar el audio y después a la
		función que controla el flujo.
	*/
	IEnumerator ShowTime() {
		yield return new WaitForSeconds (1); //espera por un segundo
		if (sec > 0) { //si el tiempo es mayor que 0
			sec--; //restamos un segundo
			timer.text = (sec<10)? "0"+min+":0" + sec:"0"+min+":" + sec; //mostramos el tiempo que queda
			StartCoroutine ("ShowTime"); //volvemos a llamar a esta función
		} else if (min>0) { //si los segundos son 0 y pero quedan minutos
			min--;
			sec = 59;
			timer.text = "0"+min+":59"; //mostramos el tiempo que queda
			StartCoroutine ("ShowTime"); //volvemos a llamar a esta función
		}else{
			Save (); //llamamos a la función para que guarde el clip de audio en formato wav
			Manager (); //llamamos a la función que controla el flujo
		}
	}
		
	//Se encarga de guardar un clip de audio en formato wav mediante un script auxiliar
	void Save(){
		timer.gameObject.SetActive (false); //ocultamos el temporizador 
		SavWav.Save(clipName, clip); //llamamos al la función auxiliar que guarda el clip
		PlayerPrefs.SetInt ("AudioClipID", id + 1); //incrementamos el indice de clips se audio
	}

	/*
		Se encarga de controlar el flujo de la pantalla. Va llamando a los siguientes ejercicios
		obligatorios, cuando se completan llama al aleatorio y después muestra el botón para poder
		continuar.
	*/
	void Manager(){
		sec = 5; //ponemos el tiempo a 5, que es la duración de todos los ejercicios menos el primero
		if (currentExe == exercices.Length) { //si ya se han realizado todos los obligatios
			if (!randomExe) { //si no se ha realizado el ejercicio aleatorio 
				randomExercise (); //mostramos el ejercicio aleatorio
			} else { //si se ha realizado el ejercicio aleatorio
				continueBtn.gameObject.SetActive (true); //mostramos el botón de continuar

				//indicamos al jugador que ya ha completado todos los ejercicios
				info.alignment = TextAnchor.MiddleCenter;
				info.fontSize = 70;
				info.text = "\n¡Ejercicios completados!";
			}
		} else { //si no se han realizado todos los ejercicios obligatorios
			nextExercise (); //cargamos el siguiente ejercicio aleatorio
		}
	}

	//Muestra el botón para grabar audio, el texto del ejercicio actual
	//y prepara el texto de contador con los segundos correspondientes
	void nextExercise(){
		recordBtn.gameObject.SetActive (true); //mostramos el botón
		info.text = "Pulse el botón y " + exercices [currentExe]; //mostramos el texto del ejercicio
		timer.text = (sec<10)? "0"+min+":0" + sec:"0"+min+":" + sec; //mostramos el tiempo que queda
		currentExe++; //avanzamos el contador del ejercicio actual
	}

	//Muestra el botón para grabar audio, el texto del ejercicio aleatorio
	//y prepara el texto de contador con los segundos correspondientes
	void randomExercise(){
		recordBtn.gameObject.SetActive (true); //mostramos el botón
		//mostramos el texto del ejercicio aleatorio
		info.text = "Pulse el botón y " + randomExercices [Random.Range (0,3)];
		timer.text = (sec<10)? "00:0" + sec:"00:" + sec; //preparamos el contador con el tiempo
		randomExe= true; //indicamos que ya se ha realizado un ejercicio aleatorio 
	}

	/*
	 	Se ejecuta cuando se pulsa el botón de continuar. Lleva al menú principal si ha sido 
		llamado desde el formulario diario, si ha sido llamado desde el menú principal lleva
		a la pantalla de inicio.
	*/
	public void Continue(){
		audioSource.Play (); //reproducimos el sonido del botón

		//Si la última pantalla fue la del formulario diario
		if(PlayerPrefs.GetString("LastScene").Equals("DiaryForm"))
			SceneManager.LoadScene ("menuPrincipal"); //cargamos la pantalla del menú principal
		else //Si la última pantalla fue el menú principal
			SceneManager.LoadScene ("inicio"); //cargamos la pantalla de incio
	}
}
