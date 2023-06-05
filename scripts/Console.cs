using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace metasploitgui.scripts;

public partial class Console : Control
{
    [Export] private LineEdit _consolePrompt;
    [Export] private LineEdit _consoleInput;
    [Export] private RichTextLabel _consoleOutput;

    private string _consoleId;

    private bool _busy;
    private bool _initialized;
    
    public override void _Ready()
    {
        // Name = "Console (Loading...)";
        _busy = false;
        _initialized = false;
        
        _consoleInput.TextSubmitted += Execute;
        _consoleInput.TextChanged += Update;
        
        InitAsync();
    }

    private async void InitAsync()
    {

        Dictionary<string, object> consoleCreationResult = await MetasploitAPI.Console.Create();

        if (consoleCreationResult.TryGetValue("id", out object idObj) && idObj is string id)
        {
            _consoleId = id;
            Name = $"Console {_consoleId}";
            GD.Print(_consoleId);
        }

        if (consoleCreationResult.TryGetValue("prompt", out object promptObj) && promptObj is string prompt)
        {
            _consolePrompt.Text = prompt;
        }

        if (consoleCreationResult.TryGetValue("busy", out object busyObj) && busyObj is bool busy)
        {
            _busy = busy;
        }

        _initialized = true;
    }

    private async Task GetConsoleData()
    {
        if (!_initialized) return;
        
        Dictionary<string, object> consoleReadResult = await MetasploitAPI.Console.Read(_consoleId.ToString());

        if (consoleReadResult.TryGetValue("result", out object resObj) && resObj is string err)
        {
            GD.PrintErr(err);
        }
        
        if (consoleReadResult.TryGetValue("data", out object dataObj) && dataObj is string data)
        {
            // GD.Print(data);
            if (!string.IsNullOrEmpty(data))
            {
                _consoleOutput.AppendText($"[code]{data}[/code]");
            }
        }

        if (consoleReadResult.TryGetValue("prompt", out object promptObj) && promptObj is string prompt)
        {
            _consolePrompt.Text = prompt;
        }

        if (consoleReadResult.TryGetValue("busy", out object busyObj) && busyObj is bool busy)
        {
            _busy = busy;
        }
    }

    private async void Execute(string s)
    {
        if (!s.EndsWith("\r\n"))
        {
            s += "\r\n";
        }
        
        GD.Print(s);

        _consoleOutput.AppendText($"[color=green]{_consolePrompt.Text}[/color]\t{s}");
        
        _consoleInput.Clear();

        Dictionary<string, object> writeResult = await MetasploitAPI.Console.Write(_consoleId.ToString(), s);

        foreach (string key in writeResult.Keys)
        {
            GD.Print(key);
            GD.Print(writeResult[key].GetType());
        }
        
        if (writeResult.TryGetValue("result", out object resObj) && resObj is string err)
        {
            GD.PrintErr(err);
        }
        
        if (writeResult.TryGetValue("wrote", out object idObj))
        {
            if (idObj is byte idByte)
                GD.Print("Wrote " + idByte + " bytes");
            else if (idObj is int idInt)
                GD.Print("Wrote " + idInt + " bytes");
        }

        GetConsoleData();
    }

    private void TabComplete()
    {
        
    }

    private void Update(string s)
    {
        
    }
    

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        GetConsoleData();
    }
}