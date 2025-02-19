using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DndSpellbook.Data.Models.Enums;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace DndSpellbook.Data.Models
{
    [JsonConverter(typeof(SpellJsonConverter))]
    public partial class Spell : ReactiveValidationObject
    {
        [Key]
        public int Id { get; set; }

        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }
        private string name = "";

        public int Level
        {
            get => level;
            set => this.RaiseAndSetIfChanged(ref level, value);
        }
        private int level;

        public SpellSchool School
        {
            get => school;
            set => this.RaiseAndSetIfChanged(ref school, value);
        }
        private SpellSchool school;

        public CastingTime CastingTime
        {
            get => castingTime;
            set => this.RaiseAndSetIfChanged(ref castingTime, value);
        }
        private CastingTime castingTime;

        public Range Range
        {
            get => range;
            set => this.RaiseAndSetIfChanged(ref range, value);
        }
        private Range range;

        public int Duration
        {
            get => duration;
            set => this.RaiseAndSetIfChanged(ref duration, value);
        }
        private int duration;

        public bool Verbal
        {
            get => verbal;
            set => this.RaiseAndSetIfChanged(ref verbal, value);
        }
        private bool verbal;

        public bool Somatic
        {
            get => somatic;
            set => this.RaiseAndSetIfChanged(ref somatic, value);
        }
        private bool somatic;

        public bool Material
        {
            get => material;
            set => this.RaiseAndSetIfChanged(ref material, value);
        }
        private bool material;

        public string Description
        {
            get => description;
            set => this.RaiseAndSetIfChanged(ref description, value);
        }
        private string description = "";

        public ObservableCollection<SpellList> SpellLists { get; } = new();

        private Spell()
        {
            // Example of existing validation rule:
            this.ValidationRule(s => s.Level,
                level => level is >= 0 and <= 9,
                "Level must be between 0 and 9.");
        }

        public Spell(string name) : this()
        {
            Name = name;
            Range = new Range(RangeType.Touch);
        }
        
        public Spell Clone()
        {
            var clone = new Spell(Name)
            {
                Level = Level,
                School = School,
                CastingTime = CastingTime,
                Range = Range,
                Duration = Duration,
                Verbal = Verbal,
                Somatic = Somatic,
                Material = Material,
                Description = Description,
            };
            
            foreach (var spellList in SpellLists)
            {
                clone.SpellLists.Add(spellList);
            }
            
            return clone;
        }

        public void CopyFrom(Spell other)
        {
            Name = other.Name;
            Level = other.Level;
            School = other.School;
            CastingTime = other.CastingTime;
            Range = other.Range;
            Duration = other.Duration;
            Verbal = other.Verbal;
            Somatic = other.Somatic;
            Material = other.Material;
            Description = other.Description;
            
            SpellLists.Clear();
            foreach (var spellList in other.SpellLists)
            {
                SpellLists.Add(spellList);
            }
        }
    }
}
