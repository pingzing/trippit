using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Trippit.ExtensionMethods
{
    public static class ObservableCollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> _this, IEnumerable<T> range)
        {
            if(range == null)
            {
                return;
            }

            if(!range.Any())
            {
                return;
            }

            foreach(T item in range)
            {
                _this.Add(item);
            }
        }

        //todo: move the AddSorteds to an IListExtensions class
        public static void AddSorted<T>(this IList<T> col, T newElement)
        {
            AddSorted(col, newElement, comparerFunc: null);
        }

        public static void AddSorted<T>(this IList<T> col, T newElement, IComparer<T> comparer)
        {
            AddSorted(col, newElement, comparer.Compare);
        }

        public static void AddSorted<T>(this IList<T> col, T newElement, Func<T, T, int> comparerFunc)
        {
            if(comparerFunc == null)
            {
                comparerFunc = Comparer<T>.Default.Compare;
            }

            int i = 0;
            while(i < col.Count && comparerFunc(col[i], newElement) < 0)
            {
                i++;
            }

            col.Insert(i, newElement);
        }

        public static void SortInPlace<TSource, TKey>(this ObservableCollection<TSource> col, Func<TSource, TKey> keySelector)
        {
            List<TSource> sorted = col.OrderBy(keySelector).ToList();

            foreach (var sortItem in sorted)
            {
                int currIndex = col.IndexOf(sortItem);
                int sortedIndex = sorted.IndexOf(sortItem);
                if (currIndex != sortedIndex)
                {
                    col.Move(currIndex, sortedIndex);
                }
            }
        }

        public static void SortInPlace<TSource, TKey>(this ObservableCollection<TSource> col, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            List<TSource> sorted = col.OrderBy(keySelector, comparer).ToList();

            foreach (var sortItem in sorted)
            {
                int currIndex = col.IndexOf(sortItem);
                int sortedIndex = sorted.IndexOf(sortItem);
                if (currIndex != sortedIndex)
                {
                    col.Move(currIndex, sortedIndex);
                }
            }
        }

        public static void SortInPlaceDescending<TSource, TKey>(this ObservableCollection<TSource> col, Func<TSource, TKey> keySelector)
        {
            List<TSource> sorted = col.OrderByDescending(keySelector).ToList();

            foreach (var sortItem in sorted)
            {
                int currIndex = col.IndexOf(sortItem);
                int sortedIndex = sorted.IndexOf(sortItem);
                if (currIndex != sortedIndex)
                {
                    col.Move(currIndex, sortedIndex);
                }
            }
        }

        public static void SortInPlaceDescending<TSource, TKey>(this ObservableCollection<TSource> col, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            List<TSource> sorted = col.OrderByDescending(keySelector, comparer).ToList();

            foreach (var sortItem in sorted)
            {
                int currIndex = col.IndexOf(sortItem);
                int sortedIndex = sorted.IndexOf(sortItem);
                if (currIndex != sortedIndex)
                {
                    col.Move(currIndex, sortedIndex);
                }
            }
        }
    }
}
