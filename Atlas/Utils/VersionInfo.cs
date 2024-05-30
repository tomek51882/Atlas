using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Utils
{
    internal static class VersionInfo
    {
        internal static string GetCommitHash()
        {
            var assembly = Assembly.GetExecutingAssembly();
            if (assembly is null)
            {
                return "Unknown";
            }

            var attribute = Attribute.GetCustomAttribute(assembly, typeof(AssemblyInformationalVersionAttribute));
            if (attribute is null)
            {
                return "Unknown";
            }
            var assemblyVersionInfo = (AssemblyInformationalVersionAttribute)attribute;
            if (assemblyVersionInfo is null)
            {
                return "Unknown";
            }
            string commit = assemblyVersionInfo.InformationalVersion.Split('+')[1].Substring(0,8);

            return commit;
        }
    }
}
