using System;

namespace Parquet.Plus.Tests.Models
{
    public class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }

        public TestModel()
        {

        }

        public TestModel(int id, string name, double value)
        {
            Id = id;
            Name = name;
            Value = value;
        }

        public bool MyEquals(TestModel a)
        {
            return Id == a.Id
                   && Name == a.Name
                   && Math.Abs(Value - a.Value) < 0.001;
        }

        public override string ToString()
        {
            return $"{Id}, {Name}, {Value}";
        }
    }
}