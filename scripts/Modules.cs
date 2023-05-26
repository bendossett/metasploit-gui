using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Modules : Node
{
	[Export] private Tree _tree;
	private TreeItem _treeRoot;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_treeRoot = _tree.CreateItem();
		_tree.HideRoot = true;
		TreeItem exploitsItem = _tree.CreateItem(_treeRoot);
		TreeItem auxiliaryItem = _tree.CreateItem(_treeRoot);
		TreeItem postItem = _tree.CreateItem(_treeRoot);
		TreeItem payloadsItem = _tree.CreateItem(_treeRoot);

		Dictionary<string, TreeItem> exploitsTree = new();

		Dictionary<string, object> exploits = MetasploitAPI.Module.GetExploitModules();

		object[] exploitStrings = (object[])exploits["modules"];
		for (int i = 0; i < exploitStrings.Length; i++)
		{
			string s = (string)exploitStrings[i];
			
			string[] parts = s.Split('/');
			for (int j = 0; j < parts.Length - 1; j++)
			{
				
			}
		}
		
		// Dictionary<string, object> auxiliary = MetasploitAPI.Module.GetAuxiliaryModules();
		// Dictionary<string, object> post = MetasploitAPI.Module.GetPostModules();
		// Dictionary<string, object> payloads = MetasploitAPI.Module.GetPayloads();
	}

	private void AddToTree(TreeItem parent, string[] path)
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
