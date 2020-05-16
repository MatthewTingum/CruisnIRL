using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CruisnIRL
{
    public class ObdDevice
    {
        public SerialPort Port;

        public void Connect(string comPort, int baud)
        {
            Port = new SerialPort(comPort, baud);
            Port.NewLine = ">";      // responses end with the > prompt character
            
            // TODO: Increase timeout and send ATZ a few times to get it calmed down
            Port.Open();

            // Reset
            WriteAndCheckResponse("ATZ");   
            Thread.Sleep(1000);

            // Linefeeds on
            WriteAndCheckResponse("ATL1");   
            Thread.Sleep(1000);

            // Headers
            WriteAndCheckResponse("ATH1");   
            Thread.Sleep(1000);

            // Long messages
            WriteAndCheckResponse("ATAL");   
            Thread.Sleep(1000);

            // Print spaces
            WriteAndCheckResponse("ATS1");   
            Thread.Sleep(1000);

            // Long messages
            WriteAndCheckResponse("ATSP0");   
            Thread.Sleep(1000);

            Port.NewLine = "\n";    // TODO: Doesn't seem like this would be needed

            // TODO: This is a hack - provides a wakeup something needed by the next command
            WriteAndCheckResponse("ATMA");   
            Thread.Sleep(1000);
        }

        public void WriteSerialLine(string line)
        {
            Port.Write(line + "\r");
        }

        public string WriteAndCheckResponse(string line)
        {
            String response;
            WriteSerialLine(line);
            response = Port.ReadLine();

            // TODO: Actually check the response for the strings: OK, ELM, SEARCHING, etc. Maybe
            // the desired response should be passed to this function and a boolean returned.
            return "";
        }
    }
}
