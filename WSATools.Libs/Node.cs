using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WSATools.Libs
{
    public sealed class Node<T, T2, T3> : IEnumerable
    {
        private readonly List<Tuple<T, T2, T3>> tuples;
        public Node()
        {
            tuples = new List<Tuple<T, T2, T3>>();
        }
        public int Count => tuples.Count;
        public void AddOrUpdate(T key, T2 value)
        {
            AddOrUpdate(key, value, default);
        }
        public void AddOrUpdate(T item1, T2 item2, T3 item3)
        {
            var item = tuples.FirstOrDefault(x => x.Item1.Equals(item1) || x.Item2.Equals(item2));
            if (item == null)
                tuples.Add(new Tuple<T, T2, T3>(item1, item2, item3));
            else
            {
                var idx = tuples.IndexOf(item);
                tuples.RemoveAt(idx);
                tuples.Insert(idx, new Tuple<T, T2, T3>(item1, item2, item3));
            }
        }
        public void Foreach(Action<Tuple<T, T2, T3>> action)
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
    }
}