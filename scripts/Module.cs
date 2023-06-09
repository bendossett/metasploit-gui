using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
namespace metasploitgui.scripts;


public class Module
{
    public string Type { get; private set; }
    public string Name { get; private set; }
    public string FullName { get; private set; }
    public string Rank { get; private set; }
    public string DisclosureDate { get; private set; }
    public string Description { get; private set; }
    public string License { get; private set; }
    public string FilePath { get; private set; }
    public List<string> Arch { get; private set; }
    public List<string> Platform { get; private set; }
    public List<string> Authors { get; private set; }
    public bool Privileged { get; private set; }
    public bool Check { get; private set; }
    public List<string> References { get; private set; }
    public Dictionary<byte, string> Targets { get; private set; }
    public byte DefaultTarget { get; private set; }
    public string Stance { get; private set; }
    public Dictionary<string, IOption> Options { get; private set; }

    public abstract class IOption
    {
        public string Name { get; internal set; }
        public string Type { get; internal set;  }
        public bool Required { get; internal set;  }
        public bool Advanced { get; internal set;  }
        public bool Evasion { get; internal set;  }
        public string Desc { get; internal set;  }
    }
    public class Option<T> : IOption
    {
        public bool HasDefault { get; internal set; }
        public T DefaultValue { get; private set; }
        public T Value { get; set; }
        public bool ValueSet { get; set; }
        public T[] Enums { get; private set; }

        public Option(string name, string type, bool required, bool advanced, bool evasion, string desc, T[] enums = default)
        {
            Name = name;
            Type = type;
            Required = required;
            Advanced = advanced;
            Evasion = evasion;
            Desc = desc;
            HasDefault = false;
            DefaultValue = default;
            ValueSet = false;
            Value = default;
            Enums = enums;
        }
        public Option(string name, string type, bool required, bool advanced, bool evasion, string desc, T defaultValue, T value, T[] enums = default)
        {
            Name = name;
            Type = type;
            Required = required;
            Advanced = advanced;
            Evasion = evasion;
            Desc = desc;
            HasDefault = true;
            DefaultValue = defaultValue;
            Value = value;
            ValueSet = true;
            Enums = enums;
        }
    }
    
    public void Parse(Dictionary<string, object> moduleInfo, Dictionary<string, object> moduleOptions)
    {
        if (moduleInfo.TryGetValue("type", out var type))
            Type = type as string;

        if (moduleInfo.TryGetValue("name", out var name))
            Name = name as string;

        if (moduleInfo.TryGetValue("fullname", out var fullName))
            FullName = fullName as string;

        if (moduleInfo.TryGetValue("rank", out var rank))
            Rank = rank as string;

        if (moduleInfo.TryGetValue("disclosuredate", out var disclosureDate))
            DisclosureDate = disclosureDate as string;

        if (moduleInfo.TryGetValue("description", out var description))
            Description = description as string;

        if (moduleInfo.TryGetValue("license", out var license))
            License = license as string;

        if (moduleInfo.TryGetValue("filepath", out var filepath))
            FilePath = filepath as string;

        if (moduleInfo.TryGetValue("arch", out var arch))
            Arch = Array.ConvertAll((object[])arch, input => ((byte[])input).GetStringFromAscii()).ToList();

        if (moduleInfo.TryGetValue("platform", out var platform))
            Platform = Array.ConvertAll((object[])platform, input => input as string).ToList();

        if (moduleInfo.TryGetValue("authors", out var authors))
            Authors = Array.ConvertAll((object[])authors, input => ((byte[])input).GetStringFromAscii()).ToList();

        if (moduleInfo.TryGetValue("privileged", out var privileged))
            Privileged = (bool)privileged;

        if (moduleInfo.TryGetValue("check", out var check))
            Check = (bool)check;

        if (moduleInfo.TryGetValue("references", out var references))
            References = null; //TODO

        if (moduleInfo.TryGetValue("targets", out var targets))
        {
            Dictionary<object, object> targetsDict = targets as Dictionary<object, object>;

            Targets = new Dictionary<byte, string>();

            foreach (KeyValuePair<object,object> kvp in targetsDict)
            {
                Targets.Add((byte)kvp.Key, ((byte[])kvp.Value).GetStringFromAscii());
            }
        }
        
        if (moduleInfo.TryGetValue("default_target", out var defaultTarget))
            DefaultTarget = (byte)defaultTarget;
        
        if (moduleInfo.TryGetValue("stance", out var stance))
            Stance = stance as string;

        if (moduleOptions == null) return;
        
        Options = new Dictionary<string, IOption>();

        foreach (KeyValuePair<string, object> kvp in moduleOptions)
        {
            string key = kvp.Key;

            Dictionary<object, object> subOptionsObj = kvp.Value as Dictionary<object, object>;

            Dictionary<string, object> subOptions =
                subOptionsObj.ToDictionary(o => ((byte[])o.Key).GetStringFromAscii(), o => o.Value);

            IOption option;

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
                        option = new Option<bool>(key, typeString, required, advanced, evasion, desc, (bool)defaultValue,
                            (bool)defaultValue);
                    }
                    else
                    {
                        option = new Option<bool>(key, typeString, required, advanced, evasion, desc);
                    }
                    break;
                }
                case "string" or "path" or "rhosts" or "address":
                {
                    if (hasDefault)
                    {
                        option = defaultValue switch
                        {
                            string defaultString => new Option<string>(key, typeString, required, advanced, evasion,
                                desc, defaultString, defaultString),
                            byte[] defaultBytes => new Option<string>(key, typeString, required, advanced, evasion,
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
                        option = new Option<string>(key, typeString, required, advanced, evasion, desc);
                    }
                    break;
                }
                case "integer" or "port":
                {
                    if (hasDefault)
                    {
                        option = new Option<int>(key, typeString, required, advanced, evasion, desc, Convert.ToInt32(defaultValue),
                            Convert.ToInt32(defaultValue));
                    }
                    else
                    {
                        option = new Option<int>(key, typeString, required, advanced, evasion, desc);
                    }
                    break;
                }
                case "float":
                {
                    if (hasDefault)
                    {
                        option = new Option<float>(key, typeString, required, advanced, evasion, desc, (float)defaultValue,
                            (float)defaultValue);
                    }
                    else
                    {
                        option = new Option<float>(key, typeString, required, advanced, evasion, desc);
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
                            option = new Option<string>(key, typeString, required, advanced, evasion, desc,
                                defaultValue as string, defaultValue as string, enumOptions);
                        }
                        else
                        {
                            option = new Option<string>(key, typeString, required, advanced, evasion, desc,
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
            
            Options.Add(key, option);
        }
    }
}