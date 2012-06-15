using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ServiceConfigurator
{
    public class Utils
    {
        /// <summary>
        /// Parses the configuration file from the application directory.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Service> ParseConfig()
        {
            List<Service> services = new List<Service>();
            var configPath = Path.Combine(Utils.GetApplicationPath(), "config.xml");
            var configFile = XDocument.Load(configPath, LoadOptions.None);
            var config = configFile.Root;
            var eleServices = config.Element("services").Elements("service");
            foreach (var eleService in eleServices) {
                var service = new Service(eleService);
                services.Add(service);
            }
            return services;
        }

        /// <summary>
        /// Gets the path of the directory in which the application is located.
        /// </summary>
        /// <returns>The path of the directory in which the application is located.</returns>
        public static string GetApplicationPath()
        {
            string fqPath = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
            return Path.GetDirectoryName(fqPath);
        }
    }
}
