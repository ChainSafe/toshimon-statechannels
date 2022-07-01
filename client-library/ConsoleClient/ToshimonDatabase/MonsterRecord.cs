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

    [CsvColumn(Name = "guid", FieldIndex = 3)]
    public string Guid { get; set; }

    [CsvColumn(Name = "rarity", FieldIndex = 4)]
    public uint Rarity { get; set; }

    [CsvColumn(Name = "evolves", FieldIndex = 5)]
    public uint Evolves { get; set; }

    [CsvColumn(Name = "evolutionStage", FieldIndex = 6)]
    public uint EvolutionStage { get; set; }

    [CsvColumn(Name = "description", CanBeNull = true, FieldIndex = 7)]
    public string? Description { get; set; }

    [CsvColumn(Name = "ethCardIndex", FieldIndex = 8)]
    public uint EthCardIndex { get; set; }

    [CsvColumn(Name = "type1", FieldIndex = 9)]
    public uint Type1 { get; set; }

    [CsvColumn(Name = "type2", FieldIndex = 10)]
    public uint Type2 { get; set; }

    [CsvColumn(Name = "maxHP", FieldIndex = 11)]
    public uint MaxHP { get; set; }

    [CsvColumn(Name = "attack", FieldIndex = 12)]
    public uint Attack { get; set; }

    [CsvColumn(Name = "defense", FieldIndex = 13)]
    public uint Defense { get; set; }

    [CsvColumn(Name = "spAttack", FieldIndex = 14)]
    public uint SpAttack { get; set; }

    [CsvColumn(Name = "spDefense", FieldIndex = 15)]
    public uint SpDefense { get; set; }

    [CsvColumn(Name = "speed", FieldIndex = 16)]
    public uint Speed { get; set; }

    [CsvColumn(Name = "move1", FieldIndex = 17)]
    public string Move1 { get; set; }

    [CsvColumn(Name = "move2", FieldIndex = 18)]
    public string Move2 { get; set; }

    [CsvColumn(Name = "move3", FieldIndex = 19)]
    public string Move3 { get; set; }

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
        ) { CardId = EthCardIndex };
    }

}