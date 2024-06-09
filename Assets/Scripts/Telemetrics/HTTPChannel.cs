using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPChannel : MonoBehaviour
{
	public static HTTPChannel Instance
	{
		get
		{
			if (instance)
				return instance;

			instance = FindObjectOfType<HTTPChannel>();
			if (instance)
				return instance;

			instance = new GameObject("HTTPChannel").AddComponent<HTTPChannel>();
			return instance;
		}
	}

	static HTTPChannel instance;

	const string urlGet = "https://tfvj.etsii.urjc.es/get";
	const string urlInsert = "https://tfvj.etsii.urjc.es/insert";
	
	const string username = "FGMVAB";
	const string password = "2024TFGjuegogestionecoPC";

	private void Awake()
	{
		if (instance)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;
		DontDestroyOnLoad(gameObject);

		var dict = new Dictionary<string, string>
		{
			{ "name", "Carmelita" }
		};

		var json = GetJsonReadRequest(dict, "test");

		//Comprueba que está bien formado el json. Yo creo que sí, lo he validado en internet y es igual que el de Maria para filtrar
		Debug.Log(json);

		StartCoroutine(SendPostReadRequest(json));
	}


	IEnumerator SendPostReadRequest(string json)
	{
		//Construye UnityWebRequest para enviar solicitud 
		UnityWebRequest request = UnityWebRequest.Post(urlGet, json);

		// Configurar la solicitud (headers, etc.) si es necesario
		request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded"); //Este es el de Maria 
		//request.SetRequestHeader("Content-Type", "application/json"); //Esto no se si está bien. En internet pone que se use application/json pero en cualquier caso no funca

		// Enviar la solicitud y esperar la respuesta
		yield return request.SendWebRequest();

		// Verificar si hay errores
		if (request.result != UnityWebRequest.Result.Success)
		{
			//Si sale error 500, no sabemos por qué es ya que el 500 es el cajon de mierda donde nadie te dice por qué no funca
			Debug.LogWarning("Error: " + request.error + request.result);
		}
		else
		{
			// La solicitud fue exitosa, puedes acceder a la respuesta
			Debug.Log("Respuesta: " + request.downloadHandler.text);
		}
	}


	static string GetJsonReadRequest(Dictionary<string, string> data, string table)
	{

		string filterLine = "";

		if (data.Count != 0)
		{
			foreach (var item in data)
			{
				filterLine += $@"""{item.Key}"":""{item.Value}"",";
			}
			filterLine = filterLine.Remove(filterLine.Length - 1);
		}

		string json = $@"
{{
  ""username"": ""{username}"",
  ""password"": ""{password}"",
  ""table"": ""{table}"",
  ""filter"": {{ {filterLine} }}
}}";

		return json;

	}
}
