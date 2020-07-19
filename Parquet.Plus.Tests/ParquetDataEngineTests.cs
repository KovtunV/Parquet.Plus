using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using Parquet.Plus.Mapping;
using Parquet.Plus.Mapping.DifferentTypes;
using Parquet.Plus.Tests.Models;

namespace Parquet.Plus.Tests
{
    [TestClass]
    public class ParquetDataEngineTests
    {
        private ParquetDataEngine _dataEngine;
        private MapperConfig<TestModel> _mapConfig1;
        private MapperConfig<TestModel2> _mapConfig2;
        private MapperConfig<TestModel3> _mapConfig3;

        [TestInitialize]
        public void Startup()
        {
            _dataEngine = new ParquetDataEngine();
            _mapConfig1 = CreateMappConfig();
            _mapConfig2 = CreateMappConfig2();
            _mapConfig3 = CreateMappConfig3();
        }

        [DataTestMethod]
        [DataRow(10)]
        public void WriteReadTest(int count)
        {
            var data = CreateModels(count);

            // write
            using var stream = new MemoryStream();
            _dataEngine.Write(_mapConfig1, stream, data);

            // read data
            var readedData = _dataEngine.Read(_mapConfig1, stream);

            // check
            Assert.AreEqual(data.Length, readedData.Length, "Arrays are different");

            for (int i = 0; i < data.Length; i++)
            {
                Assert.IsTrue(data[i].MyEquals(readedData[i]), $"Data with id \"{data[i].Id}\" aren't equal");
            }
        }


        [DataTestMethod]
        [DataRow(10)]
        public void AppendReadTest(int count)
        {
            var data1 = CreateModels(count);
            var data2 = CreateModels2(count);

            // write
            using var stream = new MemoryStream();
            _dataEngine.Write(_mapConfig1, stream, data1);
            _dataEngine.Append(_mapConfig2, stream, data2);

            // read data
            var readedData1 = _dataEngine.Read(_mapConfig1, stream);
            var readedData2 = _dataEngine.Read(_mapConfig2, stream);

            // check
            for (int i = 0; i < data1.Length; i++)
            {
                Assert.IsTrue(data1[i].MyEquals(readedData1[i]), $"Data with id \"{data1[i].Id}\" aren't equal");
            }

            for (int i = 0; i < data2.Length; i++)
            {
                Assert.IsTrue(data2[i].MyEquals(readedData2[i + count]), $"Data with id \"{data2[i].IdStr}\" aren't equal");
            }
        }

        [DataTestMethod]
        [DataRow(10)]
        public void ReadDifferentTypesTest(int count)
        {
            var data = CreateModels(count);

            // write model1
            using var stream = new MemoryStream();
            _dataEngine.Write(_mapConfig1, stream, data);

            // read data to model2
            var readedData = _dataEngine.Read(_mapConfig2, stream);

            // check
            Assert.AreEqual(data.Length, readedData.Length, "Arrays are different");
        }

        [DataTestMethod]
        [DataRow(10)]
        public void WriteDifferentTypesTest(int count)
        {
            var data = CreateModels2(count);

            // write
            using var stream = new MemoryStream();
            _dataEngine.Write(_mapConfig2, stream, data);

            // read data
            var readedData = _dataEngine.Read(_mapConfig2, stream);

            // check
            Assert.AreEqual(data.Length, readedData.Length, "Arrays are different");

            for (int i = 0; i < data.Length; i++)
            {
                Assert.IsTrue(data[i].MyEquals(readedData[i]), $"Data with id \"{data[i].IdStr}\" aren't equal");
            }
        }

        [DataTestMethod]
        [DataRow(10)]
        public void InvalidColumn(int count)
        {
            var data = CreateModels(count);

            // write
            using var stream = new MemoryStream();
            _dataEngine.Write(_mapConfig1, stream, data);

            // config with wrong property map
            var partConfig = new MapperConfig<TestModel>()
                    .MapProperty(x => x.Value, "VALUE_NO1")
                    .MapProperty(x => x.Value, "VALUE_NO2")
                    .MapProperty(x => x.Value, "VALUE");

            // read data
            var invalidCount = 0;
            _dataEngine.InvalidColumn += (s, e) => invalidCount++;
            var readedData = _dataEngine.Read(partConfig, stream);

            // check
            Assert.AreEqual(2, invalidCount);
        }

        [DataTestMethod]
        [DataRow(10)]
        public void DefaultMappersCheck(int count)
        {
            var data = CreateModels3(count);

           // write
            using var stream = new MemoryStream();
            _dataEngine.Write(_mapConfig3, stream, data);

            // read data
            var readedData = _dataEngine.Read(_mapConfig3, stream);

            // check
            Assert.AreEqual(data.Length, readedData.Length, "Arrays are different");

            for (int i = 0; i < data.Length; i++)
            {
                Assert.IsTrue(data[i].MyEquals(readedData[i]), $"Data with id \"{data[i].Id}\" aren't equal");
            }
        }

        #region Utils

        private MapperConfig<TestModel> CreateMappConfig()
        {
            return new MapperConfig<TestModel>()
                .MapProperty(x => x.Id, "ID")
                .MapProperty(x => x.Name, "NAME")
                .MapProperty(x => x.Value, "VALUE");
        }

        private MapperConfig<TestModel2> CreateMappConfig2()
        {
            return new MapperConfig<TestModel2>()
                .MapProperty(x => x.IdStr, "ID", x => int.Parse(x.Split()[0]), x => x + " modified")
                .MapProperty(x => x.NameInt, "NAME", IntToStr, StrToInt)
                .MapProperty(x => x.ValueLong, "VALUE", x => (double)x, x => (long)x);
        }

        private int StrToInt(string x)
        {
            return int.Parse(x.Split()[2]);
        }

        private string IntToStr(int x)
        {
            return $"Name is {x}";
        }

        private MapperConfig<TestModel3> CreateMappConfig3()
        {
            return new MapperConfig<TestModel3>()
                .MapProperty(x => x.Id, "ID")
                .MapProperty(x => x.Name, "NAME")
                .MapProperty(x => x.Value, "VALUE")
                .MapProperty(x => x.DateValue, "DATE", DefaultMappers.DateTimeOffsetToDateTime)
                .MapProperty(x => x.BoolValue, "BOOL_VAL", DefaultMappers.IntToBool);
        }

        private TestModel[] CreateModels(int count)
        {
            return Enumerable.Range(1, count).Select(i => new TestModel(i, $"Name is {i}", i * 1.56)).ToArray();
        }

        private TestModel2[] CreateModels2(int count)
        {
            return Enumerable.Range(1, count).Select(i => new TestModel2(i + " modified", i.ToString().Length, i * 12)).ToArray();
        }

        private TestModel3[] CreateModels3(int count)
        {
            var now = DateTime.Now;

            return Enumerable.Range(1, count).Select(i => new TestModel3
            {
                Id = i,
                Name = $"Name is {i}",
                Value = i * 1.56,
                BoolValue = i % 2 == 0,
                DateValue = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond).AddMinutes(i)
            }).ToArray();
        }

        #endregion
    }
}