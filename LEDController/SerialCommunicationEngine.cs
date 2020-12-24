using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;

using System.Management;

using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Windows;
using System.Threading;

namespace Controller
{

    public class SerialCommunicationEngine
    {
        public class SerialPortInfo
        {
            public SerialPortInfo(ManagementObject property)
            {
                this.Availability = property.GetPropertyValue("Availability") as int? ?? 0;
                this.Caption = property.GetPropertyValue("Caption") as string ?? string.Empty;
                this.ClassGuid = property.GetPropertyValue("ClassGuid") as string ?? string.Empty;
                this.CompatibleID = property.GetPropertyValue("CompatibleID") as string[] ?? new string[] { };
                this.ConfigManagerErrorCode = property.GetPropertyValue("ConfigManagerErrorCode") as int? ?? 0;
                this.ConfigManagerUserConfig = property.GetPropertyValue("ConfigManagerUserConfig") as bool? ?? false;
                this.CreationClassName = property.GetPropertyValue("CreationClassName") as string ?? string.Empty;
                this.Description = property.GetPropertyValue("Description") as string ?? string.Empty;
                this.DeviceID = property.GetPropertyValue("DeviceID") as string ?? string.Empty;
                this.ErrorCleared = property.GetPropertyValue("ErrorCleared") as bool? ?? false;
                this.ErrorDescription = property.GetPropertyValue("ErrorDescription") as string ?? string.Empty;
                this.HardwareID = property.GetPropertyValue("HardwareID") as string[] ?? new string[] { };
                this.InstallDate = property.GetPropertyValue("InstallDate") as DateTime? ?? DateTime.MinValue;
                this.LastErrorCode = property.GetPropertyValue("LastErrorCode") as int? ?? 0;
                this.Manufacturer = property.GetPropertyValue("Manufacturer") as string ?? string.Empty;
                this.Name = property.GetPropertyValue("Name") as string ?? string.Empty;
                this.PNPClass = property.GetPropertyValue("PNPClass") as string ?? string.Empty;
                this.PNPDeviceID = property.GetPropertyValue("PNPDeviceID") as string ?? string.Empty;
                this.PowerManagementCapabilities = property.GetPropertyValue("PowerManagementCapabilities") as int[] ?? new int[] { };
                this.PowerManagementSupported = property.GetPropertyValue("PowerManagementSupported") as bool? ?? false;
                this.Present = property.GetPropertyValue("Present") as bool? ?? false;
                this.Service = property.GetPropertyValue("Service") as string ?? string.Empty;
                this.Status = property.GetPropertyValue("Status") as string ?? string.Empty;
                this.StatusInfo = property.GetPropertyValue("StatusInfo") as int? ?? 0;
                this.SystemCreationClassName = property.GetPropertyValue("SystemCreationClassName") as string ?? string.Empty;
                this.SystemName = property.GetPropertyValue("SystemName") as string ?? string.Empty;
            }

            public int Availability;
            public string Caption;
            public string ClassGuid;
            public string[] CompatibleID;
            public int ConfigManagerErrorCode;
            public bool ConfigManagerUserConfig;
            public string CreationClassName;
            public string Description;
            public string DeviceID;
            public bool ErrorCleared;
            public string ErrorDescription;
            public string[] HardwareID;
            public DateTime InstallDate;
            public int LastErrorCode;
            public string Manufacturer;
            public string Name;
            public string PNPClass;
            public string PNPDeviceID;
            public int[] PowerManagementCapabilities;
            public bool PowerManagementSupported;
            public bool Present;
            public string Service;
            public string Status;
            public int StatusInfo;
            public string SystemCreationClassName;
            public string SystemName;

        }

        public string lastError = "";

        /// <summary>
        /// Compile an array of COM port names associated with given VID and PID
        /// </summary>
        /// <param name="VID">string representing the vendor id of the USB/Serial convertor</param>
        /// <param name="PID">string representing the product id of the USB/Serial convertor</param>
        /// <returns></returns>
        private static List<string> GetPortByVPid(string VID, string PID)
        {
            /*string pattern = string.Format("^VID_{0}.PID_{1}", VID, PID);
            Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
            List<string> comports = new List<string>();
            RegistryKey rk1 = Registry.LocalMachine;
            RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
            foreach (string s3 in rk2.GetSubKeyNames())
            {
                RegistryKey rk3 = rk2.OpenSubKey(s3);
                foreach (string s in rk3.GetSubKeyNames())
                {
                    if (_rx.Match(s).Success)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey(s);
                        foreach (string s2 in rk4.GetSubKeyNames())
                        {
                            RegistryKey rk5 = rk4.OpenSubKey(s2);
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            comports.Add((string)rk6.GetValue("PortName"));
                        }
                    }
                }
            }
            return comports;*/

            List<string> result = new List<string>();

            Debug.Log("Serial devices: ");
            ManagementClass processClass = new ManagementClass("Win32_PnPEntity");
            ManagementObjectCollection Ports = processClass.GetInstances();
            foreach (ManagementObject property in Ports)
            {
                var name = property.GetPropertyValue("Name");
                if (name != null && name.ToString().Contains("COM"))
                {
                    var portInfo = new SerialPortInfo(property);
                    //Thats all information i got from port.
                    //Do whatever you want with this information


                    Debug.Log("\t" + name);

                    if(portInfo.DeviceID.Contains("VID_" + VID + "&PID_" + PID))
                    {
                        //Console.WriteLine("Found");
                        string value = "";
                        string nameStr = name.ToString();
                        int start = -1, end = -1;
                        for(int i = nameStr.Length - 1;i>=0;i--)
                        {

                            if(nameStr[i] == ')')
                            {
                                end = i;
                            }
                            if(nameStr[i] == '(')
                            {
                                start = i;
                                break;
                            }
                        }

                        if(start == -1 || end == -1)
                        {
                            Debug.LogError("Błąd przetwarzania nazwy portu");
                            continue;
                        }

                        value = nameStr.Substring(start + 1, end - start - 1 /* because of ( and ) */);

                        result.Add(value);
                    }
                }
            }

            return result;
        }

        public static string FindDevicePort(string vid, string pid)
        {
            if(!MUtil.IsWindows)
            {
                return "NS"; // Not supported

            }
            List<string> ports = GetPortByVPid(vid, pid);
            if(ports.Count == 0)
            {
                Debug.Log("No devices with VID: \"" + vid + "\" and PID: \"" + pid + "\" found");
                return "NF";
            }
            /*Debug.Log("Previous seen on ports: ");
            for(int i = 0;i<ports.Count;i++)
            {
                Debug.Log(ports[i]);
            }*/
            if(ports.Count == 1)
            {
                return ports[0];
            }
            if(ports.Count > 1)
            {
                //Debug.LogWarning("Found more than 1 device with specified VIP / PID, selecting the first one");
                //for(int i = 0;i<ports.Count;i++)
                //{
                //    Debug.Log(ports[i]);
                //}
                /*string[] ps = SerialPort.GetPortNames();
                Debug.Log("Opened serial ports:");
                for(int i = 0;i<ps.Length;i++)
                {
                    Debug.Log(ps[i]);
                    if(ports.Contains(ps[i]))
                    {
                        //return ps[i];
                    }
                }*/

                //MessageBox.Show("Znaleziono 2 urzadzenia ");

                string val = "NSEL";

                //List<string> data = new List<string>();
                Dictionary<string, string> data = new Dictionary<string, string>();
                for(int i = 0;i<ports.Count;i++)
                {
                    /*SerialCommunicationEngine engine = new SerialCommunicationEngine();
                    engine.Initialize(ports[i], 115200);
                    if(!engine.initialized)
                    {
                        engine.Close();
                        break;
                    }*/
                    LEDController led = new LEDController();
                    var status = led.Connect(ports[i]);
                    if(status != InitializeStatus.Success)
                    {
                        continue;
                    }

                    string name = led.GetDeviceName();
                    //data.Add(name);
                    data.Add(name, ports[i]);

                    led.Disconnect();
                }

                SelectDeviceDialog dialog = new SelectDeviceDialog(data.Keys.ToList(), (value) =>
                {
                    //Debug.Log("Selected value: " + value);
                    val = data[value]; //value;
                });

                dialog.ShowDialog();

                while(dialog.IsActive)
                {
                    Thread.Sleep(10);
                }

                return val;
            }
            return "NF";
        }

        public InitializeStatus Initialize(string vid, string pid, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One, Handshake handshake = Handshake.None)
        {
            string port = FindDevicePort(vid, pid);
            if(port == "NF")
            {
                lastError = "Cannot find device with VID: \"" + vid + "\" and PID: \"" + pid + "\"";
                Debug.LogError(lastError);
                return InitializeStatus.NotFound;
            }
            if(port == "NSEL")
            {
                lastError = "No device selected";
                Debug.LogError(lastError);
                return InitializeStatus.NotSelected;
            }
            if(port == "NS")
            {
                lastError = "Finding the port by VID / PID is not supported on this operating system";
                Debug.LogError(lastError);
                return InitializeStatus.NotSupported;
            }

            return Initialize(port, baudRate, parity, dataBits, stopBits, handshake);
        }

        public bool initialized = false;
        SerialPort sp;

        public enum InitializeStatus
        {
            Unknown,
            Success,
            NotFound,
            NotSelected,
            NotSupported,
            UnauthorizedAccess,
            ArgumentOutOfRange,
            BadArgument,
            IOError,
            InvalidOperation,
        }
        public InitializeStatus Initialize(string portName = "COM14", int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One, Handshake handshake = Handshake.None)
        {
            sp = new SerialPort();
            sp.BaudRate = baudRate;
            sp.PortName = portName;
            sp.Parity = parity;
            sp.DataBits = dataBits;
            sp.StopBits = stopBits;
            sp.Handshake = handshake;
            sp.DtrEnable = false;

            
            try
            {
                sp.Open();
            } catch(UnauthorizedAccessException)
            {
                lastError = "Unauthorized Access";
                Debug.LogError(lastError);
                return InitializeStatus.UnauthorizedAccess;
            } catch(ArgumentOutOfRangeException)
            {
                lastError = "Argument out of range";
                Debug.LogError(lastError);
                return InitializeStatus.ArgumentOutOfRange;
            } catch(ArgumentException)
            {
                lastError = "Bad Argument";
                Debug.LogError(lastError);
                return InitializeStatus.BadArgument;
            } catch(IOException)
            {
                lastError = "IO Error";
                Debug.LogError(lastError);
                return InitializeStatus.IOError;
            } catch(InvalidOperationException)
            {
                lastError = "Invalid Operation";
                Debug.LogError(lastError);
                return InitializeStatus.InvalidOperation;
            }
            initialized = true;

            Console.WriteLine("READY");
            //WaitForStartupPacket();
            //Console.WriteLine("Got welcome packet");
            return InitializeStatus.Success;
        }

        public void Close()
        {
            sp.Close();
        }

        private void WaitForStartupPacket(string packet = "READY")
        {
            while(true)
            {
                if(ReadLine().Contains(packet))
                {
                    break;
                }
            }
        }

        public void WriteLine(string data, int timeout = 1000)
        {
            try
            {
                int og = sp.ReadTimeout;
                if (timeout != -1)
                {
                    sp.ReadTimeout = timeout;
                }

                Debug.Log(data, ConsoleColor.Gray, true, "Serial.WriteLine");
                sp.WriteLine(data);

                if (timeout != -1)
                {
                    sp.ReadTimeout = og;
                }
            }            
            catch(Exception e)
            {
                Debug.Exception(e);
                return;
            }
        }

        public void Write(string data)
        {
            //Debug.Log("[SerialEngine] |Write| " + data);
            Debug.Log(data, ConsoleColor.Gray, true, "Serial.Write");
            sp.Write(data);
        }

        public string ReadLine(int timeout = 1000)
        {
            try
            {
                int og = sp.ReadTimeout;
                sp.ReadTimeout = timeout;

                string line = sp.ReadLine();
                //Debug.Log("[SerialEngine] |Read|Line| " + line);
                Debug.Log(line, ConsoleColor.Gray, true, "Serial.ReadLine");

                sp.ReadTimeout = og;
                return line.Replace("\r", "");
            }
            catch(Exception e)
            {
                Debug.Exception(e);
                return "EXCEPTION";
            }
        }

        public string Read(int size)
        {
            try
            {
                byte[] buffer = new byte[size];
                sp.Read(buffer, 0, size);
                string line = "";
                for(int i = 0;i<size;i++)
                {
                    line += buffer[i];
                }
                //Debug.Log("[SerialEngine] |Read| " + line);
                Debug.Log(line, ConsoleColor.Gray, true, "Serial.Read");
                return line.Replace("\r", "");
            }
            catch (Exception e)
            {
                Debug.Exception(e);
                return "EXCEPTION";
            }
        }
    }
}
