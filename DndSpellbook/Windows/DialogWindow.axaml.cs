using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DndSpellbook.Navigation;

namespace DndSpellbook.Windows;

public partial class DialogWindow : Window
{
    public DialogWindow(IDialog viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
        
        viewModel.Closed += (sender, args) => Close(args);
        viewModel.Cancelled += (sender, args) => Close(null);
    }
}