namespace Chromely.CefGlue.Libui.BrowserWindow
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Chromely.Core.Infrastructure;

    using Xilium.CefGlue;

    public static class HostRuntime
    {
        private static string CurrentNativeDllFile = string.Empty;

        public static CefRuntimePlatform HostPlatform { get; set; } = CefRuntimePlatform.Windows;

        private static string NativeDllFile
        {
            get
            {
                var isWin = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
                var isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

                if (isWin)
                {
                    HostPlatform = CefRuntimePlatform.Windows;
                    return "chromely_libui.dll";
                }

                if (isLinux)
                {
                    HostPlatform = CefRuntimePlatform.Linux;
                    return "chromely_libui.so";
                }

                if (isMac)
                {
                    HostPlatform = CefRuntimePlatform.MacOSX;
                    return "chromely_libui.dylib";
                }

                HostPlatform = CefRuntimePlatform.Windows;
                return "chromely_libui.dll";
            }
        }

        private static string[] AllFileNames { get; } = new[] { "chromely_libui.dll", "chromely_libui.so", "chromely_libui.dylib" };

        public static void Load()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var directory = Path.GetDirectoryName(assembly.Location);
            CurrentNativeDllFile = Path.Combine(directory ?? "chromely_libui.dll", NativeDllFile);

            if (File.Exists(CurrentNativeDllFile))
            {
                return;
            }

            Task.Run(() =>
                {
                    // Delete all files that are not expected current native file
                    var otherFiles = AllFileNames.Where(
                        x => !x.Equals(NativeDllFile, StringComparison.InvariantCultureIgnoreCase));

                    foreach (var item in otherFiles)
                    {
                        var otherFile = Path.Combine(directory ?? Environment.CurrentDirectory, item);
                        if (File.Exists(otherFile))
                        {
                            File.Delete(otherFile); 
                        }
                    }

                    string resourcePath = $"Chromely.CefGlue.Libui.BrowserWindow.native.{NativeDllFile}";
                    using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath))
                    {
                        using (var file = new FileStream(CurrentNativeDllFile, FileMode.Create, FileAccess.Write))
                        {
                            resource?.CopyTo(file);
                        }
                    }
                });
        }

        public static void EnsureNativeFileExists()
        {
            if (string.IsNullOrEmpty(CurrentNativeDllFile))
            {
                return;
            }

            var timeout = DateTime.Now.Add(TimeSpan.FromSeconds(30));

            while (!File.Exists(CurrentNativeDllFile))
            {
                if (DateTime.Now > timeout)
                {
                    Log.Error($"File {CurrentNativeDllFile} does not exist.");
                    return;
                }

                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }
    }
}
