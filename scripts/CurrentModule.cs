using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Godot;

namespace metasploitgui.scripts;

public partial class CurrentModule : Control
{
	private Module _currentModule;

	[Export] private Label _moduleName;
	[Export] private Label _moduleDesc;

	[Export] private VBoxContainer _options;
	[Export] private VBoxContainer _advancedOptions;

	[Export] private OptionButton _payloads;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_moduleName.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		_moduleDesc.AutowrapMode = TextServer.AutowrapMode.WordSmart;
	}

	public async void LoadModule(string moduleType, string modulePath)
	{
		Dictionary<string, object> moduleInfo = await MetasploitAPI.Module.GetInfo(moduleType, modulePath);

		Dictionary<string, object> moduleOptions = await MetasploitAPI.Module.GetOptions(moduleType, modulePath);

		_currentModule = new Module();
		_currentModule.Parse(moduleInfo, moduleOptions);

		_moduleName.Text = _currentModule.Name;
		_moduleDesc.Text = _currentModule.Description;
		
		PopulateOptions();

		Dictionary<string, object> compatiblePayloads = await MetasploitAPI.Module.GetCompatiblePayloads(_currentModule.FullName);
		
		PopulatePayloads(compatiblePayloads);
	}

	private void PopulateOptions()
	{
		foreach (KeyValuePair<string, Dictionary<string, object>> option in _currentModule.Options)
		{
			Label optionName = new Label();
			optionName.Text = option.Key;

			foreach (KeyValuePair<string,object> o in option.Value)
			{
				GD.Print(o.Key);
				GD.Print(o.Value);
				GD.Print("");
			}
			
			//type
			//required
			//advanced
			//evasion
			//desc
			//default

			HBoxContainer optionBox = new HBoxContainer();
			// optionBox.
			optionBox.AddChild(optionName);
			
			GD.Print(option.Value["required"]);
			GD.Print(((byte[])option.Value["desc"]).GetStringFromAscii());

			string typeString = ((byte[])option.Value["type"]).GetStringFromAscii();
			switch (typeString)
			{
				case "bool":
				{
					CheckButton checkButton = new CheckButton();
					checkButton.SetPressedNoSignal((bool)option.Value["default"]);
					checkButton.Toggled += value =>
					{
						option.Value["value"] = value;
					};
				
					optionBox.AddChild(checkButton);
					break;
				}
				case "string" or "path" or "rhosts" or "port":
				{
					LineEdit lineEdit = new LineEdit();

					if (option.Value.TryGetValue("default", out object defaultString))
					{
						lineEdit.Text = defaultString.ToString();
					}
					
					lineEdit.TextSubmitted += text =>
					{
						option.Value["value"] = text.StripEdges().StripEscapes();
					};
				
					optionBox.AddChild(lineEdit);
					break;
				}
				case "integer":
				{
					LineEdit lineEdit = new LineEdit();
					
					if (option.Value.TryGetValue("default", out object defaultInt))
					{
						GD.Print(defaultInt.GetType());
						lineEdit.Text = defaultInt is byte ? defaultInt.ToString() : ((int)defaultInt).ToString();
					}
					
					lineEdit.TextSubmitted += text =>
					{
						option.Value["value"] = int.Parse(text);
					};
				
					optionBox.AddChild(lineEdit);
					break;
				}
				case "float":
				{
					LineEdit lineEdit = new LineEdit();
					
					if (option.Value.TryGetValue("default", out object defaultFloat))
					{
						lineEdit.Text = ((float)defaultFloat).ToString(CultureInfo.CurrentCulture);
					}
					
					lineEdit.TextSubmitted += text =>
					{
						option.Value["value"] = float.Parse(text);
					};
				
					optionBox.AddChild(lineEdit);
					break;
				}
				case "enum":
					if (option.Value.TryGetValue("default", out object defaultValue))
					{
						GD.Print(defaultValue);
					}
					break;
				default:
					GD.PrintErr("Unknown typeString " + typeString);
					break;
			}
			
			if ((bool)option.Value["advanced"] || option.Key.StartsWith("HTTP"))
			{
				_advancedOptions.AddChild(optionBox);	
			}
			else
			{
				_options.AddChild(optionBox);
			}
		}
	}

	private void PopulatePayloads(Dictionary<string, object> compatiblePayloads)
	{
		if (compatiblePayloads.ContainsKey("error"))
		{
			GD.PrintErr("Error getting payloads");
			return;
		}
		
		object[] payloadObjects = (object[])compatiblePayloads["payloads"];
		
		string[] payloads = Array.ConvertAll(payloadObjects, x => x.ToString());
		
		foreach (string payload in payloads)
		{
			_payloads.AddItem(payload);
		}
	}

	public async void Execute()
	{

		Dictionary<string, object> executionOptions = new Dictionary<string, object>();

		foreach (KeyValuePair<string,Dictionary<string,object>> moduleOption in _currentModule.Options)
		{
			string key = moduleOption.Key;

			object value = moduleOption.Value["value"];

			if (value != null)
			{
				executionOptions.Add(key, value);
			}
		}

		Dictionary<string, object> executionResponse = await MetasploitAPI.Module.Execute(_currentModule.Type, _currentModule.FullName, executionOptions);

		foreach (string key in executionResponse.Keys)
		{
			GD.Print(key);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}