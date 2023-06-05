using Godot;
using System;
using System.Collections.Generic;


public partial class MenuBar : Godot.MenuBar
{
	[Export] private PopupMenu _tabs;
	[Export] private Node _dockableContainer;

	private PackedScene _consoleScene;
	private static Dictionary<long, Action> _functionLookup;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_consoleScene = ResourceLoader.Load<PackedScene>("res://Console.tscn");
		_functionLookup = new Dictionary<long, Action>();
		
		_tabs.AddItem("Create Console", 1);
		_functionLookup.Add(1, CreateConsole);

		_tabs.IdPressed += id =>
		{
			if (_functionLookup.TryGetValue(id, out Action cb))
			{
				cb.Invoke();
			}
		};
	}

	private void CreateConsole()
	{
		Node newConsole = _consoleScene.Instantiate();
		_dockableContainer.AddChild(newConsole);
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
