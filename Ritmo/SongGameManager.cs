// Autora: Raquel Mas 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;


/*
	Es la clase principal encargada de coordinar el juego y tomar los indicadores.
	Hace que empiece el juego y la canción, que aparezcan las notas, controla los
	aciertos y fallos, toma los datos de los indicadores y finaliza el juego.
*/
public class SongGameManager : MonoBehaviour {

	//Assets gráficos
	public GameObject redNote, blueNote, greenNote, yellowNote; //notas simples
	public GameObject redNoteL, blueNoteL, greenNoteL, yellowNoteL; //notas largas
	public GameObject redNoteIni, blueNoteIni, greenNoteIni, yellowNoteIni; //inicio de notas largas
	public GameObject redNoteFin, blueNoteFin, greenNoteFin, yellowNoteFin; //final de notas largas
	public GameObject touch; //círculo de pulsación

	//Interfaz de usuario
	public Slider progress; //barra de progreso
	public Image fill; //color de la barra de progreso
	public Text songName; //nombre de la canción
	public Text songMode; //dificultad de juego

	//Variables de la canción
	public AudioSource audioSource; // reproductor de audio
	public TextAsset easyFile, mediumFile, hardFile; //archivos de texto de las canciones
	TextAsset songFile; //fichero de texto de la canción, contiene las notas e información
	string[] songNotes; //array con las notas de la canción obtenidas del fichero de la canción
	int y = 0; //auxiliar, indica la fila de notas que se tiene que leer en el árray de notas

	//Variables para puntuar al jugador: combo, mejor combo y fallos
	int combo = 0, bestcombo = 0, fails =0; 

	//Variables para los identificadores de los indicadores(precisión y presión) de cada pulsación
	string file_name = "ritmo.csv"; //nombre del fichero donde se almacenan los indicadores
	string playerID; //ID del jugador
	string date; //fecha y turno
	int song_id; //identificador de cada canción
	int accuracy_index = 0, pressure_index = 0, pos_index = 0; //indices de precisión, presión y posición
	float accuracy_avg = 0, pressure_avg = 0; //media aritmética de la precisión y la presión
	List<float> accuracy_list = new List<float>(); //lista para calcular la desviación típica de la precisión
	List<float> pressure_list = new List<float>(); //lista para calcular la desviación típica de la presión
	//Auxiliares
	bool red=false, green=false, yellow=false, blue=false; //indican si hay notas en los activadores
	bool end = false; //evita que se tomen indicadores al pulsar el botón de volver
	int invalids = 0; //auxiliar para calcular la media y desviación típica
	string mode; //dificultad de juego: fácil, media, difícil
	public HandleTextFile textWriter; //script auxiliar para escribir en el fichero
	public AudioClip bclick; //sonido del botón de volver


	//Se encarga de iniciar y preparar los datos
	void Start () {
		mode = PlayerPrefs.GetString ("SongMode");
		songMode.text = mode.ToUpper (); //mostramos la dificultad de juego por pantalla

		if (mode.Equals ("Fácil")) {
			audioSource.clip = Resources.Load("Lower_Loveday_-_You_Could_ve_Been_My_Queen (corta)") as AudioClip;
			songFile = easyFile; 
		} else if (PlayerPrefs.GetString ("SongMode").Equals ("Media")) {
			audioSource.clip = Resources.Load("Other_Noises_-_Slash (corta)") as AudioClip;
			songFile = mediumFile; 
		} else {
			audioSource.clip = Resources.Load("Kinematic_-_Peyote (corta)") as AudioClip;
			songFile = hardFile; 
		}

		playerID = PlayerPrefs.GetString ("PlayerID"); //obtenemos el ID del jugador almacenado
		fill.color = Color.yellow; //ponemos la barra de progreso amarilla

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

		song_id = PlayerPrefs.GetInt ("SongID"); //obtenemos el identificador de la canción
		PlayerPrefs.SetInt ("SongID",song_id+1); //lo incrementamos para la siguiente vez
		PlayerPrefs.SetInt ("Combo", combo); //guardamos el combo 
		PlayerPrefs.SetInt ("SongScore",0); //guardamos la puntuaión
		ReadFile (); //llamanos a la función que lee el fichero con las notas de la canción
	}

	//Se encarga de leer el fichero con las notas e información de la canción
	void ReadFile(){
		string[] auxArray = songFile.text.Split (new char[] {';'}); //separamos la información

		PlayerPrefs.SetString ("SongName",auxArray [0]); //guardamos el nombre de la canción
		songName.text = " "+auxArray [0].Split(new char[]{'-'})[0]; //mostramos el nombre por pantalla
		float offset = float.Parse(auxArray [1]); //guardamos el retraso para coordinar el audio
		//guardamos la puntuación máxima que se puede obtener en la canción
		PlayerPrefs.SetInt ("FullScore",int.Parse(auxArray [2])*100);
		songNotes = auxArray [3].Split (new char[] {','}); //guardamos las notas en un array
		StartCoroutine ("StartSong", offset); //llamamos a la función que empiece el audio
		StartCoroutine ("StartRead"); //llamamos a la función para que empiecen a leerse las notas
	}

	//Empieza a reproducir el audio de la canción pasados 5 segundos más el retraso
	IEnumerator StartSong(float offset){
		yield return new WaitForSeconds (5.0f+offset); //esperamos
		audioSource.Play (); //reproducimos el audio
	}

	//Pasados 5 segundos llamamos a la función que lee las notas
	IEnumerator StartRead(){
		yield return new WaitForSeconds (5.0f); //esperamos
		StartCoroutine ("Read");

		songName.gameObject.SetActive(false); //dejamos de mostrar el título
		songMode.gameObject.SetActive(false); //dejamos de mostrar la dificultad
	}
	
	void Update () {
		Touch (); //detectamos las pulsaciones del jugador
	}

	/*
	 Se encarga de leer del array de notas una linea (4) para mostrarlas en el juego.
	 Después espera 0.1 segundos para leer la siguiente linea o terminar la partida si 
	 se ha llegado al final del array.
	*/
	IEnumerator Read(){
		int note; //nota codificada numéricamente
		for(int x= 0; x<4;x++){ //lee una linea de 4 notas
			note = int.Parse(songNotes [y * 4 + x]);
			if (note !=0) //si hay nota
				CreateNote (note,x); //llamamos a la función que las creea
		}
		y++; //incrementamos el indice de lectura del array de notas para la próxima vez
		yield return new WaitForSeconds (0.1f); //esperamos 0.1 segundos
		if((y*4+4)<=songNotes.Length){ //si no se ha alcanzado el final del array
			StartCoroutine ("Read"); //se llama a si misma
		}else{
			EndGame (); //llamamos a la función que finaliza al partida
		}
	}

	/*
	 Termina la partida, prepara los datos para ser procesados y carga la escena
	 de puntuación de la partida. Puede ser llamada por el propio GameManager al
	 finalizar la canción o por el jugador si pulsa el botón de Volver
	*/
	public void EndGame(){

		end = true; //indicamos que se ha terminado la partida

		//calculamos las medias de ambos indicadores
		accuracy_avg = accuracy_avg/(accuracy_index-invalids);
		pressure_avg = pressure_avg/(pressure_index);
		//almacenamos las medias
		textWriter.WriteString(playerID+","+date+"," +mode+"," + song_id + ",Precisión,Media," +accuracy_avg,file_name);
		textWriter.WriteString(playerID+","+date+"," +mode+"," + song_id + ",Presión,Media," +pressure_avg,file_name);

		//calculamos las desviaciones típicas de ambos indicadores con ayuda de la función StandarDeviation
		float accuracy_deviation = StandarDeviation (accuracy_avg, accuracy_index-invalids, accuracy_list);
		float pressure_deviation = StandarDeviation (pressure_avg, pressure_index, pressure_list);
		//almacenamos las desviaciones típicas
		textWriter.WriteString(playerID+","+date+"," +mode+"," + song_id + ",Precisión,Desviación," +accuracy_deviation,file_name);
		textWriter.WriteString(playerID+","+date+"," +mode+"," + song_id + ",Presión,Desviación," +pressure_deviation,file_name);

		//comprobamos si el combo actual es mejor que el almacenado
		if (combo > bestcombo)
			bestcombo = combo; //si es mayor lo guardamos como mejor combo
		PlayerPrefs.SetInt ("BestCombo",bestcombo); //almacenamos el mejor combo
		PlayerPrefs.SetInt ("Fails", fails); //almacenamos los fallos

		SceneManager.LoadScene ("ritmoPuntuacion"); //cargamos la escena de la puntuación
	}

	//Reproduce un sonido al pulsar el botón de volver al menú
	public void BackButton(){
		audioSource.clip = bclick;
		audioSource.Play (); 
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

	//Crea una nota en el juego mediante su tipo y su carril (posición)
	void CreateNote(int type, int pos){
		
		//Tipos: 1=simple, 2=inicio de nota larga, 3=nota larga, 4=final de nota larga
		//Carril (pos): 0=amarilla, 1=verde, 2=roja, 3=azul

		//Por defecto nota azul
		GameObject note = blueNote; //simple
		if (type == 2) { //o de otro tipo
			note = blueNoteIni;
		} else if (type == 3) {
			note = blueNoteL;
		} else if (type == 4) {
			note = blueNoteFin;
		}

		//Si no es azul creamos la correspondiente
		switch (pos) {
		case 0:
			if (type == 1) {
				note = yellowNote;
			} else if (type == 2) {
				note = yellowNoteIni;
			} else if (type == 3) {
				note = yellowNoteL;
			} else {
				note = yellowNoteFin;
			}
			break;
		case 1:
			if (type == 1) {
				note = greenNote;
			} else if (type == 2) {
				note = greenNoteIni;
			} else if (type == 3) {
				note = greenNoteL;
			} else {
				note = greenNoteFin;
			}
			break;
		case 2:
			if (type == 1) {
				note = redNote;
			} else if (type == 2) {
				note = redNoteIni;
			} else if (type == 3) {
				note = redNoteL;
			} else {
				note = redNoteFin;
			}
			break;
		}
		Instantiate (note); //Creamos la nota en el juego
	}

	//Si llegan notas al final de la linea sin haber sido pulsadas las destruimos
	void OnTriggerEnter2D(Collider2D col){
		Destroy (col.gameObject);
	}

	//Es llamada por los activadores cuando una nota NO es pulsada a tiempo
	public void Fail (){
		if (combo > bestcombo) //comprobamos si el combo actual es mejor que el guarda
			bestcombo = combo; //si es mayor lo reemplazampos
		combo = 0; //reseteamos el combo
		PlayerPrefs.SetInt ("Combo", combo); //almacenamos el combo
		if (progress.value >= 1) //actualizamos la barra de progreso si es mayor que 0
			progress.value -= 1; //restando un punto por el fallo
		UpdateSliderColor (); //actualizamos el color de la barra de progreso
		fails++; //aumentamos el numero de fallos
	}

	//Es llamada por los activadores cuando una nota es pulsada a tiempo
	public void Success (){
		PlayerPrefs.SetInt ("SongScore", PlayerPrefs.GetInt ("SongScore") + 100); //aumentamos la puntuación
		combo++; //incrementamos el combo 
		PlayerPrefs.SetInt ("Combo", combo); //almacenamos el combo para mostrarlo por pantalla
		if (progress.value < 200) //actualizamos la barra de progreso si no está llena
			progress.value += 1; //sumando un punto por el acierto
		UpdateSliderColor (); //actualizamos el color de la barra de progreso
	}

	//Actualiza el color la barra de progreso dependiendo de lo llena que esté
	void UpdateSliderColor(){
		if (progress.value >= 150) { //verde si es igual o superior al 75%
			fill.color = Color.green; 
		} else if (progress.value <= 50) { //roja si es igual o inferior al 25%
			fill.color = Color.red;
		} else { //amarilla el resto de porcentajes
			fill.color = Color.yellow;
		}
	}

	/*
	 Es llamada por los activadores cuando tienen dentro una nota, ayuda a la hora
	 de tomar el indicador de precisión. De esta manera podemos saber a qué activador
	 iba destinada cada pulsación del jugador y podemos calcular la distancia.
	*/
	public void AddActiveNote(string color){
		if (color.Equals ("red")) {
			red = true;
		} else if (color.Equals ("blue")) {
			blue = true;
		}else if(color.Equals("yellow")){
			yellow = true;
		}else{
			green = true;
		}
	}


	//Es llamada por los activadores cuando dejan de tener dentro una nota, ayuda a la hora
	//de tomar el indicador de precisión.
	public void RemoveActiveNote(string color){
		if (color.Equals ("red")) {
			red = false;
		} else if (color.Equals ("blue")) {
			blue = false;
		}else if(color.Equals("yellow")){
			yellow = false;
		}else{
			green = false;
		}
	}

	//Es llamada cada vez que el jugador toca la pantalla
	void Touch(){
		//Esta condición evita tomar la precisión de la pulsación del botón Volver, cuando un jugador quitar la partida
		if (!end) {
			float[,] sizes = new float[2, 2]{ { 99, 99 }, { 99, 99 } }; //valores por defecto
			int touches = 0, activated = 0; //número de pulsaciones y número de activadores con notas, respectivamente

			for (int i = 0; i < Input.touchCount; ++i) { //por cada pulsación
				if (Input.touchCount > 0 && Input.GetTouch (i).phase == TouchPhase.Began) {
					//Vector con la posición de la pulsación en coordenadas del juego
					Vector3 pos = Camera.main.ScreenToWorldPoint (Input.GetTouch (i).position);

					//Almacenamos la posición de la pulsación
					textWriter.WriteString(playerID+","+date+","+mode+","  + song_id + ",Posición,"+pos_index +",\"("+ pos.x + "," + pos.y + ")\"",file_name);
					pos_index++; //aumentamos el indice de pulsación

					ShowTouch (pos); //llamamos a la función que pinta la zona pulsada

					int aux = 0; //auxiliar para contar los activadores con notas

					//Por cada activador con nota calculamos la distancia de la pulsación hasta el centro
					//del activador e incrementamos aux.
					if (yellow) {
						sizes [i, aux] = (new Vector2 (pos.x + 1.5f, pos.y + 3)).magnitude;
						aux++;
					}
					if (green) {
						sizes [i, aux] = (new Vector2 (pos.x + 0.5f, pos.y + 3)).magnitude;
						aux++;
					}
					if (red) {
						sizes [i, aux] = (new Vector2 (pos.x - 0.5f, pos.y + 3)).magnitude;
						aux++;
					}
					if (blue) {
						sizes [i, aux] = (new Vector2 (pos.x - 1.5f, pos.y + 3)).magnitude;
						aux++;
					}

					activated = aux; //guardamos el número de activadors con nota
					touches = i + 1; //incrementamos el número de pulsaciones
				}
			}

			/*
			 Se puede dar el caso de que haya más pulsaciones que activadores con notas y viceversa.
			 En culquier caso se comprobará qué pulsación se corresponde a cada activador para almacenar
			 la precisión correctamente. Esto lo hace la función CalculateAccuracy.
			*/
			CalculateAccuracy (sizes, touches, activated); 
		}
	}

	/*
	 Muestra en el juego por un tiempo el lugar donde ha pulsado el jugador.
	 Dibuja una 'mancha' con diferente color dependiendo de la precisión la pulsación.
	 Se considera mayor precisión si la pulsación es más cercana al centro de cualquier 
	 activador y menor cuanto más lejos.
	*/
	void ShowTouch(Vector3 pos){
		pos.z = 0;
		//Mostramos la 'mancha' donde ha pulsado el jugador
		GameObject clone = Instantiate(touch, pos, Quaternion.identity);

		//La pintamos la de diferente color dependiendo de la distancia a los activadores.
		//Rojo:mala precisión, Amarillo:precisión media, Verde:buena precisión
		Color c = Color.yellow; //por defecto amarillo
		if (pos.y <= -4.5f || pos.y >= -2.95f || pos.x <= -2.9f || pos.x >= 2.9f )
			c = Color.red;
		if (pos.y > -4.5f || pos.y < -2.95f) {
			if(pos.x >= -1.55f && pos.x <= -1.45f)
				c = Color.red;
			if(pos.x >= -0.05f && pos.x <= 0.05f)
				c = Color.red;
			if(pos.x >=  1.45f && pos.x <= 1.55f)
				c = Color.red;
		}
		if ((pos.y <= -3.5f && pos.y >= -4)) {
			if(pos.x >= -2.5f && pos.x <= -2)
				c = Color.green;
			if(pos.x >= -1 && pos.x <= -0.5f)
				c = Color.green;
			if(pos.x >= 0.5f && pos.x <= 1)
				c = Color.green;
			if(pos.x >= 2 && pos.x <= 2.5f)
				c = Color.green;
		}
		clone.GetComponent<SpriteRenderer> ().color = c; //la ponemos del color correspondiente
		Destroy (clone, 0.13f); //destruimos la 'mancha' pasado un tiempo 

	}

	//Calcula la precisión de las pulsaciones y la guarda. Decide cuál pulsación va destinada a 
	//cada activador ya que sólo puede saber comparando todas las distancias.
	void CalculateAccuracy(float[,] sizes, int touches, int activated){

		if (touches == 1) { //si sólo hay una pulsación 
			//Tomamos la distancia menor por si hay dos activadores con notas
			float size = Math.Min(sizes [0, 0],sizes [0, 1]); 
			SaveAccuracy (size); //almacenamos la precisión
		}else if (touches==2){ //si hay dos pulsaciones
			if (activated == 2) { //si hay dos activadores con notas
				//Calculamos cúal pulsación va destinada a cada activador
				//y almacenamos ambas ambas
				if (sizes [0, 0] < sizes [1, 0]) {
					if (sizes [0, 1] < sizes [1, 1]) {
						if (sizes [0, 0] < sizes [0, 1]) {
							SaveAccuracy (sizes [0, 0]);
							SaveAccuracy (sizes [1, 1]);
						} else {
							SaveAccuracy (sizes [0, 1]);
							SaveAccuracy (sizes [1, 0]);
						}
					} else {
						SaveAccuracy (sizes [0, 0]);
						SaveAccuracy (sizes [1, 1]);
					}
				} else {
					if (sizes [0, 1] > sizes [1, 1]) {
						if (sizes [1, 0] < sizes [1, 1]) {
							SaveAccuracy (sizes [0, 1]);
							SaveAccuracy (sizes [1, 0]);
						} else {
							SaveAccuracy (sizes [0, 0]);
							SaveAccuracy (sizes [1, 1]);
						}
					} else {
						SaveAccuracy (sizes [0, 1]);
						SaveAccuracy (sizes [1, 0]);
					}
				}
			} else if (activated == 1) { //si hay un activador con nota
				//Almacenamos la precisión del de menor distancia
				SaveAccuracy (Math.Min (sizes [0, 0], sizes [1, 0]));
				SaveAccuracy (99); //Almacenamos el otro como fallo
			} else { //si no hay ningún activador con nota
				//Alamcenamos ambos como fallo
				SaveAccuracy (99);
				SaveAccuracy (99);
			}
		}
	}

	//Almacena la precisión al pulsar notas simples
	void SaveAccuracy(float size){
		textWriter.WriteString(playerID+","+date+"," +mode+"," + song_id + ",Precisión,"+accuracy_index +","+size,file_name);
		accuracy_index++;
		if (size != 99) {  //si la pulsación NO está fuera de tiempo
			accuracy_avg += size; //sumamos la precisión para calcular la media
			accuracy_list.Add (size); //añadimos a la lista para calcular la desviación típica
		} else { //si la pulsación está fuera de tiempo
			invalids++; //auxiliar para calcular correctamente la media y la desviación típica
		}

	}

	//Almacena la presión al pulsar notas largas
	public void SavePressure(float pressure){
		textWriter.WriteString(playerID+","+date+","+mode+"," + song_id + ",Presión,"+pressure_index +","+pressure,file_name);
		pressure_index++;
		pressure_avg += pressure; //sumamos la presión para calcular la media
		pressure_list.Add (pressure); //añadimos a la lista para calcular la desviación típica
	}
}
