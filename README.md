ServiceConfigurator
===================

Install, uninstall and control remote and local windows services.

Totally configurable via an XML config file. No recompilation necessary, just edit the XML and run the exe.

# Configuration
The app uses config.xml which is in the application directory. You can monitor as many services as needed. Eg:
For a service which is only deployed on the Alpha server, the config would look like this:
<service name="My Great Service" exepath="C:\path\to\service.exe">
  <server name="Alpha" machineName="alpha"/>
</service>

To monitor 3 servers, the config could look like this:
<service name="My Great Service" exepath="C:\path\to\service.exe">
  <server name="Localhost (Test)" machineName="localhost"/>
  <server name="Alpha" machineName="alpha"/>
  <server name="Omega" machineName="176.192.1.1"/>
</service>

The available options are:
*MachineName* - The host name to which to connect (not displayed)                                               
*ExePath* - The path on the server where the service is located 

# Usage
1. Copy the service executable to it's destination on the server, Eg: Explorer (the tool doesn't do this).
2. Enter the path on the server where the service was copied
3. Click the Install button.
4. Use the start/stop buttons to control the service

# Troubleshooting
If the service fails to start, it's likely the exe path is wrong. If you have permission, it should 'just work'