using Newtonsoft.Json;
using StreamLabsSceneSwitcher.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;

namespace StreamLabsSceneSwitcher
{
    class Program
    {
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            var s = Configuration.Instance.SceneSwitches;

            Program p = new Program();
            p.Run(args);
        }

        private void Run(string[] args)
        {
            Console.WriteLine("Ctrl-C to exit!");

            while (true)
            {
                foreach (var sceneSwitch in Configuration.Instance.SceneSwitches)
                {

                    var processes = System.Diagnostics.Process.GetProcessesByName(sceneSwitch.ExecutableToMonitor);

                    if (processes.Length > 0 && sceneSwitch.IsUp == false)
                    {
                        Console.Write($"Scene Switch Detected for {sceneSwitch.ExecutableToMonitor}, Changing to: {sceneSwitch.SceneWhenUp}...");

                        sceneSwitch.IsUp = true;

                        SwitchSceneWithSlobsCommand(sceneSwitch.SceneWhenUp);

                        Console.WriteLine("Complete!");
                    }
                    else if (processes.Length == 0 && sceneSwitch.IsUp == true)
                    {
                        Console.Write($"Scene Switch Detected for {sceneSwitch.ExecutableToMonitor}, Changing to: {sceneSwitch.SceneWhenDown}...");

                        sceneSwitch.IsUp = false;

                        SwitchSceneWithSlobsCommand(sceneSwitch.SceneWhenDown);

                        Console.WriteLine("Complete!");
                    }
                }

                // One second polling interval.
                Thread.Sleep(1000);
            }
        }

        private void SwitchSceneWithSlobsCommand(string targetSceneName)
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "slobs", PipeDirection.InOut))
            {

                // Connect to the pipe or wait until the pipe is available.
                //Console.Write("Attempting to connect to pipe...");
                pipeClient.Connect();

                //Console.WriteLine("Connected to pipe.");
                //Console.WriteLine("There are currently {0} pipe server instances open.", pipeClient.NumberOfServerInstances);
                using (StreamReader sr = new StreamReader(pipeClient))
                {
                    using (StreamWriter sw = new StreamWriter(pipeClient))
                    {
                        SlobsRequest getScenesReqeust = new SlobsRequest()
                        {
                            method = "getScenes",
                            Params = new Params()
                            {
                                resource = "ScenesService"
                            }
                        };

                        string command = JsonConvert.SerializeObject(getScenesReqeust, Formatting.None); // .Replace("\n", "");
                    
                        sw.WriteLine(command);
                        sw.Flush();

                        var results = JsonConvert.DeserializeObject<GetScenesResult>(sr.ReadLine());

                        var targetSceneID = results.result.FirstOrDefault(p => p.name == targetSceneName).id;

                        SlobsRequest makeSceneActiveRequest = new SlobsRequest()
                        {
                            method = "makeSceneActive",
                            Params = new Params()
                            {
                                resource = "ScenesService",
                                args = new List<string>(new string[] { targetSceneID })
                            }
                        };

                        command = JsonConvert.SerializeObject(makeSceneActiveRequest, Formatting.None);

                        sw.WriteLine(command);
                        sw.Flush();

                        var makeSceneActiveResult = sr.ReadLine();
                    }
                }
            }
        }
    }
}
