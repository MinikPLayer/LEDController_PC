using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime;

namespace Controller
{
    public class LEDController
    {
        public static string okResponse = "=OK";

        public string lastError
        {
            get
            {
                return serial.lastError;
            }
        }

        public string VID = "1EAF";
        public string PID = "0004";
        //string VID = "1A86";
        //string PID = "7523";

        int baudrate = 115200;

        SerialCommunicationEngine serial = new SerialCommunicationEngine();

        public class State
        {
            public string name = "unknown";
            string _paramName = "";
            public string paramName //= ""; // ex. "static" or "rainbow"
            {
                get
                {
                    if (_paramName.Length == 0) return name;
                    return _paramName;
                }
                set
                {
                    _paramName = value;
                }
            }
            public Type classType;

            public State(string name, Type classType, string paramName = "")
            {
                this.name = name;
                this.classType = classType;
                this.paramName = paramName;
            }

            public bool Init()
            {
                try
                {

                    string modeName = name;
                    if (paramName.Length != 0)
                    {
                        modeName = paramName;
                    }

                    string actualState = Global.controller.GetState();
                    Debug.Log("ActualState: " + actualState + ", name: " + name);
                    if (actualState != modeName)
                    {
                        Debug.Log("Setting new state");
                        if (!Global.controller.SetGlobalParam("state", modeName))
                        {
                            Debug.LogError("Cannot change state");
                            return false;
                        }
                    }

                    if (classType != null)
                    {
                        System.Windows.Window window = (System.Windows.Window)Activator.CreateInstance(classType);

                        Debug.Log("Setting window content");

                        PageManager.SetPage(window.Content);
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Debug.Exception(e);
                    return false;
                }
            }
        }

        public State FindState(int index)
        {
            if(index < 0 || index >= states.Length)
            {
                Debug.LogError("State index outside of states range");
                return null;
            }

            return states[index];
        }

        public State FindState(string name)
        {

            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].paramName.Length != 0)
                {
                    if (states[i].paramName == name)
                        return states[i];
                }
                else if (states[i].name == name)
                    return states[i];
            }
            return null;
        }

        public static State[] states { get; private set; } = new State[] { new State("Static", typeof(Solid), "static"), new State("Breathing", typeof(Breathing), "breathing"), new State("Rainbow", typeof(Rainbow), "rainbow"), new State("Rising And Falling", typeof(RisingAndFalling), "risingandfalling"), new State("Burning dot", typeof(BurningDot), "burning"), new State("Gradient", typeof(Gradient), "gradient") };

        public bool connected
        {
            get
            {
                string s = _Send("+test");
                if (s.StartsWith("Test")) return true;
                else
                {
                    return false;
                }
            }
        }

        public bool initialized
        {
            get
            {
                return serial.initialized;
            }
        }

        public LEDController()
        {
        }

        public LEDController(string VID, string PID)
        {
            this.VID = VID;
            this.PID = PID;
        }

        public string GetState()
        {
            return GetGlobalParam<string>("state");
        }

        public bool SetState(State state)
        {
            if(state == null)
            {
                Debug.LogError("[LEDController] State not found!");
                return false;
            }

            return state.Init();
        }
        public bool SetState(int index)
        {
            if(index < 0 || index >= states.Length)
            {
                Debug.LogError("Unknown state index");
                return false;
            }

            return SetState(states[index]);
        }

        public bool SetState(string state)
        {
            Debug.Log("Setting new state: " + state, ConsoleColor.White, true, "LedController");
            return SetState(FindState(state));
        }

        public static string ColorToString(System.Windows.Media.Color color)
        {
            return "(" + color.R + "," + color.G + "," + color.B + ")";
        }

        public static System.Windows.Media.Color StringToColor(string color)
        {
            if(color.Length == 0)
            {
                Debug.LogError("Cannot convert empty string to color");
                return System.Windows.Media.Color.FromRgb(0, 0, 0);
            }

            if(color[0] != '(' || color[color.Length - 1] != ')')
            {
                Debug.LogError("Invalid color format [Error code:1]");
                return System.Windows.Media.Color.FromRgb(0, 0, 0);
            }

            color = color.Substring(0, color.Length - 1).Remove(0, 1);

            string[] parts = color.Split(',');
            if(parts.Length != 3)
            {
                Debug.LogError("Invalid color format [Error code:2]");
                return System.Windows.Media.Color.FromRgb(0, 0, 0);
            }

            System.Windows.Media.Color clr = System.Windows.Media.Color.FromRgb(0, 0, 0);

            clr.R = MUtil.Parse<byte>(parts[0]);
            if(MUtil.lastParseError)
            {
                Debug.LogError("Invalid color format [Error code:3/R]");
                return clr;
            }
            clr.G = MUtil.Parse<byte>(parts[1]);
            if (MUtil.lastParseError)
            {
                Debug.LogError("Invalid color format [Error code:3/G]");
                return clr;
            }
            clr.B = MUtil.Parse<byte>(parts[2]);
            if (MUtil.lastParseError)
            {
                Debug.LogError("Invalid color format [Error code:3/B]");
                return clr;
            }

            return clr;
        }

        public SerialCommunicationEngine.InitializeStatus Connect()
        {

            var result = serial.Initialize(VID, PID, baudrate);

            return result;
        }

        public SerialCommunicationEngine.InitializeStatus Connect(string comPort)
        {
            var result = serial.Initialize(comPort, baudrate);

            return result;
        }

        public void Disconnect()
        {
            serial.Close();
        }

        public static bool CheckResponse(string response, string errorMessage = "")
        {
            if (response == okResponse)
            {
                return true;
            }
            else
            {
                if(errorMessage.Length > 0)
                {
                    Debug.LogError(errorMessage);
                }
                
                return false;
            }
        }

        public byte GetBrightness()
        {
            return GetGlobalParam<byte>("bright");
        }
        
        public string GetDeviceName()
        {
            return GetGlobalParam<string>("deviceName");
        }

        public T GetGlobalParam<T>(string param)
        {
            serial.WriteLine("+get " + param);

            string response = Read();
            if (response.StartsWith("| "))
            {
                response = response.Remove(0, 2);
            }

            serial.ReadLine();

            if(typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(response, typeof(T));
            }

            return MUtil.Parse<T>(response);
        }
        
        public bool SetGlobalParam(string param, string value)
        {
            string read = _Send("+set " + param + " " + value);

            return CheckResponse(read, "Set \"" + param + "\" command error: \"" + read + "\"");
        }

        public T GetParam<T>(string paramName)
        {
            return MUtil.Parse<T>(GetParam(paramName));
        }

        public string GetParam(string paramName)
        {
            serial.WriteLine("+getparam " + paramName);

            string response = Read();
            if(response.StartsWith("| "))
            {
                response = response.Remove(0, 2);
            }
            string check = serial.ReadLine(); // "=OK"
            if(check != okResponse)
            {
                Debug.FatalError("Bad response: " + check);

            }
            return response;
        }

        public bool SetParam(string param, string value)
        {
            string read = _Send("+setparam " + param + " " + value);

            return CheckResponse(read, "Set param \"" + param + "\" command error: \"" + read + "\"");
        }

        public bool SetBrightness(byte brightness)
        {
            //string read = _Send("+brightness " + brightness.ToString());

            byte val = (byte)((float)brightness * (float)brightness / 253f);
            return SetGlobalParam("bright", val.ToString());
        }

        public string Read()
        {
            string message = "";

            string response = serial.ReadLine();
            while (response[0] == '#')
            {
                message += response + "\n";
                response = serial.ReadLine();
            }
            message += response;

            return message;
        }

        public bool Save()
        {
            string read = _Send("+save");

            return CheckResponse(read, "Set brightness command error: \"" + read + "\"");
        }

        public string _Send(string data)
        {
            if (data.Length == 0) return "EMPTY";
            if(data[data.Length - 1] == '\n')
            {
                data = data.Substring(0, data.Length - 1);
            }
            if(data.Length == 0) return "EMPTY";
            serial.WriteLine(data);

            return Read();//serial.ReadLine();
        }
    }
}
