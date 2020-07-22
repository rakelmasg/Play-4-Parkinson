// Autora: Raquel Mas

using UnityEngine;
using System.IO;

/*
	Script auxiliar para crear y escribir en ficheros de cualquier formato.
*/
public class HandleTextFile : MonoBehaviour
{
	//Escribe el texto al final del fichero
	public void WriteString(string text, string file)
    {
		//ruta completa donde se encuentra el fichero indicado
		string filepath =  Path.Combine(Application.persistentDataPath+"/Indicadores", file);
		//string filepath =  Path.Combine(Application.dataPath+"/Indicadores", file); //SOLO DEBUG
		Directory.CreateDirectory(Path.GetDirectoryName(filepath)); //creamos los directorios si no existen
		StreamWriter writer = new StreamWriter(filepath, true); //clase para escribir en un fichero
		writer.WriteLine(text); //escribe la linea indicada al final del fichero (crea el fichero si no existe)
        writer.Close();
    }
}