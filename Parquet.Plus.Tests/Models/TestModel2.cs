using System;
using System.Collections.Generic;
using System.Text;

namespace Parquet.Plus.Tests.Models
{
    public class TestModel2
    {
        public string IdStr { get; set; }
        public int NameInt { get; set; }
        public long ValueLong { get; set; }

        public TestModel2()
        {

        }

        public TestModel2(string idStr, int nameInt, long valueLong)
        {
            IdStr = idStr;
            NameInt = nameInt;
            ValueLong = valueLong;
        }

        public bool MyEquals(TestModel2 a)
        {
            return IdStr == a.IdStr
                   && NameInt == a.NameInt
                   && ValueLong == a.ValueLong;
        }

        public override string ToString()
        {
            return $"{IdStr}, {NameInt}, {ValueLong}";
        }
    }
}