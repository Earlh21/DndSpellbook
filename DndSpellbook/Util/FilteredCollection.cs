using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicData;
using DynamicData.Binding;

namespace DndSpellbook.Util;

public class FilteredCollection<T> where T : notnull
{
    private readonly SourceCache<T, int> sourceCache;
    private ReadOnlyObservableCollection<T> view;
    
    public ReadOnlyObservableCollection<T> View => view;
    public IReadOnlyList<T> AllItems => sourceCache.Items;

    private IObservable<Func<T, bool>>[] filters;
    private IComparer<T>? sorter;
    private IObservable<IPageRequest>? pagination;
    
    public FilteredCollection(Func<T, int> idSelector, IComparer<T>? sorter, IObservable<IPageRequest>? pagination, params IObservable<Func<T, bool>>[] filters)
    {
        this.filters = filters;
        this.sorter = sorter;
        this.pagination = pagination;
        sourceCache = new(idSelector);

        var observable = sourceCache.Connect();
        
        foreach (var filter in filters)
        {
            observable = observable.Filter(filter);
        }

        if (pagination == null && sorter != null)
        {
            observable.SortAndBind(out view, sorter).Subscribe();
            return;
        }
        else if (pagination != null && sorter == null)
        {
            sorter = SortExpressionComparer<T>.Ascending(x => idSelector(x));

            observable = observable.SortAndPage(sorter, pagination);
        }
        else if(pagination != null && sorter != null)
        {
            observable = observable.SortAndPage(sorter, pagination);
        }
        
        observable.Bind(out view).Subscribe();
    }

    public FilteredCollection(Func<T, int> idSelector, params IObservable<Func<T, bool>>[] filters) : this(idSelector, null, null, filters)
    {
        
    }

    public T? Get(int key)
    {
        var optional = sourceCache.Lookup(key);
        return optional.HasValue ? optional.Value : default;
    }
    
    public void Remove(T item)
    {
        sourceCache.Remove(item);
    }
    
    public void RemoveKey(int key)
    {
        sourceCache.RemoveKey(key);
    }
    
    public void AddOrUpdate(T item)
    {
        sourceCache.AddOrUpdate(item);
    }
    
    public void AddOrUpdate(IEnumerable<T> items)
    {
        sourceCache.AddOrUpdate(items);
    }
    
    public void ReplaceAll(IEnumerable<T> items)
    {
        sourceCache.Edit(innerCache =>
        {
            innerCache.Clear();
            innerCache.AddOrUpdate(items);
        });
    }
    
    public void Clear()
    {
        sourceCache.Clear();
    }
}