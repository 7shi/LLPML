﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:2.0.50727.1433
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Compiler.Properties {
    using System;
    
    
    /// <summary>
    ///   ローカライズされた文字列などを検索するための、厳密に型指定されたリソース クラスです。
    /// </summary>
    // このクラスは StronglyTypedResourceBuilder クラスが ResGen
    // または Visual Studio のようなツールを使用して自動生成されました。
    // メンバを追加または削除するには、.ResX ファイルを編集して、/str オプションと共に
    // ResGen を実行し直すか、または VS プロジェクトをビルドし直します。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   このクラスで使用されているキャッシュされた ResourceManager インスタンスを返します。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Compiler.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   厳密に型指定されたこのリソース クラスを使用して、すべての検索リソースに対し、
        ///   現在のスレッドの CurrentUICulture プロパティをオーバーライドします。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   struct ArrayList
        ///{
        ///  var Ptr : var*;
        ///  int Count, Capacity;
        ///  
        ///  function ctor
        ///  {
        ///    Capacity = 16;
        ///    Ptr = calloc(Capacity, sizeof(var));
        ///    //Ptr = malloc(Capacity * sizeof(var));
        ///    Count = 0;
        ///  }
        ///  
        ///  function dtor
        ///  {
        ///    free(Ptr);
        ///  }
        ///  
        ///  function Clear
        ///  {
        ///    Count = 0;
        ///  }
        ///  
        ///  function Add(obj)
        ///  {
        ///    var cap = Capacity;
        ///    while (Count &gt;= Capacity)
        ///      Capacity += Capacity;
        ///    if (Capacity != cap)
        ///    {
        ///      var oldPtr = Ptr;
        ///      Ptr = calloc(Capacit [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ArrayList {
            get {
                return ResourceManager.GetString("ArrayList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   struct BinaryWriter
        ///{
        ///  var ptr : byte* = null;
        ///  var current = null, end = null;
        ///  
        ///  function Init(ptr, size)
        ///  {
        ///    current = this.ptr = ptr;
        ///    end = ptr + size;
        ///  }
        ///  
        ///  function WriteByte(b)
        ///  {
        ///    if (current + 1 &gt; end) return false;
        ///    ((byte*)current)[0] = b;
        ///    current++;
        ///    return true;
        ///  }
        ///  
        ///  function WriteShort(s)
        ///  {
        ///    if (current + 2 &gt; end) return false;
        ///    ((short*)current)[0] = s;
        ///    current += 2;
        ///    return true;
        ///  }
        ///  
        ///  function Write(v)
        ///  {
        ///    i [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string Binary {
            get {
                return ResourceManager.GetString("Binary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   extern &quot;kernel32.dll&quot; __stdcall
        ///{
        ///  InitializeCriticalSection(lpCriticalSection : CRITICAL_SECTION);
        ///  DeleteCriticalSection(lpCriticalSection : CRITICAL_SECTION);
        ///  EnterCriticalSection(lpCriticalSection : CRITICAL_SECTION);
        ///  LeaveCriticalSection(lpCriticalSection : CRITICAL_SECTION);
        ///  TryEnterCriticalSection(lpCriticalSection : CRITICAL_SECTION) : bool;
        ///}
        ///
        ///struct CRITICAL_SECTION
        ///{
        ///  var DebugInfo;
        ///  var LockCount;
        ///  var RecursionCount;
        ///  var OwningThread;
        ///  var LockSemaphore;
        ///  int Spin [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CriticalSection {
            get {
                return ResourceManager.GetString("CriticalSection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   class GCList : ArrayList
        ///{
        ///  function dtor
        ///  {
        ///    Clear();
        ///  }
        ///  
        ///  function Clear
        ///  {
        ///    ForEachRev(\obj =&gt; __dereference(obj));
        ///    base.Clear();
        ///  }
        ///  
        ///  function Add(obj)
        ///  {
        ///    __reference(obj);
        ///    base.Add(obj);
        ///  }
        ///  
        ///  function Remove(obj)
        ///  {
        ///    __dereference(obj);
        ///    return base.Remove(obj);
        ///  }
        ///  
        ///  function RemoveAt(order)
        ///  {
        ///    var ret = base.RemoveAt(order);
        ///    __reference(ret);
        ///    return ret;
        ///  }
        ///}
        /// に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string GCList {
            get {
                return ResourceManager.GetString("GCList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   extern &quot;kernel32.dll&quot; __stdcall
        ///  GetStdHandle(nStdHandle);
        ///extern &quot;kernel32.dll&quot; __stdcall __widecharset
        ///{
        ///  WriteConsole(hConsoleOutput, lpBuffer, nNumberOfCharsToWrite, lpNumberOfCharsWritten, lpReserved);
        ///  ReadConsole(hConsoleInput, lpBuffer, nNumberOfCharsToRead, lpNumberOfCharsRead, pInputControl);
        ///  lstrlen(lpString);
        ///}
        ///extern &quot;user32.dll&quot; __widecharset
        ///  wsprintf(lpOutput, lpFmt, args : params);
        ///extern &quot;user32.dll&quot; __stdcall __widecharset
        ///  wvsprintf(lpOutput, lpFmt, arglist);
        ///
        ///const i [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string IO {
            get {
                return ResourceManager.GetString("IO", resourceCulture);
            }
        }
        
        /// <summary>
        ///   const int _JIT_Page_Size    = 65536; // 64KB
        ///const int _JIT_Alloc_Size   = 64;    // 64バイト固定
        ///const int _JIT_Alloc_DWords = 16;
        ///const int _JIT_Alloc_Shift  = 6;
        ///
        ///CriticalSection _jit_critical_section;
        ///_JIT_Manager _jit_manager;
        ///
        ///function __jit_alloc(size)
        ///{
        ///  var ret = _jit_manager.Alloc(size);
        ///  //printfln(&quot;%s(%d) =&gt; %p&quot;, __FUNCTION__, size, ret);
        ///  return ret;
        ///}
        ///
        ///function __jit_free(ptr)
        ///{
        ///  //printfln(&quot;%s(%p)&quot;, __FUNCTION__, ptr);
        ///  return _jit_manager.Free(ptr);
        ///}
        ///
        ///function __jit_dup [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string JIT {
            get {
                return ResourceManager.GetString("JIT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   extern &quot;kernel32.dll&quot; __stdcall
        ///{
        ///  GetProcessHeap();
        ///  HeapAlloc(hHeap, dwFlags, dwBytes);
        ///  HeapReAlloc(hHeap, dwFlags, lpMem, dwBytes);
        ///  HeapFree(hHeap, dwFlags, lpMem);
        ///  HeapSize(hHeap, dwFlags, lpMem);
        ///}
        ///
        ///function __operator_new(type : Type, size, len, izer, ctor)
        ///{
        ///  var ptr;
        ///  if (len == -1)
        ///    ptr = calloc(1, size + 16);
        ///  else
        ///    ptr = calloc(1, (len + 1) * size + 16);
        ///  if (ptr == null) return null;
        ///  
        ///  var ret = ptr + 16;
        ///  var h = (var*)ptr;
        ///  h[0] = type;
        ///  h[1] = 1;
        /// [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string Memory {
            get {
                return ResourceManager.GetString("Memory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   extern &quot;kernel32.dll&quot; __stdcall
        ///{
        ///  WideCharToMultiByte(
        ///    CodePage, dwFlags, lpWideCharStr, cchWideChar,
        ///    lpMultiByteStr, cchMultiByte, lpDefaultChar, lpUsedDefaultChar);
        ///  MultiByteToWideChar(
        ///    CodePage, dwFlags, lpMultiByteStr, cchMultiByte,
        ///    lpWideCharStr, cchWideChar);
        ///}
        ///
        ///function memcpy(dest, src, count)
        ///{
        ///  if (dest &lt;= src || dest &gt;= src + count)
        ///    __memcpy(dest, src, count);
        ///  else
        ///    __memcpy_rev(dest + count - 1, src + count - 1, count);
        ///  return dest;
        ///}
        ///
        ///function  [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string String {
            get {
                return ResourceManager.GetString("String", resourceCulture);
            }
        }
        
        /// <summary>
        ///   class Type
        ///{
        ///  var Name : string;
        ///  var Destructor;
        ///  int Size;
        ///  var Base : Type;
        ///}
        ///
        ///function __type_is(a : Type, b : Type)
        ///{
        ///  //printfln(&quot;%s(%p{%s}, %p{%s})&quot;, __FUNCTION__, a, a.Name, b, b.Name);
        ///  if (b == null) return false;
        ///  while (a != null)
        ///  {
        ///    if (a == b) return true;
        ///    a = a.Base;
        ///  }
        ///  return false;
        ///}
        ///
        ///function __type_as(a, b : Type)
        ///{
        ///  //printfln(&quot;%s(%p, %p{%s})&quot;, __FUNCTION__, a, b, b.Name);
        ///  var h = (var*)(a - 16);
        ///  if (__type_is(h[0], b)) return a;
        ///  return  [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string Type {
            get {
                return ResourceManager.GetString("Type", resourceCulture);
            }
        }
    }
}
