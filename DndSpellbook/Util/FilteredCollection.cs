using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;
using DynamicData.Binding;

namespace DndSpellbook.Util;

public class FilteredCollection<T> where T : notnull
{
    private readonly SourceCache<T, int> sourceCache;
    private ReadOnlyObservableCollection<T> view;
    
    public ReadOnlyObservableCollection<T> View => view;
    public IReadOnlyList<T> AllItems => sourceCache.Items;
    
    public FilteredCollection(Func<T, int> idSelector, SortExpressionComparer<T>? sorter, params IObservable<Func<T, bool>>[] filters)
    {
        sourceCache = new(idSelector);

        var observable = sourceCache.Connect();
        
        foreach (var filter in filters)
        {
            observable = observable.Filter(filter);
        }

        if (sorter == null)
        {
            observable.Bind(out view).Subscribe();
        }
        else
        {
            observable.SortAndBind(out view, sorter).Subscribe();
        }
    }
    
    public FilteredCollection(Func<T, int> idSelector, params IObservable<Func<T, bool>>[] filters)
    {
        sourceCache = new(idSelector);

        var observable = sourceCache.Connect();
        
        foreach (var filter in filters)
        {
            observable = observable.Filter(filter);
        }

        observable.Bind(out view).Subscribe();
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