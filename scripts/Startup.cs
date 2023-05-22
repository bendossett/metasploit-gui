using Godot;
using System;

public partial class Startup : Node
{
	private string _username = "msf";
	private string _password = "msf";

	[Export] private RichTextLabel _errorLabel;
	
	public override void _Ready()
	{
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UsernameChanged(string newUsername)
	{
		_username = newUsername;
	}

	public void PasswordChanged(string newPassword)
	{
		_password = newPassword;
	}


	public void OnStartButtonPressed()
	{
		if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
		{
			_errorLabel.Text = "[color=red]Username and password are required![/color]";
			_errorLabel.Visible = true;
			return;
		}

		string escapedUsername = _username.StripEscapes();
		string escapedPassword = _password.StripEscapes();

		try
		{
			MetasploitConnector.Connect(escapedUsername, escapedPassword);

			GetTree().ChangeSceneToFile("res://Main.tscn");
		}
		catch (Exception e)
		{
			GD.PrintErr(e.Message);
			_errorLabel.Text = "[color=red]Unable to connect to Metasploit. Did you start it?[/color]";
			_errorLabel.Visible = true;
		}
	}
}
