using AutoTestSystem.Model;

namespace LoadSeqJson
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Global.InitStation();
            Global.LoadSequnces();
            //Console.ReadKey();
        }
    }
}