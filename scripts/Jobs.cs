using Godot;
using System;
using System.Collections.Generic;

public partial class Jobs : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InitAsync();
	}

	private async void InitAsync()
	{
		GD.Print("here");
		Dictionary<string, object> jobList = await MetasploitAPI.Job.List();

		foreach (string k in jobList.Keys)
		{
			GD.Print(k);
			GD.Print(jobList[k].GetType());
		}
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
