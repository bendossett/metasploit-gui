using System;
using System.Collections.Generic;
using System.Linq;

namespace metasploitgui.scripts;

public class StringTree
{
    public class StringTreeNode
    {
        private string _value;
        private string _fullPath;
        private StringTreeNode _parent;
        private Dictionary<string, StringTreeNode> _children;
        private int _depth;

        public string Value => _value;
        public string FullPath => _fullPath;
        public StringTreeNode Parent => _parent;
        public List<StringTreeNode> Children => _children.Values.ToList();
        public int Depth => _depth;
        

        public StringTreeNode(string value = null, StringTreeNode parent = null)
        {
            _value = value;
            _parent = parent;
            _children = new Dictionary<string, StringTreeNode>();

            if (parent != null)
            {
                _depth = parent._depth + 1;
            }
            else
            {
                _depth = 0;
            }
        }

        public StringTreeNode(string value = null, string fullPath = null, StringTreeNode parent = null)
        {
            _value = value;
            _fullPath = fullPath;
            _parent = parent;
            _children = new Dictionary<string, StringTreeNode>();
            
            if (parent != null)
            {
                _depth = parent._depth + 1;
            }
            else
            {
                _depth = 0;
            }
        }

        public StringTreeNode AddChild(string childValue)
        {
            StringTreeNode newChild = new StringTreeNode(childValue, this);
            _children.Add(childValue, newChild);
            return newChild;
        }

        public StringTreeNode AddChild(string childValue, string childFullPath)
        {
            StringTreeNode newChild = new StringTreeNode(childValue, childFullPath, this);
            _children.Add(childValue, newChild);
            return newChild;
        }
        
        public StringTreeNode AddChild(StringTreeNode child)
        {
            _children.Add(child._value, child);
            return child;
        }

        public bool TryGetChild(StringTreeNode child, out StringTreeNode found)
        {
            return _children.TryGetValue(child._value, out found);
        }

        public bool TryGetChild(string childValue, out StringTreeNode found)
        {
            return _children.TryGetValue(childValue, out found);
        }
    }

    private StringTreeNode _root;

    public StringTreeNode Root => _root;
    
    public StringTree()
    {
        _root = new StringTreeNode(null, null);
    }

    public void AddPath(string path)
    {
        string[] parts = path.Split('/');

        StringTreeNode nodeRef = _root;
        
        for (int i = 0; i < parts.Length; i++)
        {
            if (nodeRef.TryGetChild(parts[i], out StringTreeNode existingChild))
            {
                nodeRef = existingChild;
            }
            else
            {
                if (i == parts.Length - 1)
                {
                    nodeRef = nodeRef.AddChild(parts[i], path);
                }
                else
                {
                    nodeRef = nodeRef.AddChild(parts[i]);
                }
            }
        }
    }
}