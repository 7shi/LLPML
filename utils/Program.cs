using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Girl.LLPML;

namespace Compiler
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Console.WriteLine("Andromeda ver.{0}", Root.VERSION);
            var start = DateTime.Now;

            var tests = Directory.GetFiles("not-Tests");
            for (int i = 0; i < tests.Length; i++)
            {
                var adm = tests[i];
                if (Path.GetExtension(adm) == ".adm")
                    compile(adm);
            }

            Console.WriteLine();
            var end = DateTime.Now;
            Console.WriteLine("総所要時間: {0}", end - start);
            Console.WriteLine();
            Console.WriteLine("Press [Enter] to exit.");
            Console.ReadLine();
        }

        static void compile(string src)
        {
            var output = Path.ChangeExtension(src, ".exe");
            if (File.Exists(output)) return;

            Console.WriteLine();
            var s = DateTime.Now;

            Console.WriteLine("パースしています...");
            var root = new Root();
            readDir(root, "lib-Core");
            //readDir(root, "lib-System");
            parse(root, src);

            Console.WriteLine("コンパイルしています...");
            var module = new Girl.PE.Module();
            module.Specific.SubSystem = Girl.PE.IMAGE_SUBSYSTEM.WINDOWS_CUI;
            var codes = OpModule.Create(module);
            root.AddCodes(codes);
            module.Text.OpCodes = codes.ToArray();

            Console.WriteLine("リンクしています...");
            module.Link(output);
            Console.WriteLine("出力しました: {0}", output);

            var e = DateTime.Now;
            Console.WriteLine("所要時間: {0}", e - s);
        }

        static void parse(Root root, string src)
        {
            var src2 = src.Replace("\\", "/");
            //Console.WriteLine("パースしています: {0}", src2);
            using (var sr = new StreamReader(src))
                root.ReadText(src2, sr.ReadToEnd());
        }

        static void readDir(Root root, string dir)
        {
            var dirs = new List<string>(Directory.GetDirectories(dir));
            dirs.Sort(Compare);
            for (int i = 0; i < dirs.Count; i++)
                readDir(root, dirs[i]);
            var files = new List<string>(Directory.GetFiles(dir));
            files.Sort(Compare);
            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (Path.GetExtension(file).ToLower() == ".adm")
                    parse(root, file);
            }
        }

        static int Compare(string a, string b)
        {
            if (a == null)
            {
                if (b == null) return 0;
                return -1;
            }
            if (b == null) return 1;
            for (int i = 0; i < a.Length; i++)
            {
                if (i >= b.Length) return 1;
                int cmp = a[i] - b[i];
                if (cmp != 0) return Math.Sign(cmp);
            }
            return Math.Sign(a.Length - b.Length);
        }
    }
}
