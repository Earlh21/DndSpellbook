using ReactiveUI;

namespace DndSpellbook.Views;

public abstract class ViewModelBase : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => null;
    public IScreen HostScreen => null;
}