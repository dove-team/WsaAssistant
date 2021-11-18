using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WSATools.Libs
{
    public sealed class Node<T, T2, T3, T4> : IEnumerable
    {
        private readonly List<Tuple<T, T2, T3, T4>> tuples;
        public Node()
        {
            tuples = new List<Tuple<T, T2, T3, T4>>();
        }
        public int Count => tuples.Count;
        public void AddOrUpdate(T key, T2 value)
        {
            AddOrUpdate(key, value, default, default);
        }
        public void AddOrUpdate(T item1, T2 item2, T3 item3, T4 item4)
        {
            var item = tuples.FirstOrDefault(x => x.Item1.Equals(item1) || x.Item2.Equals(item2));
            if (item == null)
                tuples.Add(new Tuple<T, T2, T3, T4>(item1, item2, item3, item4));
            else
            {
                item4 = item.Item4;
                var idx = tuples.IndexOf(item);
                tuples.RemoveAt(idx);
                tuples.Insert(idx, new Tuple<T, T2, T3, T4>(item1, item2, item3, item4));
            }
        }
        public void Foreach(Action<Tuple<T, T2, T3, T4>> action)
        {
            tuples.ForEach(action);
        }
        public void Clear()
        {
            tuples.Clear();
        }
        public IEnumerator GetEnumerator()
        {
            foreach (var tuple in tuples)
                yield return tuple;
        }
        public Tuple<T, T2, T3, T4> FindItem(string address)
        {
            return tuples.FirstOrDefault(x => x.Item2.Equals(address));
        }
        public int GetCount(Func<Tuple<T, T2, T3, T4>, bool> func)
        {
            return tuples.Count(func);
        }
        public IEnumerable<Tuple<T, T2, T3, T4>> Where(Func<Tuple<T, T2, T3, T4>, bool> func)
        {
            return tuples.Where(func);
        }
        public Tuple<T, T2, T3, T4> GetIndex(int idx)
        {
            return tuples.ElementAt(idx);
        }
    }
}