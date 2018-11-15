﻿using CSharpExt.Rx;
using DynamicData;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class SourceCacheExt
    {
        public static void SetTo<V, K>(this ISourceCache<V, K> list, IEnumerable<V> items)
        {
            list.Edit((l) =>
            {
                l.Load(items);
            });
        }

        public static void SetToWithDefault<V, K>(
            this ISourceSetCache<V, K> not,
            IHasBeenSetItemGetter<IEnumerable<V>> rhs,
            IHasBeenSetItemGetter<IEnumerable<V>> def)
        {
            if (rhs.HasBeenSet)
            {
                not.SetTo(rhs.Item);
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.SetTo(def.Item);
            }
            else
            {
                not.Unset();
            }
        }

        public static void SetToWithDefault<V, K>(
            this ISourceSetCache<V, K> not,
            IObservableSetCache<V, K> rhs,
            IObservableSetCache<V, K> def,
            Func<V, V, V> converter)
        {
            if (rhs.HasBeenSet)
            {
                if (def == null)
                {
                    not.SetTo(
                        rhs.Item.Select((t) => converter(t, default(V))));
                }
                else
                {
                    int i = 0;
                    not.SetTo(
                        rhs.KeyValues.Select((t) =>
                        {
                            if (!def.TryGetValue(t.Key, out var defVal))
                            {
                                defVal = default(V);
                            }
                            return converter(t.Value, defVal);
                        }));
                }
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.SetTo(
                    def.Item.Select((t) => converter(t, default(V))));
            }
            else
            {
                not.Unset();
            }
        }

        public static TObject AddOrUpdateReturn<TObject, TKey>(this ISourceCache<TObject, TKey> source, TObject item)
        {
            source.AddOrUpdate(item);
            return item;
        }
    }
}
