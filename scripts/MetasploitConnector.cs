using Godot;
using System;
using MessagePack;

public partial class MetasploitConnector : Node
{
	private HttpClient _client = new();

	public void Init(string msfHost = "127.0.0.1", int msfPort = 55552)
	{
		Error e = _client.ConnectToHost(msfHost, msfPort);

		if (e != 0)
		{
			throw new Exception("Client was unable to connect.");
		}
	}

	private async void RPCCall(string method, params object[] args)
	{
		if (String.IsNullOrEmpty(method))
		{
			throw new Exception("Method is required.");
		}

		
	}
}
