using System;
using System.Text.Json.Serialization;
using DndSpellbook.Converters;
using DndSpellbook.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace DndSpellbook.Data.Models;

[Owned]
public class CastingTime : ReactiveObject
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CastingTimeType Type
    {
        get => type;
        set => this.RaiseAndSetIfChanged(ref type, value);
    }
    private CastingTimeType type;

    
    [JsonPropertyName("ritual")]
    public bool Ritual
    {
        get => ritual;
        set => this.RaiseAndSetIfChanged(ref ritual, value);
    }
    private bool ritual;

    [JsonPropertyName("time")]
    public int? Time
    {
        get => time;
        set => this.RaiseAndSetIfChanged(ref time, value);
    }
    private int? time;

    [JsonPropertyName("reactionText")]
    public string ReactionText
    {
        get => reactionText;
        set => this.RaiseAndSetIfChanged(ref reactionText, value);
    }
    private string reactionText;

    [JsonIgnore]
    public string DisplayString => ToString();

    public CastingTime()
    {
        this.WhenAnyValue(thisViewModel => thisViewModel.Type, x => x.Time)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(DisplayString)));
    }

    private CastingTime(CastingTimeType type, int? time = null, string reactionText = "", bool ritual = false) : this()
    {
        Type = type;
        Time = time;
        ReactionText = reactionText;
        Ritual = ritual;
    }
    
    public static CastingTime CreateBonusAction(bool ritual = false)
    {
        return new CastingTime(CastingTimeType.BonusAction, ritual: ritual);
    }
    
    public static CastingTime CreateAction(bool ritual = false)
    {
        return new CastingTime(CastingTimeType.Action, ritual: ritual);
    }
    
    public static CastingTime CreateReaction(string reactionText, bool ritual = false)
    {
        return new CastingTime(CastingTimeType.Reaction, reactionText: reactionText, ritual: ritual);
    }
    
    public static CastingTime CreateTime(int time, bool ritual = false)
    {
        return new CastingTime(CastingTimeType.Time, time, ritual: ritual);
    }

    public override string ToString()
    {
        if (Type == CastingTimeType.Reaction) return ReactionText ?? "";
        
        
        if (Type == CastingTimeType.BonusAction)
        {
            return Ritual ? "Bonus Action, Ritual" : "Bonus Action";
        }
        
        if (Type == CastingTimeType.Action)
        {
            return Ritual ? "Action, Ritual" : "Action";
        }

        if (Type == CastingTimeType.Time)
        {
            if (Time == null) return "";
            if (Time == 0) return "Instant";
            
            var timeText = SecondsToDurationTextConverter.Convert(Time.Value);
            return Ritual ? $"{timeText}, Ritual" : $"{timeText} minutes";
        }

        return "";
    }
}