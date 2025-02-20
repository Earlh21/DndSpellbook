using System;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace DndSpellbook.Util;

public class PageRequest : ReactiveObject, IPageRequest
{
    private int page;
    public int Page
    {
        get => page;
        set => this.RaiseAndSetIfChanged(ref page, value);
    }

    private int size;
    public int Size
    {
        get => size;
        set => this.RaiseAndSetIfChanged(ref size, value);
    }
    
    public PageRequest(int page, int size)
    {
        Page = page;
        Size = size;
    }
    
    public IObservable<IPageRequest> AsObservable() => this.WhenAnyPropertyChanged().Select(_ => new PageRequest(Page, Size));
}