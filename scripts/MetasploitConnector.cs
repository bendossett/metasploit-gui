/**
*  Based largely on https://github.com/VolatileMindsLLC/metasploit-sharp/blob/master/metasploit-sharp/MetasploitSession.cs , with modifications.
*  Also uses:
* 		https://docs.godotengine.org/en/stable/tutorials/networking/http_client_class.html
*
*
*/


using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MessagePack;

public partial class MetasploitConnector : Node
{
	private static string _msfHost;
	private static int _msfPort;
	private static string _msfURI;
	private static string _msfToken;

	private static readonly string[] Headers = new string[] { "Content-Type: binary/message-pack" };

	public static async Task Connect(string username, string password, string msfHost = "127.0.0.1", int msfPort = 55553)
	{
		_msfHost = msfHost;
		_msfPort = msfPort;

		_msfURI = "http://" + _msfHost + ":" + msfPort + "/api";
		
		Dictionary<string, object> authResponse = await Authenticate(username, password);

		bool authenticated = !authResponse.ContainsKey("error");

		if (!authenticated)
		{
			GD.PrintErr("Error authenticating with Metasploit");
			throw new Exception("Error authenticating with Metasploit");
		}

		if ((authResponse["result"] as string) == "success")
		{
			_msfToken = authResponse["token"] as string;
		}
		GD.Print("Connected!");
	}

	public static async Task<Dictionary<string, object>> RPCCall(string method, params object[] args)
	{
		if (string.IsNullOrEmpty(_msfHost))
		{
			throw new Exception("Host is required.");
		}

		if (method != "auth.login" && string.IsNullOrEmpty(_msfToken))
		{
			throw new Exception("Not authenticated");
		}

		if (string.IsNullOrEmpty(method))
		{
			throw new Exception("Method is required.");
		}

		HttpRequest httpRequest = new HttpRequest();
		((SceneTree)Engine.GetMainLoop()).Root.AddChild(httpRequest);

		// TaskCompletionSource httpReady = new TaskCompletionSource();
		//
		// httpRequest.Ready += () =>
		// {
		// 	httpReady.SetResult();
		// };

		// await httpReady.Task;

		TaskCompletionSource<Dictionary<string, object>> tcs = new TaskCompletionSource<Dictionary<string, object>>();

		httpRequest.RequestCompleted += ((result, code, headers, body) =>
		{
			HttpRequestCompleted(result, code, headers, body, tcs);
		});

		List<object> message = new List<object> {method};

		if (method != "auth.login")
		{
			message.Add(_msfToken);
		}

		message.AddRange(args);

		byte[] messageBin = MessagePackSerializer.Serialize(message);

		httpRequest.RequestRaw(_msfURI, Headers, HttpClient.Method.Post, messageBin);

		return await tcs.Task;
	}

	private static void HttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body, TaskCompletionSource<Dictionary<string, object>> tcs)
	{
		if (result != (long)HttpRequest.Result.Success)
		{
			throw new Exception("Request failed.");
		}

		Dictionary<object, object> responseDict = MessagePackSerializer.Deserialize<Dictionary<object, object>>(body.ToArray());

		tcs.SetResult(ConvertDict(responseDict));
	}

	private static Dictionary<string, object> ConvertDict(Dictionary<object, object> input)
	{
		Dictionary<string, object> converted = new();
		
		foreach (var (key, value) in input)
		{
			string keyString = key switch
			{
				byte[] b => b.GetStringFromAscii(),
				string s => s,
				_ => ""
			};

			object valueObj = value switch
			{
				byte[] b => b.GetStringFromAscii(),
				string s => s,
				object[] => value,
				_ => value
			};

			if (valueObj == null)
			{
				GD.Print(value.GetType());
			}

			converted.Add(keyString, valueObj);
		}
	
		return converted;
	}

	private static async Task<Dictionary<string, object>> Authenticate(string username, string password)
	{
		return await RPCCall("auth.login", username, password);
	}
}
