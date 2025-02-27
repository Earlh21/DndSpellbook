using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using DynamicData.Binding;

namespace DndSpellbook.Controls;

public class SpellSlotPip : Border
{
    public static readonly StyledProperty<bool> IsFilledProperty =
        AvaloniaProperty.Register<SpellSlotPip, bool>(nameof(IsFilled));
    
    public bool IsFilled
    {
        get => GetValue(IsFilledProperty);
        set => SetValue(IsFilledProperty, value);
    }
    
    public SpellSlotPip()
    {
        this.WhenPropertyChanged(x => x.IsFilled).Subscribe(_ =>
        {
            if (IsFilled)
            {
                Background = new SolidColorBrush(Colors.Firebrick);
            }
            else
            {
                Background = new SolidColorBrush(Colors.Transparent);
            }
        });
    }
}