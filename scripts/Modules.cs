using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using metasploitgui.scripts;

public partial class Modules : Node
{
	[Export] private CurrentModule _currentModulePanel;
	[Export] private Tree _tree;
	private TreeItem _treeRoot;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InitAsync();
	}

	private async void InitAsync()
	{
		_treeRoot = _tree.CreateItem();
		
		Dictionary<string, object> exploits = await MetasploitAPI.Module.GetExploitModules();
		InitTreeSection(exploits, "Exploits", "exploit");
		
		Dictionary<string, object> auxiliary = await MetasploitAPI.Module.GetAuxiliaryModules();
		InitTreeSection(auxiliary, "Auxiliary", "auxiliary");
		
		Dictionary<string, object> post = await MetasploitAPI.Module.GetPostModules();
		InitTreeSection(post, "Post", "post");
		
		Dictionary<string, object> payloads = await MetasploitAPI.Module.GetPayloads();
		InitTreeSection(payloads, "Payloads", "payload");

		_tree.ItemActivated += ItemActivated;
	}

	private void InitTreeSection(Dictionary<string, object> modules, string label, string pathRoot)
	{
		string[] modulesStrings = Array.ConvertAll((object[])modules["modules"], o => o.ToString());
		TreeItem modulesItem = _tree.CreateItem(_treeRoot);
		modulesItem.SetText(0, label);
		modulesItem.SetCollapsedRecursive(true);
		
		StringTree stringTree = new StringTree();
		
		foreach (string p in modulesStrings)
		{
			stringTree.AddPath(p);
		}
		
		Queue<StringTree.StringTreeNode> stringTreeQ = new Queue<StringTree.StringTreeNode>();
		stringTreeQ.Enqueue(stringTree.Root);
		
		Queue<TreeItem> treeItemQ = new Queue<TreeItem>();
		treeItemQ.Enqueue(modulesItem);

		while (stringTreeQ.Count > 0)
		{
			StringTree.StringTreeNode stringTreeRef = stringTreeQ.Dequeue();
			TreeItem treeItemRef = treeItemQ.Dequeue();
			
			foreach (StringTree.StringTreeNode child in stringTreeRef.Children)
			{
				TreeItem newItem = _tree.CreateItem(treeItemRef);

				if (child.Value != null && child.FullPath != null)
				{
					newItem.SetText(0, child.Value);
					newItem.SetMeta("full_path", pathRoot + "/" + child.FullPath);
					newItem.SetMeta("module_type", pathRoot);
					newItem.SetMeta("name", child.FullPath);
				}
				else if (child.Value != null)
				{
					newItem.SetText(0, child.Value);
				}
			
				stringTreeQ.Enqueue(child);
				treeItemQ.Enqueue(newItem);
			}
		}
	}

	private void ItemActivated()
	{
		TreeItem selected = _tree.GetSelected();

		if (selected.HasMeta("full_path"))
		{
			GD.Print(selected.GetMeta("full_path"));
			_currentModulePanel.LoadModule(selected.GetMeta("module_type").AsString(), selected.GetMeta("name").AsString());
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
