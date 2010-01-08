using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Girl.X86;

namespace Sample
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
#if DEBUG
            I386.Test();
            MMX.Test();
            SSE2.Test();
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}