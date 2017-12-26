namespace GatewayServer.Air.Configuration
{
    using System.Collections;
    using System.Collections.Generic;

    public class INIKeyCollection : IEnumerable, IEnumerable<INIKey>
    {
        private ArrayList m_items = new ArrayList();

        public INIKey Add(string key, string value)
        {
            return this.Add(new INIKey() { Name = key, Value = value });
        }

        public INIKey Add(INIKey key)
        {
            this.m_items.Add(key);
            return key;
        }

        public INIKey Add(char[] key, char[] value)
        {
            return this.Add(new string(key), new string(value));
        }

        public int IndexOf(INIKey key, int startIndex)
        {
            return this.m_items.IndexOf(key, startIndex);
        }

        public int IndexOf(INIKey key)
        {
            return this.m_items.IndexOf(key);
        }

        public int LastIndexOf(INIKey key, int startIndex)
        {
            return this.m_items.LastIndexOf(key, startIndex);
        }

        public int LastIndexOf(INIKey key)
        {
            return this.m_items.LastIndexOf(key);
        }

        public void Remove(INIKey key)
        {
            this.m_items.Remove(key);
        }

        public void RemoveAt(int index)
        {
            this.m_items.Remove(index);
        }

        public void RemoveRange(int index, int count)
        {
            this.m_items.RemoveRange(index, count);
        }

        public void RemoveAll()
        {
            while (this.Count > 0)
                this.RemoveAt(0);
        }

        public void Insert(int index, INIKey section)
        {
            this.m_items.Insert(index, section);
        }

        public void InsertRange(int index, ICollection c)
        {
            this.m_items.InsertRange(index, c);
        }

        public int Count
        {
            get
            {
                return this.m_items.Count;
            }
        }

        public INIKey this[int index]
        {
            get
            {
                if (index < this.Count && index > -1)
                    return this.m_items[index] as INIKey;
                return null;
            }
        }

        public INIKey this[string name]
        {
            get
            {
                for (int i = 0; i < this.m_items.Count; i++)
                {
                    INIKey key = this.m_items[i] as INIKey;
                    if (key.Name == name)
                    {
                        return key;
                    }
                }
                return null;
            }
        }

        public IEnumerator GetEnumerator()
        {
            for (var i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator<INIKey> IEnumerable<INIKey>.GetEnumerator()
        {
            for (var i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }
    }
}
