using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace SiSubs
{
    public static class Lpt
    {
        public enum LptPorts { Lpt1 = 0x378, Lpt2 = 0x278 }
        public static LptPorts Port;
        private const int TimeOut = 3;

        //[DllImport("inpout32.dll", EntryPoint = "Out32")]
        [DllImport("inpoutx64.dll", EntryPoint = "Out32")]
        private static extern void Output(int adress, int value);

        public static void Send(int match, int targetMatch)
        {            
            try
            {
                var sw = Stopwatch.StartNew();
                double d = 0;
                Output((int)Port, match);

                do
                {
                    d = Math.Sin(d);
                } while ((double)sw.ElapsedTicks * 1000 / Stopwatch.Frequency < TimeOut);

                //UnityEngine.Debug.Log("Success in LPT");

                Output((int)Port, 0);

                do
                {
                    d = Math.Sin(d);
                } while ((double)sw.ElapsedTicks * 1000 / Stopwatch.Frequency < TimeOut);

                var sw = Stopwatch.StartNew();
                double d = 0;
                Output((int)Port, targetMatch);

                do
                {
                    d = Math.Sin(d);
                } while ((double)sw.ElapsedTicks * 1000 / Stopwatch.Frequency < TimeOut);

                Output((int)Port, 0);

            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log("Exception in LPT " + ex);
            }            
        }


    }
}
