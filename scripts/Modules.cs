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

	private string[] _exploitStrings;
	private string[] _auxiliaryStrings;
	private string[] _postStrings;
	private string[] _payloadStrings;

	private TreeItem _exploitTreeRoot;
	private TreeItem _auxiliaryTreeRoot;
	private TreeItem _postTreeRoot;
	private TreeItem _payloadTreeRoot;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_treeRoot = _tree.CreateItem();
		
		InitTree();
		
		_tree.ItemActivated += ItemActivated;
	}

	private async void InitTree()
	{
		Dictionary<string, object> exploits = await MetasploitAPI.Module.GetExploitModules();
		_exploitStrings = Array.ConvertAll((object[])exploits["modules"], o => o.ToString());
		_exploitTreeRoot = InitTreeSection(_exploitStrings, "Exploits", "exploit", true);
		
		Dictionary<string, object> auxiliary = await MetasploitAPI.Module.GetAuxiliaryModules();
		_auxiliaryStrings = Array.ConvertAll((object[])auxiliary["modules"], o => o.ToString());
		_auxiliaryTreeRoot = InitTreeSection(_auxiliaryStrings, "Auxiliary", "auxiliary", true);
		
		Dictionary<string, object> post = await MetasploitAPI.Module.GetPostModules();
		_postStrings = Array.ConvertAll((object[])post["modules"], o => o.ToString());
		_postTreeRoot = InitTreeSection(_postStrings, "Post", "post", true);
		
		Dictionary<string, object> payloads = await MetasploitAPI.Module.GetPayloads();
		_payloadStrings = Array.ConvertAll((object[])payloads["modules"], o => o.ToString());
		_payloadTreeRoot = InitTreeSection(_payloadStrings, "Payloads", "payload", true);
	}

	private void ClearTree()
	{
		foreach (TreeItem item in _treeRoot.GetChildren())
		{
			if (item == _exploitTreeRoot || item == _auxiliaryTreeRoot || item == _postTreeRoot ||
			    item == _payloadTreeRoot)
			{
				_treeRoot.RemoveChild(item);
			}
			else
			{
				item.Free();
			}
		}
	}
	
	private void ResetTree()
	{
		ClearTree();
		
		// TODO
		// This sucks to have to do, but is required right now since the ability to add a node back to the tree
		//	just got added like 4 days before I wrote this, so it's not in the engine yet.
		InitTreeSection(_exploitStrings, "Exploits", "exploit", true);
		InitTreeSection(_auxiliaryStrings, "Auxiliary", "auxiliary", true);
		InitTreeSection(_postStrings, "Post", "post", true);
		InitTreeSection(_payloadStrings, "Payloads", "payload", true);
		
		// Once that feature makes it into the engine, we can do this:
		// _treeRoot.AddItem(_exploitTreeRoot);
		// _treeRoot.AddItem(_auxiliaryTreeRoot);
		// _treeRoot.AddItem(_postTreeRoot);
		// _treeRoot.AddItem(_payloadTreeRoot);
	}

	private void Search(string searchTerm)
	{
		if (String.IsNullOrEmpty(searchTerm))
		{
			ResetTree();
			return;
		}
		
		ClearTree();
		
		string[] filteredExploitStrings = _exploitStrings.Where(s => s.Contains(searchTerm)).ToArray();
		string[] filteredAuxiliaryStrings = _auxiliaryStrings.Where(s => s.Contains(searchTerm)).ToArray();
		string[] filteredPostStrings = _postStrings.Where(s => s.Contains(searchTerm)).ToArray();
		string[] filteredPayloadStrings = _payloadStrings.Where(s => s.Contains(searchTerm)).ToArray();

		InitTreeSection(filteredExploitStrings, "Exploits", "exploit", false);
		InitTreeSection(filteredAuxiliaryStrings, "Auxiliary", "auxiliary", false);
		InitTreeSection(filteredPostStrings, "Post", "post", false);
		InitTreeSection(filteredPayloadStrings, "Payloads", "payload", false);
	}
		
	private TreeItem InitTreeSection(string[] modulesStrings, string label, string pathRoot, bool itemsCollapsed)
	{
		TreeItem modulesItem = _tree.CreateItem(_treeRoot);
		modulesItem.SetText(0, label);
		modulesItem.SetCollapsedRecursive(itemsCollapsed);
		
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
		modulesItem.SetCollapsedRecursive(itemsCollapsed);
		return modulesItem;
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
