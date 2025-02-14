using System;
using System.Collections.ObjectModel;
using DndSpellbook.Data.Models.Enums;
using ReactiveUI;
    using ReactiveUI.Validation.Extensions;
    using ReactiveUI.Validation.Helpers;

    namespace DndSpellbook.Data.Models;
    
    public partial class Spell : ReactiveValidationObject
    {
        public int Id { get; set; }
    
        private string name = "";
        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }
    
        private int level;
        public int Level
        {
            get => level;
            set => this.RaiseAndSetIfChanged(ref level, value);
        }
    
        private SpellSchool school;
        public SpellSchool School
        {
            get => school;
            set => this.RaiseAndSetIfChanged(ref school, value);
        }
    
        private CastingTime castingTime;
        public CastingTime CastingTime
        {
            get => castingTime;
            set => this.RaiseAndSetIfChanged(ref castingTime, value);
        }
    
        private int range;
        public int Range
        {
            get => range;
            set => this.RaiseAndSetIfChanged(ref range, value);
        }
    
        private int duration;
        public int Duration
        {
            get => duration;
            set => this.RaiseAndSetIfChanged(ref duration, value);
        }
    
        private bool verbal;
        public bool Verbal
        {
            get => verbal;
            set => this.RaiseAndSetIfChanged(ref verbal, value);
        }
    
        private bool somatic;
        public bool Somatic
        {
            get => somatic;
            set => this.RaiseAndSetIfChanged(ref somatic, value);
        }
    
        private bool material;
        public bool Material
        {
            get => material;
            set => this.RaiseAndSetIfChanged(ref material, value);
        }
        
        private string description = "";
        public string Description
        {
            get => description;
            set => this.RaiseAndSetIfChanged(ref description, value);
        }
        
        private ObservableCollection<SpellList> spellLists = new();
        public ObservableCollection<SpellList> SpellLists
        {
            get => spellLists;
            set => this.RaiseAndSetIfChanged(ref spellLists, value);
        }

        private Spell()
        {
            this.ValidationRule(s => s.Level,
                level => level is >= 0 and <= 9,
                "Level must be between 0 and 9.");
        }

        public Spell(string name) : this()
        {
            Name = name;
        }
        
        public Spell Clone()
        {
            return new Spell(Name)
            {
                Level = Level,
                School = School,
                CastingTime = CastingTime,
                Range = Range,
                Duration = Duration,
                Verbal = Verbal,
                Somatic = Somatic,
                Material = Material,
                Description = Description
            };
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
        }
    }