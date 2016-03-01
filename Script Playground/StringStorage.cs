using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Script_Playground

{
    class StringStorage
    {
        //Storage is the only persistent variable
        public string Storage { get; set; }
        
        private const char D_D_DELIM = '|';
        private const char ID_D_DELIM = '§';

        public Dictionary<string, string> ReadBuffer { get; private set; }

        public StringStorage()
        {
            CreateReadBuffer();
            PopulateReadBuffer();
        }

        public StringStorage(Dictionary<string, string> transferBuffer)
        {
            ReadBuffer = transferBuffer;
        }

        public StringStorage(StringStorage transferClass)
        {
            ReadBuffer = transferClass.ReadBuffer;
        }

        private string[,] Disassemble(string d)
        {
            //input compiled string, return string array
            string[] ds = d.Split(new char[] { D_D_DELIM }, StringSplitOptions.RemoveEmptyEntries);
            string[,] da = new string[ds.Length, 2];
            int s;
            for (int i = 0; i < ds.Length; i++)
            {
                s = ds[i].IndexOf(ID_D_DELIM);
                if (s >= 0)
                {
                    da[i, 0] = ds[i].Substring(0, s);
                    da[i, 1] = ds[i].Substring(s + 1);
                }
            }

            return da;
        }

        //WIP Disassemble to return a Dictionary instead of a 2D-array to test for potential optimizations, if successful this is a precursor to an Assemble() that assembles from a Dictionary
        private Dictionary<string, string> DisassembleOpt(string d)
        {
            //input compiled string, return dictionary
            string[] ds = d.Split(new char[] { D_D_DELIM }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> dict = new Dictionary<string,string>();
            int s;
            for (int i = 0; i < ds.Length; i++)
            {
                s = ds[i].IndexOf(ID_D_DELIM);
                if (s >= 0)
                {
                    dict[ds[i].Substring(0, s)] = ds[i].Substring(s + 1);
                }
            }

            return dict;
        }

        private string Assemble(string[,] da)
        {
            //input string array, return compiled string
            //null entries in array are post-remove holes, these need to be ignored
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < da.GetLength(0); i++)
            {
                if (da[i, 0] != null && da[i, 1] != null && da.GetLength(1) == 2)
                {
                    b.Append(da[i, 0].ToString()).Append(ID_D_DELIM).Append(da[i, 1].ToString()).Append(D_D_DELIM);
                }
            }
            return b.ToString();
        }

        public void Store(string id, string data)
        {
            if (ReadBuffer.ContainsKey(id))
            {
                Update(id, data);
            }
            else
            {
                Storage = new StringBuilder().Append(Storage).Append(id.ToString()).Append(ID_D_DELIM).Append(data.ToString()).Append(D_D_DELIM).ToString();
            }

            ReadBuffer[id] = data;
        }

        public string Remove(string id)
        {
            //input id, disassemble, get AND REMOVE data, assemble AND SAVE storage, return data
            string d = null;
            string[,] da = Disassemble(Storage);
            for (int i = 0; i < da.GetLength(0); i++)
            {
                if (da[i, 0].Equals(id))
                {
                    d = da[i, 1];
                    da[i, 0] = null;
                    da[i, 1] = null;
                }
            }
            Storage = Assemble(da);

            ReadBuffer.Remove(id);

            return d;
        }

        public string Read(string id)
        {
            string s = null;
            ReadBuffer.TryGetValue(id, out s);
            return s;
        }

        public void Clear()
        {
            Storage = "";
            ReadBuffer.Clear();
        }

        public void CreateReadBuffer()
        {
            //Initialize a Dictionay to serve as a Read() buffer
            ReadBuffer = new Dictionary<string, string>();
        }

        public void PopulateReadBuffer()
        {
        //Populates an empty ReadBuffer with the contents of Storage.
        if (!Object.Equals(Storage, default(string)))
            {
                string[] ds = Storage.Split(new char[] { D_D_DELIM }, StringSplitOptions.RemoveEmptyEntries);
                int s;
                for (int i = 0; i < ds.Length; i++)
                {
                    s = ds[i].IndexOf(ID_D_DELIM);
                    //Dump(ds[i]);
                    if (s >= 0)
                    {
                        ReadBuffer[ds[i].Substring(0, s)] = ds[i].Substring(s + 1);
                    }
                }
            }
        }

        private bool Update(string id, string data)
        {
            //input id + data, disassemble, update id with data, assemble AND SAVE storage, return true if update happened, false for no id match
            string[,] da = Disassemble(Storage);
            for (int i = 0; i < da.GetLength(0); i++)
            {
                if (da[i, 0].Equals(id))
                {
                    da[i, 1] = data;
                    Storage = Assemble(da);
                    ReadBuffer[id] = data;
                    return true;
                }
            }
            return false;
        }

        //WIP on Refactoring Update(), optimization-tests using string-level search instead of full parsing of Storage pre-lookup
        private bool UpdateOpt(string id, string data)
        {

            //Several variables required for proper overview of id-data pair in string:
            //      [id]§[data]|
            //The above is a representation of our id-data pair somewhere inside the string.
            //In order to successfully find the data in our id-data pair we only need its start index and id-length, which we know.
            //We also know the length of our delimiters, which are (in this case) both a char, so they occupy 1 "length" each.
            //In order to successfully manipulate the data-part of our id-data pair we also need two crucial pieces of information:
            //The end of our id-data pair and the length of the data. We do not know either of these from the get-go but we have a way of finding them;
            //We can find the position of our data delimiter by way of knowing the position of our id in the string!
            //In order to acquire these four pieces of information we need to do it in a few steps, as outlined here:
            //
            //Step 1: get the index of our id-data pair:
            //      [id]§[data]|
            //      ^
            //We must keep in mind that the id might show up as data for a different id! Therefore we need to make sure that we only get the index of our id.
            //To do this we simply add the id-delimiter to the id passed as an argument, essentially telling String.IndexOf to look for "[id]|".
            //Before we write code for step two, we might as well check if the id we are looking for really exists inside the string.
            //This is smart to do in case we mistype the id, or if we ask for it at the wrong point in time, and it saves on instruction count if it turns out that it isn't there after all.
            //Thus we do a simple if-condition since String.IndexOf returns -1 if what we are looking for is not contained in the string.
            //
            //Step 2: we need to find the start of the data in our id-data pair.
            //Simply adding the length of the id and the delimiter (which is 1) to the id-data pair index will give us this info.
            //      [id]§[data]|
            //           ^
            //
            //Step 3: We need to find the end of our id-data pair.
            //We know where the id-data pair starts, thus we also know that the next data-delimiter tells us the end of our id-data pair!
            //      [id]§[data]|
            //                 ^
            //Handily enough, String.IndexOf allows us to look for the first occurence of the data delimiter after a given index,
            //so we give it the start-index of our id data pair (that we got in step one) and voila! Now only one step remains:
            //
            //Step 4: getting the length of our data.
            //Since we know where our data starts and where it ends, we subtract the start index from the end index. 
            //
            //We now possess all the information we need to either remove the id-data pair, update its data or just return the data value :)

            //Step one
            int idIndex = Storage.IndexOf(id + ID_D_DELIM, StringComparison.Ordinal);

            if (idIndex == -1)
            {
                //If IndexOf does not find our id we exit the method and return false to indicate that the Storage-string is unchanged.
                return false;
            }

            //Step two
            int dataStart = idIndex + id.Length + 1;
            //Step three
            int dataEnd = Storage.IndexOf(D_D_DELIM, dataStart);
            //Step four
            int dataLength = dataEnd - dataStart;

            //We split the Storage string into two parts;
            //first part contains the string from the start up to our id-data pair
            //second part contains the string after our id-data pair up until the end
            string p1Storage = Storage.Substring(0, idIndex);
            string p2Storage = Storage.Substring(dataEnd+1);

            //MARK
            //Now comes the updating part :)
            //We simply assemble the new Storage string, including the new, updated id-data pair, remembering to include delimiters in the correct places.
            //Here we consider that the last value updated is quite likely to be updated again, therefore it is placed at the start of the storage-string.
            //This allows for a hierarchy where the most often updated values are nearer the start of the storage-string, potentially improving seek times as the iterator for indexOf starts at index 0.

            Storage = new StringBuilder().Append(id.ToString()).Append(ID_D_DELIM.ToString()).Append(data.ToString()).Append(D_D_DELIM.ToString()).Append(p1Storage).Append(p2Storage).ToString();

            //We have a read-buffer, and we need to make sure the value there gets updated as well, in case we wish to access it there!

            ReadBuffer[id] = data;

            //we can now return true, since the Storage-string (and the read-buffer) has been updated with the data passed to this method.

            return true;
        }

        #region test methods

        public void TestStore()
        {
            Store("test", "this is a test");
            Dump(Storage);
            if (Storage.Equals("test§this is a test|"))
            {
                Dump("Test of Store() successful!");
            }
        }

        public void TestAssemble()
        {
            string s = Assemble(new string[,] { { "1", "2" }, { "3", "4" }, { "5", "6" } });
            Dump(s);
            if (s.Equals("1§2|3§4|5§6|"))
            {
                Dump("Test of Assemble() successful!");
            }
        }

        public void TestDisassemble()
        {
            string[,] sa = Disassemble("0§0|1§1|2§2");
            DumpA2(sa);
            bool isEqual = true;
            for (int i = 0; i < sa.GetLength(0); i++)
            {
                if (!sa[i, 0].Equals(i.ToString()))
                {
                    isEqual = false;
                }
            }
            if (isEqual)
            {
                Dump("Test of Disassemble() successful!");
            }
        }

        public void TestDisassembleOpt()
        {
            Dictionary<string, string> dict = DisassembleOpt("0§0|1§1|2§2");
            dumpD(dict);
            bool isEqual = true;
            int i = 0;
            foreach(KeyValuePair<string, string> kvp in dict)
            {
                if (!kvp.Key.Equals(i.ToString()))
                {
                    isEqual = false;
                }
                i++;
            }
            if(isEqual)
            {
                Dump("Test of DisassembleOpt() successful!");
            }

        }

        public void TestRead()
        {
            Store("airLevel", "wan milian perzint!");
            Dump(Read("airLevel"));
            if (Read("airLevel").Equals("wan milian perzint!"))
            {
                Dump("Test of Read() successful!");
            }
        }

        public void TestRetrieve()
        {
            Store("Remove me", "NO! I wish to be retrieved!");
            Dump(Remove("Remove me"));
            if (Storage.Length == 0 && ReadBuffer.Count == 0)
            {
                Dump("Test of Retrieve() successful!");
            }
        }

        public void TestRemove()
        {
            Store("Fuck you", "I'm a bad influence and need to be purged!");
            Remove("Fuck you");
            if (Storage.Length == 0 && ReadBuffer.Count == 0)
            {
                Dump("Test of Remove() successful!");
            }
        }

        public void TestUpdate()
        {
            Store("The times", "Is a magazine for old ppl");
            Update("The times", "They are 'a changin'");
            if (Read("The times").Equals("They are 'a changin'"))
            {
                Dump("Test of Update() successful!");
            }
        }

        public void TestUpdateOpt()
        {
            Store("The times", "Is a magazine for old ppl");
            UpdateOpt("The times", "They are 'a changin'");
            if (Read("The times").Equals("They are 'a changin'"))
            {
                Dump("Test of Update() successful!");
            }
        }

        #endregion

        #region benchmark methods

        public void BenchmarkStore(int iterations)
        {
            Stopwatch stopwatch = new Stopwatch();
            Dump("Current time: " + System.DateTime.Now.ToShortTimeString() + ", Stopwatch START!");
            stopwatch.Start();

            for (int i = 0; i < iterations; i++)
            {
                Store(i.ToString(), (i + 1).ToString());
            }

            stopwatch.Stop();
            Dump("Bench of Store: Time elapsed over " + iterations + " iterations: " + stopwatch.Elapsed.ToString() + ", size of Storage is now " + Storage.Length.ToString() + " characters.");
        }

        public void BenchmarkAssemble(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                Store(i.ToString(), (i + 1).ToString());
            }
            string[,] ta = Disassemble(Storage);

            Stopwatch stopwatch = new Stopwatch();
            Dump("Current time: " + System.DateTime.Now.ToShortTimeString() + ", Stopwatch START!");
            stopwatch.Start();

            Assemble(ta);

            stopwatch.Stop();
            Dump("Bench of Assemble: Time elapsed over ONE array of " + iterations + " data points: " + stopwatch.Elapsed.ToString());
            stopwatch.Reset();
            StringBuilder t = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                t.Append(i.ToString()).Append(ID_D_DELIM).Append(i + 1.ToString()).Append(D_D_DELIM);
            }
            ta = Disassemble(t.ToString());
            Dump("Current time: " + System.DateTime.Now.ToShortTimeString() + ", Stopwatch START!");
            stopwatch.Start();

            for (int i = 0; i < iterations; i++)
            {
                Assemble(ta);
            }

            stopwatch.Stop();
            Dump("Bench of Assemble: Time elapsed over " + iterations + " iterations on a 10-point string: " + stopwatch.Elapsed.ToString());
            stopwatch.Reset();
            t = new StringBuilder();
            for (int i = 0; i < 100; i++)
            {
                t.Append(i.ToString()).Append(ID_D_DELIM).Append(i + 1.ToString()).Append(D_D_DELIM);
            }
            ta = Disassemble(t.ToString());
            Dump("Current time: " + System.DateTime.Now.ToShortTimeString() + ", Stopwatch START!");
            stopwatch.Start();

            for (int i = 0; i < iterations; i++)
            {
                Disassemble(t.ToString());
            }

            stopwatch.Stop();
            Dump("Bench of Assemble: Time elapsed over " + iterations + " iterations on a 100-point string: " + stopwatch.Elapsed.ToString());
        }

        public void BenchmarkDisassemble(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                Store(i.ToString(), (i + 1).ToString());
            }
            Stopwatch stopwatch = new Stopwatch();
            Dump("Current time: " + System.DateTime.Now.ToShortTimeString() + ", Stopwatch START!");
            stopwatch.Start();

            Disassemble(Storage);

            stopwatch.Stop();
            Clear();
            Dump("Bench of Disassemble: Time elapsed over ONE string with size of " + iterations + " data points: " + stopwatch.Elapsed.ToString());
            stopwatch.Reset();
            StringBuilder t = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                t.Append(i.ToString()).Append(ID_D_DELIM).Append(i + 1.ToString()).Append(D_D_DELIM);
            }
            Dump("Current time: " + System.DateTime.Now.ToShortTimeString() + ", Stopwatch START!");
            stopwatch.Start();

            for (int i = 0; i < iterations; i++)
            {
                Disassemble(t.ToString());
            }

            stopwatch.Stop();
            Dump("Bench of Disassemble: Time elapsed over " + iterations + " iterations on a 10-point string: " + stopwatch.Elapsed.ToString());
            stopwatch.Reset();
            t = new StringBuilder();
            for (int i = 0; i < 100; i++)
            {
                t.Append(i.ToString()).Append(ID_D_DELIM).Append(i + 1.ToString()).Append(D_D_DELIM);
            }
            Dump("Current time: " + System.DateTime.Now.ToShortTimeString() + ", Stopwatch START!");
            stopwatch.Start();

            for (int i = 0; i < iterations; i++)
            {
                Disassemble(t.ToString());
            }

            stopwatch.Stop();
            Dump("Bench of Disassemble: Time elapsed over " + iterations + " iterations on a 100-point string: " + stopwatch.Elapsed.ToString());
        }

        public void BenchmarkDisassembleOpt(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                Store(i.ToString(), (i + 1).ToString());
            }
            Stopwatch stopwatch = new Stopwatch();
            Dump("Current time: " + System.DateTime.Now.ToShortTimeString() + ", Stopwatch START!");
            stopwatch.Start();

            DisassembleOpt(Storage);

            stopwatch.Stop();
            Clear();
            Dump("Bench of Disassemble: Time elapsed over ONE string with size of " + iterations + " data points: " + stopwatch.Elapsed.ToString());
            stopwatch.Reset();
            StringBuilder t = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                t.Append(i.ToString()).Append(ID_D_DELIM).Append(i + 1.ToString()).Append(D_D_DELIM);
            }
            Dump("Current time: " + System.DateTime.Now.ToShortTimeString() + ", Stopwatch START!");
            stopwatch.Start();

            for (int i = 0; i < iterations; i++)
            {
                DisassembleOpt(t.ToString());
            }

            stopwatch.Stop();
            Dump("Bench of Disassemble: Time elapsed over " + iterations + " iterations on a 10-point string: " + stopwatch.Elapsed.ToString());
            stopwatch.Reset();
            t = new StringBuilder();
            for (int i = 0; i < 100; i++)
            {
                t.Append(i.ToString()).Append(ID_D_DELIM).Append(i + 1.ToString()).Append(D_D_DELIM);
            }
            Dump("Current time: " + System.DateTime.Now.ToShortTimeString() + ", Stopwatch START!");
            stopwatch.Start();

            for (int i = 0; i < iterations; i++)
            {
                DisassembleOpt(t.ToString());
            }

            stopwatch.Stop();
            Dump("Bench of Disassemble: Time elapsed over " + iterations + " iterations on a 100-point string: " + stopwatch.Elapsed.ToString());
        }

        public void BenchmarkRead(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                Store(i.ToString(), (i + 1).ToString());
            }
            Stopwatch stopwatch = new Stopwatch();
            Dump("Current time: " + System.DateTime.Now.ToShortTimeString() + ", Stopwatch START!");
            stopwatch.Start();

            for (int i = 0; i < iterations; i++)
            {
                Read(i.ToString());
            }

            stopwatch.Stop();
            Dump("Bench of Read: Time elapsed over " + iterations + " iterations: " + stopwatch.Elapsed.ToString());
        }

        public void BenchmarkRemove(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                Store(i.ToString(), (i + 1).ToString());
            }
            Stopwatch sw = new Stopwatch();
            Dump("Current time: " + System.DateTime.Now.ToShortTimeString() + ", Stopwatch START!");
            sw.Start();

            for (int i = 0; i < iterations; i++)
            {
                Remove(i.ToString());
            }

            sw.Stop();
            Dump("Bench of Remove: Time elapsed over " + iterations + " iterations: " + sw.Elapsed.ToString());
        }

        public void BenchmarkUpdate(int iterations)
        {
            //prepare Storage with data
            for (int i = 0; i < iterations; i++)
            {
                Store(i.ToString(), (i + 1).ToString());
            }

            //prepare array of new data for Storage
            string[] newData = new string[iterations];
            for (int i = 0; i < iterations; i++)
            {
                newData[i] = (i*2).ToString();
            }

            Stopwatch sw = new Stopwatch();
            Dump("Current time: " + System.DateTime.Now.ToShortTimeString() + ", Stopwatch START!");
            sw.Start();

            for (int i = 0; i < iterations; i++)
            {
                Update(i.ToString(), newData[i]);
            }

            sw.Stop();
            Dump("Bench of Update: Time elapsed over " + iterations + " iterations: " + sw.Elapsed.ToString());
        }

        public void BenchmarkUpdateOpt(int iterations)
        {
            //prepare Storage with data
            for (int i = 0; i < iterations; i++)
            {
                Store(i.ToString(), (i + 1).ToString());
            }

            //prepare array of new data for Storage
            string[] newData = new string[iterations];
            for (int i = 0; i < iterations; i++)
            {
                newData[i] = (i * 2).ToString();
            }

            Stopwatch sw = new Stopwatch();
            Dump("Current time: " + System.DateTime.Now.ToShortTimeString() + ", Stopwatch START!");
            sw.Start();

            for (int i = 0; i < iterations; i++)
            {
                UpdateOpt(i.ToString(), newData[i]);
            }

            sw.Stop();
            Dump("Bench of Optimized Update: Time elapsed over " + iterations + " iterations: " + sw.Elapsed.ToString());
        }

        #endregion

        #region debug methods

        public void Dump(string d){
            Debug.WriteLine(d);
        }

        public void DumpA(string[] d)
        {
            foreach (string l in d)
            {
                Debug.WriteLine(l);
            }
        }

        public void DumpA2(string[,] d)
        {
            for (int i = 0; i < d.GetLength(0); i++)
            {
                Debug.WriteLine("id:   " + d[i, 0]);
                Debug.WriteLine("data: " + d[i, 1]);
            }
        }

        public void dumpD(Dictionary<string, string> d)
        {
            foreach(string id in d.Keys)
            {
                Debug.WriteLine("id:   " + id);
                Debug.WriteLine("data: " + d[id]);
            }
        }

        #endregion
    }
}
