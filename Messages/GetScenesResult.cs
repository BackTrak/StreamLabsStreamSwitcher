using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLabsSceneSwitcher.Messages
{
    public class Position
    {
        public double x { get; set; }
        public double y { get; set; }
    }

    public class Scale
    {
        public double x { get; set; }
        public double y { get; set; }
    }

    public class Crop
    {
        public int top { get; set; }
        public int bottom { get; set; }
        public int left { get; set; }
        public int right { get; set; }
    }

    public class Transform
    {
        public Position position { get; set; }
        public Scale scale { get; set; }
        public Crop crop { get; set; }
        public int rotation { get; set; }
    }

    public class Node
    {
        public string id { get; set; }
        public string sceneId { get; set; }
        public string sceneNodeType { get; set; }
        public string parentId { get; set; }
        public string sourceId { get; set; }
        public string sceneItemId { get; set; }
        public string name { get; set; }
        public string resourceId { get; set; }
        public Transform transform { get; set; }
        public bool visible { get; set; }
        public bool locked { get; set; }
        public bool streamVisible { get; set; }
        public bool recordingVisible { get; set; }
    }

    public class Result
    {
        public string _type { get; set; }
        public string resourceId { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public IList<Node> nodes { get; set; }
    }

    public class GetScenesResult
    {
        public int id { get; set; }
        public IList<Result> result { get; set; }
        public string jsonrpc { get; set; }
    }
}
