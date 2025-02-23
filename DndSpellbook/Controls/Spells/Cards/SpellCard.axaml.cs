using System;
using System.Linq;
using Avalonia.Controls;
using DynamicData.Binding;

namespace DndSpellbook.Controls;

public partial class SpellCard : UserControl
{
    public SpellCard()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is not SpellCardViewModel vm) return;

        vm.WhenPropertyChanged(x => x.IsEditing).Subscribe(_ =>
        {
            if (vm.IsEditing)
            {
                RemoveViewer();
                AddEditor();
            }
            else
            {
                RemoveEditor();
                AddViewer();
            }
        });
    }

    private Button? GetViewer()
    {
        return MainPanel.Children.FirstOrDefault(x => x is Button) as Button;
    }
    
    private SpellCardEditor? GetEditor()
    {
        return MainPanel.Children.FirstOrDefault(x => x is SpellCardEditor) as SpellCardEditor;
    }

    private void AddViewer()
    {
        if (GetViewer() != null) return;
        
        if (DataContext is not SpellCardViewModel vm) return;

        var fullViewer = new SpellCardViewer
        {
            Spell = vm.Spell
        };

        var compactViewer = new CompactSpellCardViewer
        {
            Spell = vm.Spell
        };
        
        ToolTip.SetTip(compactViewer, fullViewer);
        
        var button = new Button
        {
            Padding = new(0),
            Command = vm.EditCommand,
            Content = compactViewer
        };
        
        MainPanel.Children.Add(button); 
    }

    private void AddEditor()
    {
        if (GetEditor() != null) return;
        
        if (DataContext is not SpellCardViewModel vm) return;
        if (vm.SpellEditor == null) return;
        
        var editor = new SpellCardEditor();

        editor.SpellEditor = vm.SpellEditor;
        editor.SaveCommand = vm.SaveCommand;
        editor.CancelCommand = vm.CancelCommand;
        
        MainPanel.Children.Add(editor);
    }
    
    private void RemoveViewer()
    {
        var viewer = GetViewer();
        if (viewer == null) return;
        
        MainPanel.Children.Remove(viewer);
    }

    private void RemoveEditor()
    {
        var editor = GetEditor();
        if (editor == null) return;
        
        MainPanel.Children.Remove(editor);
    }
}