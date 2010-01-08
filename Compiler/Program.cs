using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Girl.LLPML;
using Girl.PE;
using Girl.X86;

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
            for (; ; )
            {
                Compile();
                Console.WriteLine();
                Console.WriteLine("[Enter] で再度コンパイルします。");
                Console.WriteLine("中断する場合はウィンドウを閉じてください。");
                Console.ReadLine();
            }
        }

        static void Compile()
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath));
            var sources = dir.GetFiles("*.adm");
            if (sources.Length == 0)
            {
                Console.WriteLine("ソースがありません。");
                return;
            }

            DateTime start = DateTime.Now;
            try
            {
                var root = new Root();

                Console.WriteLine("パースしています: 標準ライブラリ");
                ReadLibraries(root);

                foreach (var src in sources)
                {
                    Console.WriteLine("パースしています: {0}", src.Name);
                    var sr = src.OpenText();
                    var text = sr.ReadToEnd();
                    sr.Close();
                    root.ReadText(src.Name, text);
                }

                var main = root.GetFunction("main");
                if (main == null)
                {
                    Console.WriteLine("main() がありません。");
                    return;
                }

                if (main.HasRetVal)
                    root.ReadText("CRT", "return main();");
                else
                    root.ReadText("CRT", "main();");

                var module = new Module();
                module.Specific.SubSystem = root.Subsystem;

                Console.WriteLine("コンパイルしています。");
                var codes = new OpModule(module);
                root.AddCodes(codes);
                module.Text.OpCodes = codes.ToArray();

                Console.WriteLine("リンクしています。");
                var exe = Path.Combine(dir.FullName, root.Output);
                module.Link(exe);

                var time = (DateTime.Now - start).TotalMilliseconds;
                Console.WriteLine("出力しました: {0}", root.Output);
                Console.WriteLine("所要時間: {0}ms", time);
                Process.Start(exe);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void ReadLibraries(Root root)
        {
            root.ReadText("IO", Properties.Resources.IO);
            root.ReadText("Memory", Properties.Resources.Memory);
            root.ReadText("JIT", Properties.Resources.JIT);
            root.ReadText("ArrayList", Properties.Resources.ArrayList);
            root.ReadText("Binary", Properties.Resources.Binary);
            root.ReadText("CriticalSection", Properties.Resources.CriticalSection);
            root.ReadText("GCList", Properties.Resources.GCList);
            root.ReadText("String", Properties.Resources.String);
            root.ReadText("Type", Properties.Resources.Type);
        }
    }
}
