// Assets/Scripts/Utils/DebugCheckpoint.cs
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Lightweight checkpoints & tracing.
/// Enable/disable globally by adding/removing "CHK" in:
/// Project Settings → Player → Other Settings → Scripting Define Symbols.
/// Usage:
///   DebugCheckpoint.Hit("TrainWarrior clicked");
///   using (new DebugCheckpoint.Scope("Spawn sequence")) { ... }
/// </summary>
public static class DebugCheckpoint
{
    // Single line marker
    public static void Hit(
        string id = "",
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
#if CHK
        Debug.Log($"[CHK] {System.IO.Path.GetFileName(file)}:{line}::{member} {id}");
#endif
    }

    // Single line marker that logs only once per call site
    private static readonly HashSet<string> _once = new HashSet<string>();

    public static void HitOnce(
        string id = "",
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
#if CHK
        string key = $"{file}:{line}:{member}:{id}";
        if (_once.Add(key))
        {
            Debug.Log($"[CHK-ONCE] {System.IO.Path.GetFileName(file)}:{line}::{member} {id}");
        }
#endif
    }

    /// <summary>
    /// Scope helper that logs on enter and exit.
    /// </summary>
    public sealed class Scope : IDisposable
    {
#if CHK
        private readonly string _msg;
        private readonly string _file;
        private readonly int _line;
        private readonly string _member;
#endif

        public Scope(
            string id = "",
            [CallerMemberName] string member = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0)
        {
#if CHK
            _msg = id; _file = file; _line = line; _member = member;
            Debug.Log($"[CHK>] {System.IO.Path.GetFileName(file)}:{line}::{member} {id}");
#endif
        }

        public void Dispose()
        {
#if CHK
            Debug.Log($"[CHK<] {System.IO.Path.GetFileName(_file)}:{_line}::{_member} {_msg}");
#endif
        }
    }
}
