// Autora: Raquel Mas

using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


/*
	Script auxiliar con una única función para mandar los ficheros de los inficadores 
	comprimidos en un zip por email. Después borra los archivos originales y vuelve a 
	crear los ficheros csv de los indicadores de los juegos con sus columnas
	correspondientes (pero vacíos).
*/
public class Mail : MonoBehaviour {

	public HandleTextFile textWriter; //script auxiliar para escribir en ficheros

	public bool SendMail ()
	{
		string nick = PlayerPrefs.GetString ("PlayerID"); //obtenemos el ID del usuario

		//Creamos el email con los campos correspondientes
		MailMessage mail = new MailMessage(); //creamos un mensaje vacio
		mail.From = new MailAddress("danirakeltfg2018@gmail.com"); //añadimos el remitente
		mail.To.Add("danirakeltfg2018@gmail.com"); //añadimos el destinatario
		mail.Subject = "Play4Parkinson resumen diario de " + nick; //añadimos el asunto
		//añadimos el cuerpo del mensaje
		mail.Body = "Hola, te informo que se ha enviado información de "+nick+". Atentamente, equipo Play4Parkinson.";


		//Prearamos los datos para hacer el zip
		string zipName = PlayerPrefs.GetString("PlayerID")+"_"+ DateTime.Now.ToString("yyyy-MM-dd"); //nombre del zip 
		string exportZip = Application.temporaryCachePath + "/"+zipName+".zip"; //ruta completa del zip
		int numAudios = PlayerPrefs.GetInt ("AudioClipID"); //numero de audios que se incluirán
		string[] files = new string[numAudios+3] ; //array donde se guardarán las rutas de todos los ficheros a incluir 
		//string path = Application.dataPath +"/Indicadores/"; //SOLO DEBUG
		string path = Application.persistentDataPath +"/Indicadores/"; //ruta de los ficheros de los indicadores

		//Ponemos en el array las rutas de todos los ficheros que queremos añadir al zip
		for(int i=0; i<numAudios;i++){ //añadimos los archivos de audio que existan
			files[i] =path + "Audio/audio"+i+".wav";
		}
		files[numAudios] =path + "formulario.txt"; //añadimos el fichero de las respuestas de los formularios
		files[numAudios+1] =path + "conducir.csv"; //añadimos el fichero con los indicadores del juego de conducir
		files[numAudios+2] =path + "ritmo.csv"; //añadimos el fichero con los indicadores del juego de ritmo

		try{ 
			//Creamos el zip con todos los archivos indicadores, utilizando un plugin que nos facilita la tarea
			ZipUtil.Zip (exportZip, files);
			//Después adjuntamos el zip al mensaje para mandarlo por email
			mail.Attachments.Add(new Attachment(exportZip, System.Net.Mime.MediaTypeNames.Application.Zip));
		}catch{
			Debug.Log ("No se ha podido crear el Zip.");
			return false;
		}


		//Creamos un cliente SMTP para enviar el email
		SmtpClient smtpServer = new SmtpClient("smtp.gmail.com"); //utilizamos el servidor de gmail
		smtpServer.Port = 587; //indicamos su puerto
		//rellenamos las credenciales para poder mandar el correo
		smtpServer.Credentials = new System.Net.NetworkCredential("danirakeltfg2018@gmail.com", "Dorami93") as ICredentialsByHost;
		smtpServer.EnableSsl = true;
		ServicePointManager.ServerCertificateValidationCallback = 
			delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) 
		{ return true; };

		try{
		smtpServer.Send(mail); //mandamos el mensaje por email con el zip adjunto
		}catch{
			Debug.Log ("No se ha podido enviar el mail.");
			return false;
		}

		//Borramos los todos ficheros que se han envíado en el zip
		File.Delete (path + "formulario.txt");
		File.Delete (path + "conducir.csv");
		File.Delete (path + "ritmo.csv");
		for(int i=0; i<numAudios;i++){
			File.Delete (path + "Audio/audio"+i+".wav");
		}
		PlayerPrefs.SetInt ("AudioClipID", 0); //reseteamos el indice de clips de audios

		//Creamos de nuevo los csv de los indicadores, con los campos vacíos
		textWriter.WriteString ("ID_Usuario,Fecha,Turno,Dificultad,Indice_Partida,Indicador,Indice_Indicador,Valor","ritmo.csv");
		textWriter.WriteString ("ID_Usuario,Fecha,Turno,Dificultad,Indice_Partida,Indicador,Indice_Indicador,\"Valor/X,Y,Z\"","conducir.csv");

		Debug.Log ("Mail enviado.");
		return true;
	}
}