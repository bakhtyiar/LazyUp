using System;
using System.Diagnostics;
using System.Timers;
using Timer = System.Timers.Timer;

class Program
{

    static void Main(string[] args)
    {

        // Имя программы, которую нужно проверить
        string processName = "LazyUp";

        void CheckAndStartProcess()
        {
            while (true)
            {
                int foundProcesses = Process.GetProcessesByName(processName).Length;
                // Проверяем, запущена ли программа
                if (foundProcesses < 1)
                {
                    // Программа не запущена, запускаем ее
                    Process process = Process.Start(processName + ".exe");
                    process.WaitForExit();
                }

                // Ждем перед следующей проверкой
                Thread.Sleep(1000);
            }
        }

        // Запускаем проверку в фоновом потоке
        Thread thread = new Thread(CheckAndStartProcess);
        thread.Start();
    }
}