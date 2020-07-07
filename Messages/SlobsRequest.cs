using System.Collections.Generic;

namespace StreamLabsSceneSwitcher.Messages
{
    public class Params
    {
        public string resource { get; set; }
        public List<string> args { get; set; } = new List<string>();
    }

    public class SlobsRequest
    {
        public string jsonrpc { get; set; } = "2.0";
        public int id { get; set; } = 1;
        public string method { get; set; }

        [Newtonsoft.Json.JsonProperty("params")]
        public Params Params { get; set; }
    }
}
