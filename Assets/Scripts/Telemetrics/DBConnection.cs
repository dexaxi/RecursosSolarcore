using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class DBConnection : MonoBehaviour
{
	private string token;
	private const string
		_hello = "https://tfvj.etsii.urjc.es/rest/hello",
		_login = "https://tfvj.etsii.urjc.es/rest/login",
		_get = "https://tfvj.etsii.urjc.es/rest/get",
		_insert = "https://tfvj.etsii.urjc.es/rest/insert",
		_update = "https://tfvj.etsii.urjc.es/rest/update";


	int user_id = 0;
	public string username;
	public string gender;
	public int age;
	public int machines_placed;
	public int machines_sold;
	public int phase_sucess;
	public int phase_fail;
	public int completition;
	public float duration;

	[Range(0, 10)]
	public float updateTimeInMinutes = 2;

	public static DBConnection Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null)
		{
			//Nope, nope, nope Estoy cansado jefe
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(this);

	}

	void Start()
	{
		if (PlayerInfoManager.Instance != null)
		{
			age = PlayerInfoManager.Instance.Age;
			username = PlayerInfoManager.Instance.name;
			gender = PlayerInfoManager.Instance.gender.ToString();
		}

		GetToken();
	}

	//---------- TOKEN ----------
	public void GetToken()
	{
		string data = @"{""username"":""TFGMVAB"", ""password"":""2024TFGjuegogestionecoPC""}";
		Debug.Log(data);
		StartCoroutine(DBAccess(data, _login, (request) =>
		{
			if (request.result != UnityWebRequest.Result.Success)
			{
				Debug.Log("Error al obtener TOKEN");
			}
			else
			{
				Debug.Log("TOKEN obtenido");
				TokenResponse response = JsonUtility.FromJson<TokenResponse>(request.downloadHandler.text);
				token = response.token;

				InsertNewUser(username, age, gender, machines_placed, machines_sold, phase_sucess, phase_fail, (int)(duration / 60), completition, null);

			}

			request.Dispose();
		}));
	}

	public void GetAllUsers(Action<int> callback)
	{
		string data = $@"{{
            ""username"":""TFGMVAB"",  ""password"":""2024TFGjuegogestionecoPC"", ""token"":""{token}"",
            ""table"":""users"",
            ""filter"":{{}}
        }}";
		StartCoroutine(DBAccess(data, _get, (request) =>
		{
			if (request.result != UnityWebRequest.Result.Success)
			{
				Debug.Log("Error: " + request.error);
				callback?.Invoke(-1);
			}
			else
			{
				// La solicitud fue exitosa, puedes acceder a la respuesta
				UsersResponse response = JsonUtility.FromJson<UsersResponse>(request.downloadHandler.text);
				int newId = 1;
				if (response.data.Count != 0)
				{
					List<User> list = response.data.OrderBy(x => x.id).ToList();
					User user = list[list.Count - 1];
					newId += user.id;
				}
				callback?.Invoke(newId);
			}

			request.Dispose();
		}));
	}

	public void GetUser(int id, Action<User> callback)
	{
		string data = $@"{{
            ""username"":""TFGMVAB"", ""password"":""2024TFGjuegogestionecoPC"", ""token"":""{token}"",
            ""table"":""users"",
            ""filter"":{{""id"":""{id}""}}
        }}";

		StartCoroutine(DBAccess(data, _get, (request) =>
		{
			if (request.result != UnityWebRequest.Result.Success)
			{
				callback?.Invoke(null);
			}
			else
			{
				UsersResponse response = JsonUtility.FromJson<UsersResponse>(request.downloadHandler.text);
				if (response.data.Count == 0)
				{
					callback?.Invoke(null);     //Se ha enviado bien la petici�n pero no existe el usuario
				}
				else
				{
					callback?.Invoke(response.data[0]);
				}
			}

			request.Dispose();
		}));
	}

	public void InsertNewUser(string username, int age, string gender, int machines_placed, int machines_sold, int phase_success, int phase_fail, int duration, int completition, Action<int> callback)   //Registra un nuevo usuario en la bbdd
	{
		GetAllUsers((newId) =>
		{
			if (newId != -1)
			{
				Debug.Log("Insertando usuario con id: " + newId);
				string data = $@"{{
                        ""username"":""TFGMVAB"", ""password"":""2024TFGjuegogestionecoPC"", ""token"":""{token}"",
                        ""table"":""users"",
                        ""data"":{{""user_id"":""{newId}"",""name"": ""{username}"", ""age"": ""{age}"", ""gender"": ""{gender}"", ""machines_placed"": ""{machines_placed}"", ""machines_sold"" : ""{machines_sold}"", ""progress"" : ""{completition}"", ""success_phase"" : ""{phase_success}"", ""failure_phase"" : ""{phase_fail}"", ""duration"" : ""{duration}""}}
                }}";

				user_id = newId;

				Debug.Log(data);
				StartCoroutine(DBAccess(data, _insert, (request) =>
				{
					if (request.result != UnityWebRequest.Result.Success)
					{
						Debug.Log(request.downloadHandler.data);
						callback?.Invoke(-1);
					}
					else
					{
						// La solicitud fue exitosa, puedes acceder a la respuesta
						Debug.Log(request.downloadHandler.text);
						callback?.Invoke(newId);
					}

					request.Dispose();
				}));
			}
			else
			{
				callback?.Invoke(-1);
			}
		});
	}

	public void InsertUser(int id, string username, int age, string gender, int machines_placed, int machines_sold, int phase_success, int phase_fail, int duration, int completition, Action<int> callback)   //Registra un nuevo usuario en la bbdd
	{
		string data = $@"{{
                        ""username"":""TFGMVAB"", ""password"":""2024TFGjuegogestionecoPC"", ""token"":""{token}"",
                        ""table"":""users"",
                        ""data"":{{""user_id"":""{id}"",""name"": ""{username}"", ""age"": ""{age}"", ""gender"": ""{gender}"", ""machines_placed"": ""{machines_placed}"", ""machines_sold"" : ""{machines_sold}"", ""progress"" : ""{completition}"", ""success_phase"" : ""{phase_success}"", ""failure_phase"" : ""{phase_fail}"", ""duration"" : ""{duration}""}}
                }}";

		Debug.Log(data);
		StartCoroutine(DBAccess(data, _insert, (request) =>
		{
			if (request.result != UnityWebRequest.Result.Success)
			{
				Debug.Log(request.downloadHandler.data);
			}
			else
			{
				// La solicitud fue exitosa, puedes acceder a la respuesta
				Debug.Log(request.downloadHandler.text);
			}

			request.Dispose();

		}));


	}

	public void UpdateUser()   //Registra un nuevo usuario en la bbdd
	{
		InsertUser(user_id, username, age, gender, machines_placed, machines_sold, phase_sucess, phase_fail, (int)(duration / 60), completition, null);
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

		request.disposeUploadHandlerOnDispose = true;
		request.disposeDownloadHandlerOnDispose = true;

		callback?.Invoke(request);
	}


	float elapsedTime = 0;
	private void Update()
	{
		elapsedTime += Time.deltaTime;
		duration += Time.deltaTime;
		if (elapsedTime >= updateTimeInMinutes * 60)
		{
			if (BiomePhaseHandler.Instance)
			{
				var dict = BiomePhaseHandler.Instance.CurrentCompletion;

				var total = 0;
				foreach (var item in dict)
				{
					total += Mathf.Clamp(item.Value, 0, 100);
				}

				completition = 0;
				if (dict.Count > 0)
					completition = total / dict.Count;
			
			}

			UpdateUser();
			elapsedTime = 0;
		}

	}
}

//---------- RESPONSE CLASSES ----------
[Serializable]
public class LevelResponse
{
	public string result;
	//public List<LevelData> data;
}

[Serializable]
public class TokenResponse
{
	public string result;
	public string token;
	public string until;
}

[Serializable]
public class UsersResponse
{
	public string result;
	public List<User> data;
}

[Serializable]
public class User
{
	public int id;
	public string name;
	public int age;
	public string gender;
	public int machines_placed;
}
