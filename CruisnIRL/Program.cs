using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using vJoyInterfaceWrap;

namespace CruisnIRL
{
    class Program
    {
        public static vJoy Joystick;
        public static vJoy.JoystickState JoyState;
        public static uint JoystickID = 1;

        public static SteeringDevice SteeringDevice;
        public static PedalDevice PedalDevice;

        static void Main(string[] args)
        {
            if(!ConfigureJoystick())
            {
                return;
            }

            SteeringDevice = new SteeringDevice("COM4", 115200);
            PedalDevice = new PedalDevice("COM5", 115200);

            while (true)
            {
                SetSteeringAngle();
                // TODO: The following line causes steering angle to not be recognized by the gamepad device.
                // Everything works fine with PedalDevice up and functional, but actually collecting the data and
                // setting a joystick value causes a problem. Maybe swapping SetSteeringAngle and SetPedalAngle
                // will yield unique results?
                SetPedalAngle();
            }

        }

        static void SetSteeringAngle()
        {
            int steeringAngle = SteeringDevice.GetSteeringAngle();
            Joystick.SetAxis(steeringAngle, JoystickID, HID_USAGES.HID_USAGE_X);
        }

        static void SetPedalAngle()
        {
            int brakePressure = PedalDevice.GetBrakePressure();
            if (brakePressure > 5)
            {
                Joystick.SetBtn(true, JoystickID, 1);
            } else
            {
                Joystick.SetBtn(false, JoystickID, 1);
            }
        }

        static bool ConfigureJoystick()
        {
            Joystick = new vJoy();
            JoyState = new vJoy.JoystickState();

            // Get the driver attributes (Vendor ID, Product ID, Version Number)
            if (!Joystick.vJoyEnabled())
            {
                Console.WriteLine("vJoy driver not enabled: Failed Getting vJoy attributes.\n");
                return false;
            }

            VjdStat status = Joystick.GetVJDStatus(JoystickID);
            switch (status)
            {
                case VjdStat.VJD_STAT_OWN:
                    Console.WriteLine("vJoy Device {0} is already owned by this feeder\n", JoystickID);
                    break;
                case VjdStat.VJD_STAT_FREE:
                    Console.WriteLine("vJoy Device {0} is free\n", JoystickID);
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    Console.WriteLine("vJoy Device {0} is already owned by another feeder\nCannot continue\n", JoystickID);
                    return false;
                case VjdStat.VJD_STAT_MISS:
                    Console.WriteLine("vJoy Device {0} is not installed or disabled\nCannot continue\n", JoystickID);
                    return false;
                default:
                    Console.WriteLine("vJoy Device {0} general error\nCannot continue\n", JoystickID);
                    return false;
            };

            // Test if DLL matches the driver
            UInt32 DllVer = 0, DrvVer = 0;
            bool match = Joystick.DriverMatch(ref DllVer, ref DrvVer);
            if (!match)
            {
                Console.WriteLine("WARNING! Version of Driver ({0:X}) does NOT match vJoyInterface DLL Version ({1:X})\n", DrvVer, DllVer);
                //return false;
            }

            // Acquire the target
            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!Joystick.AcquireVJD(JoystickID))))
            {
                Console.WriteLine("Failed to acquire vJoy device number {0}.\n", JoystickID);
                return false;
            }

            // Reset this device to default values
            Joystick.ResetVJD(JoystickID);

            Console.WriteLine("Joystick configured successfully");

            return true;
        }
    }
}
