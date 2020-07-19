using Parquet.Data;
using Parquet.Plus.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Parquet.Plus.Mapping.DifferentTypes;

namespace Parquet.Plus.Mapping
{
    /// <summary>
    /// Mapping config for properties
    /// </summary>
    /// <typeparam name="TModel">Type of model</typeparam>
    public class MapperConfig<TModel>
    {
        private readonly Dictionary<string, MapperPropertyConfig<TModel>> _mapDict;

        /// <summary>
        /// Mapping config for properties
        /// </summary>
        public MapperConfig()
        {
            _mapDict = new Dictionary<string, MapperPropertyConfig<TModel>>();
        }

        /// <summary>
        /// Adds property mapping info
        /// </summary>
        /// <typeparam name="TProperty">Type of property, also column</typeparam>
        /// <param name="propertySelector">Target property selector</param>
        /// <param name="columnName">Column name</param>
        /// <returns>Itself</returns>
        public MapperConfig<TModel> MapProperty<TProperty>(Expression<Func<TModel, TProperty>> propertySelector, string columnName)
        {
            var setter = propertySelector.BuildSetter();
            var getter = propertySelector.BuildGetter();

            var mapConfig = new MapperPropertyConfig<TModel, TProperty>(columnName, setter, getter);
            AddToMapper(mapConfig);

            return this;
        }

        /// <summary>
        /// Adds property mapping info with different types
        /// </summary>
        /// <typeparam name="TProperty">Type of property</typeparam>
        /// <typeparam name="TColumn">Type of column</typeparam>
        /// <param name="propertySelector">Target property selector</param>
        /// <param name="columnName">Column name</param>
        /// <param name="typesMapper">Types mapper</param>
        /// <returns>Itself</returns>
        public MapperConfig<TModel> MapProperty<TProperty, TColumn>(Expression<Func<TModel, TProperty>> propertySelector, string columnName, IDifferentTypesMapper<TProperty, TColumn> typesMapper)
        {
            if (typesMapper == null)
            {
                throw new ArgumentNullException(nameof(typesMapper));
            }

            return MapProperty(propertySelector, columnName, typesMapper.ToColumn, typesMapper.ToProperty);
        }

        /// <summary>
        /// Adds property mapping info with different types
        /// </summary>
        /// <typeparam name="TProperty">Type of property</typeparam>
        /// <typeparam name="TColumn">Type of column</typeparam>
        /// <param name="propertySelector">Target property selector</param>
        /// <param name="columnName">Column name</param>
        /// <param name="mapperToColumn">Mapper from property to column</param>
        /// <param name="mapperToProperty">Mapper from column to property</param>
        /// <returns>Itself</returns>
        public MapperConfig<TModel> MapProperty<TProperty, TColumn>(Expression<Func<TModel, TProperty>> propertySelector, string columnName, Func<TProperty, TColumn> mapperToColumn, Func<TColumn, TProperty> mapperToProperty)
        {
            Expression<Func<TProperty, TColumn>> mapperToColumnExp = x => mapperToColumn(x);
            Expression<Func<TColumn, TProperty>> mapperToPropertyExp = x => mapperToProperty(x);

            var setter = propertySelector.BuildSetter(mapperToPropertyExp);
            var getter = propertySelector.BuildGetter(mapperToColumnExp);

            var mapConfig = new MapperPropertyConfig<TModel, TColumn>(columnName, setter, getter);
            AddToMapper(mapConfig);

            return this;
        }

        /// <summary>
        /// Makes parquet columns from models
        /// </summary>
        /// <param name="models">Models with data</param>
        /// <returns>Parquet columns</returns>
        public DataColumn[] ToDataColumns(TModel[] models)
        {
            var dataColumns = new DataColumn[_mapDict.Count];

            var propertyConfigs = _mapDict.Select(s => s.Value).ToArray();
            for (int i = 0; i < propertyConfigs.Length; i++)
            {
                var propertyConfig = propertyConfigs[i];
                dataColumns[i] = propertyConfig.ToDataColumn(models);
            }

            return dataColumns;
        }

        /// <summary>
        /// Contains column name in configs
        /// </summary>
        /// <param name="columnName">column name</param>
        /// <returns>bool if contains</returns>
        public bool Contains(string columnName)
        {
            return _mapDict.ContainsKey(columnName.ToUpperInvariant());
        }

        /// <summary>
        /// Returns property configs
        /// </summary>
        /// <returns>Property configs</returns>
        public MapperPropertyConfig<TModel>[] GetPropertyConfigs()
        {
            return _mapDict.Select(s => s.Value).ToArray();
        }

        private void AddToMapper(MapperPropertyConfig<TModel> config)
        {
            if (_mapDict.ContainsKey(config.ColumnName))
            {
                throw new Exception("Property mapping already exists");
            }

            _mapDict.Add(config.ColumnName, config);
        }
    }
}
