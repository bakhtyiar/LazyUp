using System;
using System.Diagnostics;
using System.Timers;
using Timer = System.Threading.Timer;

class Program
{
    static void Main(string[] args)
    {
        Process process = new Process();
        Timer timer = new Timer(TimerElapse, null, 0, 1000);
        void TimerElapse(object? state)
        {
            Process[] processes = Process.GetProcessesByName("LazyUp");
            if (processes.Length == 0)
            {
                string exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString() + @"\LazyUp.exe";
                Process.Start(exePath);
            }
        }
    }
}