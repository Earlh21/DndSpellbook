using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;

namespace DndSpellbook.Controls;

public partial class LabelBox : UserControl
{
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<LabelBox, string?>(nameof(Text), defaultBindingMode: BindingMode.TwoWay);

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<LabelBox, string>(nameof(Label));

    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }
    
    private static readonly StyledProperty<bool> AllowsReturnProperty =
        AvaloniaProperty.Register<LabelBox, bool>(nameof(AllowsReturn), defaultValue: false);

    public bool AllowsReturn
    {
        get => GetValue(AllowsReturnProperty);
        set => SetValue(AllowsReturnProperty, value);
    }

    public LabelBox()
    {
        InitializeComponent();
    }
}