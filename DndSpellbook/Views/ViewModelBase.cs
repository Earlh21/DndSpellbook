﻿using ReactiveUI;

namespace DndSpellbook.Views;

public abstract class ViewModelBase : ReactiveObject, IRoutableViewModel
{
    public abstract string? UrlPathSegment { get; }
    public abstract IScreen HostScreen { get; }
}