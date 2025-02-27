using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using ReactiveUI;

namespace DndSpellbook.Controls;

public partial class SpellSlotPips : UserControl
{
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<SpellSlotPips, string>(nameof(Text));

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<int> MaxPipsProperty =
        AvaloniaProperty.Register<SpellSlotPips, int>(nameof(MaxPips),
            defaultValue: 1,
            defaultBindingMode: BindingMode.TwoWay);

    public int MaxPips
    {
        get => GetValue(MaxPipsProperty);
        set => SetValue(MaxPipsProperty, value);
    }

    public static readonly StyledProperty<int> UsedPipsProperty =
        AvaloniaProperty.Register<SpellSlotPips, int>(nameof(UsedPips),
            defaultValue: 0,
            defaultBindingMode: BindingMode.TwoWay);

    public int UsedPips
    {
        get => GetValue(UsedPipsProperty);
        set => SetValue(UsedPipsProperty, value);
    }

    public SpellSlotPips()
    {
        InitializeComponent();
        PipButton.AddHandler(PointerPressedEvent, PipButton_OnPointerPressed, handledEventsToo: true);

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

        this.WhenAnyValue(x => x.UsedPips).Subscribe(_ => UpdateAllPips());
    }

    private void UpdateAllPips()
    {
        for (int i = 0; i < PipsPanel.Children.Count; i++)
        {
            if (PipsPanel.Children[i] is SpellSlotPip pip)
            {
                pip.IsFilled = i < (MaxPips - UsedPips);
            }
        }
    }

    private void AddPip()
    {
        var pip = new SpellSlotPip
        {
            Width = 24, Height = 24,
            BorderBrush = new SolidColorBrush(Colors.Goldenrod),
            BorderThickness = new(2),
            CornerRadius = new(6)
        };
        
        int index = PipsPanel.Children.Count;
        pip.IsFilled = index < (MaxPips - UsedPips);

        PipsPanel.Children.Add(pip);
    }

    private void RemovePip()
    {
        PipsPanel.Children.RemoveAt(PipsPanel.Children.Count - 1);
    }

    private void UpButton_OnTapped(object? sender, TappedEventArgs e)
    {
        MaxPips++;
    }

    private void DownButton_OnTapped(object? sender, TappedEventArgs e)
    {
        MaxPips = Math.Max(MaxPips - 1, 0);
    }

    private void PipButton_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control control) return;
        var point = e.GetCurrentPoint(control);

        if (point.Properties.IsLeftButtonPressed)
        {
            UsedPips = Math.Min(UsedPips + 1, MaxPips);
        }
        else if (point.Properties.IsRightButtonPressed)
        {
            UsedPips = Math.Max(UsedPips - 1, 0);
        }
        else if (point.Properties.IsMiddleButtonPressed)
        {
            UsedPips = 0;
        }
    }
}