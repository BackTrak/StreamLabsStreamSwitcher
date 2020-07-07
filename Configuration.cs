using System;
using System.Collections.Generic;
using System.Text;

namespace StreamLabsSceneSwitcher
{
    public class Configuration : JsonConfiguration<Configuration>
    {
        public List<SceneSwitch> SceneSwitches { get; set; }
    }

    public class SceneSwitch
    {
        public bool IsUp = false;

        public string ExecutableToMonitor { get; set; }
        public string SceneWhenUp { get; set; }
        public string SceneWhenDown { get; set; }
    }
}
