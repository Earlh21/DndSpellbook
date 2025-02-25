using System;
using System.Linq;
using Avalonia.Controls;
using DynamicData.Binding;

namespace DndSpellbook.Controls;

public partial class SpellExpander : UserControl
{
    public SpellExpander()
    {
        InitializeComponent();
    }
    
    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is not SpellExpanderViewModel vm) return;

        // React to editing state changes
        vm.WhenPropertyChanged(x => x.IsEditing).Subscribe(_ => UpdateContent());
        
        // React to expander state changes
        vm.WhenPropertyChanged(x => x.IsExpanded).Subscribe(_ => UpdateContent());
        
        // Also subscribe to the Expander's own IsExpanded property
        MainExpander.PropertyChanged += (_, args) =>
        {
            if (args.Property.Name == "IsExpanded")
            {
                // Only update content if the expansion state has changed
                UpdateContent();
            }
        };
        
        // Initial setup
        UpdateContent();
    }

    private void UpdateContent()
    {
        if (DataContext is not SpellExpanderViewModel vm) return;
        
        // Only create content if the expander is open
        if (MainExpander.IsExpanded)
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
        }
        else
        {
            // If closed, remove everything to save resources
            RemoveViewer();
            RemoveEditor();
        }
    }

    private SpellExpanderViewer? GetViewer()
    {
        return MainExpander.Content as SpellExpanderViewer;
    }
    
    private SpellExpanderEditor? GetEditor()
    {
        return MainExpander.Content as SpellExpanderEditor;
    }

    private void AddViewer()
    {
        if (GetViewer() != null) return;
        
        if (DataContext is not SpellExpanderViewModel vm) return;

        var viewer = new SpellExpanderViewer
        {
            Spell = vm.Spell
        };
        
        MainExpander.Content = viewer;
    }

    private void AddEditor()
    {
        if (GetEditor() != null) return;
        
        if (DataContext is not SpellExpanderViewModel vm) return;
        if (vm.SpellEditor == null) return;
        
        var editor = new SpellExpanderEditor
        {
            SpellEditor = vm.SpellEditor
        };
        
        MainExpander.Content = editor;
    }
    
    private void RemoveViewer()
    {
        if (GetViewer() != null)
        {
            MainExpander.Content = null;
        }
    }

    private void RemoveEditor()
    {
        if (GetEditor() != null)
        {
            MainExpander.Content = null;
        }
    }
}