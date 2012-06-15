using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceConfigurator.ServiceUtils
{
    public class WmiService
    {
        static readonly WmiService instance=new WmiService(new WmiAccess());
        static WmiService() {
        }
        WmiService() {
        }
        public static WmiService Instance
        {
            get { return instance; }
        }

        private const string CLASS_NAME = "Win32_Service";
        private readonly IWmiAccess _wmi;

        /// <summary>
        /// Creates a new WmiService for use to access Windows Services
        /// </summary>
        /// <param name="wmi">The WMI access object - the tool that does the low level work</param>
        public WmiService(IWmiAccess wmi)
        {
            _wmi = wmi;
        }

        public ServiceReturnCode Install(string name, string displayName, string physicalLocation, ServiceStartMode startMode, string userName, string password, string[] dependencies)
        {
            return Install(Environment.MachineName, name, displayName, physicalLocation, startMode, userName, password, dependencies, false);
        }

        public ServiceReturnCode Install(string machineName, string name, string displayName, string physicalLocation, ServiceStartMode startMode, string userName, string password, string[] dependencies)
        {
            return Install(machineName, name, displayName, physicalLocation, startMode, userName, password, dependencies, false);
        }

        /// <summary>
        /// Installs a service on any machine
        /// </summary>
        /// <param name="machineName">Name of the computer to perform the operation on</param>
        /// <param name="name">The name of the service in the registry</param>
        /// <param name="displayName">The display name of the service in the service manager</param>
        /// <param name="physicalLocation">The physical disk location of the executable</param>
        /// <param name="startMode">How the service starts - usually Automatic</param>
        /// <param name="userName">The user for the service to run under</param>
        /// <param name="password">The password fo the user</param>
        /// <param name="dependencies">Other dependencies the service may have based on the name of the service in the registry</param>
        /// <param name="interactWithDesktop">Should the service interact with the desktop?</param>
        /// <returns>A service return code that defines whether it was successful or not</returns>
        public ServiceReturnCode Install(string machineName, string name, string displayName, string physicalLocation, ServiceStartMode startMode, string userName, string password, string[] dependencies, bool interactWithDesktop)
        {
            const string methodName = "Create";
            //string[] serviceDependencies = dependencies != null ? dependencies.Split(',') : null;
            if (userName.IndexOf('\\') < 0)
            {
                //userName = ".\\" + userName;
                //UNCOMMENT the line above - it caused issues with color coding in THIS ARTICLE
            }

            try
            {
                object[] parameters = new object[]
                                      {
                                          name, // Name
                                          displayName, // Display Name
                                          physicalLocation, // Path Name | The Location "E:\somewhere\something"
                                          Convert.ToInt32(ServiceType.OwnProcess), // ServiceType
                                          Convert.ToInt32(ServiceErrorControl.UserNotified), // Error Control
                                          startMode.ToString(), // Start Mode
                                          interactWithDesktop, // Desktop Interaction
                                          userName, // StartName | Username
                                          password, // StartPassword |Password
                                          null, // LoadOrderGroup | Service Order Group
                                          null, // LoadOrderGroupDependencies | Load Order Dependencies
                                          dependencies // ServiceDependencies
                                      };
                return (ServiceReturnCode)_wmi.InvokeStaticMethod(machineName, CLASS_NAME, methodName, parameters);
            }
            catch
            {
                return ServiceReturnCode.UnknownFailure;
            }
        }

        public ServiceReturnCode Uninstall(string name)
        {
            return Uninstall(Environment.MachineName, name);
        }

        /// <summary>
        /// Uninstalls a service on any machine
        /// </summary>
        /// <param name="machineName">Name of the computer to perform the operation on</param>
        /// <param name="name">The name of the service in the registry</param>
        /// <returns>A service return code that defines whether it was successful or not</returns>
        public ServiceReturnCode Uninstall(string machineName, string name)
        {
            try
            {
                const string methodName = "Delete";
                return (ServiceReturnCode)_wmi.InvokeInstanceMethod(machineName, CLASS_NAME, name, methodName);
            }
            catch
            {
                return ServiceReturnCode.UnknownFailure;
            }
        }
    }
}
