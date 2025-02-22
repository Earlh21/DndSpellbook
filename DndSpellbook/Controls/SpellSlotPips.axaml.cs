using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using ReactiveUI;

namespace DndSpellbook.Controls;

public partial class SpellSlotPips : UserControl
{
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<SpellSlotPips, string>(nameof(Text));

    public string Text
    {
        get;
        set;
    }
    
    public static readonly StyledProperty<int> MaxPipsProperty =
        AvaloniaProperty.Register<SpellSlotPips, int>(nameof(MaxPips),
            defaultValue: 1,
            defaultBindingMode: BindingMode.TwoWay);

    public int MaxPips
    {
        get;
        set;
    }
    
    public static readonly StyledProperty<int> UsedPipsProperty =
        AvaloniaProperty.Register<SpellSlotPips, int>(nameof(UsedPips),
            defaultValue: 0,
            defaultBindingMode: BindingMode.TwoWay);

    public int UsedPips
    {
        get;
        set;
    }
    
    public SpellSlotPips()
    {
        InitializeComponent();

        AddPip();
        
        this.WhenAnyValue(x => x.MaxPips).Subscribe(maxPips =>
        {
            while (PipsPanel.Children.Count < maxPips)
            {
                AddPip();
            }
            while (PipsPanel.Children.Count > maxPips)
            {
                RemovePip();
            }
        });
    }

    private void AddPip()
    {
        var checkbox = new CheckBox();
        int index = PipsPanel.Children.Count;
        
        this.WhenAnyValue(x => x.UsedPips).Subscribe(usedPips =>
        {
            checkbox.IsChecked = index <= usedPips;
        });
        
        PipsPanel.Children.Add(checkbox);
    }
    
    private void RemovePip()
    {
        PipsPanel.Children.RemoveAt(PipsPanel.Children.Count - 1);
    }
}