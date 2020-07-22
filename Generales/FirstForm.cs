// Autora: Raquel Mas

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

/*
	Script de la pantalla del formulario de registro.
	Muestra en tres partes las preguntas del formulario,
	comprueba que se rellenan todos los campos obligatorios
	y correctamente. Luego guarda los datos en un fichero de
	texto para que puedan ser enviados por correo.
*/
public class FirstForm : MonoBehaviour {
	public AudioSource audioSource; //reproductor de sonido
	public HandleTextFile textWriter; //script para escribir en ficheros
	string date; // día actual

	public GameObject p1, p2, p3; //partes del formulario
	public InputField nick, age, city, country; //campos de la primera parte
	public InputField updrs, hy, PDMed, PDMedAmount, nosocomial; //campos de la segunda parte
	public InputField otherDis, otherMed; // campos de la tercera parte
	public Toggle woman, tobaco, alcohol; // respuestas que se pueden marcar y desmarcar
	public Dropdown impairment; // respuestas de la pregunta de discapacidad en un menú desplegable
	public Text info; //información para indicar si faltan preguntas por responder o no son válidas

	//auxiliares para el menú desplegable
	int impairmentInt;
	string impairmentString;

	void Update(){
		//actualizamos la respuesta seleccionada en el menú desplegable
		impairmentInt = impairment.value;
	}

	//Se ejecuta cuando se pulsa el botón de continuar. 
	//Llama a las funciones que se encargan de evaluar cada parte del formulario.
	public void Continue(){
		audioSource.Play (); //reproducimos el sonido del botón 
		//Dependiendo de la parte del formulario que se esté mostrando llamamos a su función
		if (p1.activeSelf) {
			partOne ();
		} else if (p2.activeSelf) {
			partTwo ();
		} else {
			partThree ();
		}
	}

	/*
	 Se encarga de evaluar que todas las respuestas de la primera parte del formulario
	 estén respondidas y de forma correcta. Si lo están carga la siguiente parte del 
	 formulario, si no informa al usuario.
	*/
	void partOne(){
		//Si se han respondido a todas las preguntas 
		if (nick.text != "" && age.text != "" && city.text != "" && country.text != "" ) {
			bool ok = true; //auxiliar para ver que edad es tiene un valor válido

			//Comprobamos que la edad tenga un valor válido
			try{
				int ageInt = int.Parse (age.text);
				if(ageInt<20 || ageInt>110 ){ //en el caso de que esté fuera de rango
					ok = false; //indicamos que no se puede continuar
				}
			}catch{ //en el caso de que no sea un valor numérico
				ok = false; //indicamos que no se puede continuar
			}
				
			if (ok) { //si la edad tiene un valor válido
				info.text = "";
				p1.SetActive (false); //ocultamos la parte actual del formulario
				p2.SetActive (true); //mostramos la siguiente parte del formulario
			} else { //si no se lo indicamos al jugador
				info.text = "Por favor, introduzca una edad válida";
			}
		} else { //si no se han respondido todas las preguntas se lo indicamos al jugador
			info.text = "Por favor, rellene todos los campos";
		}
	}

	/*
	 Se encarga de evaluar que todas las respuestas de la segunda parte del formulario
	 estén respondidas y de forma correcta. Si lo están carga la siguiente parte del 
	 formulario, si no informa al usuario.
	*/
	void partTwo(){
		//Si se han respondido a todas las preguntas obligatorias
		if (updrs.text != "" && hy.text != "" && PDMed.text != "" && PDMedAmount.text != "" ) {
			bool ok = true; //auxiliar para ver que edad es tiene un valor válido

			//Comprobamos que se haya introducido un valor válido de la escala Hoehn y Yahr
			try{
				float hyInt = float.Parse (hy.text); 

				// Comprobamos que el valor esté dentro del rango
				if(hyInt!=0 && hyInt!=1 &&hyInt!=1.5 &&hyInt!=2 &&hyInt!=2.5 &&hyInt!=3 && hyInt!=4 && hyInt!=5){
					ok = false;//indicamos que no se puede continuar
					info.text = "Por favor, introduzca una escala de Hoëhn y Yahr válida";
				}
			}catch{ //en el caso de que no sea un valor numérico
				ok = false; //indicamos que no se puede continuar
				info.text = "Por favor, introduzca una escala de Hoëhn y Yahr válida";
			}

			//Comprobamos que se haya introducido un valor válido de la escala UPDRS
			try{
				int updrsInt = int.Parse (updrs.text);
				// Comprobamos que el valor esté dentro del rango
				if(updrsInt<0 || updrsInt>159){
					ok = false; //indicamos que no se puede continuar
					info.text = "Por favor, introduzca una escala de UPDRS válida";
				}
			}catch{ //en el caso de que no sea un valor numérico
				ok = false; //indicamos que no se puede continuar
				info.text = "Por favor, introduzca una escala de UPDRS válida";
			}

			if (ok) { //si los valores de ambas escalas son válidos
				info.text = "";
				p2.SetActive (false); //ocultamos esta parte del formulario
				p3.SetActive (true); //mostramos la siguiente parte del formulario
			}
		} else { //si no se han respondido todas las preguntas obligatorias se lo indicamos al jugador
			info.text = "Por favor, rellene todos los campos";
		}
	}

	/*	 
		Ya que en esta parte del formulario las respuestas no son obligatorias no las evalua.
		Escribe todos las respuestas del formulario en un fichero de texto plano para que luego
		sean enviadas por correo. También prepara los ficheros csv donde se van a apuntar las
		mediciones de los indicadores durante los juegos. Por último carga la pantalla de los 
		ejercicios de voz.
	*/
	void partThree(){
		date = DateTime.Now.ToString("yyyyMMdd"); // obtenemos fecha para usarla de ID
		//Guardamos el ID del usuario como la combinación de la fecha de hoy y su apodo
		PlayerPrefs.SetString ("PlayerID", nick.text + date);

		//Escribimos en el fichero de texto todas las respuestas del formulario
		textWriter.WriteString ("\nID de Usuario: "+PlayerPrefs.GetString("PlayerID"),"formulario.txt");
		if (woman.isOn) //si está marcada la opción de mujer guardamos mujer
			textWriter.WriteString ("\nSexo: Mujer","formulario.txt");
		else //si no guardamos hombre
			textWriter.WriteString ("\nSexo: Hombre","formulario.txt");
		textWriter.WriteString ("\nEdad: "+age.text,"formulario.txt");
		textWriter.WriteString ("\nCiudad: "+city.text,"formulario.txt");
		textWriter.WriteString ("\nPaís: "+country.text,"formulario.txt");
		textWriter.WriteString ("\nEscala de Parkinson UPDRS: "+updrs.text,"formulario.txt");
		textWriter.WriteString ("\nEscala de Parkinson Hoën Yahr: "+hy.text,"formulario.txt");
		textWriter.WriteString ("\nMedicación para el Parkinson: ","formulario.txt");
		textWriter.WriteString ("\n\tNombre: "+PDMed.text,"formulario.txt");
		textWriter.WriteString ("\n\tPosología: "+PDMedAmount.text,"formulario.txt");
		textWriter.WriteString ("\nInfecciones nosocomiales: "+nosocomial.text,"formulario.txt");
		textWriter.WriteString ("\nOtras enfermedades: "+otherDis.text,"formulario.txt");
		textWriter.WriteString ("\nOtras medicación crónica: "+otherMed.text,"formulario.txt");
		//Guardamos la opción escogida el menú desplegable sobre la pregunta de discapacidad
		switch(impairmentInt){
		case 0:
			impairmentString = "Ninguna";
			break;
		case 1:
			impairmentString = "Motora";
			break;
		case 2:
			impairmentString = "Visual";
			break;
		case 3:
			impairmentString = "Auditiva";
			break;
		case 4:
			impairmentString = "Psíquica";
			break;
		default:
			impairmentString = "Intelectual";
			break;
		}
		textWriter.WriteString ("\nDiscapacidad: "+impairmentString,"formulario.txt");
		if (tobaco.isOn) //Si está marcada la opción del tabaco ponemos "Sí"
			textWriter.WriteString ("\nTabaco: Sí","formulario.txt");
		else //si no apuntamos "No"
			textWriter.WriteString ("\nTabaco: No","formulario.txt");
		if (alcohol.isOn) //Si está marcada la opción del alcohol ponemos "Sí"
			textWriter.WriteString ("\nAlcohol: Sí","formulario.txt");
		else //si no apuntamos "No"
			textWriter.WriteString ("\nAlcohol: No","formulario.txt");
		
		//Creamos los ficheros csv donde se van a guardar las mediciones de los indicadores durante los videojuegos,
		//rellenando la primera linea con las columnas correspondientes.
		textWriter.WriteString ("ID_Usuario,Fecha,Turno,Dificultad,Indice_Partida,Indicador,Indice_Indicador,Valor","ritmo.csv");
		textWriter.WriteString ("ID_Usuario,Fecha,Turno,Dificultad,Indice_Partida,Indicador,Indice_Indicador,\"Valor/X,Y,Z\"","conducir.csv");

		//Indicamos que no ya se ha respondido el formulario de registro para que no se vuelva a llamar desde
		//la pantalla de incio las próximas veces que se ejecute la aplicación
		PlayerPrefs.SetInt ("FirstTime",1);
		SceneManager.LoadScene ("cuestionarioDiario"); //cargamos siguiente formulario

	}
}
