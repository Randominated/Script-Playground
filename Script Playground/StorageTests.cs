using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script_Playground
{
    class StorageTests
    {
        static void Main(string[] args)
        {
            StringStorage p = new StringStorage();

            #region TestDefinitions

            /*
            //100 000 iterations go for ~1min 40sec
            p.TestStore();
            p.Dump("---");
            p.BenchmarkStore(10000);
            p.Dump("---");
            */

            /*
            //100 000 iterations go for <1sec
            p.TestAssemble();
            p.Dump("---");
            p.BenchmarkAssemble(100000);
            p.Dump("---");
            */

            /*
            //100 000 iterations go for <1sec
            p.TestAssembleOpt();
            p.Dump("---");
            p.BenchmarkAssembleOpt(100000);
            p.Dump("---");
            */

            /*
            //100 000 iterations go for <1sec
            p.TestDisassemble();
            p.Dump("---");
            p.BenchmarkDisassemble(10000);
            p.Dump("---");
            */

            /*
            //100 000 iterations go for ~1.16 sec
            p.TestDisassembleOpt();
            p.Dump("---");
            p.BenchmarkDisassembleOpt(100000);
            p.Dump("---");
            */

            /*
            //100 000 iterations go for <0.02sec
            p.TestRead();
            p.Dump("---");
            p.BenchmarkRead(10000);
            p.Dump("---");
            */

            /*
            //This version uses unoptimized Assemble() and Disassemble()!!!
            //100 000 iterations go for ~30 mins
            p.TestRemove();
            p.Dump("---");
            p.BenchmarkRemove(10000);
            p.Dump("---");
            */

            /*
            //This version uses unoptimized Assemble() and Disassemble()!!!
            //100 000 iterations go for 30++++ mins
            p.Dump("Testing Update...");
            p.TestUpdate();
            p.Dump("---");
            p.BenchmarkUpdate(1000);
            p.Dump("---");
            */

            /*
            //100 000 iterations: 00:06:52 with Ordinal!!
            p.Dump("Testing UpdateOpt...");
            p.TestUpdateOpt();
            p.Dump("---");
            p.BenchmarkUpdateOpt(1000);
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
    }
}
