using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArdMeteo.Model
{
    public class ChartTestModel : ObservableCollection<ChartVal>
    {
        public ChartTestModel() : base()
        {
            Add(new ChartVal("Ivan", 23));
            Add(new ChartVal("Stefan", 34));
            Add(new ChartVal("Maria", 16));
            Add(new ChartVal("Michael", 78));
        }
    }

    public class ChartVal
    {
        private string _key;
        private double _val;

        public ChartVal(string key, double val)
        {
            this.Key = key;
            this.Val = val;
        }

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public double Val
        {
            get { return _val; }
            set { _val = value; }
        }

        


        //public class ChartTestModel : ObservableCollection<KeyValuePair<string, int>>
        //{
        //    public ChartTestModel()
        //    {
        //        init();
        //    }
        //    public void init()
        //    {
        //        Add(new KeyValuePair<string, int>("Dog", 30));
        //        Add(new KeyValuePair<string, int>("Cat", 25));
        //        Add(new KeyValuePair<string, int>("Rat", 5));
        //        Add(new KeyValuePair<string, int>("Hampster", 8));
        //        Add(new KeyValuePair<string, int>("Rabbit", 12));
        //    }
        //    public ObservableCollection<KeyValuePair<string, int>> getData()
        //    {
        //        return this;
        //    }
        //}

    }
}


