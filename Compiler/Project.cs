using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Girl.LLPML;
using Girl.PE;

namespace Compiler
{
    public class Project
    {
        public static Projects GetProjects(string path)
        {
            var projs = new Projects(path);
            var proj = new Project();
            proj.ReadDirectory(projs, new DirectoryInfo(path), "", false);
            if (proj.sources.Count > 0) projs.Add(proj);
            return projs;
        }

        public string Name { get; private set; }
        public string BaseDir { get; set; }
        public bool IsAnonymous { get { return string.IsNullOrEmpty(Name); } }

        private List<AdmInfo> libsrcs = new List<AdmInfo>();
        private List<AdmInfo> sources = new List<AdmInfo>();
        private Dictionary<string, Project> generators = new Dictionary<string, Project>();

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
            string lastError = null;
            root.Error += ex =>
            {
                ret.Exceptions.Add(ex);
                if (ex.Message == lastError)
                    throw new Exception("エラーのため続行できません。");
                lastError = ex.Message;
            };
            if (!IsAnonymous) root.Output = Name + ".exe";
            ret.Output = root.Output;
            ret.Exe = Path.Combine(BaseDir, root.Output);
            ret.NoBuild = File.Exists(ret.Exe);
            if (ret.NoBuild)
            {
                var exet = File.GetLastWriteTime(ret.Exe);
                foreach (var src in Sources)
                {
                    if (src.NeedsBuild(exet))
                    {
                        ret.NoBuild = false;
                        break;
                    }
                }
                if (ret.NoBuild) return ret;
            }
#if !DEBUG
            try
#endif
            {
                foreach (var src in Sources)
                {
                    if (src.NeedsGenerate())
                    {
                        var p = new Process();
                        p.StartInfo.FileName = src.Generator + ".exe";
                        p.StartInfo.Arguments = "\"" + src.Source + "\"";
                        p.StartInfo.UseShellExecute = false;
                        if (verbose)
                            Console.WriteLine("生成しています: {0} => {1}",
                                src.SourceName, src.FileName);
                        if (p.Start()) p.WaitForExit();
                    }
                    if (verbose)
                        Console.WriteLine("パースしています: {0}", src.FileName);
                    var sr = src.FileInfo.OpenText();
                    var text = sr.ReadToEnd();
                    sr.Close();
                    root.ReadText(src.FileName, text);
                }

                var main = root.GetFunction("main");
                if (main != null)
                {
                    var call_main = "main();";
                    if (main.Args.Count == 1)
                        call_main = "main(__get_main_args());";
                    if (main.HasRetVal)
                        root.ReadText("CRT", "return " + call_main);
                    else
                        root.ReadText("CRT", call_main);
                }

                var module = new Module();
                module.Specific.SubSystem = root.Subsystem;

                if (verbose) Console.WriteLine("コンパイルしています。");
                var codes = OpModule.New(module);
                root.AddCodes(codes);
                if (ret.Exceptions.Count > 0) return ret;
                module.Text.OpCodes = codes.ToArray();

                if (verbose) Console.WriteLine("リンクしています。");
                ret.Exe = Path.Combine(BaseDir, root.Output);
                module.Link(ret.Exe);

                ret.Output = root.Output;
                ret.Time = (DateTime.Now - start).TotalMilliseconds;
            }
#if !DEBUG
            catch (Exception ex)
            {
                ret.Exceptions.Add(ex);
            }
#endif
            return ret;
        }

        private Project() { }
        private Project(string name) { Name = name; }

        private Project(Project parent, string name)
            : this(name)
        {
            Name = name;
            Set(parent);
        }

        private void Set(Project parent)
        {
            libsrcs.AddRange(parent.libsrcs);
            libsrcs.AddRange(parent.sources);
            foreach (var e in parent.generators)
                generators.Add(e.Key, e.Value);
        }

        private void ReadDirectory(Projects projs, DirectoryInfo dir, string path, bool lib)
        {
            var libs = new List<DirectoryInfo>();
            var exes = new List<DirectoryInfo>();
            var dirs = new List<DirectoryInfo>();
            var gens = new Dictionary<string, DirectoryInfo>();
            foreach (var di in dir.GetDirectories())
            {
                var dn = di.Name;
                if (dn.StartsWith("lib-"))
                    libs.Add(di);
                else if (dn.StartsWith("exe-"))
                    exes.Add(di);
                else if (dn.StartsWith("gen-"))
                {
                    var ext = "." + dn.Substring(4).ToLower();
                    gens.Add(ext, di);
                    generators.Add(ext, new Project(dn));
                }
                else if (!dn.StartsWith("not-"))
                    dirs.Add(di);
            }

            bool hassrc = false;
            var exes2 = new List<AdmInfo>();
            var srcs = new List<AdmInfo>();
            var without = new List<string>();
            foreach (var fi in dir.GetFiles("src-*.*"))
            {
                var ext = fi.Extension.ToLower();
                if (!generators.ContainsKey(ext)) continue;
                var name = Path.ChangeExtension(fi.Name, null);
                var adm = name + ".adm";
                without.Add(Combine(path, adm));
                var ai = new AdmInfo(name, path,
                    Path.Combine(fi.DirectoryName, adm));
                ai.Source = fi.FullName;
                ai.Generator = generators[ext].Name;
                srcs.Add(ai);
                hassrc = true;
                projs.Generated.Add(ai.FileName);
            }
            foreach (var fi in dir.GetFiles("*.adm"))
            {
                if (without.Contains(Combine(path, fi.Name))) continue;
                var name = Path.ChangeExtension(fi.Name, null);
                var ai = new AdmInfo(name, path, fi.FullName);
                if (name.StartsWith("exe-"))
                    exes2.Add(ai);
                else if (!name.StartsWith("not-"))
                    srcs.Add(ai);
                hassrc = true;
            }

            foreach (var di in libs)
                ReadDirectory(projs, di, Combine(path, di.Name), true);
            foreach (var e in gens)
            {
                var di = e.Value;
                var proj = generators[e.Key];
                proj.Set(this);
                proj.ReadDirectory(projs, di, Combine(path, di.Name), false);
                if (proj.sources.Count > 0) projs.Add(proj);
            }
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
        public List<string> Generated = new List<string>();

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

        private string fullPath;
        public FileInfo FileInfo { get { return new FileInfo(fullPath); } }

        public string Source { get; set; }
        public string Generator { get; set; }

        public AdmInfo(string name, string path, string fullPath)
        {
            Name = name;
            Path = path;
            this.fullPath = fullPath;
        }

        public string FileName
        {
            get { return Project.Combine(Path, FileInfo.Name); }
        }

        public string SourceName
        {
            get { return Project.Combine(Path, SourceFileInfo.Name); }
        }

        public FileInfo SourceFileInfo
        {
            get
            {
                if (Source == null) return null;
                return new FileInfo(Source);
            }
        }

        public bool NeedsBuild(DateTime exet)
        {
            if (NeedsGenerate()) return true;
            var fi = FileInfo;
            var sfi = SourceFileInfo;
            return (fi.Exists && fi.LastWriteTime > exet)
                || (sfi != null && sfi.LastWriteTime > exet);
        }

        public bool NeedsGenerate()
        {
            var sfi = SourceFileInfo;
            if (sfi == null) return false;

            var fi = FileInfo;
            if (!fi.Exists) return true;

            var last = fi.LastWriteTime;
            if (sfi.LastWriteTime > last) return true;
            if (File.GetLastWriteTime(Generator + ".exe") > last) return true;
            return false;
        }
    }

    public class ResultInfo
    {
        public bool NoBuild = true;
        public string Output, Exe;
        public double Time;
        public List<Exception> Exceptions = new List<Exception>();

        public void WriteLine()
        {
            if (Exceptions.Count > 0)
            {
                foreach (var ex in Exceptions)
                    Console.WriteLine(ex.Message);
            }
            else if (NoBuild)
                Console.WriteLine("更新は不要です。");
            else
            {
                Console.WriteLine("出力しました: {0}", Output);
                Console.WriteLine("所要時間: {0}ms", Time);
            }
        }
    }
}
