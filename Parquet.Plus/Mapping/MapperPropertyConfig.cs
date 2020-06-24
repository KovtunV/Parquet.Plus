using Parquet.Data;
using System;

namespace Parquet.Plus.Mapping
{
    internal abstract class MapperPropertyConfig<TModel>
    {
        public string ColumnName { get; }

        protected MapperPropertyConfig(string columnName)
        {
            ColumnName = columnName.ToUpperInvariant();
        }

        public abstract void Map(TModel[] models, DataColumn dataColumn, long modelOffset);
        public abstract DataColumn ToDataColumn(TModel[] models);
    }

    internal class MapperPropertyConfig<TModel, TColumn> : MapperPropertyConfig<TModel>
    {
        public Action<TModel, TColumn> SetterToProperty { get; }
        public Func<TModel, TColumn> GetterFromProperty { get; }

        public MapperPropertyConfig(string columnName, Action<TModel, TColumn> setterToProperty, Func<TModel, TColumn> getterFromProperty)
            : base(columnName)
        {
            SetterToProperty = setterToProperty;
            GetterFromProperty = getterFromProperty;
        }

        public override void Map(TModel[] models, DataColumn dataColumn, long modelOffset)
        {
            var allData = (TColumn[])dataColumn.Data;

            for (long i = 0; i < allData.LongLength; i++)
            {
                var model = models[i + modelOffset];
                var data = allData[i];

                SetterToProperty(model, data);
            }
        }

        public override DataColumn ToDataColumn(TModel[] models)
        {
            var dataField = new DataField<TColumn>(ColumnName);
            var data = GetData(models);
            var column = new DataColumn(dataField, data);

            return column;
        }

        private TColumn[] GetData(TModel[] models)
        {
            var data = new TColumn[models.LongLength];

            for (long i = 0; i < models.LongLength; i++)
            {
                data[i] = GetterFromProperty(models[i]);
            }

            return data;
        }
    }
}
