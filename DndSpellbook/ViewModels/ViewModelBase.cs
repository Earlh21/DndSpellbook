using ReactiveUI;

namespace DndSpellbook.ViewModels;

public abstract class ViewModelBase : ReactiveObject, IRoutableViewModel
{
    public abstract string? UrlPathSegment { get; }
    public abstract IScreen HostScreen { get; }
}