using LINQtoCSV;
using System;

public record MonsterRecord
{
    [CsvColumn(Name = "name", FieldIndex = 1)]
    public string Name { get; set; }

    [CsvColumn(Name = "toshidexNumber", FieldIndex = 2)]
    public int ToshidexNumber { get; set; }

    [CsvColumn(Name = "rarity", FieldIndex = 3)]
    public int Rarity { get; set; }

    [CsvColumn(Name = "evolves", FieldIndex = 4)]
    public int Evolves { get; set; }

    [CsvColumn(Name = "evolutionStage", FieldIndex = 5)]
    public int EvolutionStage { get; set; }

    [CsvColumn(Name = "description", CanBeNull = true, FieldIndex = 6)]
    public string? Description { get; set; }

    [CsvColumn(Name = "ethCardIndex", FieldIndex = 7)]
    public int EthCardIndex { get; set; }

    [CsvColumn(Name = "type1", FieldIndex = 8)]
    public int Type1 { get; set; }

    [CsvColumn(Name = "type2", FieldIndex = 9)]
    public int Type2 { get; set; }

    [CsvColumn(Name = "maxHP", FieldIndex = 10)]
    public int MaxHP { get; set; }

    [CsvColumn(Name = "attack", FieldIndex = 11)]
    public int Attack { get; set; }

    [CsvColumn(Name = "defense", FieldIndex = 12)]
    public int Defense { get; set; }

    [CsvColumn(Name = "spAttack", FieldIndex = 13)]
    public int SpAttack { get; set; }

    [CsvColumn(Name = "spDefense", FieldIndex = 14)]
    public int SpDefense { get; set; }

    [CsvColumn(Name = "speed", FieldIndex = 15)]
    public int Speed { get; set; }

    [CsvColumn(Name = "knownMoves", FieldIndex = 16)]
    public string KnownMoves { get; set; }

}