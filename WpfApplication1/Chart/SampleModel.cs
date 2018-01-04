using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArdMeteo.Chart
{
    public class SampleModel
    {
        private ObservableCollection<DataItem> _collection;

        private ObservableCollection<DataItem> Data
        {
            get
            {
                if (_collection == null)
                {
                    _collection = new ObservableCollection<DataItem>
                    {
                        new DataItem("First", 70),
                        new DataItem("Second", 20),
                        new DataItem("Thrid", 10)
                    };
                }
                return _collection;
            }
        }



    }
}
