using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;

namespace DndSpellbook.Controls;

public partial class LabelComboBox : UserControl
{
    public static readonly StyledProperty<object?> SelectedValueProperty =
        AvaloniaProperty.Register<LabelComboBox, object?>(nameof(SelectedValue), defaultBindingMode: BindingMode.TwoWay);

    public object? SelectedValue
    {
        get => GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }
    
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<LabelComboBox, IEnumerable?>(nameof(ItemsSource));

    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }
    
    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<LabelComboBox, string>(nameof(Label));

    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }
    
    public LabelComboBox()
    {
        InitializeComponent();
    }
}