using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruisnIRL
{
    public class PedalDevice : ObdDevice
    {
        public PedalDevice(string comPort, int baud)
        {
            Connect(comPort, baud);

            // Important responses end with 214
            Port.NewLine = "214";
            // Filter providing mostly brake pressure messages. Pressure may be the wrong word to use here.
            WriteAndCheckResponse("ATMT14");
        }

        public int GetBrakePressure()
        {
            string response;
            string hex;

            Port.ReadExisting();
            Port.DiscardInBuffer();
            Port.ReadLine();

            response = Port.ReadLine();
            hex = response.Substring(4, 2);
            byte bytea = Convert.ToByte(hex.Substring(0, 2), 16);
            //Console.WriteLine("Brake Pressure: {0}\n", hex);
            return Convert.ToInt32(hex.Substring(0, 2), 16);
        }
    }
}
