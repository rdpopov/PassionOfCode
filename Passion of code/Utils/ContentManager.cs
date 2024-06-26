using System.Collections;

namespace Passion_of_code.Utils.Content {
    public class ContentManager
    {
        private static Hashtable read_files = null;
        private static ContentManager instance = null;
        private static readonly object padlock = new object();

        public static ContentManager Instance
        {
            get
            {
                if (instance== null)
                {
                    lock (padlock)
                    {
                        if (instance== null)
                        {
                            instance= new ContentManager();
                        }

                        if (read_files== null)
                        {
                            read_files= new Hashtable();
                        }
                    }
                }
                return instance;
            }
        }

       public string GetFile(string fname){

           if (!read_files.ContainsKey(fname)){
                using (StreamReader sr = System.IO.File.OpenText(fname))
                {
                    read_files.Add(fname, sr.ReadToEnd());
                }
           }
           return (string)read_files[fname]; 
       }

    }
}
