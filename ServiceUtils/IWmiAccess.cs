using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace ServiceConfigurator.ServiceUtils
{
    //
    // Credit to http://geekswithblogs.net/robz/archive/2008/09/21/how-to-programmatically-install-a-windows-service-.net-on-a.aspx
    //
    //private readonly string _machineName = Environment.MachineName;
    //private const string _serviceName = "_TESTDELETEME";
    //private const string _remoteMachineName = "remoteMachineName";
    //private const string _serviceLocation = @"c:\WINDOWS\system32\taskmgr.exe";
    //private const string _serviceDisplayName = "_TEST DELETE ME";
    //private const string _username = "username";
    //private const string _password = "password";
    //private readonly string[] _dependency = new[] { "MSMQ" };
    //private readonly string[] _multipleDependencies = new[] { "MSMQ", "helpsvc" };
    //private WmiService _wmiService;
    //private const string _localSystemAccount = "LocalSystem";
    //private const string _networkAccount = @"NT AUTHORITY\NetworkService";

    public interface IWmiAccess
    {
        int InvokeInstanceMethod(string machineName, string className, string name, string methodName);

        /// <summary>
        /// Calls a named instance of WMI on the remote machine invoking a method on a WMI Class
        /// </summary>
        /// <param name="machineName">Name of the computer to perform the operation on</param>
        /// <param name="className">The WMI Class to invoke</param>
        /// <param name="name">The name of the WMI Instance</param>
        /// <param name="methodName">The method to call on the WMI Class</param>
        /// <param name="parameters">Parameters for the method</param>
        /// <returns>A return code from the invoked method on the WMI Class</returns>
        int InvokeInstanceMethod(string machineName, string className, string name, string methodName, object[] parameters);

        int InvokeStaticMethod(string machineName, string className, string methodName);

        /// <summary>
        /// Calls WMI on the remote machine invoking a method on a WMI Class
        /// </summary>
        /// <param name="machineName">Name of the computer to perform the operation on</param>
        /// <param name="className">The WMI Class to invoke</param>
        /// <param name="methodName">The method to call on the WMI Class</param>
        /// <param name="parameters">Parameters for the method</param>
        /// <returns>A return code from the invoked method on the WMI Class</returns>
        int InvokeStaticMethod(string machineName, string className, string methodName, object[] parameters);
    }


    public class WmiAccess : IWmiAccess
    {
        private static ManagementScope Connect(string machineName)
        {
            ConnectionOptions options = new ConnectionOptions();
            string path = string.Format("\\\\{0}\\root\\cimv2", machineName);
            ManagementScope scope = new ManagementScope(path, options);
            scope.Connect();

            return scope;
        }

        private static ManagementObject GetInstanceByName(string machineName, string className, string name)
        {
            ManagementScope scope = Connect(machineName);
            ObjectQuery query = new ObjectQuery("SELECT * FROM " + className + " WHERE Name = '" + name + "'");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection results = searcher.Get();
            foreach (ManagementObject manObject in results)
                return manObject;

            return null;
        }

        private static ManagementClass GetStaticByName(string machineName, string className)
        {
            ManagementScope scope = Connect(machineName);
            ObjectGetOptions getOptions = new ObjectGetOptions();
            ManagementPath path = new ManagementPath(className);
            ManagementClass manClass = new ManagementClass(scope, path, getOptions);
            return manClass;
        }

        public int InvokeInstanceMethod(string machineName, string className, string name, string methodName)
        {
            return InvokeInstanceMethod(machineName, className, name, methodName, null);
        }

        public int InvokeInstanceMethod(string machineName, string className, string name, string methodName, object[] parameters)
        {
            try
            {
                ManagementObject manObject = GetInstanceByName(machineName, className, name);
                object result = manObject.InvokeMethod(methodName, parameters);
                return Convert.ToInt32(result);
            }
            catch
            {
                return -1;
            }
        }

        public int InvokeStaticMethod(string machineName, string className, string methodName)
        {
            return InvokeStaticMethod(machineName, className, methodName, null);
        }

        public int InvokeStaticMethod(string machineName, string className, string methodName, object[] parameters)
        {
            try
            {
                ManagementClass manClass = GetStaticByName(machineName, className);
                object result = manClass.InvokeMethod(methodName, parameters);
                return Convert.ToInt32(result);
            }
            catch
            {
                return -1;
            }
        }
    }
}
