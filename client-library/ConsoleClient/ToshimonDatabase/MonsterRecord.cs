using LINQtoCSV;
using System;
using Protocol;

/**
 * A representation of how the Toshimon data is stored in CSV format
 */
public record MonsterRecord
{
    [CsvColumn(Name = "name", FieldIndex = 1)]
    public string Name { get; set; }

    [CsvColumn(Name = "toshidexNumber", FieldIndex = 2)]
    public uint ToshidexNumber { get; set; }

    [CsvColumn(Name = "rarity", FieldIndex = 3)]
    public uint Rarity { get; set; }

    [CsvColumn(Name = "evolves", FieldIndex = 4)]
    public uint Evolves { get; set; }

    [CsvColumn(Name = "evolutionStage", FieldIndex = 5)]
    public uint EvolutionStage { get; set; }

    [CsvColumn(Name = "description", CanBeNull = true, FieldIndex = 6)]
    public string? Description { get; set; }

    [CsvColumn(Name = "ethCardIndex", FieldIndex = 7)]
    public uint EthCardIndex { get; set; }

    [CsvColumn(Name = "type1", FieldIndex = 8)]
    public uint Type1 { get; set; }

    [CsvColumn(Name = "type2", FieldIndex = 9)]
    public uint Type2 { get; set; }

    [CsvColumn(Name = "maxHP", FieldIndex = 10)]
    public uint MaxHP { get; set; }

    [CsvColumn(Name = "attack", FieldIndex = 11)]
    public uint Attack { get; set; }

    [CsvColumn(Name = "defense", FieldIndex = 12)]
    public uint Defense { get; set; }

    [CsvColumn(Name = "spAttack", FieldIndex = 13)]
    public uint SpAttack { get; set; }

    [CsvColumn(Name = "spDefense", FieldIndex = 14)]
    public uint SpDefense { get; set; }

    [CsvColumn(Name = "speed", FieldIndex = 15)]
    public uint Speed { get; set; }

    [CsvColumn(Name = "knownMoves", FieldIndex = 16)]
    public string KnownMoves { get; set; }

    public MonsterCard toMonsterCard() {
        Stats stats = new Stats {
            Hp = MaxHP,
            Attack = Attack,
            Defense = Attack,
            SpAttack = SpAttack,
            SpDefense = SpDefense,
            Speed = Speed,
            // Move PP not supported yet
            PP = new List<uint>(new uint[4]{ 0, 0, 0, 0 }),
        };
        return new MonsterCard(
            stats,
            stats,
            new string[]{ "0x0000000000000000000000000000000000000000", "0x0000000000000000000000000000000000000000", "0x0000000000000000000000000000000000000000", "0x0000000000000000000000000000000000000000" } // Moves also not yet supported
        );
    }

}