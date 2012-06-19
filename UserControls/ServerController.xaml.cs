using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ServiceConfigurator.Properties;
using ServiceConfigurator.ServiceUtils;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;
using ServiceStartMode = System.ServiceProcess.ServiceStartMode;

namespace ServiceConfigurator
{
    /// <summary>
    /// Shows the start/stop/install/uninstall buttons for a single server
    /// </summary>
    public partial class ServerController : UserControl
    {
        public static readonly DependencyProperty RefreshCommandProperty =
            DependencyProperty.Register("RefreshCommand", typeof (ICommand), typeof (ServerController), new PropertyMetadata(default(ICommand)));

        public ICommand RefreshCommand {
            get { return (ICommand) GetValue(RefreshCommandProperty); }
            set { SetValue(RefreshCommandProperty, value); }
        }

        public static readonly DependencyProperty ServerProperty =
            DependencyProperty.Register("Server", typeof(Server), typeof(ServerController), 
            new PropertyMetadata(new Server(),
                new PropertyChangedCallback(ServerChanged)));

        private static void ServerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var controller = d as ServerController;
            var server = e.NewValue as Server;
            if(e.NewValue!=e.OldValue&& server!=null) {
                controller._serviceName = server.ServiceName;
                controller.Controller = new ServiceController(server.ServiceName, server.MachineName);
                controller.RefreshStatus();
            }
        }

        public Server Server
        {
            get { return (Server)GetValue(ServerProperty); }
            set { SetValue(ServerProperty, value); }
        }
        private ServiceController Controller { get; set; }
        private string _serviceName { get; set; }

        public ServerController()
        {
            InitializeComponent();
        }

        public ServerController(string serviceName, Server server) 
        :this() {
            this._serviceName = serviceName;
            this.Server = server;
            this.Controller = new ServiceController(serviceName, server.MachineName);
        }

        #region Starting/Stopping
        private void btnStartStop_Click(object sender, RoutedEventArgs e) {
            ChangeStatus(sender);
        }

        /// <summary>
        /// Changes the status of a controller based on the CommandParameter of the sender (A button), then updates the form controls
        /// </summary>
        /// <param name="sender"></param>
        private void ChangeStatus(object sender)
        {
            var btn = sender as Button;
            bool shouldStartService = string.Equals(Convert.ToString(btn.CommandParameter), "1", StringComparison.InvariantCultureIgnoreCase);
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, dwe) =>
            {
                if (shouldStartService) {
                    Controller.Start();
                } else {
                    Controller.Stop();
                }
                Thread.Sleep(5000);
            };
            worker.RunWorkerCompleted += (s, rwe) =>
            {
                busyIndicator.IsBusy = false;
                if (rwe.Error != null)
                {
                    MessageBox.Show(rwe.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
                RefreshStatus();
            };
            busyIndicator.IsBusy = true;
            worker.RunWorkerAsync();
        }
        #endregion

        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            //Settings.Default.ExePathProduction = txtPath.Text;
            Settings.Default.Save();
            var btn = sender as Button;
            bool shouldInstall = string.Equals(Convert.ToString(btn.CommandParameter), "1", StringComparison.InvariantCultureIgnoreCase);
            InstallService(shouldInstall);
        }

        private void InstallService(bool shouldInstall) {
            string exePath = txtPath.Text;
            if (string.IsNullOrEmpty(exePath)) {
                MessageBox.Show("Path to executable is required", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }
            ServiceReturnCode installStatus = ServiceReturnCode.NotSupported;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, dwe) =>
            {
                if (shouldInstall) {
                    installStatus = WmiService.Instance.Install(Controller.MachineName, _serviceName, _serviceName, exePath, ServiceUtils.ServiceStartMode.Automatic, "LocalSystem", null, null);
                } else {
                    installStatus = WmiService.Instance.Uninstall(Controller.MachineName, _serviceName);
                }
                Thread.Sleep(5000);
            };
            worker.RunWorkerCompleted += (s, rwe) =>
            {
                busyIndicator.IsBusy = false;
                if (rwe.Error != null) {
                    MessageBox.Show(rwe.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                } else if (installStatus != ServiceReturnCode.Success) {
                    MessageBox.Show(installStatus.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
                RefreshStatus();
            };
            busyIndicator.IsBusy = true;
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Updates the status of a single service
        /// </summary>
        private void RefreshStatus()
        {
            bool isInstalled = false;
            ServiceControllerStatus status = ServiceControllerStatus.Stopped;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, dwe) =>
            {
                isInstalled = ServiceController.GetServices(Controller.MachineName).Any(svc => svc.ServiceName == _serviceName);
                if (isInstalled)
                {
                    Controller.Refresh();
                    status = Controller.Status;
                }
            };
            worker.RunWorkerCompleted += (s, rwe) =>
            {
                if (isInstalled)
                {
                    switch (status)
                    {
                        case ServiceControllerStatus.Running:
                            lblStatus.Foreground = Brushes.LimeGreen;
                            lblStatus.Content = "Running";
                            btnStop.IsEnabled = true;
                            btnStart.IsEnabled = false;
                            break;
                        default:
                            lblStatus.Foreground = Brushes.Red;
                            lblStatus.Content = "Stopped";
                            btnStop.IsEnabled = false;
                            btnStart.IsEnabled = true;
                            break;
                    }
                    btnInstall.IsEnabled = false;
                    btnUninstall.IsEnabled = true;
                }
                else
                {
                    lblStatus.Foreground = Brushes.Red;
                    lblStatus.Content = "Not installed";
                    btnStart.IsEnabled = false;
                    btnStop.IsEnabled = false;
                    btnInstall.IsEnabled = true;
                    btnUninstall.IsEnabled = false;
                }
            };
            worker.RunWorkerAsync();
        }

        private void UpdateStatus(object sender, ExecutedRoutedEventArgs e) {
            RefreshStatus();
        }

        private void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }
    }
}
