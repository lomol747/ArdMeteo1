
namespace ArdMeteo.Chart
{
    class DataItem
    {
        public DataItem (string name, int value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public int Value { get; set; }
    }
}
