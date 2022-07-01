using LINQtoCSV;
using System;
using Protocol;

/**
 * A representation of how the a toshimon move is stored in CSV format
 */
public record MoveRecord
{
    [CsvColumn(Name = "moveNumber", FieldIndex = 1)]
    public uint MoveNumber { get; set; }

    [CsvColumn(Name = "contractAddress", FieldIndex = 2)]
    public string ContractAddress { get; set; }

    [CsvColumn(Name = "guid", FieldIndex = 3)]
    public string Guid { get; set; }

    [CsvColumn(Name = "name", FieldIndex = 4)]
    public string Name { get; set; }

    [CsvColumn(Name = "description", CanBeNull = true, FieldIndex = 7)]
    public string? Description { get; set; }

    [CsvColumn(Name = "type", FieldIndex = 6)]
    public uint Type { get; set; }

    [CsvColumn(Name = "sp", FieldIndex = 10)]
    public uint Sp { get; set; }  

}