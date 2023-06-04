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

	[Export] private OptionButton _payloadOptionButton;
	private string[] _payloads;
	
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
		foreach (var option in _currentModule.Options)
		{
			Label optionName = new Label();
			optionName.Text = option.Value.Name;

			HBoxContainer optionBox = new HBoxContainer();
			// optionBox.
			optionBox.AddChild(optionName);

			switch (option.Value)
			{
				case Module.Option<bool> o:
				{
					CheckButton checkButton = new CheckButton();
					
					if (o.HasDefault)
						checkButton.SetPressedNoSignal(o.DefaultValue);
					
					checkButton.Toggled += value => { o.Value = value; };

					optionBox.AddChild(checkButton);
					break;
				}
				case Module.Option<string> o:
				{
					LineEdit lineEdit = new LineEdit();

					if (o.HasDefault)
						lineEdit.Text = o.DefaultValue;

					lineEdit.TextSubmitted += text => { o.Value = text.StripEdges().StripEscapes(); };

					optionBox.AddChild(lineEdit);
					break;
				}
				case Module.Option<int> o:
				{
					LineEdit lineEdit = new LineEdit();

					if (o.HasDefault)
						lineEdit.Text = o.DefaultValue.ToString();

					lineEdit.TextSubmitted += text =>
						o.Value = int.Parse(text);
				
					optionBox.AddChild(lineEdit);
					break;
				}
				case Module.Option<float> o:
				{
					LineEdit lineEdit = new LineEdit();

					if (o.HasDefault)
						lineEdit.Text = o.DefaultValue.ToString(CultureInfo.InvariantCulture);
					
					lineEdit.TextSubmitted += text =>
					{
						o.Value = float.Parse(text);
					};
				
					optionBox.AddChild(lineEdit);
					break;
				}
				default:
					GD.PrintErr("Was not a known Option Type");
					break;
			}

			if ((bool)option.Value.Advanced || option.Key.StartsWith("HTTP"))
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
		
		_payloadOptionButton.Clear();
		
		object[] payloadObjects = (object[])compatiblePayloads["payloads"];
		
		_payloads = Array.ConvertAll(payloadObjects, x => x.ToString());
		
		foreach (string payload in _payloads)
		{
			_payloadOptionButton.AddItem(payload);
		}
	}

	public async void Execute()
	{

		Dictionary<string, object> executionOptions = new Dictionary<string, object>();

		foreach (Module.IOption moduleOption in _currentModule.Options.Values)
		{
			string key = moduleOption.Name;

			object value = moduleOption switch
			{
				Module.Option<bool> o => o.Value,
				Module.Option<string> o => o.Value,
				Module.Option<int> o => o.Value,
				Module.Option<float> o => o.Value,
				_ => null
			};

			if (value != null)
			{
				executionOptions.Add(key, value);
			}
		}
		
		executionOptions.Add("payload", _payloads[_payloadOptionButton.GetSelectedId()]);

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