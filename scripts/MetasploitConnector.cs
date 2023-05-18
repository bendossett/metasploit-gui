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
using MessagePack;

public partial class MetasploitConnector : Node
{
	private string _msfHost;
	private int _msfPort;
	private string _msfToken;

	public void Init(string username, string password, string msfHost = "127.0.0.1", int msfPort = 55552)
	{
		_msfHost = msfHost;
		_msfPort = msfPort;

		Dictionary<string, object> authResponse = this.Authenticate(username, password);

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

		this.Test2();
	}

	private Dictionary<string, object> RPCCall(string method, params object[] args)
	{
		if (String.IsNullOrEmpty(_msfHost))
		{
			throw new Exception("Host is required.");
		}

		if (method != "auth.login" && string.IsNullOrEmpty(_msfToken))
		{
			throw new Exception("Not authenticated");
		}

		if (String.IsNullOrEmpty(method))
		{
			throw new Exception("Method is required.");
		}

		HttpClient client = new();

		Error e = client.ConnectToHost(_msfHost, _msfPort);
		if (e != Error.Ok)
		{
			throw new Exception("Could not connect to Metasploit.");
		}

		while (client.GetStatus() == HttpClient.Status.Connecting || client.GetStatus() == HttpClient.Status.Resolving)
		{
			client.Poll();
			GD.Print("Connecting...");
			OS.DelayMsec(500);
		}

		if (client.GetStatus() != HttpClient.Status.Connected)
		{
			GD.PrintErr("Could not connect to Metasploit.");
			throw new Exception("Could not connect to Metasploit.");
		}

		List<object> message = new List<object> {method};

		if (method != "auth.login")
		{
			message.Add(_msfToken);
		}

		foreach (object arg in args)
		{
			message.Add(arg);
		}
		

		byte[] messageBin = MessagePackSerializer.Serialize(message);

		var json = MessagePackSerializer.ConvertToJson(messageBin);
		GD.Print(json);

		string[] headers = { "Content-Type: binary/message-pack" };

		e = client.RequestRaw(HttpClient.Method.Post, "/api", headers, messageBin);
		
		if (e != Error.Ok)
		{
			throw new Exception("Request failed to send.");
		}

		while (client.GetStatus() == HttpClient.Status.Requesting)
		{
			client.Poll();
			GD.Print("Requesting...");

			OS.DelayMsec(500);
		}

		if (client.GetStatus() != HttpClient.Status.Body && client.GetStatus() != HttpClient.Status.Connected)
		{
			GD.Print(client.GetStatus());
			throw new Exception("Request Failed");
		}

		if (client.HasResponse())
		{
			List<byte> responseBin = new();

			while (client.GetStatus() == HttpClient.Status.Body)
			{
				client.Poll();
				byte[] chunk = client.ReadResponseBodyChunk();
				if (chunk.Length == 0)
				{
					OS.DelayMsec(500);
				}
				else
				{
					responseBin.AddRange(chunk);
				}
			}

			client.Close();
			
			Dictionary<object, object> responseDict = MessagePackSerializer.Deserialize<Dictionary<object, object>>(responseBin.ToArray());
			
			ConvertDict(responseDict);

			Dictionary<string, object> returnDict = new Dictionary<string, object>();

			foreach (KeyValuePair<object, object> kvp in responseDict)
			{
				returnDict.Add((kvp.Key as byte[]).GetStringFromAscii(), (kvp.Value as byte[]).GetStringFromAscii());
			}

			return returnDict;
		}
		else
		{
			throw new Exception("Did not get response from Metasploit.");
		}
	}

	private Dictionary<string, object> ConvertDict(Dictionary<object, object> input)
	{
		Dictionary<string, object> converted = new();

		foreach (KeyValuePair<object, object> kvp in input)
		{
			string key = (kvp.Key as byte[]).GetStringFromAscii();

			object value = kvp.Value;

			GD.Print(value.GetType());
			GD.Print(value);
		}
	
		return converted;
	}

	private Dictionary<string, object> Authenticate(string username, string password)
	{
		return this.RPCCall("auth.login", username, password);
	}

	private Dictionary<string, object> Test()
	{
		return this.RPCCall("modules.search", new Dictionary<string, object>() {
			{ "include", new List<string>() { "exploits", "payloads"}},
			{ "keywords", new List<string>() { "windows" }},
			{ "maximum", 200 }
		});
	}

	private Dictionary<string, object> Test2()
	{
		return this.RPCCall("module.exploits"	);
	}

	public void _on_button_pressed()
	{
		GD.Print("test");
		this.Init("msf", "3MRs4x6H");
	}
}
