namespace GatewayServer.Air.Configuration
{
    using System.IO;

    public class INISection // 节
    {
        public string Name
        {
            get;
            set;
        }

        public INISection()
        {
            this.Keys = new INIKeyCollection();
        }

        public INIDocument Document
        {
            get;
            set;
        }

        public INISection(INIDocument document)
            : this()
        {
            this.Document = document;
        }

        public void Load()
        {
            if (File.Exists(this.Document.FileName))
            {
                int i, len;
                if ((len = this.Document.Length) > 0)
                {
                    string buffer = new string('\0', len);
                    if (NativeMethods.GetPrivateProfileSection(this.Name, ref buffer, len, this.Document.FileName) > 0)
                    {
                        string[] items = buffer.Split('\0');
                        foreach (string str in items)
                            if (str.Length > 0 && (i = str.IndexOf('=')) > -1)
                                this.Keys.Add(str.Substring(0, i), str.Substring(i + 1));
                    }
                }
            }
        }

        public INIKeyCollection Keys
        {
            get;
            set;
        }
    }
}
