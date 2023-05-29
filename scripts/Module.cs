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
    public Dictionary<string, Dictionary<string, object>> Options { get; private set; }

    public void Parse(Dictionary<string, object> moduleInfo, Dictionary<string, object> moduleOptions)
    {
        if (moduleInfo.TryGetValue("type", out object type))
            Type = type as string;

        if (moduleInfo.TryGetValue("name", out object name))
            Name = name as string;

        if (moduleInfo.TryGetValue("fullname", out object fullName))
            FullName = fullName as string;

        if (moduleInfo.TryGetValue("rank", out object rank))
            Rank = rank as string;

        if (moduleInfo.TryGetValue("disclosuredate", out object disclosureDate))
            DisclosureDate = disclosureDate as string;

        if (moduleInfo.TryGetValue("description", out object description))
            Description = description as string;

        if (moduleInfo.TryGetValue("license", out object license))
            License = license as string;

        if (moduleInfo.TryGetValue("filepath", out object filepath))
            FilePath = filepath as string;

        if (moduleInfo.TryGetValue("arch", out object arch))
            Arch = Array.ConvertAll((object[])arch, input => ((byte[])input).GetStringFromAscii()).ToList();

        if (moduleInfo.TryGetValue("platform", out object platform))
            Platform = Array.ConvertAll((object[])platform, input => input as string).ToList();

        if (moduleInfo.TryGetValue("authors", out object authors))
            Authors = Array.ConvertAll((object[])authors, input => ((byte[])input).GetStringFromAscii()).ToList();

        if (moduleInfo.TryGetValue("privileged", out object privileged))
            Privileged = (bool)privileged;

        if (moduleInfo.TryGetValue("check", out object check))
            Check = (bool)check;

        if (moduleInfo.TryGetValue("references", out object references))
            References = null; //TODO

        if (moduleInfo.TryGetValue("targets", out object targets))
        {
            Dictionary<object, object> targetsDict = targets as Dictionary<object, object>;

            Targets = new Dictionary<byte, string>();

            foreach (KeyValuePair<object,object> kvp in targetsDict)
            {
                Targets.Add((byte)kvp.Key, ((byte[])kvp.Value).GetStringFromAscii());
            }
        }
        
        if (moduleInfo.TryGetValue("default_target", out object defaultTarget))
            DefaultTarget = (byte)defaultTarget;
        
        if (moduleInfo.TryGetValue("stance", out object stance))
            Stance = stance as string;

        if (moduleOptions != null)
        {
            Options = new Dictionary<string, Dictionary<string, object>>();

            foreach (KeyValuePair<string, object> kvp in moduleOptions)
            {
                string key = kvp.Key;

                Dictionary<object, object> subOptions = kvp.Value as Dictionary<object, object>;

                Dictionary<string, object> value = new Dictionary<string, object>();

                foreach (KeyValuePair<object, object> subOption in subOptions)
                {
                    value.Add(((byte[])subOption.Key).GetStringFromAscii(), subOption.Value);
                }

                value.Add("value", value.TryGetValue("default", out object defaultValue) ? defaultValue : null);

                Options.Add(key, value);
            }
        }
    }
}