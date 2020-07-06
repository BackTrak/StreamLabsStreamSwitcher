using Newtonsoft.Json;
using StreamLabsSceneSwitcher.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StreamLabsSceneSwitcher
{
    class Program
    {
        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(
                                        IntPtr hWnd,
                                        int Msg,
                                        int wParam,
                                        [MarshalAs(UnmanagedType.LPStr)] string lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);


        private bool _controlKeyDown = false;
        private bool _shiftKeyDown = false;
        private int _currentLine = 0;

        [STAThreadAttribute]
        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run(args);
        }

        private void Run(string[] args)
        {
            Console.WriteLine("Ctrl-C to exit!");

            bool hasLeagueClientBeenDetected = false;
            while (true)
            {
                var processes = System.Diagnostics.Process.GetProcessesByName("League of Legends");
                //var processes = System.Diagnostics.Process.GetProcessesByName("Notepad");

                if (processes.Length > 0 && hasLeagueClientBeenDetected == false)
                {
                    Console.Write("Scene Switch Detected, Changing to In Game scene... ");

                    hasLeagueClientBeenDetected = true;

                    SwitchSceneWithSlobsCommand("In Game");

                    //for (int i = 0; i < 20; i++)
                    //{
                    //    KeySender.SendKeyPressToActiveApplication(Keys.Left);
                    //    Thread.Sleep(250);
                    //}

                    Console.WriteLine("Complete!");
                }
                else if (processes.Length == 0 && hasLeagueClientBeenDetected == true)
                {
                    Console.Write("Scene Switch Detected, Changing to Lobby scene... ");

                    hasLeagueClientBeenDetected = false;

                    SwitchSceneWithSlobsCommand("Main");

                    //for (int i = 0; i < 20; i++)
                    //{
                    //    KeySender.SendKeyPressToActiveApplication(Keys.Right);
                    //    Thread.Sleep(250);
                    //}

                    Console.WriteLine("Complete!");
                }


                Thread.Sleep(100);
            }
            //});

            //Application.Run();




            //while (Console.ReadKey().Key != ConsoleKey.X)
            //    continue;
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

        private void Hook_KeyPress(object sender, KeyPressEventArgs e)
        {
            Console.WriteLine("press: " + e.KeyChar.ToString());
        }

        private void Hook_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            Console.WriteLine("up: " + e.KeyCode.ToString());

            if (e.KeyCode == System.Windows.Forms.Keys.Scroll)
            {
                e.Handled = true;

                Task.Run(() =>
                {
                    //_currentLine = 0;
                    // SendTheFingerToAll();

                    //bool wasControlKeyDown = _controlKeyDown;
                    //if (wasControlKeyDown == true)
                    //    KeySender.SendKeyUpToActiveApplication(Keys.LControlKey);

                    KeySender.SendKeyPressToActiveApplication(Keys.Left);

                    //if (wasControlKeyDown == true)
                    //    KeySender.SendKeyDownToActiveApplication(Keys.LControlKey);
                });
            }

            //if (e.KeyCode == System.Windows.Forms.Keys.V)
            //{
            //    if (_currentLine > 0)
            //    {
            //        Task.Run(() =>
            //        {
            //            bool wasControlKeyDown = _controlKeyDown;
            //            //if (wasControlKeyDown == true)
            //            //    KeySender.SendKeyUpToActiveApplication(Keys.LControlKey);

            //            //KeySender.SendKeyPressToActiveApplication(Keys.Return);
            //            SendTheFingerToAll();
            //            //KeySender.SendKeyPressToActiveApplication(Keys.Return);

            //            //if (wasControlKeyDown == true)
            //            //    KeySender.SendKeyDownToActiveApplication(Keys.LControlKey);
            //        });
            //    }
            //}

            //if (e.KeyCode == System.Windows.Forms.Keys.LControlKey)
            //{
            //    _controlKeyDown = false;
            //    //e.Handled = true;
            //}

            //if (e.KeyCode == System.Windows.Forms.Keys.LShiftKey)
            //{
            //    _shiftKeyDown = false;
            //    //e.Handled = true;
            //}

            //ShowState();
        }

        private void Hook_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            Console.WriteLine("down: " + e.KeyCode.ToString());

            //if (e.KeyCode == Keys.Subtract)
            //    SendTheFingerToAll();

            //if (e.KeyCode == Keys.Add)
            //    SendTheFingerToAll();

            //Console.WriteLine(e.KeyCode.ToString());

            //if (e.KeyCode == System.Windows.Forms.Keys.LControlKey)
            //{
            //    _controlKeyDown = true;
            //    //e.Handled = true;
            //}

            //if (e.KeyCode == System.Windows.Forms.Keys.LShiftKey)
            //{
            //    _shiftKeyDown = true;
            //    //e.Handled = true;
            //}

            //if (e.KeyCode == Keys.F12)
            //    SendTheFingerToAll();

            //ShowState();
        }

        private void SendTheFingerToAll()
        {
            Console.WriteLine("The Finger!");

            //Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

            //Thread.Sleep(250);

            //KeySender.SendKeyPressToActiveApplication(Keys.Enter);

            //( ︶︿︶)_╭∩╮

            string lines = @"
/all               _             
/all             /_  /)          
/all            /    /           
/all        _  /    /__            
/all      /  `/'  '/   `\ _
/all   /'/   /    /    _ / `\
/all  ('(    `   `     ~/'  ')
/all   \              '     /
/all    \    \          _ /
/all     \              (
/all      \              \
";


            int keyDelay = 10;

            //var lineArray = lines.Split('\n');
            //for (; _currentLine < lineArray.Length && String.IsNullOrWhiteSpace(lineArray[_currentLine]) == true; _currentLine++) ;

            //if (_currentLine >= lineArray.Length)
            //{
            //    _currentLine = 0;
            //    SetClipboard(String.Empty);
            //    return;
            //}

            //var line = lineArray[_currentLine];
            //_currentLine++;

            //if (String.IsNullOrWhiteSpace(line) == true)
            //{
            //    return;
            //}

            //char replaceChar = '.';

            //line = line.Trim();
            //line = line.PadRight(35, replaceChar);
            //line = line.Replace(' ', replaceChar);
            //line = line.Replace("/all" + replaceChar, "/all ");
            ////line += Environment.NewLine;

            //SetClipboard(line);





            //Thread thread = new Thread(() =>
            //{
            //    foreach (var line in lines.Split('\n'))
            //    {
            //        if (String.IsNullOrWhiteSpace(line) == true)
            //            continue;

            //        Clipboard.SetText(line, TextDataFormat.UnicodeText);
            //        var current = Clipboard.GetText(TextDataFormat.UnicodeText);
            //        Console.WriteLine(current);

            //        KeySender.SendKeyPressToActiveApplication(Keys.Enter);
            //        Thread.Sleep(keyDelay);
            //        KeySender.SendKeyDownToActiveApplication(Keys.LControlKey);
            //        Thread.Sleep(keyDelay);
            //        KeySender.SendKeyPressToActiveApplication(Keys.V);
            //        Thread.Sleep(keyDelay);
            //        KeySender.SendKeyUpToActiveApplication(Keys.LControlKey);
            //        Thread.Sleep(keyDelay);
            //        KeySender.SendKeyPressToActiveApplication(Keys.Enter);
            //        Thread.Sleep(keyDelay);
            //    }
            //});

            //thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            //thread.Start();
            //thread.Join();



            Queue<KeyQueueItem> outputChars = new Queue<KeyQueueItem>(); ;

            foreach (var splitLine in lines.Split('\n'))
            {
                if (String.IsNullOrWhiteSpace(splitLine) == true)
                    continue;

                char replaceChar = '.';

                string line = splitLine.Trim();
                line = line.PadRight(35, replaceChar);
                line = line.Replace(' ', replaceChar);
                line = line.Replace("/all" + replaceChar, "/all ");
                //line += Environment.NewLine;

                outputChars.Enqueue(new KeyQueueItem(Keys.Return, KeyAction.KeyPress, keyDelay));

                for (int i = 0; i < line.Length; i++)
                {
                    switch (line[i])
                    {
                        case ' ':
                            if (i < 5)
                                outputChars.Enqueue(new KeyQueueItem(Keys.Space, KeyAction.KeyPress, keyDelay));
                            else
                            {
                                //outputChars.Enqueue(new KeyQueueItem(Keys.LShiftKey, KeyAction.KeyDown, 0));
                                outputChars.Enqueue(new KeyQueueItem(Keys.OemPeriod, KeyAction.KeyPress, keyDelay));
                                //outputChars.Enqueue(new KeyQueueItem(Keys.LShiftKey, KeyAction.KeyUp, 0));
                            }
                            break;

                        case ')':
                            outputChars.Enqueue(new KeyQueueItem(Keys.LShiftKey, KeyAction.KeyDown, 0));
                            outputChars.Enqueue(new KeyQueueItem(Keys.D0, KeyAction.KeyPress, keyDelay));
                            outputChars.Enqueue(new KeyQueueItem(Keys.LShiftKey, KeyAction.KeyUp, 0));
                            break;

                        case '(':
                            outputChars.Enqueue(new KeyQueueItem(Keys.LShiftKey, KeyAction.KeyDown, 0));
                            outputChars.Enqueue(new KeyQueueItem(Keys.D9, KeyAction.KeyPress, keyDelay));
                            outputChars.Enqueue(new KeyQueueItem(Keys.LShiftKey, KeyAction.KeyUp, 0));
                            break;

                        case '~':
                            outputChars.Enqueue(new KeyQueueItem(Keys.LShiftKey, KeyAction.KeyDown, 0));
                            outputChars.Enqueue(new KeyQueueItem(Keys.Oemtilde, KeyAction.KeyPress, keyDelay));
                            outputChars.Enqueue(new KeyQueueItem(Keys.LShiftKey, KeyAction.KeyUp, 0));
                            break;

                        case '.':
                            outputChars.Enqueue(new KeyQueueItem(Keys.OemPeriod, KeyAction.KeyPress, keyDelay));
                            break;

                        case '_':
                            outputChars.Enqueue(new KeyQueueItem(Keys.LShiftKey, KeyAction.KeyDown, 0));
                            outputChars.Enqueue(new KeyQueueItem(Keys.OemMinus, KeyAction.KeyPress, keyDelay));
                            outputChars.Enqueue(new KeyQueueItem(Keys.LShiftKey, KeyAction.KeyUp, 0));
                            break;

                        case '`':
                            outputChars.Enqueue(new KeyQueueItem(Keys.Oemtilde, KeyAction.KeyPress, keyDelay));
                            break;

                        case '\'':
                            outputChars.Enqueue(new KeyQueueItem(Keys.Oem7, KeyAction.KeyPress, keyDelay));
                            break;

                        case '\n':
                            outputChars.Enqueue(new KeyQueueItem(Keys.Return, KeyAction.KeyPress, keyDelay));
                            break;

                        case '/':
                            outputChars.Enqueue(new KeyQueueItem(Keys.OemQuestion, KeyAction.KeyPress, keyDelay));
                            break;

                        case '\\':
                            outputChars.Enqueue(new KeyQueueItem(Keys.OemPipe, KeyAction.KeyPress, keyDelay));
                            break;

                        case 'a':
                            outputChars.Enqueue(new KeyQueueItem(Keys.A, KeyAction.KeyPress, keyDelay));
                            break;

                        case 'l':
                            outputChars.Enqueue(new KeyQueueItem(Keys.L, KeyAction.KeyPress, keyDelay));
                            break;

                        default:
                            outputChars.Enqueue(new KeyQueueItem(Keys.X, KeyAction.KeyPress, keyDelay));
                            break;

                            //outputChars.Enqueue(new KeyQueueItem(Keys.LMenu, KeyAction.KeyDown, 0));

                            //foreach (char c in ((int)(char)line[i]).ToString("D4"))
                            //{
                            //    switch (c)
                            //    {
                            //        case '0':
                            //            outputChars.Enqueue(new KeyQueueItem(Keys.NumPad0, KeyAction.KeyPress, keyDelay));
                            //            break;

                            //        case '1':
                            //            outputChars.Enqueue(new KeyQueueItem(Keys.NumPad1, KeyAction.KeyPress, keyDelay));
                            //            break;

                            //        case '2':
                            //            outputChars.Enqueue(new KeyQueueItem(Keys.NumPad2, KeyAction.KeyPress, keyDelay));
                            //            break;

                            //        case '3':
                            //            outputChars.Enqueue(new KeyQueueItem(Keys.NumPad3, KeyAction.KeyPress, keyDelay));
                            //            break;

                            //        case '4':
                            //            outputChars.Enqueue(new KeyQueueItem(Keys.NumPad4, KeyAction.KeyPress, keyDelay));
                            //            break;

                            //        case '5':
                            //            outputChars.Enqueue(new KeyQueueItem(Keys.NumPad5, KeyAction.KeyPress, keyDelay));
                            //            break;

                            //        case '6':
                            //            outputChars.Enqueue(new KeyQueueItem(Keys.NumPad6, KeyAction.KeyPress, keyDelay));
                            //            break;

                            //        case '7':
                            //            outputChars.Enqueue(new KeyQueueItem(Keys.NumPad7, KeyAction.KeyPress, keyDelay));
                            //            break;

                            //        case '8':
                            //            outputChars.Enqueue(new KeyQueueItem(Keys.NumPad8, KeyAction.KeyPress, keyDelay));
                            //            break;

                            //        case '9':
                            //            outputChars.Enqueue(new KeyQueueItem(Keys.NumPad9, KeyAction.KeyPress, keyDelay));
                            //            break;

                            //            //case '0':
                            //            //    outputChars.Enqueue(new KeyQueueItem(Keys.Insert, KeyAction.KeyPress, keyDelay));
                            //            //    break;

                            //            //case '1':
                            //            //    outputChars.Enqueue(new KeyQueueItem(Keys.End, KeyAction.KeyPress, keyDelay));
                            //            //    break;

                            //            //case '2':
                            //            //    outputChars.Enqueue(new KeyQueueItem(Keys.Down, KeyAction.KeyPress, keyDelay));
                            //            //    break;

                            //            //case '3':
                            //            //    outputChars.Enqueue(new KeyQueueItem(Keys.Next, KeyAction.KeyPress, keyDelay));
                            //            //    break;

                            //            //case '4':
                            //            //    outputChars.Enqueue(new KeyQueueItem(Keys.Left, KeyAction.KeyPress, keyDelay));
                            //            //    break;

                            //            //case '5':
                            //            //    outputChars.Enqueue(new KeyQueueItem(Keys.Clear, KeyAction.KeyPress, keyDelay));
                            //            //    break;

                            //            //case '6':
                            //            //    outputChars.Enqueue(new KeyQueueItem(Keys.Right, KeyAction.KeyPress, keyDelay));
                            //            //    break;

                            //            //case '7':
                            //            //    outputChars.Enqueue(new KeyQueueItem(Keys.Home, KeyAction.KeyPress, keyDelay));
                            //            //    break;

                            //            //case '8':
                            //            //    outputChars.Enqueue(new KeyQueueItem(Keys.Up, KeyAction.KeyPress, keyDelay));
                            //            //    break;

                            //            //case '9':
                            //            //    outputChars.Enqueue(new KeyQueueItem(Keys.PageUp, KeyAction.KeyPress, keyDelay));
                            //            //    break;

                            //    }

                            //int value = Int32.Parse(c.ToString());
                            //outputChars.Enqueue(new KeyQueueItem(Keys.NumPad0 + value, KeyAction.KeyPress, keyDelay));
                   // }

                           // outputChars.Enqueue(new KeyQueueItem(Keys.LMenu, KeyAction.KeyUp, 0));
                            //break;
                    }
                }

                outputChars.Enqueue(new KeyQueueItem(Keys.Return, KeyAction.KeyPress, keyDelay));

                //Console.Write(outputChar.ToString());

                while (outputChars.Count > 0)
                {
                    var nextKey = outputChars.Dequeue();
                    switch (nextKey.KeyAction)
                    {
                        case KeyAction.KeyPress:
                            KeySender.SendKeyPressToActiveApplication(nextKey.Key);
                            break;

                        case KeyAction.KeyUp:
                            KeySender.SendKeyUpToActiveApplication(nextKey.Key);
                            break;

                        case KeyAction.KeyDown:
                            KeySender.SendKeyDownToActiveApplication(nextKey.Key);
                            break;


                        default:
                            throw new NotSupportedException(nextKey.KeyAction.ToString());
                    }

                    Thread.Sleep(nextKey.MillisecondsToPause);
                }
            }
        }

        private void SetClipboard(string line)
        {
            Thread thread = new Thread(() =>
            {
                if (String.IsNullOrWhiteSpace(line) == true)
                    Clipboard.Clear();
                else
                    Clipboard.SetText(line, TextDataFormat.UnicodeText);
            });

            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join();
        }

        private void ShowState()
        {
            Console.WriteLine($"{DateTime.Now}: {_controlKeyDown}, {_shiftKeyDown}");
        }
    }
}
