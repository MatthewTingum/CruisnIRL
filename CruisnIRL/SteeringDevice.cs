using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruisnIRL
{
    public class SteeringDevice : ObdDevice
    {
        private int LastAngle;

        public SteeringDevice(string comPort, int baud)
        {
            Connect(comPort, baud);

            // The responses we want end with "1E5"
            Port.NewLine = "1E5";
            // Filter providing mostly steering angle messages
            WriteAndCheckResponse("ATMTE5");
        }

        // TODO: Write this better
        public int GetSteeringAngle()
        {
            int degrees = 0;
            string hex = "";
            string response = Port.ReadLine();
            // Cut off the data error -- Why do we get this error? It occurs on stock hardware
            // in a putty terminal with a basic filter.
            /*
                TODO: This gets odd responses like: response == " 5C " or "1E\n\n\n"
                Use the last known value if this happens. This should not be an exception,
                but rather something that is checked for validity.
            */
            try
            {
                response = response.Substring(0, 24);
            } catch(Exception e)
            {
                return LastAngle;
            }
            hex = response.Substring(4, 5);
            byte bytea = Convert.ToByte(hex.Substring(0, 2), 16);
            byte byteb = Convert.ToByte(hex.Substring(3, 2), 16);
            List<byte> stuff = new List<byte>();
            stuff.Add(new byte());
            stuff.Add(new byte());
            stuff.Add(bytea);
            stuff.Add(byteb);
            stuff.Reverse();
            degrees = BitConverter.ToInt32(stuff.ToArray(), 0);
            //byte[] tmp = Enumerable.Range(0, hex.Length / 2).Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16)).ToArray();
            int zed = 0;

            byte[] bytes = Encoding.ASCII.GetBytes(response);

            //Console.WriteLine(response0.Substring(4, 5));
            
            long maxAxis = 32767;
            // If the max axis is otherwise unknown, you can get it this way:
            //joystick.GetVJDAxisMax(JoystickID, HID_USAGES.HID_USAGE_X, ref maxval);
            int midRange = (int)maxAxis / 2;
            int steeringAngle = 0;
            if (degrees > 32767)
            {
                // turning right
                steeringAngle = midRange + (65537 - degrees) * 2;
            }
            else
            {
                // turing left
                steeringAngle = midRange - (degrees * 2);
            }

            //Console.WriteLine(degrees / 16);
            LastAngle = steeringAngle;
            return steeringAngle;
        }
    }
}
