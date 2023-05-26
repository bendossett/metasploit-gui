using Godot;
using System;
using System.Collections.Generic;

public partial class MetasploitAPI : Node
{
	public class Core
	{
		public static Dictionary<string, object> GetModuleStats()
		{
			return MetasploitConnector.RPCCall("core.module_stats");
		}

		public static Dictionary<string, object> GetVersionInfo()
		{
			return MetasploitConnector.RPCCall("core.version");
		}

		public static Dictionary<string, object> AddModulePath(string modulePath)
		{
			return MetasploitConnector.RPCCall("core.add_module_path", modulePath);
		}
		
		public static Dictionary<string, object> ReloadModules()
		{
			return MetasploitConnector.RPCCall("core.reload_modules");
		}
		
		public static Dictionary<string, object> Save()
		{
		    return MetasploitConnector.RPCCall("core.save");
		}
		
		public static Dictionary<string, object> SetGlobalVariable(string optionName, string optionValue)
		{
		    return MetasploitConnector.RPCCall("core.setg", optionName, optionValue);
		}
		
		public static Dictionary<string, object> UnsetGlobalVariable(string optionName)
		{
		    return MetasploitConnector.RPCCall("core.unsetg", optionName);
		}
		
		public static Dictionary<string, object> GetThreadList()
		{
		    return MetasploitConnector.RPCCall("core.thread_list");
		}
		
		public static Dictionary<string, object> KillThread(string threadID)
		{
		    return MetasploitConnector.RPCCall("core.thread_kill", threadID);
		}
		
		public static Dictionary<string, object> Stop()
		{
		    return MetasploitConnector.RPCCall("core.stop");
		}
	}

	public class Console
	{
		public static Dictionary<string, object> Create()
		{
		    return MetasploitConnector.RPCCall("console.create");
		}
		
		public static Dictionary<string, object> Destroy(string consoleID)
		{
		    return MetasploitConnector.RPCCall("console.destroy", consoleID);
		}
		
		public static Dictionary<string, object> List()
		{
		    return MetasploitConnector.RPCCall("console.list");
		}
		
		public static Dictionary<string, object> Write(string consoleID, string data)
		{
		    return MetasploitConnector.RPCCall("console.write", consoleID, data);
		}
		
		public static Dictionary<string, object> Read(string consoleID)
		{
		    return MetasploitConnector.RPCCall("console.read", consoleID);
		}
		
		public static Dictionary<string, object> DetachSession(string consoleID)
		{
		    return MetasploitConnector.RPCCall("console.session_detach", consoleID);
		}
		
		public static Dictionary<string, object> KillSession(string consoleID)
		{
		    return MetasploitConnector.RPCCall("console.session_kill", consoleID);
		}
		
		public static Dictionary<string, object> Tabs(string consoleID, string input)
		{
		    return MetasploitConnector.RPCCall("console.tabs", consoleID, input);
		}
	}

	public class Job
	{
		public static Dictionary<string, object> List()
		{
		    return MetasploitConnector.RPCCall("job.list");
		}
		
		public static Dictionary<string, object> GetInfo(string jobID)
		{
		    return MetasploitConnector.RPCCall("job.info", jobID);
		}
		
		public static Dictionary<string, object> Stop(string jobID)
		{
		    return MetasploitConnector.RPCCall("job.stop", jobID);
		}
	}

	public class Module
	{
		public static Dictionary<string, object> GetExploitModules()
		{
		    return MetasploitConnector.RPCCall("module.exploits");
		}
		
		public static Dictionary<string, object> GetAuxiliaryModules()
		{
		    return MetasploitConnector.RPCCall("module.auxiliary");
		}
		
		public static Dictionary<string, object> GetPostModules()
		{
		    return MetasploitConnector.RPCCall("module.post");
		}
		
		public static Dictionary<string, object> GetPayloads()
		{
		    return MetasploitConnector.RPCCall("module.payloads");
		}
		
		public static Dictionary<string, object> GetEncoders()
		{
		    return MetasploitConnector.RPCCall("module.encoders");
		}
		
		public static Dictionary<string, object> GetNops()
		{
		    return MetasploitConnector.RPCCall("module.nops");
		}
		
		public static Dictionary<string, object> GetInfo(string moduleType, string moduleName)
		{
		    return MetasploitConnector.RPCCall("module.info", moduleType, moduleName);
		}
		
		public static Dictionary<string, object> GetOptions(string moduleType, string moduleName)
		{
		    return MetasploitConnector.RPCCall("module.options", moduleType, moduleName);
		}
		
		public static Dictionary<string, object> GetCompatiblePayloads(string moduleName)
		{
		    return MetasploitConnector.RPCCall("module.compatible_payloads", moduleName);
		}
		
		public static Dictionary<string, object> GetTargetCompatiblePayloads(string moduleName, int targetIndex)
		{
		    return MetasploitConnector.RPCCall("module.target_compatible_payloads", moduleName, targetIndex);
		}
		
		public static Dictionary<string, object> GetCompatibleSessions(string moduleName)
		{
		    return MetasploitConnector.RPCCall("module.compatible_sessions", moduleName);
		}
		
		public static Dictionary<string, object> Encode(string data, string encoderModule, Dictionary<string, object> options)
		{
		    return MetasploitConnector.RPCCall("module.encode", data, encoderModule, options);
		}
		
		public static Dictionary<string, object> Execute(string moduleType, string moduleName, Dictionary<string, object> options)
		{
		    return MetasploitConnector.RPCCall("module.execute", moduleType, moduleName, options);
		}
	}

	public class Plugin
	{
		public static Dictionary<string, object> Load(string pluginName, Dictionary<string, object> options)
		{
		    return MetasploitConnector.RPCCall("plugin.load", pluginName, options);
		}
		
		public static Dictionary<string, object> Unload(string pluginName)
		{
		    return MetasploitConnector.RPCCall("plugin.unload", pluginName);
		}
		
		public static Dictionary<string, object> ListLoaded()
		{
		    return MetasploitConnector.RPCCall("plugin.loaded");
		}
	}

	public class Session
	{
		public static Dictionary<string, object> List()
		{
		    return MetasploitConnector.RPCCall("session.list");
		}
		
		public static Dictionary<string, object> Stop(string sessionID)
		{
		    return MetasploitConnector.RPCCall("session.stop", sessionID);
		}
		
		public static Dictionary<string, object> ReadShell(string sessionID)
		{
		    return ReadShell(sessionID, null);
		}
		
		public static Dictionary<string, object> ReadShell(string sessionID, int? readPointer)
		{
			if (readPointer.HasValue)
			{
				return MetasploitConnector.RPCCall("session.shell_read", sessionID, readPointer.Value);
			}
			else
			{
				return MetasploitConnector.RPCCall("session.shell_read", sessionID);
			}
		}
		
		public static Dictionary<string, object> WriteToShell(string sessionID, string data)
		{
		    return MetasploitConnector.RPCCall("session.shell_write", sessionID, data);
		}
		
		public static Dictionary<string, object> WriteToMeterpreter(string sessionID, string data)
		{
		    return MetasploitConnector.RPCCall("session.meterpreter_write", sessionID, data);
		}
		
		public static Dictionary<string, object> ReadMeterpreter(string sessionID)
		{
		    return MetasploitConnector.RPCCall("session.meterpreter_read", sessionID);
		}
		
		public static Dictionary<string, object> RunMeterpreterSingleCommand(string sessionID, string command)
		{
		    return MetasploitConnector.RPCCall("session.meterpreter_run_single", sessionID, command);
		}
		
		public static Dictionary<string, object> RunMeterpreterScript(string sessionID, string scriptName)
		{
		    return MetasploitConnector.RPCCall("session.meterpreter_script", sessionID, scriptName);
		}
		
		public static Dictionary<string, object> DetachMeterpreter(string sessionID)
		{
		    return MetasploitConnector.RPCCall("session.meterpreter_session_detach", sessionID);
		}
		
		public static Dictionary<string, object> KillMeterpreter(string sessionID)
		{
		    return MetasploitConnector.RPCCall("session.meterpreter_session_kill", sessionID);
		}
		
		public static Dictionary<string, object> TabMeterpreter(string sessionID, string input)
		{
		    return MetasploitConnector.RPCCall("session.meterpreter_tabs", sessionID, input);
		}
		
		public static Dictionary<string, object> CompatibleModules(string sessionID)
		{
		    return MetasploitConnector.RPCCall("session.compatible_modules", sessionID);
		}
		
		public static Dictionary<string, object> UpgradeShellToMeterpreter(string sessionID, string host, string port)
		{
		    return MetasploitConnector.RPCCall("session.shell_upgrade", sessionID, host, port);
		}
		
		public static Dictionary<string, object> ClearRing(string sessionID)
		{
		    return MetasploitConnector.RPCCall("session.ring_clear", sessionID);
		}
		
		public static Dictionary<string, object> LastRing(string sessionID)
		{
		    return MetasploitConnector.RPCCall("session.ring_last", sessionID);
		}
		
		public static Dictionary<string, object> WriteToRing(string sessionID, string data)
		{
		    return MetasploitConnector.RPCCall("session.ring_put", sessionID, data);
		}
		
		public static Dictionary<string, object> ReadRing(string sessionID, int? readPointer)
		{
			if (readPointer.HasValue)
			{
				return MetasploitConnector.RPCCall("session.ring_read", sessionID, readPointer.Value);
			}
			else
			{
				return MetasploitConnector.RPCCall("session.ring_read", sessionID);
			}
		}
	}
}
