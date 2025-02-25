using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;

namespace DndSpellbook.Controls;

public partial class LabelComboBox : UserControl
{
    public static readonly StyledProperty<object?> SelectedItemProperty =
        AvaloniaProperty.Register<LabelComboBox, object?>(nameof(SelectedItem), defaultBindingMode: BindingMode.TwoWay);

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
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