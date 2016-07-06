using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DigiTransit10.ExtensionMethods
{
    public static class ObservableCollectionExtensions
    {
        public static void AddSorted<T>(this ObservableCollection<T> col, T newElement, IComparer<T> comparer = null)
        {
            if (comparer == null)
            {
                comparer = Comparer<T>.Default;
            }

            int i = 0;
            while (i < col.Count && comparer.Compare(col[i], newElement) < 0)
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
