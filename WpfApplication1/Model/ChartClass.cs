using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArdMeteo.Model
{
    public class ChartClass
    {
        public ChartClass(string name, int age)
        {
            this.Key = name;
            this.Value = age;
        }
        public string Key
        {
            get;
            set;
        }
        public double Value
        {
            get;
            set;
        }
    }
}
