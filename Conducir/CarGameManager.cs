// Autora: Raquel Mas

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

/*
	Representa el controlador del juego. Se encarga de mostrar el tiempo restante
	de la partida, guardar los indicadores, finalizar el juego y crear los tramos
	de carretera aleatoriamente.
*/
public class CarGameManager : MonoBehaviour {

	//Variables para el indicador
	string date; //fecha y turno
	int race_id; //identificador de cada partida
	int distance_index = 0; //índice de la distancia al centro de la carretera 
	float distance_avg = 0; //media de la distancia
	List<float> d_list; //lista con las distancias para calcular la desviación típica
	int pos_index = 0; //índice de la posición del coche
	public HandleTextFile textWriter; //Script auxiliar para escribir en ficheros
	string playerID; //ID del jugador
	string mode; //modo de juego: Arcade o Primera Persona

	//Variables del juego y efectos
	int min = 2, seg = 00; //tiempo de duración de la partida
	public Text timer_txt; //texto donde se muestra el tiempo restante
	public AudioSource audioSource; //reproductor de audio
	public AudioClip bclick; //sonido del botón de volver
	public Text modeTxt;

	//Variables para la creación aleatoria en tiempo real de la carretera
	public GameObject[] roads = new GameObject[11]; //Array de los tipos de tramos de carretera 
	float distance = 1.5f; //distancia a la que se van creando los tramos de carretera
	int currentRoad = -1; // tramo de carretera actual
	int nextRoad = 10; //siguiente tramo de carretera

	/*
		Se ejecuta cuando carga la pantalla. Prepara todos los datos para guardar
		los indicacores, reinicia la puntuación, pone a funcionar el temporizador
		y llama a la función que crea la carretera.
	*/
	void Awake () {
		playerID = PlayerPrefs.GetString ("PlayerID"); // ID de usuario
		date = DateTime.Now.ToString("d"); // obtenemos fecha
		int time = int.Parse(DateTime.Now.ToString("HH")); //obtenemos la hora
		//Dependiendo de la hora se considera si es turno de mañana, tarde, noche o madrugada
		if (time >= 6 && time <= 11) { 
			date = date + ",Mañana";
		} else if (time >= 12 && time <= 17) {
			date = date + ",Tarde";
		} else if (time >= 18 && time <= 23) {
			date = date + ",Noche";
		} else {
			date = date + ",Madrugada";
		}

		mode = PlayerPrefs.GetString ("DriveMode"); //obtenemos el modo de juego
		race_id = PlayerPrefs.GetInt ("RaceID"); //obtenemos el identificador de la partida
		PlayerPrefs.SetInt ("RaceID",race_id+1); //lo incrementamos para la siguiente vez
		d_list = new List<float> (); //inicializamos la lista de distancias

		CreateRoad (); //llamamos a la función que crea los tramos de carretera
		PlayerPrefs.SetInt ("CarScore",0); //reiniciamos la puntuación del juego
		StartCoroutine ("Wait"); //llamamos a la función que inicia el juego
	}

	//Se encarga de iniciar el juego pasados 4 segundos
	IEnumerator Wait(){
		yield return new WaitForSeconds (4f); //espera por 4 segundos
		modeTxt.gameObject.SetActive(false); //dejamos de mostrar el modo por pantalla
		StartCoroutine ("Timer"); //iniciamos el temporizador
	}

	//Función recursiva que actualiza el temporizador de la partida, cuando el tiempo 
	//se acaba llama a la función que finaliza la partida.
	IEnumerator Timer(){
		//Para que no se vea mal en la interfaz el tiempo añadimos un 0
		//manualmente cuando los segundos son menores de 10.
		if (seg < 10) {
			timer_txt.text = min + ":0" + seg; //mostramos el tiempo que queda
		} else {
			timer_txt.text = min+":"+seg; //mostramos el tiempo que queda
		}
		yield return new WaitForSeconds (1f); //espera por 1 segundo
		if (seg == 0) { //cuando los segundos llegan a 0
			if (min == 0) { //si los minutos han llegado a 0 también
				EndGame (); //llamamos a la función que finaliza la partida
			} else { //si quedan minutos
				min--; //restamos un minuto
				seg = 59; //ponemos los segundos restantes
			}
		} else { //si los segundos son mayores de 0
			seg--; //restamos un segundo
		}
		StartCoroutine ("Timer"); //volvemos a llamar a la función
	}

	/*
		Función encargada de guardar los indicadores en un fichero de texto con el script
		auxiliar. Guarda la distancia al centro de la carretera y la posición actual del
		coche. Además almacena las distancias para poder calcular la media y la desviación
		típica cuando se finalice la partida.
	*/
	public void AddIndicator(float dis, Vector3 pos){
		//escribimos las mediciones del indicador en el fichero de texto con el script auxiliar
		textWriter.WriteString (playerID+","+date + "," +mode+"," + race_id + ",Distancia," + distance_index + "," + dis,"conducir.csv");
		distance_avg += dis; //sumamos las distancias para después calcular la media
		d_list.Add (dis); //añadimos la distancia a la lista para después calcular la desviación típica 
		distance_index++; //incrementamos el índice de distancia

		//escribimos la posición del coche en el fichero de texto con el script auxiliar
		textWriter.WriteString (playerID+","+date + "," +mode+"," + race_id + ",Posición," + pos_index +",\""+ pos.x + "," + pos.y + "," + pos.z + "\"","conducir.csv");
		pos_index++; //incrementamos el índice de la posición 
	}

	//Calcula la desviación típica con la media y la lista de valores
	float StandarDeviation(float avg, int n, List<float> list){
		float aux = 0;
		foreach (float x in list) {
			aux += (float)Math.Pow((x-avg),2);
		}
		aux = (float)Math.Sqrt (aux / (n - 1));
		return aux;
	}

	/*
		Función para finalizar la partida, se ejecuta cuando se termina el tiempo o cuando
		el jugador pulsa el botón de volver. Se encarga calcular la media y guardarla, 
		llamar a la función	de calcular la desviación típica y guardarla y por último
		cargar la escena de la puntuación.
	*/
	public void EndGame(){
		distance_avg = distance_avg/(distance_index); //calculamos la media
		//con ayuda del script auxiliar guardamos la distancia media en el fichero de los indicadores
		textWriter.WriteString (playerID+","+date + "," +mode+"," + race_id + ",distancia,Media," + distance_avg,"conducir.csv");
		//calculamos la desviación típica del indicador con ayuda de la función StandarDeviation
		float deviation = StandarDeviation (distance_avg, distance_index, d_list);
		//con ayuda del script auxiliar guardamos la desviación típica de la distancia en el fichero de los indicadores
		textWriter.WriteString (playerID+","+date + "," +mode+"," + race_id + ",distancia,Desviación," + deviation,"conducir.csv");
		SceneManager.LoadScene ("conducirPuntuacion"); //cargamos la escena de la puntuación
	}

	/*
		Función encargada de crear la carretera progresiva y aleatoriamente con los
		diferentes tramos. La primera vez es llamada por el controlador pero las siguientes
		es llamada por el script del coche cada vez que alcanza un checkpoint.
	*/
	public void CreateRoad(){
		//Auxiliar para seguir buscando números aleatorios hasta que encuentra uno
		//que no se esté utilizando.
		bool endLoop = false; 
		int rnd = -1; //número aleatorio
		while (!endLoop) { //se ejecuta hasta que encuentra un número disponible
			rnd = UnityEngine.Random.Range (0,roads.Length); //número aleatorio entre la cantidad de tipos de tramos
			if (rnd != nextRoad && rnd != currentRoad) { //si el número no es una de las carreteras que se están mostrando
				endLoop = true; //finalizamos el bucle
			}
		}
		currentRoad = nextRoad; //asignamos la carretera actual como la siguiente
		nextRoad = rnd; //asignamos la carretera siguiente como la nueva (aletoria)

		GameObject newRoad = roads [rnd]; //seleccionamos la carretera aleatoria del array
		Vector3 roadPos = newRoad.transform.position; //obtenemos la posición de la carretera
		distance += 40; //incrementamos la distancia donde se tiene que crear la carretera

		if (newRoad.tag == "Left") { //si el tramo es una curva a la izquierda
			roadPos.z = distance + 37; //añadimos un desfase de 37 y la distancia donde se tiene que crear
			//Esto se debe a que este tramo es la inversión del tramo de curva a la derecha y
			//su origen está en el lado opuesto.
		} else { //si no
			roadPos.z = distance; //añadimos solo la distancia donde se tiene que crear
		}

		newRoad.transform.position = roadPos; //asignamos la posición nueva a la carretera
		newRoad.SetActive (true); //mostramos la carretera si no está activa porque es la primera vez que se usa
	}

	//Función auxiliar para que suene el botón de volver si se pulsa
	public void BackButton(){
		audioSource.clip = bclick; //asignamos el sonido al reproductor de audio
		audioSource.Play (); //reproducimos el sonido

	}
}
