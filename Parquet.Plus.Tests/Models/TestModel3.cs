using System;

namespace Parquet.Plus.Tests.Models
{
    public class TestModel3
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public bool BoolValue { get; set; }
        public DateTime DateValue { get; set; }

        public bool MyEquals(TestModel3 a)
        {
            return Id == a.Id
                   && Name == a.Name
                   && Math.Abs(Value - a.Value) < 0.001
                   && BoolValue == a.BoolValue
                   && DateValue == a.DateValue;
        }

        public override string ToString()
        {
            return $"{Id}, {Name}, {Value}, {BoolValue}, {DateValue}";
        }
    }
}
