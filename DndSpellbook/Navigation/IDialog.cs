using System;

namespace DndSpellbook.Navigation;

public interface IDialog
{
    public event EventHandler<object> Closed;
    public event EventHandler Cancelled;
}