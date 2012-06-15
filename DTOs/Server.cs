using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ServiceConfigurator
{
    public class Server
    {
        public string MachineName { get; set; }

        /// <summary>
        /// Gets or sets the friendly name of the server.
        /// </summary>
        /// <value>
        /// The friendly name of the server
        /// </value>
        public string Name { get; set; }

        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the path to the executeble on this server.
        /// </summary>
        /// <value>
        /// The path to the executeble on this server.
        /// </value>
        public string ExePath { get; set; }

        public Server() {
            //System.Diagnostics.Debugger.Break();
        }

        public Server(XElement element)
        :this() {
            this.MachineName = element.Attribute("machineName").Value;
            this.Name = element.Attribute("name").Value;
            var eleExePath = element.Attribute("exepath");
            if (eleExePath != null)
                ExePath = eleExePath.Value;
        }

        public XElement ToXml() {
            var root = new XElement("service",
                                    new XAttribute("name", Name),
                                    new XAttribute("machineName", MachineName));
            if (!string.IsNullOrEmpty(ExePath))
                root.Add(new XAttribute("exepath", ExePath));
            return root;
        }
    }
}
