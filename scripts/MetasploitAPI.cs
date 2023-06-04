using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class MetasploitAPI : Node
{
	
	public static class Core
	{
		public static async Task<Dictionary<string, object>> GetModuleStats()
		{
			return await MetasploitConnector.RPCCall("core.module_stats");
		}

		public static async Task<Dictionary<string, object>> GetVersionInfo()
		{
			return await MetasploitConnector.RPCCall("core.version");
		}

		public static async Task<Dictionary<string, object>> AddModulePath(string modulePath)
		{
			return await MetasploitConnector.RPCCall("core.add_module_path", modulePath);
		}
		
		public static async Task<Dictionary<string, object>> ReloadModules()
		{
			return await MetasploitConnector.RPCCall("core.reload_modules");
		}
		
		public static async Task<Dictionary<string, object>> Save()
		{
		    return await MetasploitConnector.RPCCall("core.save");
		}
		
		public static async Task<Dictionary<string, object>> SetGlobalVariable(string optionName, string optionValue)
		{
		    return await MetasploitConnector.RPCCall("core.setg", optionName, optionValue);
		}
		
		public static async Task<Dictionary<string, object>> UnsetGlobalVariable(string optionName)
		{
		    return await MetasploitConnector.RPCCall("core.unsetg", optionName);
		}
		
		public static async Task<Dictionary<string, object>> GetThreadList()
		{
		    return await MetasploitConnector.RPCCall("core.thread_list");
		}
		
		public static async Task<Dictionary<string, object>> KillThread(string threadID)
		{
		    return await MetasploitConnector.RPCCall("core.thread_kill", threadID);
		}
		
		public static async Task<Dictionary<string, object>> Stop()
		{
		    return await MetasploitConnector.RPCCall("core.stop");
		}
	}

	public static class Console
	{
		public static async Task<Dictionary<string, object>> Create()
		{
		    return await MetasploitConnector.RPCCall("console.create");
		}
		
		public static async Task<Dictionary<string, object>> Destroy(string consoleID)
		{
		    return await MetasploitConnector.RPCCall("console.destroy", consoleID);
		}
		
		public static async Task<Dictionary<string, object>> List()
		{
		    return await MetasploitConnector.RPCCall("console.list");
		}
		
		public static async Task<Dictionary<string, object>> Write(string consoleID, string data)
		{
		    return await MetasploitConnector.RPCCall("console.write", consoleID, data);
		}
		
		public static async Task<Dictionary<string, object>> Read(string consoleID)
		{
		    return await MetasploitConnector.RPCCall("console.read", consoleID);
		}
		
		public static async Task<Dictionary<string, object>> DetachSession(string consoleID)
		{
		    return await MetasploitConnector.RPCCall("console.session_detach", consoleID);
		}
		
		public static async Task<Dictionary<string, object>> KillSession(string consoleID)
		{
		    return await MetasploitConnector.RPCCall("console.session_kill", consoleID);
		}
		
		public static async Task<Dictionary<string, object>> Tabs(string consoleID, string input)
		{
		    return await MetasploitConnector.RPCCall("console.tabs", consoleID, input);
		}
	}

	public static class Job
	{
		public static async Task<Dictionary<string, object>> List()
		{
		    return await MetasploitConnector.RPCCall("job.list");
		}
		
		public static async Task<Dictionary<string, object>> GetInfo(string jobID)
		{
		    return await MetasploitConnector.RPCCall("job.info", jobID);
		}
		
		public static async Task<Dictionary<string, object>> Stop(string jobID)
		{
		    return await MetasploitConnector.RPCCall("job.stop", jobID);
		}
	}

	public static class Module
	{
		public static async Task<Dictionary<string, object>> GetExploitModules()
		{
		    return await MetasploitConnector.RPCCall("module.exploits");
		}
		
		public static async Task<Dictionary<string, object>> GetAuxiliaryModules()
		{
		    return await MetasploitConnector.RPCCall("module.auxiliary");
		}
		
		public static async Task<Dictionary<string, object>> GetPostModules()
		{
		    return await MetasploitConnector.RPCCall("module.post");
		}
		
		public static async Task<Dictionary<string, object>> GetPayloads()
		{
		    return await MetasploitConnector.RPCCall("module.payloads");
		}
		
		public static async Task<Dictionary<string, object>> GetEncoders()
		{
		    return await MetasploitConnector.RPCCall("module.encoders");
		}
		
		public static async Task<Dictionary<string, object>> GetNops()
		{
		    return await MetasploitConnector.RPCCall("module.nops");
		}
		
		public static async Task<Dictionary<string, object>> GetInfo(string moduleType, string moduleName)
		{
		    return await MetasploitConnector.RPCCall("module.info", moduleType, moduleName);
		}
		
		public static async Task<Dictionary<string, object>> GetOptions(string moduleType, string moduleName)
		{
		    return await MetasploitConnector.RPCCall("module.options", moduleType, moduleName);
		}
		
		public static async Task<Dictionary<string, object>> GetCompatiblePayloads(string moduleName)
		{
		    return await MetasploitConnector.RPCCall("module.compatible_payloads", moduleName);
		}
		
		public static async Task<Dictionary<string, object>> GetTargetCompatiblePayloads(string moduleName, int targetIndex)
		{
		    return await MetasploitConnector.RPCCall("module.target_compatible_payloads", moduleName, targetIndex);
		}
		
		public static async Task<Dictionary<string, object>> GetCompatibleSessions(string moduleName)
		{
		    return await MetasploitConnector.RPCCall("module.compatible_sessions", moduleName);
		}
		
		public static async Task<Dictionary<string, object>> Encode(string data, string encoderModule, Dictionary<string, object> options)
		{
		    return await MetasploitConnector.RPCCall("module.encode", data, encoderModule, options);
		}
		
		public static async Task<Dictionary<string, object>> Execute(string moduleType, string moduleName, Dictionary<string, object> options)
		{
		    return await MetasploitConnector.RPCCall("module.execute", moduleType, moduleName, options);
		}
	}

	public static class Plugin
	{
		public static async Task<Dictionary<string, object>> Load(string pluginName, Dictionary<string, object> options)
		{
		    return await MetasploitConnector.RPCCall("plugin.load", pluginName, options);
		}
		
		public static async Task<Dictionary<string, object>> Unload(string pluginName)
		{
		    return await MetasploitConnector.RPCCall("plugin.unload", pluginName);
		}
		
		public static async Task<Dictionary<string, object>> ListLoaded()
		{
		    return await MetasploitConnector.RPCCall("plugin.loaded");
		}
	}

	public static class Session
	{
		public static async Task<Dictionary<string, object>> List()
		{
		    return await MetasploitConnector.RPCCall("session.list");
		}
		
		public static async Task<Dictionary<string, object>> Stop(string sessionID)
		{
		    return await MetasploitConnector.RPCCall("session.stop", sessionID);
		}
		
		public static async Task<Dictionary<string, object>> ReadShell(string sessionID)
		{
		    return await ReadShell(sessionID, null);
		}
		
		public static async Task<Dictionary<string, object>> ReadShell(string sessionID, int? readPointer)
		{
			if (readPointer.HasValue)
			{
				return await MetasploitConnector.RPCCall("session.shell_read", sessionID, readPointer.Value);
			}
			else
			{
				return await MetasploitConnector.RPCCall("session.shell_read", sessionID);
			}
		}
		
		public static async Task<Dictionary<string, object>> WriteToShell(string sessionID, string data)
		{
		    return await MetasploitConnector.RPCCall("session.shell_write", sessionID, data);
		}
		
		public static async Task<Dictionary<string, object>> WriteToMeterpreter(string sessionID, string data)
		{
		    return await MetasploitConnector.RPCCall("session.meterpreter_write", sessionID, data);
		}
		
		public static async Task<Dictionary<string, object>> ReadMeterpreter(string sessionID)
		{
		    return await MetasploitConnector.RPCCall("session.meterpreter_read", sessionID);
		}
		
		public static async Task<Dictionary<string, object>> RunMeterpreterSingleCommand(string sessionID, string command)
		{
		    return await MetasploitConnector.RPCCall("session.meterpreter_run_single", sessionID, command);
		}
		
		public static async Task<Dictionary<string, object>> RunMeterpreterScript(string sessionID, string scriptName)
		{
		    return await MetasploitConnector.RPCCall("session.meterpreter_script", sessionID, scriptName);
		}
		
		public static async Task<Dictionary<string, object>> DetachMeterpreter(string sessionID)
		{
		    return await MetasploitConnector.RPCCall("session.meterpreter_session_detach", sessionID);
		}
		
		public static async Task<Dictionary<string, object>> KillMeterpreter(string sessionID)
		{
		    return await MetasploitConnector.RPCCall("session.meterpreter_session_kill", sessionID);
		}
		
		public static async Task<Dictionary<string, object>> TabMeterpreter(string sessionID, string input)
		{
		    return await MetasploitConnector.RPCCall("session.meterpreter_tabs", sessionID, input);
		}
		
		public static async Task<Dictionary<string, object>> CompatibleModules(string sessionID)
		{
		    return await MetasploitConnector.RPCCall("session.compatible_modules", sessionID);
		}
		
		public static async Task<Dictionary<string, object>> UpgradeShellToMeterpreter(string sessionID, string host, string port)
		{
		    return await MetasploitConnector.RPCCall("session.shell_upgrade", sessionID, host, port);
		}
		
		public static async Task<Dictionary<string, object>> ClearRing(string sessionID)
		{
		    return await MetasploitConnector.RPCCall("session.ring_clear", sessionID);
		}
		
		public static async Task<Dictionary<string, object>> LastRing(string sessionID)
		{
		    return await MetasploitConnector.RPCCall("session.ring_last", sessionID);
		}
		
		public static async Task<Dictionary<string, object>> WriteToRing(string sessionID, string data)
		{
		    return await MetasploitConnector.RPCCall("session.ring_put", sessionID, data);
		}
		
		public static async Task<Dictionary<string, object>> ReadRing(string sessionID, int? readPointer)
		{
			if (readPointer.HasValue)
			{
				return await MetasploitConnector.RPCCall("session.ring_read", sessionID, readPointer.Value);
			}
			else
			{
				return await MetasploitConnector.RPCCall("session.ring_read", sessionID);
			}
		}
	}
}
