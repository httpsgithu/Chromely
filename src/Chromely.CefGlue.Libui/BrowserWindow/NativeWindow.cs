// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeWindow.cs" company="Chromely Projects">
//   Copyright (c) 2017-2018 Chromely Projects
// </copyright>
// <license>
//      See the LICENSE.md file in the project root for more information.
// </license>
// --------------------------------------------------------------------------------------------------------------------

namespace Chromely.CefGlue.Libui.BrowserWindow
{
    using System;
    using System.Drawing;
    using Chromely.Core;

    /// <summary>
    /// The native window.
    /// </summary>
    public class NativeWindow
    {
        /// <summary>
        /// The m host config.
        /// </summary>
        private readonly ChromelyConfiguration mHostConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeWindow"/> class.
        /// </summary>
        public NativeWindow()
        {
            Handle = IntPtr.Zero;
         }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeWindow"/> class.
        /// </summary>
        /// <param name="hostConfig">
        /// Chromely configuration.
        /// </param>
        public NativeWindow(ChromelyConfiguration hostConfig)
        {
            Handle = IntPtr.Zero;
            mHostConfig = hostConfig;
        }

        /// <summary>
        /// Gets the handle.
        /// </summary>
        public static IntPtr Handle { get; private set; }

        /// <summary>
        /// The run message loop.
        /// </summary>
        public static void RunMessageLoop()
        {
            Application.Main();
        }

        /// <summary>
        /// The exit.
        /// </summary>
        public static void Exit()
        {
            Application.Quit();
        }

        /// <summary>
        /// The show window.
        /// </summary>
        public void ShowWindow()
        {
            CreateWindow();
        }

        /// <summary>
        /// The get client size.
        /// </summary>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        public Size GetClientSize()
        {
            var size = new Size();
            if (Handle != IntPtr.Zero)
            {
                int width;
                int height;
                NativeMethods.uiWindowContentSize(Handle, out width, out height);

                size.Width = width;
                size.Height = height;
            }

            return size;
        }

        /// <summary>
        /// The get window size.
        /// </summary>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        public Size GetWindowSize()
        {
            var size = new Size();
            if (Handle != IntPtr.Zero)
            {
                int width;
                int height;
                NativeMethods.uiWindowContentSize(Handle, out width, out height);

                size.Width = width;
                size.Height = height;
            }

            return size;
        }

        /// <summary>
        /// The center to screen.
        /// </summary>
        /// <param name="useWorkArea">
        /// The use work area.
        /// </param>
        public void CenterToScreen(bool useWorkArea = true)
        {
            //var monitor = User32Methods.MonitorFromWindow(Handle, MonitorFlag.MONITOR_DEFAULTTONEAREST);
            //User32Helpers.GetMonitorInfo(monitor, out var monitorInfo);
            //var screenRect = useWorkArea ? monitorInfo.WorkRect : monitorInfo.MonitorRect;
            //var midX = screenRect.Width / 2;
            //var midY = screenRect.Height / 2;
            //var size = GetWindowSize();
            //var left = midX - (size.Width / 2);
            //var top = midY - (size.Height / 2);

            //User32Methods.SetWindowPos(
            //    Handle,
            //    IntPtr.Zero,
            //    left,
            //    top,
            //    -1,
            //    -1,
            //    WindowPositionFlags.SWP_NOACTIVATE | WindowPositionFlags.SWP_NOSIZE | WindowPositionFlags.SWP_NOZORDER);
        }

        /// <summary>
        /// The on create.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        protected virtual void OnCreate(IntPtr hwnd, int width, int height)
        {
        }

        /// <summary>
        /// The on size.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        protected virtual void OnSize(int width, int height)
        {
        }

        /// <summary>
        /// The on exit.
        /// </summary>
        protected virtual void OnExit()
        {
        }

        /// <summary>
        /// The create window.
        /// </summary>
        private void CreateWindow()
        {
            Handle = NativeMethods.uiNewWindow(mHostConfig.HostTitle, mHostConfig.HostWidth, mHostConfig.HostHeight, false);

            var cefContentHandle = NativeMethods.uiCefContentHandle(Handle);
            OnCreate(cefContentHandle, mHostConfig.HostWidth, mHostConfig.HostHeight);

            NativeMethods.uiWindowOnContentSizeChanged(Handle, (b, f) =>
            { OnSizeChanged(new EventArgs()); }, IntPtr.Zero);

            NativeMethods.uiWindowOnClosing(Handle, (b, f) =>
            { OnClosing(new EventArgs()); }, IntPtr.Zero);

            ShowWindow(Handle);
        }

        internal virtual void OnClosing(EventArgs e)
        {
            OnExit();
            Exit();
        }

        internal virtual void OnSizeChanged(EventArgs e)
        {
            int width;
            int height;
            NativeMethods.uiWindowContentSize(Handle, out width, out height);
            OnSize(width, height);
        }

        /// <summary>
        /// The get icon handle.
        /// </summary>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        private IntPtr GetIconHandle()
        {
            //IntPtr? hIcon = NativeMethods.LoadIconFromFile(this.mHostConfig.HostIconFile);
            //if (hIcon == null)
            //{
            //    return User32Helpers.LoadIcon(IntPtr.Zero, SystemIcon.IDI_APPLICATION);
            //}

            //return hIcon.Value;

            return IntPtr.Zero;
        }

        internal static void ShowWindow(IntPtr handle)
        {
            NativeMethods.uiControlShow(handle);
        }

        internal static void SetWindowSize(IntPtr handle, int width, int height)
        {
            NativeMethods.uiWindowSetContentSize(handle, width, height);
        }

        public static void Destroy(IntPtr handle)
        {
            NativeMethods.uiControlDestroy(handle);
        }

        public static void Dispose(IntPtr handle)
        {
            Destroy(handle);
        }
    }
}