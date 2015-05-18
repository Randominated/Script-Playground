using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script_Playground

{
    class Program
    {
        //Storage is the only persistent variable
        public string Storage { get; set; }
        
        private const char D_D_DELIM = '|';
        private const char ID_D_DELIM = '§';

        private Dictionary<string, string> ReadBuffer;

        static void Main(string[] args)
        {
            Program p = new Program();

            if (Object.Equals(p.ReadBuffer, default(Dictionary<string, string>)))
            {
                p.CreateReadBuffer();
                p.PopulateReadBuffer();
            }

            #region TestDefinitions

            /*
            p.TestStore();
            p.Dump("---");
            p.BenchmarkStore(10);
            p.Dump("---");
            */

            /*
            p.TestAssemble();
            p.Dump("---");
            p.BenchmarkAssemble(100000);
            */

            /*
            p.TestDisassemble();
            p.Dump("---");
            p.BenchmarkDisassemble(10000, false);
            p.Dump("---");
            p.BenchmarkDisassemble(10000, true);
            */
            
            /*
            p.TestRead();
            p.Dump("---");
            p.BenchmarkRead(100000);
            p.Dump("---");
            */
            
            /*
            //100 000 iterations go for 30 mins
            p.TestRetrieve();
            p.Dump("---");
            p.BenchmarkRetrieve(10000);
            p.Dump("---");
            */

            /*
            //100 000 iterations go for ~30 mins
            p.TestRemove();
            p.Dump("---");
            p.BenchmarkRemove(1000);
            p.Dump("---");
            */

            /*
            //100 000 iterations go for 30++++ mins
            p.TestUpdate();
            p.Dump("---");
            p.BenchmarkUpdate(10000);
            p.Dump("---");
            */

            #endregion
            #region old tests
            /*
            //TEST Store
            for (int i = 0; i < 10; i++ )
            {
                int data = i * 10;
                p.Store(i.ToString(), data.ToString());
            }
            
            //TEST add empty to check for corruption
            p.Store("1337", "");
            p.Store("101", "shit");
            p.Store("AirLevel", "42");

            //TEST unprocessed control
            p.Dump(p.Storage);

            //TEST of Assemble and Disassemble
            //EXPECTED: identical string as unprocessed control
            p.Dump(p.Assemble(p.Disassemble(p.Storage)));
            p.Dump("zero for identical - " + p.Storage.CompareTo(p.Assemble(p.Disassemble(p.Storage))).ToString());

            //TEST of Disassemble
            //EXPECTED exploded view of 2D array without delims
            p.DumpA2(p.Disassemble(p.Storage));
            
            //TEST of Read
            //EXPECTED with input id=4, test alg should return 70 and 42.
            p.Dump(p.Read("7"));
            p.Dump(p.Read("AirLevel"));

            //TEST of Retrieve
            //EXPECTED return shit and storage string should miss the entry for id "101"
            p.Dump(p.Storage);
            p.Dump(p.Retrieve("101"));
            p.Dump(p.Storage);
            
            //TEST of Remove
            //Expected storage string should miss the AirLevel variable
            p.Remove("AirLevel");
            p.Dump(p.Storage);
            
            //TEST of Update
            //Expected a new value for id 1337, original is "", new value is "CONGRATS, DONE, GG!"
            if (p.Update("1337", "CONGRATS, DONE, GG!"))
            {
                p.Dump(p.Storage);
            }
            else
            {
                p.Dump("something is wrong, id not in storage!");
            }
            */
            #endregion
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

        #region DEPRECATED Disassemble()
        private string[,] Disassemble(string d, bool splitversion)
        {
            //input compiled string, return string array
            string[] ds = d.Split(new char[] { D_D_DELIM }, StringSplitOptions.RemoveEmptyEntries);
            string[,] da = new string[ds.Length, 2];
            int s;

            if (!splitversion)
            {
                for (int i = 0; i < ds.Length; i++)
                {
                    s = ds[i].IndexOf(ID_D_DELIM);
                    if (s >= 0)
                    {
                        da[i, 0] = ds[i].Substring(0, s);
                        da[i, 1] = ds[i].Substring(s + 1);
                    }
                }
            }

            if (splitversion)
            {
                string[] t;
                for (int i = 0; i < ds.Length; i++)
                {
                    t = ds[i].Split(ID_D_DELIM);
                    for (int j = 0; j < t.Length; j++)
                    {
                        da[i, j] = t[j];
                    }
                }
            }
            return da;
        }
        #endregion

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

        private void Store(string id, string data)
        {
            Storage = new StringBuilder().Append(Storage).Append(id.ToString()).Append(ID_D_DELIM).Append(data.ToString()).Append(D_D_DELIM).ToString();
            ReadBuffer[id] = data;
        }

        #region DEPRECATED Store()
        private void Store(string id, string data, bool plusversion)
        {
            if (!plusversion)
            {
                Storage = new StringBuilder().Append(Storage).Append(id.ToString()).Append(ID_D_DELIM).Append(data.ToString()).Append(D_D_DELIM).ToString();
            }
            if (plusversion)
            {
                Storage = new StringBuilder().Append(Storage + id.ToString() + ID_D_DELIM + data.ToString() + D_D_DELIM).ToString();
            }
        }
        #endregion

        private string Retrieve(string id)
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

        private void Remove(string id)
        {
            //input id, disassemble, remove data, assemble AND SAVE storage
            string[,] da = Disassemble(Storage);
            for (int i = 0; i < da.GetLength(0); i++)
            {
                if (da[i, 0].Equals(id))
                {
                    da[i, 0] = null;
                    da[i, 1] = null;
                }
            }
            Storage = Assemble(da);

            ReadBuffer.Remove(id);
        }

        private string Read(string id)
        {
            string s = null;
            ReadBuffer.TryGetValue(id, out s);
            return s;
        }

        #region DEPRECATED Unbuffered Read
        private string UBRead(string id)
        {
            //input id, disassemble, get data, return data
            string[,] da = Disassemble(Storage);
            for (int i = 0; i < da.GetLength(0); i++)
            {
                if (da[i, 0].Equals(id))
                {
                    return da[i, 1];
                }
            }
            return null;
        }

        #endregion

        private void CreateReadBuffer()
        {
            //Initialize a Dictionay to serve as a Read() buffer
            ReadBuffer = new Dictionary<string, string>();
        }

        private void PopulateReadBuffer()
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
            bool c = false;
            string[,] da = Disassemble(Storage);
            for (int i = 0; i < da.GetLength(0); i++)
            {
                if (da[i, 0].Equals(id))
                {
                    da[i, 1] = data;
                    Storage = Assemble(da);
                    c = true;
                    break;
                }
            }

            ReadBuffer[id] = data;

            return c;
        }

        #region test methods

        private void TestStore()
        {
            Store("test", "this is a test");
            Dump(Storage);
            if (Storage.Equals("test§this is a test|"))
            {
                Dump("Test of Store() successful!");
            }
        }

        private void TestAssemble()
        {
            string s = Assemble(new string[,] { { "1", "2" }, { "3", "4" }, { "5", "6" } });
            Dump(s);
            if (s.Equals("1§2|3§4|5§6|"))
            {
                Dump("Test of Assemble() successful!");
            }
        }

        private void TestDisassemble()
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

        private void TestRead()
        {
            Store("airLevel", "wan milian perzint!");
            Dump(Read("airLevel"));
            if (Read("airLevel").Equals("wan milian perzint!"))
            {
                Dump("Test of Read() successful!");
            }
        }

        private void TestRetrieve()
        {
            Store("Remove me", "NO! I wish to be retrieved!");
            Dump(Retrieve("Remove me"));
            if (Storage.Length == 0 && ReadBuffer.Count == 0)
            {
                Dump("Test of Retrieve() successful!");
            }
        }

        private void TestRemove()
        {
            Store("Fuck you", "I'm a bad influence and need to be purged!");
            Remove("Fuck you");
            if (Storage.Length == 0 && ReadBuffer.Count == 0)
            {
                Dump("Test of Remove() successful!");
            }
        }

        private void TestUpdate()
        {
            Store("The times", "Is a magazine for old ppl");
            Update("The times", "They are 'a changin'");
            if (Read("The times").Equals("They are 'a changin'"))
            {
                Dump("Test of Update() successful!");
            }
        }

        #endregion

        #region benchmark methods

        private void BenchmarkStore(int iterations)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < iterations; i++)
            {
                Store(i.ToString(), (i + 1).ToString());
            }

            stopwatch.Stop();
            Dump("Bench of Store: Time elapsed over " + iterations + " iterations: " + stopwatch.Elapsed.ToString());
        }

        private void BenchmarkAssemble(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                Store(i.ToString(), (i + 1).ToString());
            }
            string[,] ta = Disassemble(Storage);

            Stopwatch stopwatch = new Stopwatch();
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
            stopwatch.Start();

            for (int i = 0; i < iterations; i++)
            {
                Disassemble(t.ToString());
            }

            stopwatch.Stop();
            Dump("Bench of Assemble: Time elapsed over " + iterations + " iterations on a 100-point string: " + stopwatch.Elapsed.ToString());
        }

        private void BenchmarkDisassemble(int iterations, bool splitversion)
        {
            for (int i = 0; i < iterations; i++)
            {
                Store(i.ToString(), (i + 1).ToString());
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Disassemble(Storage, splitversion);

            stopwatch.Stop();
            Dump("Bench of Disassemble: Time elapsed over ONE string with size of " + iterations + " data points: " + stopwatch.Elapsed.ToString() + ", splitversion " + splitversion);
            stopwatch.Reset();
            StringBuilder t = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                t.Append(i.ToString()).Append(ID_D_DELIM).Append(i + 1.ToString()).Append(D_D_DELIM);
            }
            Dump("Working on set: " + t.ToString());
            stopwatch.Start();

            for (int i = 0; i < iterations; i++)
            {
                Disassemble(t.ToString());
            }

            stopwatch.Stop();
            Dump("Bench of Disassemble: Time elapsed over " + iterations + " iterations on a 10-point string: " + stopwatch.Elapsed.ToString() + ", splitversion " + splitversion);
            stopwatch.Reset();
            t = new StringBuilder();
            for (int i = 0; i < 100; i++)
            {
                t.Append(i.ToString()).Append(ID_D_DELIM).Append(i + 1.ToString()).Append(D_D_DELIM);
            }
            stopwatch.Start();

            for (int i = 0; i < iterations; i++)
            {
                Disassemble(t.ToString());
            }

            stopwatch.Stop();
            Dump("Bench of Disassemble: Time elapsed over " + iterations + " iterations on a 100-point string: " + stopwatch.Elapsed.ToString() + ", splitversion " + splitversion);
        }

        private void BenchmarkRead(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                Store(i.ToString(), (i + 1).ToString());
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < iterations; i++)
            {
                Read(i.ToString());
            }

            stopwatch.Stop();
            Dump("Bench of Read: Time elapsed over " + iterations + " iterations: " + stopwatch.Elapsed.ToString());
        }

        private void BenchmarkRetrieve(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                Store(i.ToString(), (i + 1).ToString());
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < iterations; i++)
            {
                Retrieve(i.ToString());
            }

            sw.Stop();
            Dump("Bench of Retrieve: Time elapsed over " + iterations + " iterations: " + sw.Elapsed.ToString());
        }

        private void BenchmarkRemove(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                Store(i.ToString(), (i + 1).ToString());
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < iterations; i++)
            {
                Remove(i.ToString());
            }

            sw.Stop();
            Dump("Bench of Remove: Time elapsed over " + iterations + " iterations: " + sw.Elapsed.ToString());
        }

        private void BenchmarkUpdate(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                Store(i.ToString(), (i + 1).ToString());
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < iterations; i++)
            {
                Update(i.ToString(), (i+1).ToString());
            }

            sw.Stop();
            Dump("Bench of Update: Time elapsed over " + iterations + " iterations: " + sw.Elapsed.ToString());
        }

        #endregion

        #region debug methods

        private void Dump(string d){
            Debug.WriteLine(d);
        }

        private void DumpA(string[] d)
        {
            foreach (string l in d)
            {
                Debug.WriteLine(l);
            }
        }

        private void DumpA2(string[,] d)
        {
            for (int i = 0; i < d.GetLength(0); i++)
            {
                Debug.WriteLine("id:   " + d[i, 0]);
                Debug.WriteLine("data: " + d[i, 1]);
            }
        }

        #endregion
    }
}
