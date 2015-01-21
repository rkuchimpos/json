using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace Json
{
    public class JsonObject : DynamicObject, IDictionary<string, object>, IEnumerable
    {
        private IDictionary<string, object> items = new Dictionary<string, object>();

        public void Add(string key, object value)
        {
            items.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return this.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return items.Keys; }
        }

        public bool Remove(string key)
        {
            return items.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return items.TryGetValue(key, out value);
        }

        public ICollection<object> Values
        {
            get { return items.Values; }
        }

        public object this[string key]
        {
            get
            {
                return items[key];
            }
            set
            {
                items[key] = value;
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            items.Add(item);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return items.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return items.Count; }
        }

        public bool IsReadOnly
        {
            get { return items.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return items.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return items.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            items[binder.Name] = value;

            return true;
        }
    }
}
