// Autora: Raquel Mas

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	Representa a un coche en el juego y es controlado por el jugador.
	El jugador deberá digirirlo por la carretera lo más centrado 
	posible, recogiendo las moneas para obtener puntos y evitando
	chocar con los obstaculos que restan puntos. El coche se controla
	con el acelerómetro del dispositivo móvil, si se inclina a 
	derecha o izquiera el coche girará en la dirección correspondiente.
	Si se inclina el móvil hacia el jugador el coche realizará un
	salto, para poder esquivar los obstaculos.
*/
public class Car : MonoBehaviour {

	//Variables del juego
	AudioSource audioS; //reproductor de audio 
	public AudioClip crash, jump, coin; //efectos de sonido
	Rigidbody carRB; //componente del coche que controla su velocidad
	float speed = 2.5f; //velocidad del coche
	public GameObject gm; //referencia al controlador del juego

	//Variables auxiliares
	bool started = false; //variable que indica si se ha iniciado el juego
	bool isJumping = false; //variable para saber si el coche está saltando
	int count = 1; //variable para el salto, controla cuanto sube el coche
	bool onCollision = false; //variable para saber si el coche ha chocado con un obstaculo


	//Al cargar la escena
	void Awake(){
		carRB = GetComponent<Rigidbody> (); //obtenemos el rigidbody del coche
		audioS = GetComponent<AudioSource> (); //obtenemos el reproductor de audio del coche
		StartCoroutine("Wait"); //da la velocidad al coche y permite que lo mueva el jugador
	}

	//Se encarga de iniciar el coche
	IEnumerator Wait(){
		yield return new WaitForSeconds (4f); //espera por 4 segundos
		carRB.velocity = new Vector3 (0,0,speed); //asignamos la velocidad al coche
		started = true; //indicamos que ha comenzado el juego y se puede controlar
	}

	void Update () {
		if (started) { //si el juego ha comenzado
			//Hace que el coche gire según la inclinación del móvil
			Move ();
			if (isJumping) { //si el coche está efectuando un salto
				Jumping (); //llamamos a la función mueve el coche arriba y luego abajo
			} else if (!onCollision) { //si el coche no está en una colisión
				Jump (); //Hace que el coche salte si se inclina el móvil hacia el jugador
			}
		}
	}

	//Controla la dirección del coche según la inclinación del móvil
	void Move (){
		//Operación auxiliar para mirar la posición del acelerómetro
		float x = Mathf.Round(Input.acceleration.x*10);
		//Según la inclinación calculamos el ángulo de giro
		if (x > 0) {
			transform.Rotate(Vector3.up * Time.deltaTime*20);
		}else if(x < 0){
			transform.Rotate(Vector3.up * Time.deltaTime*-20);
		}
		//Aplicamos el angulo de giro al vector de velocidad para cambiar
		//la dirección del coche
		carRB.velocity = transform.forward*speed;
	}

	//Controla cuando salta el coche
	void Jump(){
		//Operación auxiliar para mirar la posición del acelerómetro
		float x = Mathf.Round(Input.acceleration.y*10);
		//Si el móvil está suficientemente inclinado hacia el jugador
		if (x < -8) {
			audioS.clip = jump; //asignamos al reproductor el audio de salto
			audioS.Play (); //reproducimos el audio de salto
			isJumping = true; //indicamos que el coche está saltando
		}
	}

	/*
	 	Se ejecuta cuando el coche está saltando. Primero hace subir al
	 	coche poco a poco hasta que alcanza cierta altura. Luego hace que
	 	el coche baje hasta volver a tocar el suelo.
	*/
	void Jumping(){
		Vector3 y = transform.position; //posición vertical del coche
		if (count <= 75) { //contador auxiliar para que el coche suba hasta cierta altura
			//si no se ha alcanzado la altura máxima subimos un poco el coche
			transform.position = new Vector3(transform.position.x,transform.position.y+0.06f,transform.position.z);
			count++; //aumentamos el contador auxiliar
		} else if(count <= 85){ //mantenemos el coche un tiempo en el aire
			count++; //aumentamos el contador auxiliar
		}else { //cuando el coche ha alcanzado la altura máxima hacemos que baje hasta el suelo
			if (transform.position.y > -0.25f) { //si el coche no ha alcanzado el suelo
				//bajamos un poco el coche
				transform.position = new Vector3(transform.position.x,transform.position.y-0.06f,transform.position.z);
			} else { //si el coche ya ha bajado al suelo
				//ajustamos su altura por si ha bajado demasiado y está atravesando el suelo
				transform.position = new Vector3(transform.position.x,-0.25f,transform.position.z);
				count = 0; //colocamos el contador a 0 para el próximo salto
				isJumping = false; //indicamos que ya no está saltando
			}
		}
	}

	/*
	 	Se ejecuta cuando el coche atraviesa un trigger, sólo controla las
	 	los checkpoint y las monedas ya que las trigger de los lados de la
	 	carretera se controlan en su propio script.
	*/
	void OnTriggerEnter(Collider col){
		if (col.gameObject.tag == "CheckPoint") { //si el objeto es un checkpoint
			col.gameObject.SetActive (false); //desactivamos el objeto
			/*
			 	Activamos pasado un tiempo el objeto de nuevo ya que los tramos de
				la carretera siempre son los mismos y van intercambiandose a lo largo
				del recorrido.
			*/
			StartCoroutine ("Reset",col.gameObject);
			//Llamamos a la función del manager que crea un nuevo tramo de carretera
			gm.GetComponent<CarGameManager> ().CreateRoad(); 
		}
		if (col.gameObject.tag == "Coin") { //si el objeto es una moneda
			//aumentamos la puntuación 
			PlayerPrefs.SetInt ("CarScore", PlayerPrefs.GetInt ("CarScore") + 50);
			audioS.clip = coin; //asignamos al reproductor de audio el sonido de la moneda
			audioS.Play (); //reproducimos el sonido
			col.gameObject.SetActive (false); //desactivamos el objeto
			/*
			 	Activamos pasado un tiempo el objeto de nuevo ya que los tramos de
				la carretera siempre son los mismos y van intercambiandose a lo largo
				del recorrido.
			*/
			StartCoroutine ("Reset",col.gameObject);
		}
	}

	/*
		Se ejecuta cuando se produce una colisión. Sólo se evaluan las colisiones
		con objetos marcados como obstaculos y no con el resto de decoración.
		Los obstaculos son piedras y vallas que se encuentran en mitad de la
		carretera.
	*/
	void OnCollisionEnter(Collision col){
		//Evitamos que se reproduzca el sonido y vibre cuando el coche cae en el suelo
		if(!col.gameObject.name.Equals("Way")){
			audioS.clip = crash; //asignamos al reproductor de audio el sonido de colisión
			audioS.Play (); //reproducimos el sonido
			Handheld.Vibrate (); //hacemos vibrar el móvil
		}
		if (col.gameObject.tag == "Obstacle") //si el objeto es un obstaculo
			StartCoroutine ("Collision",col.gameObject); //llamamos al la función de colisión
	}

	//Se llama cuando se produce una colisión con un obstaculo de la carretera
	IEnumerator Collision(GameObject obstacle){
		if(PlayerPrefs.GetInt("CarScore")>=25) //si la puntuación no es menor de 25
			PlayerPrefs.SetInt ("CarScore",PlayerPrefs.GetInt("CarScore")-25); //restamos puntos
		onCollision = true; //indicamos que se está hay una colisión para que el coche no pueda saltar
		yield return new WaitForSeconds (1.5f); //esperamos por 1.5 segundos
		obstacle.SetActive (false); //desactivamos el obstaculo para que el coche pueda pasar
		/*
		 	Activamos pasado un tiempo el obstaculo de nuevo ya que los tramos de
			la carretera siempre son los mismos y van intercambiandose a lo largo
			del recorrido.
		*/
		StartCoroutine ("Reset",obstacle); //reinciamos 
		onCollision = false; //indicamos que ya no se está produciendo una colisión
	}

	//Reinicia los objetos de la carretera pasado un tiempo para que el tramo de 
	//carretera se pueda reutilizar.
	IEnumerator Reset(GameObject obj){
		yield return new WaitForSeconds (6f); //pasados 6 segundos
		obj.SetActive (true); //volvemos a activar el objeto
	}

}
