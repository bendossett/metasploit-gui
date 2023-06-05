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
    [Export] private TextEdit _moduleDesc;

    [Export] private VBoxContainer _options;
    [Export] private VBoxContainer _advancedOptions;

    [Export] private Button _exploitButton;

    [Export] private OptionButton _payloadOptionButton;
    private string[] _payloads;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _moduleName.AutowrapMode = TextServer.AutowrapMode.WordSmart;
    }

    public async void LoadModule(string moduleType, string modulePath)
    {
        foreach (Node node in _options.GetChildren())
        {
            _options.RemoveChild(node);
        }

        foreach (Node node in _advancedOptions.GetChildren())
        {
            _advancedOptions.RemoveChild(node);
        }

        Dictionary<string, object> moduleInfo = await MetasploitAPI.Module.GetInfo(moduleType, modulePath);

        Dictionary<string, object> moduleOptions = await MetasploitAPI.Module.GetOptions(moduleType, modulePath);

        _currentModule = new Module();
        _currentModule.Parse(moduleInfo, moduleOptions);

        _moduleName.Text = _currentModule.Name;
        _moduleDesc.Text = _currentModule.Description;

        PopulateOptions(_currentModule.Options);

        Dictionary<string, object> compatiblePayloads =
            await MetasploitAPI.Module.GetCompatiblePayloads(_currentModule.FullName);

        PopulatePayloads(compatiblePayloads);
        RefreshExploitButton();
    }

    private void PopulateOptions(Dictionary<string, Module.IOption> optionDict)
    {
        foreach (var option in optionDict)
        {
            Label optionName = new Label();
            optionName.Text = option.Value.Name;

            HBoxContainer optionBox = new HBoxContainer();
            optionBox.AddChild(optionName);

            switch (option.Value)
            {
                case Module.Option<bool> o:
                {
                    CheckButton checkButton = new CheckButton();

                    if (o.HasDefault)
                        checkButton.SetPressedNoSignal(o.DefaultValue);

                    checkButton.Toggled += value =>
                    {
                        o.ValueSet = true;
                        o.Value = value;
                        RefreshExploitButton();
                    };

                    optionBox.AddChild(checkButton);
                    break;
                }
                case Module.Option<string> o:
                {
                    if (o.Type == "enum")
                    {
                        OptionButton enumOptions = new OptionButton();

                        foreach (string s in o.Enums)
                        {
                            enumOptions.AddItem(s);
                        }

                        if (o.HasDefault)
                            enumOptions.Select(Array.IndexOf(o.Enums, o.DefaultValue));

                        enumOptions.ItemSelected += value =>
                        {
                            o.ValueSet = true;
                            o.Value = o.Enums[value];
                            RefreshExploitButton();
                        };

                        optionBox.AddChild(enumOptions);
                    }
                    else
                    {
                        LineEdit lineEdit = new LineEdit();
                        lineEdit.SizeFlagsHorizontal = SizeFlags.ExpandFill;

                        if (o.HasDefault)
                        {
                            lineEdit.Text = o.DefaultValue;
                            GD.Print(o.DefaultValue);
                        }

                        lineEdit.TextChanged += text =>
                        {
                            string newText = text.StripEdges().StripEscapes();

                            if (string.IsNullOrEmpty(newText))
                            {
                                o.ValueSet = false;
                            }
                            else
                            {
                                o.ValueSet = true;
                                o.Value = newText;
                            }

                            RefreshExploitButton();
                        };

                        optionBox.AddChild(lineEdit);
                    }

                    break;
                }
                case Module.Option<int> o:
                {
                    LineEdit lineEdit = new LineEdit();
                    lineEdit.SizeFlagsHorizontal = SizeFlags.ExpandFill;

                    if (o.HasDefault)
                        lineEdit.Text = o.DefaultValue.ToString();

                    lineEdit.TextChanged += text =>
                    {
                        string newText = text.StripEdges().StripEscapes();

                        if (string.IsNullOrEmpty(newText))
                        {
                            o.ValueSet = false;
                        }
                        else
                        {
                            o.ValueSet = true;
                            o.Value = int.Parse(newText);
                        }

                        RefreshExploitButton();
                    };

                    optionBox.AddChild(lineEdit);
                    break;
                }
                case Module.Option<float> o:
                {
                    LineEdit lineEdit = new LineEdit();
                    lineEdit.SizeFlagsHorizontal = SizeFlags.ExpandFill;

                    if (o.HasDefault)
                        lineEdit.Text = o.DefaultValue.ToString(CultureInfo.InvariantCulture);

                    lineEdit.TextChanged += text =>
                    {
                        string newText = text.StripEdges().StripEscapes();

                        if (string.IsNullOrEmpty(newText))
                        {
                            o.ValueSet = false;
                        }
                        else
                        {
                            o.ValueSet = true;
                            o.Value = float.Parse(newText);
                        }

                        RefreshExploitButton();
                    };

                    optionBox.AddChild(lineEdit);
                    break;
                }
                default:
                    GD.PrintErr("Was not a known Option Type");
                    break;
            }

            if (option.Value.Advanced || option.Key.StartsWith("HTTP"))
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

        _payloadOptionButton.ItemSelected -= PopulatePayloadOptions;
        _payloadOptionButton.ItemSelected += PopulatePayloadOptions;
    }

    private async void PopulatePayloadOptions(long index)
    {
        Dictionary<string, object> payloadOptions =
            await MetasploitAPI.Module.GetOptions("payload", _payloads[index]);

        Dictionary<string, Module.IOption> options = new Dictionary<string, Module.IOption>();

        foreach (KeyValuePair<string, object> kvp in payloadOptions)
        {
            string key = kvp.Key;

            Dictionary<object, object> subOptionsObj = kvp.Value as Dictionary<object, object>;

            Dictionary<string, object> subOptions =
                subOptionsObj.ToDictionary(o => ((byte[])o.Key).GetStringFromAscii(), o => o.Value);

            Module.IOption option;

            string typeString = null;
            bool required = false;
            bool advanced = false;
            bool evasion = false;
            string desc = null;
            bool hasDefault = false;
            object defaultValue = null;

            if (subOptions.TryGetValue("type", out var typeObj))
            {
                typeString = typeObj switch
                {
                    string typeStr => typeStr,
                    byte[] typeBytes => typeBytes.GetStringFromAscii(),
                    _ => typeString
                };
            }

            if (subOptions.TryGetValue("required", out var requiredObj) && requiredObj is bool requiredValue)
                required = requiredValue;

            if (subOptions.TryGetValue("advanced", out var advancedObj) && advancedObj is bool advancedValue)
                advanced = advancedValue;

            if (subOptions.TryGetValue("evasion", out var evasionObj) && evasionObj is bool evasionValue)
                evasion = evasionValue;

            if (subOptions.TryGetValue("desc", out var descObj))
            {
                desc = descObj switch
                {
                    string descStr => descStr,
                    byte[] descBytes => descBytes.GetStringFromAscii(),
                    _ => desc
                };
            }

            if (subOptions.TryGetValue("default", out var defaultObj))
            {
                defaultValue = defaultObj;
                hasDefault = true;
            }
            else
            {
                hasDefault = false;
            }

            switch (typeString)
            {
                case "bool":
                {
                    if (hasDefault)
                    {
                        option = new Module.Option<bool>(key, typeString, required, advanced, evasion, desc,
                            (bool)defaultValue,
                            (bool)defaultValue);
                    }
                    else
                    {
                        option = new Module.Option<bool>(key, typeString, required, advanced, evasion, desc);
                    }

                    break;
                }
                case "string" or "path" or "rhosts" or "meterpreterdebuglogging" or "address":
                {
                    if (hasDefault)
                    {
                        option = defaultValue switch
                        {
                            string defaultString => new Module.Option<string>(key, typeString, required, advanced,
                                evasion,
                                desc, defaultString, defaultString),
                            byte[] defaultBytes => new Module.Option<string>(key, typeString, required, advanced,
                                evasion,
                                desc, defaultBytes.GetStringFromAscii(), defaultBytes.GetStringFromAscii()),
                            _ => null
                        };
                        if (option == null)
                        {
                            GD.PrintErr(defaultValue.GetType());
                        }
                    }
                    else
                    {
                        option = new Module.Option<string>(key, typeString, required, advanced, evasion, desc);
                    }

                    break;
                }
                case "integer" or "port":
                {
                    if (hasDefault)
                    {
                        option = new Module.Option<int>(key, typeString, required, advanced, evasion, desc,
                            Convert.ToInt32(defaultValue),
                            Convert.ToInt32(defaultValue));
                    }
                    else
                    {
                        option = new Module.Option<int>(key, typeString, required, advanced, evasion, desc);
                    }

                    break;
                }
                case "float":
                {
                    if (hasDefault)
                    {
                        option = new Module.Option<float>(key, typeString, required, advanced, evasion, desc,
                            (float)defaultValue,
                            (float)defaultValue);
                    }
                    else
                    {
                        option = new Module.Option<float>(key, typeString, required, advanced, evasion, desc);
                    }

                    break;
                }
                case "enum":
                    if (subOptions.TryGetValue("enums", out object enumsObj))
                    {
                        string[] enumOptions = Array.ConvertAll((object[])enumsObj, o => o switch
                            {
                                string s => s,
                                byte[] b => b.GetStringFromAscii(),
                                _ => o.ToString()
                            }
                        );

                        if (hasDefault)
                        {
                            option = new Module.Option<string>(key, typeString, required, advanced, evasion, desc,
                                defaultValue as string, defaultValue as string, enumOptions);
                        }
                        else
                        {
                            option = new Module.Option<string>(key, typeString, required, advanced, evasion, desc,
                                enumOptions[0], enumOptions[0], enumOptions);
                        }
                    }
                    else
                    {
                        GD.PrintErr("Typestring was \"enum\" but no enum options were provided.");
                        option = null;
                    }

                    break;
                default:
                    option = null;
                    GD.PrintErr("Unknown typeString " + ((byte[])subOptions["type"]).GetStringFromAscii());
                    break;
            }

            options.Add(key, option);
        }

        PopulateOptions(options);
    }

    private void RefreshExploitButton()
    {
        List<string> unsetOptions = new List<string>();

        bool shouldDisable = false;

        foreach (Module.IOption moduleOption in _currentModule.Options.Values)
        {
            bool required = moduleOption switch
            {
                Module.Option<bool> o => o.Required,
                Module.Option<string> o => o.Required,
                Module.Option<int> o => o.Required,
                Module.Option<float> o => o.Required,
                _ => false
            };

            bool valueSet = moduleOption switch
            {
                Module.Option<bool> o => o.ValueSet,
                Module.Option<string> o => o.ValueSet,
                Module.Option<int> o => o.ValueSet,
                Module.Option<float> o => o.ValueSet,
                _ => false
            };

            if (required && !valueSet)
            {
                GD.Print("Required");
                shouldDisable = true;
                unsetOptions.Add(moduleOption.Name);
            }
        }

        string tooltip = "Need to set:\n";

        foreach (string s in unsetOptions)
        {
            tooltip += s + "\n";
        }

        _exploitButton.TooltipText = tooltip;
        _exploitButton.Disabled = shouldDisable;
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

        Dictionary<string, object> executionResponse =
            await MetasploitAPI.Module.Execute(_currentModule.Type, _currentModule.FullName, executionOptions);

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