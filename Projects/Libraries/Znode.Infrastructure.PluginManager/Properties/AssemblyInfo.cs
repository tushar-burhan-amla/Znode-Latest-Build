using System.Web;
using System.Reflection;

[assembly: PreApplicationStartMethod(typeof(Znode.Infrastructure.PluginManager.PreApplicationInit), "InitializePlugins")]

[assembly: AssemblyTitle("Znode.Infrastructure.PluginManager")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Znode")]
[assembly: AssemblyProduct("Znode.Infrastructure.PluginManager")]
[assembly: AssemblyCopyright("Copyright © 2023")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("9.7.5.0")]
[assembly: AssemblyFileVersion("9.7.5.0")]
