using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class TestConnection : MonoBehaviour
{
	const string urlGet = "https://tfvj.etsii.urjc.es/get";
	const string urlInsert = "https://tfvj.etsii.urjc.es/insert";
	const string urlLogin = "https://tfvj.etsii.urjc.es/rest/login";

	//const string username = "TFGMVAB";
	const string username = "FPJGI";
	//const string password = "2024TFGjuegogestionecoPC";
	const string password = "2024FPpatrones";

	string token;

	private void Awake()
	{
		GetToken();
	}

	//---------- ACCESO BASE DE DATOS ----------
	IEnumerator DBAccess(string data, string typeOfRequest, Action<UnityWebRequest> callback)
	{
		byte[] bodyRaw = Encoding.UTF8.GetBytes(data);

		UnityWebRequest request = new UnityWebRequest(typeOfRequest, "POST");
		request.uploadHandler = new UploadHandlerRaw(bodyRaw);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");

		yield return request.SendWebRequest();

		callback?.Invoke(request);
	}

	public void GetToken()
	{
		string data = @"{""username"":""FPJGI"", ""password"":""2024FPpatrones""}";
		//string data = @"{""username"":""TFGMVAB"", ""password"":""2024TFGjuegogestionecoPC""}";
		Debug.Log(data);
		StartCoroutine(DBAccess(data, urlLogin, (request) =>
		{
			if (request.result != UnityWebRequest.Result.Success)
			{
				Debug.Log("Error al obtener TOKEN");
			}
			else
			{
				TokenResponse response = JsonUtility.FromJson<TokenResponse>(request.downloadHandler.text);
				token = response.token;

				string json = $@"
{{
  ""username"": ""{username}"",
  ""token"": ""{token}"",
  ""password"": ""{password}"",
  ""table"": ""users"",
  ""filter"":  """" 
}}";

				var action = new Action<UnityWebRequest>((request) =>
				{
					if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
					{
						Debug.LogError(request.error);
					}
					else
					{
						Debug.Log(request.downloadHandler.text);
					}
				});
				StartCoroutine(DBAccess(json, urlGet, action));
			}


		}));
	}

	//---------- OTROS ----------
	public string GetTimeSpent(string start, string end)
	{
		Debug.Log("Start: " + start + ", End: " + end);
		string format = "HH:mm:ss.fff";

		DateTime time1 = DateTime.ParseExact(start, format, null);
		DateTime time2 = DateTime.ParseExact(end, format, null);

		TimeSpan difference = time2 - time1;

		return $"{difference.Hours}:{difference.Minutes}:{difference.Seconds}:{difference.Milliseconds}";
		//return difference;
	}


	[Serializable]
	public class TokenResponse
	{
		public string result;
		public string token;
		public string until;
	}
}