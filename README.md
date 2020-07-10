# Parquet.Plus 

[![Build Status](https://travis-ci.org/KovtunV/Parquet.Plus.svg?branch=master)](https://travis-ci.org/KovtunV/Parquet.Plus)

Provides reading and writing Parquet file 

<!--ts-->
   * [Mapping configuration](#Mapping-configuration)
      * [Direct mapping](#Direct-mapping)
      * [Mapping with different types](#Mapping-with-different-types)
      * [Mapping from an interface](#Mapping-from-an-interface)
   * [Reading](#Reading)
   * [Writing](#Writing)
   * [Appending](#Appending)
<!--te-->

## Mapping configuration
First of all we need to make mapping configuration

### Direct mapping
If we have the same types of data between columns and properties

```csharp
var mapConfig = new MapperConfig<TestModel>()
      .MapProperty(x => x.Id, "ID")
      .MapProperty(x => x.Name, "NAME")
      .MapProperty(x => x.Value, "VALUE");
```
### Mapping with different types
If we have different types between columns and properties
```csharp
var mapConfig = new MapperConfig<TestModel2>()
      .MapProperty(x => x.IdStr, "ID", x => int.Parse(x.Split()[0]), x => x + " modified")
      .MapProperty(x => x.NameInt, "NAME", IntToStr, StrToInt)
      .MapProperty(x => x.Value, "VALUE");

private int StrToInt(string x)
{
    return int.Parse(x.Split()[2]);
}

private string IntToStr(int x)
{
    return $"Name is {x}";
}
```
### Mapping from an interface
We can use a special class to map types. For instance, a Parquet.Net doesn't know about a type *DateTime* it uses *DateTimeOffset* so we can use:
```csharp
var mapConfig = new MapperConfig<TestModel3>()
           .MapProperty(x => x.DateValue, "DATE", DefaultMappers.DateTimeOffsetToDateTime);
```
Furthermore, you can use custom mappers, who implement an Interface IDifferentTypesMapper<TProperty, TColumn>.
## Reading
If we have a file
```csharp
var parquerEngine = new ParquetDataEngine();
var data = parquerEngine.Read(mapConfig, "test.parquet");
```
If we have a stream
```csharp
var parquerEngine = new ParquetDataEngine();
var data = parquerEngine.Read(mapConfig, parquetStream);
```
## Writing
We can write data to file or stream
```csharp
var parquetEngine = new ParquetDataEngine();

// We can use a file
parquetEngine.Write(mapConfig, "test.parquet", testData);

// Or we can use a stream
// parquetEngine.Write(mapConfig, parquetStream, testData);
```
## Appending
We can append data to new row group
```csharp
var parquetEngine = new ParquetDataEngine();
parquetEngine.Append(mapConfig, "test.parquet", data);
```
