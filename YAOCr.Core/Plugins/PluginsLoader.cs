using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using YAOCr.Plugins;

namespace YAOCr.Core.Plugins;
public static class PluginsLoader {
    private static IConfiguration _configuration;

    private static Dictionary<string, IPlugin> _plugins = new();

    public static void Initialize(IConfiguration configuration) {
        _configuration = configuration;

        LoadPlugins();
    }

    public static Dictionary<string, IPlugin> GetPlugins() => _plugins;

    private static void LoadPlugins() {
        var pluginsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _configuration["AppSettings:PluginsFolder"]);

        if (Directory.Exists(pluginsFolder)) {
            var pluginsPath = Directory.GetFiles(pluginsFolder, "*.dll");

            foreach (var path in pluginsPath) {
                //var plugin = LoadPlugin<IFileParserPlugin>(path);
                var plugin = LoadPlugin<IPlugin>(path);

                _plugins.Add(plugin.Id, plugin);
            }
        } else {
            Debug.WriteLine($"Plugins folder doesn't exists: ${pluginsFolder}");
        }
    }

    private static T LoadPlugin<T>(string pluginPath) where T : class, IPlugin {
        var loadContext = new PluginLoadContext(pluginPath);
        var assembly = loadContext.LoadFromAssemblyName(AssemblyName.GetAssemblyName(pluginPath));

        foreach (Type type in assembly.GetTypes()) {
            if (typeof(T).IsAssignableFrom(type)) {
                T plugin = Activator.CreateInstance(type) as T;

                return plugin;
            }
        }
        return null;
    }
}
