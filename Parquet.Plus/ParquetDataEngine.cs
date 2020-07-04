using System;
using Parquet.Data;
using Parquet.Plus.Mapping;
using System.IO;
using System.Linq;
using Parquet.Plus.Events;

namespace Parquet.Plus
{
    /// <summary>
    /// Works with parquet data
    /// </summary>
    public class ParquetDataEngine
    {
        /// <summary>
        /// Works with parquet data
        /// </summary>
        public ParquetDataEngine()
        {

        }

        #region Read

        /// <summary>
        /// Reads data from parquet file
        /// </summary>
        /// <typeparam name="TModel">Type of model</typeparam>
        /// <param name="mapConfig">Mapping configuration</param>
        /// <param name="filePath">Parquet file path</param>
        /// <returns>Parsed data</returns>
        public TModel[] Read<TModel>(MapperConfig<TModel> mapConfig, string filePath)
            where TModel : new()
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Read(mapConfig, fileStream);
        }

        /// <summary>
        /// Reads data from parquet stream
        /// </summary>
        /// <typeparam name="TModel">Type of model</typeparam>
        /// <param name="mapConfig">Mapping configuration</param>
        /// <param name="fileStream">Parquet stream</param>
        /// <returns>parsed data</returns>
        public TModel[] Read<TModel>(MapperConfig<TModel> mapConfig, Stream fileStream)
            where TModel : new()
        {
            using var parquetReader = new ParquetReader(fileStream);
            var dataFields = parquetReader.Schema.GetDataFields();

            long modelOffset = 0;
            var resArr = CreateArray<TModel>(parquetReader.ThriftMetadata.Num_rows);
            for (int i = 0; i < parquetReader.RowGroupCount; i++)
            {
                using var groupReader = parquetReader.OpenRowGroupReader(i);
                var columns = dataFields.Select(groupReader.ReadColumn).ToArray();

                ReadColumns(mapConfig, resArr, columns, modelOffset);

                // increment offset to read next rowGroup
                modelOffset += groupReader.RowCount;
            }

            return resArr;
        }

        private void ReadColumns<TModel>(MapperConfig<TModel> mapConfig, TModel[] models, DataColumn[] columns, long modelOffset)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                var column = columns[i];
                
                if (!mapConfig.TryMapFromColumn(models, column, modelOffset))
                {
                    OnInvalidColumn($"I have no idea how to map the column \"{column.Field.Name}\"");
                }
            }
        }

        private TModel[] CreateArray<TModel>(long length)
            where TModel : new()
        {
            var arr = new TModel[length];

            for (long i = 0; i < length; i++)
            {
                arr[i] = new TModel();
            }

            return arr;
        }

        #endregion

        #region Write

        /// <summary>
        /// Writes data to parquet file
        /// </summary>
        /// <typeparam name="TModel">Type of model</typeparam>
        /// <param name="mapConfig">Mapping configuration</param>
        /// <param name="filePath">Parquet file path</param>
        /// <param name="models">Models</param>
        public void Write<TModel>(MapperConfig<TModel> mapConfig, string filePath, params TModel[] models)
        {
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write);
            Write(mapConfig, fileStream, models);
        }

        /// <summary>
        /// Writes data to parquet stream
        /// </summary>
        /// <typeparam name="TModel">Type of model</typeparam>
        /// <param name="mapConfig">Mapping configuration</param>
        /// <param name="fileStream">Parquet stream</param>
        /// <param name="models">Models</param>
        public void Write<TModel>(MapperConfig<TModel> mapConfig, Stream fileStream, params TModel[] models)
        {
            OnWrite(mapConfig, fileStream, isAppend: false, models);
        }

        private void OnWrite<TModel>(MapperConfig<TModel> mapConfig, Stream fileStream, bool isAppend, params TModel[] models)
        {
            var dataColumns = mapConfig.ToDataColumns(models);
            var columnDataFields = dataColumns.Select(s => s.Field).ToArray();

            var schema = new Schema(columnDataFields);
            using var parquetWriter = new ParquetWriter(schema, fileStream, append: isAppend);
            using var groupWriter = parquetWriter.CreateRowGroup();

            for (int i = 0; i < dataColumns.Length; i++)
            {
                groupWriter.WriteColumn(dataColumns[i]);
            }
        }

        #endregion

        #region Append

        /// <summary>
        /// Appends data to parquet file
        /// </summary>
        /// <typeparam name="TModel">Type of model</typeparam>
        /// <param name="mapConfig">Mapping configuration</param>
        /// <param name="filePath">Parquet file path</param>
        /// <param name="models">Models</param>
        public void Append<TModel>(MapperConfig<TModel> mapConfig, string filePath, params TModel[] models)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            Append(mapConfig, fileStream, models);
        }

        /// <summary>
        /// Appends data to parquet stream
        /// </summary>
        /// <typeparam name="TModel">Type of model</typeparam>
        /// <param name="mapConfig">Mapping configuration</param>
        /// <param name="fileStream">Parquet stream</param>
        /// <param name="models">Models</param>
        public void Append<TModel>(MapperConfig<TModel> mapConfig, Stream fileStream, params TModel[] models)
        {
            OnWrite(mapConfig, fileStream, isAppend: true, models);
        }

        #endregion

        #region Events

        /// <summary>
        /// Event when I don't have the column map info
        /// </summary>
        public event EventHandler<InvalidColumnEventArgs> InvalidColumn;

        private void OnInvalidColumn(string message)
        {
            var args = new InvalidColumnEventArgs(message);
            InvalidColumn?.Invoke(this, args);
        }

        #endregion
    }
}
