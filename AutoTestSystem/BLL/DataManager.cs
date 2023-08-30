using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTestSystem.BLL
{
    public class DataManager
    {

        private Dictionary<string, string> _data = new Dictionary<string, string>();

        private static readonly Lazy<DataManager> _instance = new Lazy<DataManager>(() => new DataManager());

        private DataManager()
        {


        }

        public static DataManager ShareInstance
        {
            get { return _instance.Value; }
        }

        public void SaveData(string key, string value) { 
            _data.Add(key, value);
        
        }
        public string GetData(string key) {
            if (_data.ContainsKey(key) == false) { 
              return "";
            }
            return _data[key]; 
        }

       public void ClearData() { 
           _data.Clear();
        }
    }
}
