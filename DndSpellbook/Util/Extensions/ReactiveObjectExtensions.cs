using System;
using System.Reactive.Disposables;
using System.Reflection;
using System.Threading.Tasks;
using ReactiveUI;

namespace DndSpellbook.Extensions;

public static class ReactiveObjectExtensions
{
    public static IDisposable SubscribeToAllChanges(this ReactiveObject root, Func<Task> onChanged)
    {
        var disposables = new CompositeDisposable();

        disposables.Add(root.Changed.Subscribe(_ => onChanged()));

        foreach (var property in root.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (typeof(ReactiveObject).IsAssignableFrom(property.PropertyType))
            {
                if (property.GetValue(root) is ReactiveObject nested)
                {
                    disposables.Add(nested.SubscribeToAllChanges(onChanged));
                }
            }
        }

        return disposables;
    }
}