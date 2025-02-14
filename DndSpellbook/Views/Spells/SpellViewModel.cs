using ReactiveUI;

namespace DndSpellbook.Views;

public class SpellViewModel : ViewModelBase
{
    public override string? UrlPathSegment { get; }
    public override IScreen HostScreen { get; }
}