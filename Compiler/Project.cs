using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Girl.LLPML;
using Girl.PE;

namespace Compiler
{
    public class Project
    {
        public static Project[] GetProjects(string path)
        {
            var projs = new Projects(path);
            var proj = new Project();
            proj.ReadDirectory(projs, new DirectoryInfo(path), "", false);
            if (proj.sources.Count > 0) projs.Add(proj);
            return projs.ToArray();
        }

        public string Name { get; private set; }
        public string BaseDir { get; set; }
        public bool IsAnonymous { get { return string.IsNullOrEmpty(Name); } }

        private List<AdmInfo> libsrcs = new List<AdmInfo>();
        private List<AdmInfo> sources = new List<AdmInfo>();

        public AdmInfo[] Sources
        {
            get
            {
                var ret = new List<AdmInfo>();
                ret.AddRange(libsrcs);
                ret.AddRange(sources);
                return ret.ToArray();
            }
        }

        public void WriteLine()
        {
            if (IsAnonymous)
                Console.WriteLine("メインプロジェクト");
            else
                Console.WriteLine("プロジェクト: {0}", Name);
        }

        public ResultInfo Compile(bool verbose)
        {
            var ret = new ResultInfo();
            var start = DateTime.Now;
            var root = new Root();
            if (!IsAnonymous) root.Output = Name + ".exe";
            var exe = Path.Combine(BaseDir, root.Output);
            if (File.Exists(exe))
            {
                var exet = File.GetLastWriteTime(exe);
                var nobuild = true;
                foreach (var src in Sources)
                {
                    if (src.FileInfo.LastWriteTime > exet)
                    {
                        nobuild = false;
                        break;
                    }
                }
                if (nobuild) return ret;
            }
#if !DEBUG
            try
#endif
            {
                foreach (var src in Sources)
                {
                    if (verbose)
                        Console.WriteLine("パースしています: {0}", src.FullName);
                    var sr = src.FileInfo.OpenText();
                    var text = sr.ReadToEnd();
                    sr.Close();
                    root.ReadText(src.Name, text);
                }

                var main = root.GetFunction("main");
                if (main != null)
                {
                    if (main.HasRetVal)
                        root.ReadText("CRT", "return main();");
                    else
                        root.ReadText("CRT", "main();");
                }

                var module = new Module();
                module.Specific.SubSystem = root.Subsystem;

                if (verbose) Console.WriteLine("コンパイルしています。");
                var codes = new OpModule(module);
                root.AddCodes(codes);
                module.Text.OpCodes = codes.ToArray();

                if (verbose) Console.WriteLine("リンクしています。");
                exe = Path.Combine(BaseDir, root.Output);
                module.Link(exe);

                ret.Exe = exe;
                ret.Output = root.Output;
                ret.Time = (DateTime.Now - start).TotalMilliseconds;
            }
#if !DEBUG
            catch (Exception ex)
            {
                ret.Exception = ex;
            }
#endif
            return ret;
        }

        private Project() { }
        private Project(Project parent, string name)
        {
            libsrcs.AddRange(parent.libsrcs);
            libsrcs.AddRange(parent.sources);
            Name = name;
        }

        private void ReadDirectory(Projects projs, DirectoryInfo dir, string path, bool lib)
        {
            var libs = new List<DirectoryInfo>();
            var exes = new List<DirectoryInfo>();
            var dirs = new List<DirectoryInfo>();
            foreach (var di in dir.GetDirectories())
            {
                if (di.Name.StartsWith("lib-"))
                    libs.Add(di);
                else if (di.Name.StartsWith("exe-"))
                    exes.Add(di);
                else
                    dirs.Add(di);
            }

            bool hassrc = false;
            var exes2 = new List<AdmInfo>();
            var srcs = new List<AdmInfo>();
            foreach (var fi in dir.GetFiles("*.adm"))
            {
                var ai = new AdmInfo(fi.Name.Substring(0, fi.Name.Length - 4), path, fi);
                if (ai.Name.StartsWith("exe-"))
                    exes2.Add(ai);
                else
                    srcs.Add(ai);
                hassrc = true;
            }

            foreach (var di in libs)
                ReadDirectory(projs, di, Combine(path, di.Name), true);
            foreach (var di in exes)
            {
                var proj = new Project(this, di.Name.Substring(4));
                proj.ReadDirectory(projs, di, Combine(path, di.Name), false);
                if (proj.sources.Count > 0) projs.Add(proj);
            }
            foreach (var di in dirs)
            {
                if (hassrc)
                    ReadDirectory(projs, di, Combine(path, di.Name), true);
                else
                {
                    var proj = new Project(this, di.Name);
                    proj.ReadDirectory(projs, di, Combine(path, di.Name), false);
                    if (proj.sources.Count > 0) projs.Add(proj);
                }
            }
            if (lib || exes2.Count > 0)
                libsrcs.AddRange(srcs);
            else
                sources.AddRange(srcs);
            foreach (var ai in exes2)
            {
                var proj = new Project(this, ai.Name.Substring(4));
                proj.sources.Add(ai);
                projs.Add(proj);
            }
        }

        public static string Combine(string path, string name)
        {
            if (string.IsNullOrEmpty(path)) return name;
            return path + "/" + name;
        }
    }

    public class Projects : List<Project>
    {
        public string BaseDir { get; private set; }

        public Projects(string baseDir)
        {
            BaseDir = baseDir;
        }

        public new void Add(Project proj)
        {
            proj.BaseDir = BaseDir;
            base.Add(proj);
        }
    }

    public class AdmInfo
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public FileInfo FileInfo { get; private set; }

        public AdmInfo(string name, string path, FileInfo fi)
        {
            Name = name;
            Path = path;
            FileInfo = fi;
        }

        public string FullName
        {
            get { return Project.Combine(Path, Name); }
        }
    }

    public class ResultInfo
    {
        public string Output, Exe;
        public double Time;
        public Exception Exception;

        public void WriteLine()
        {
            if (Exception != null)
                Console.WriteLine(Exception.Message);
            else if (string.IsNullOrEmpty(Output))
                Console.WriteLine("更新は不要です。");
            else
            {
                Console.WriteLine("出力しました: {0}", Output);
                Console.WriteLine("所要時間: {0}ms", Time);
            }
        }
    }
}
