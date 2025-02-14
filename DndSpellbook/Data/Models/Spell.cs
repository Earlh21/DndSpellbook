using System;
    using ReactiveUI;
    
    namespace DndSpellbook.Data.Models;
    
    public partial class Spell : ReactiveObject
    {
        public int Id { get; set; }
    
        private string name;
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
    
        private TimeSpan duration;
        public TimeSpan Duration
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
        
        private string description;
        public string Description
        {
            get => description;
            set => this.RaiseAndSetIfChanged(ref description, value);
        }
    
        private Spell() { }
    
        public Spell(string name)
        {
            Name = name;
        }
    }