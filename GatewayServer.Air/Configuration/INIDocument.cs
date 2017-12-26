namespace GatewayServer.Air.Configuration
{
    using System;
    using System.IO;

    public class INIDocument // ini
    {
        public string FileName
        {
            get;
            set;
        }

        public INIDocument()
        {
            this.Sections = new INISectionCollection(this);
        }

        public INIDocument(string filename)
            : this()
        {
            this.FileName = filename;
        }

        public void Load()
        {
            int len; char[] buffer;
            if ((len = this.Length) > 0)
            {
                buffer = new char[len];
                if ((len = NativeMethods.GetPrivateProfileSectionNames(buffer, len, this.FileName)) > 0)
                {
                    unsafe
                    {
                        fixed (char* str = buffer)
                        {
                            char* ptr = str; int index = 0, last = 0;
                            while (index < len)
                            {
                                if (*ptr++ == '\0')
                                {
                                    int size = index - last;
                                    char[] name = new char[size];
                                    Array.Copy(buffer, last, name, 0, size);
                                    this.Sections.Add(name);
                                    last = index + 1;
                                }
                                index++;
                            }
                        }
                    }
                }
            }
        }

        public bool Create()
        {
            try
            {
                Stream stream = null;
                try
                {
                    stream = File.Create(this.FileName);
                }
                finally
                {
                    if (stream != null)
                        stream.Dispose();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Save()
        {
            this.Delete(); this.Create(); int[] len = new int[2];
            if ((len[0] = Sections.Count) > 0)
                for (int i = 0; i < len[0]; i++)
                {
                    INISection section = this.Sections[i];
                    if (section != null && (len[1] = section.Keys.Count) > 0)
                    {
                        for (int j = 0; j < len[1]; j++)
                        {
                            INIKey key = section.Keys[j];
                            if (section != null)
                                NativeMethods.WritePrivateProfileString(section.Name, key.Name, key.Value, this.FileName);
                        }
                    }
                }
        }

        public bool Delete()
        {
            try
            {
                if (File.Exists(this.FileName))
                    File.Delete(this.FileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public INISectionCollection Sections // app name.
        {
            get;
            set;
        }

        public int Length
        {
            get
            {
                if (File.Exists(this.FileName))
                {
                    FileInfo f_info = new FileInfo(this.FileName);
                    return (int)f_info.Length;
                }
                return 0;
            }
        }
    }
}
