using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            for (; ; )
            {
                var start = DateTime.Now;
                Console.WriteLine("Compiler ver." + Root.VERSION);
                int success = 0;
                var projs = Project.GetProjects(GetCurrentDirectory());
                var verbose = projs.Length <= 10;
                foreach (var proj in projs)
                {
                    Console.WriteLine();
                    proj.WriteLine();
                    var result = proj.Compile(verbose);
                    result.WriteLine();
                    if (result.Exception == null)
                    {
                        success++;
                        if (proj.IsAnonymous) Process.Start(result.Exe);
                    }
                }
                Console.WriteLine();
                Console.WriteLine("全プロジェクト: {0}, 成功: {1}, 失敗: {2}",
                    projs.Length, success, projs.Length - success);
                var time = (DateTime.Now - start).TotalMilliseconds;
                Console.WriteLine("所要時間: {0}ms", time);
                Console.WriteLine();
                Console.WriteLine("[Enter] で再度コンパイルします。");
                Console.WriteLine("終了する場合はウィンドウを閉じてください。");
                Console.ReadLine();
            }
        }

        public static String GetCurrentDirectory()
        {
            return Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly()
                .ManifestModule.FullyQualifiedName);
        }
    }
}
