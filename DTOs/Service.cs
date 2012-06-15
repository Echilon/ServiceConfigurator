using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ServiceConfigurator
{
    public class Service
    {
        public string ServiceName { get; set; }
        public List<Server> Servers { get; set; }

        public Service(string serviceName) {
            this.ServiceName = serviceName;
            this.Servers = new List<Server>();
        }

        public Service(string serviceName, IEnumerable<Server> servers) 
            :this(serviceName) {
            this.Servers.AddRange(servers);
        }

        public Service(XElement element)
            : this(element.Attribute("name").Value) {
            var eleServers = element.Elements("server");
            foreach (var eleServer in eleServers) {
                this.Servers.Add(new Server(eleServer)
                                     {
                                         ServiceName = this.ServiceName
                                     });
            }
        }

        public XElement ToXml() {
            var root = new XElement("service",
                                    new XAttribute("name", ServiceName));
            foreach(var server in this.Servers) {
                root.Add(server.ToXml());
            }
            return root;
        }
    }
}
