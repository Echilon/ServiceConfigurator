using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public static readonly DependencyProperty ServicesProperty =
            DependencyProperty.Register("Services", typeof(ObservableCollection<Service>), typeof(MainWindow), new PropertyMetadata(new ObservableCollection<Service>()));

        public ObservableCollection<Service> Services
        {
            get { return (ObservableCollection<Service>)GetValue(ServicesProperty); }
            set { SetValue(ServicesProperty, value); }
        }
        
        public MainWindow()
        {
            InitializeComponent();
            
            var services = Utils.ParseConfig();
            foreach (var s in services) {
                this.Services.Add(s);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e) {
            Utils.SaveConfig(this.Services);
        }

        private void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }
    }
}
