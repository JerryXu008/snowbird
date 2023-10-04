using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AutoTestSystem.Model
{
    public class Val
    {
        public int PDClassAltA { get; set; }
        public int PDClassAltB { get; set; }
        public string CurrentState { get; set; }
        public int PSEType { get; set; }
        public string PDStructure { get; set; }
        public int PowerLimit { get; set; }
        public int PowerReserved { get; set; }
        public int PowerConsumption { get; set; }
        public int CurrentConsumption { get; set; }
        public int Temperature { get; set; }
    }

    public class Result
    {
        public string key { get; set; }
        public Val val { get; set; }
    }

    public class Root
    {
        public int id { get; set; }
        public object error { get; set; }
        public List<Result> result { get; set; }
    }

}
