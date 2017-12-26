namespace GatewayServer.Air.Configuration
{
    using System.Collections;
    using System.Collections.Generic;

    public class INISectionCollection : IEnumerable, IEnumerable<INISection>
    {
        private ArrayList m_items = new ArrayList();

        public INIDocument Document
        {
            get;
            set;
        }

        public INISection Add(string name)
        {
            INISection section = new INISection(this.Document);
            section.Name = name;
            section.Load();
            return this.Add(section);
        }

        public INISection Add(INISection section)
        {
            this.m_items.Add(section);
            return section;
        }

        public INISection Add(char[] name)
        {
            return this.Add(new string(name));
        }

        public INISectionCollection(INIDocument document)
        {
            this.Document = document;
        }

        public int IndexOf(INISection section, int startIndex)
        {
            return this.m_items.IndexOf(section, startIndex);
        }

        public int IndexOf(INISection section)
        {
            return this.m_items.IndexOf(section);
        }

        public int LastIndexOf(INISection section, int startIndex)
        {
            return this.m_items.LastIndexOf(section, startIndex);
        }

        public int LastIndexOf(INISection section)
        {
            return this.m_items.LastIndexOf(section);
        }

        public void Remove(INISection section)
        {
            this.m_items.Remove(section);
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

        public void Insert(int index, INISection section)
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

        public INISection this[int index]
        {
            get
            {
                if (index < this.Count && index > -1)
                    return this.m_items[index] as INISection;
                return null;
            }
        }

        public INISection this[string name]
        {
            get
            {
                for (int i = 0; i < this.m_items.Count; i++)
                {
                    INISection section = this.m_items[i] as INISection;
                    if (section.Name == name)
                    {
                        return section;
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

        IEnumerator<INISection> IEnumerable<INISection>.GetEnumerator()
        {
            for (var i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }
    }
}
