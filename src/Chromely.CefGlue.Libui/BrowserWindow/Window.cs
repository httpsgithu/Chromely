// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Window.cs" company="Chromely Projects">
//   Copyright (c) 2017-2018 Chromely Projects
// </copyright>
// <license>
//      See the LICENSE.md file in the project root for more information.
// </license>
// --------------------------------------------------------------------------------------------------------------------

namespace Chromely.CefGlue.Libui.BrowserWindow
{
    using System;

    using Chromely.CefGlue.Libui.Browser;
    using Chromely.Core;
    using Xilium.CefGlue;

    /// <summary>
    /// The window.
    /// </summary>
    public class Window : NativeWindow
    {
        /// <summary>
        /// The host/app/window application.
        /// </summary>
        private readonly HostBase mApplication;

        /// <summary>
        /// The host config.
        /// </summary>
        private readonly ChromelyConfiguration mHostConfig;

        /// <summary>
        /// The CefGlueBrowser object.
        /// </summary>
        private CefGlueBrowser mBrowser;

        /// <summary>
        /// The browser window handle.
        /// </summary>
        private IntPtr mBrowserWindowHandle;

        private IntPtr newHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        /// <param name="application">
        /// The application.
        /// </param>
        /// <param name="hostConfig">
        /// The host config.
        /// </param>
        public Window(HostBase application, ChromelyConfiguration hostConfig)
            : base(hostConfig)
        {
            mHostConfig = hostConfig;
            mBrowser = new CefGlueBrowser(this, hostConfig, new CefBrowserSettings());
            mBrowser.Created += OnBrowserCreated;
            mApplication = application;

            // Set event handler
            mBrowser.SetEventHandlers();

            ShowWindow();
        }

        /// <summary>
        /// The browser.
        /// </summary>
        public CefGlueBrowser Browser => mBrowser;

        #region Dispose

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            if (mBrowser != null)
            {
                mBrowser.Dispose();
                mBrowser = null;
                mBrowserWindowHandle = IntPtr.Zero;
            }
        }

        #endregion Dispose

        /// <summary>
        /// The on realized.
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
        /// <exception cref="NotSupportedException">
        /// Exception returned for MacOS not supported.
        /// </exception>
        protected override void OnCreate(IntPtr hwnd, int width, int height)
        {
            var windowInfo = CefWindowInfo.Create();
            windowInfo.SetAsChild(hwnd, new CefRectangle(0, 0, mHostConfig.HostWidth, mHostConfig.HostHeight));
            mBrowser.Create(windowInfo);
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
        protected override void OnSize(int width, int height)
        {
            //https://magpcss.org/ceforum/viewtopic.php?f=7&t=452
            /*
             *I think it's better to use the OS API functions for this.
             * You can retrieve the window handle by calling CefBrowser::GetWindowHandle().
             * It's up to the application to determine if SetWindowPos() or DeferWindowPos()
             * is more appropriate.
             */
            if (mBrowserWindowHandle != IntPtr.Zero)
            {
                SetSize(mBrowserWindowHandle, width, height);
            }
        }

        /// <summary>
        /// The on exit.
        /// </summary>
        protected override void OnExit()
        {
            mApplication.Quit();
        }

        /// <summary>
        /// The browser created.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnBrowserCreated(object sender, EventArgs e)
        {
            mBrowserWindowHandle = mBrowser.CefBrowser.GetHost().GetWindowHandle();
            var size = GetClientSize();
            if (mBrowserWindowHandle != IntPtr.Zero)
            {
                SetSize(mBrowserWindowHandle, size.Width, size.Height);
            }
        }

        private void SetSize(IntPtr handle, int width, int height)
        {
            switch (HostRuntime.HostPlatform)
            {
                case CefRuntimePlatform.Windows:
                    SetSizeForWin(handle, width, height);
                    break;
                case CefRuntimePlatform.Linux:
                    NativeMethods.uiWindowSetContentSize(Handle, width, height);
                    break;
                case CefRuntimePlatform.MacOSX:
                    NativeMethods.uiWindowSetContentSize(Handle, width, height);
                    break;
            }
        }

        private void SetSizeForWin(IntPtr handle, int width, int height)
        {
            if (width == 0 && height == 0)
            {
                // For windowed browsers when the frame window is minimized set the
                // browser window size to 0x0 to reduce resource usage.
                NativeMethods.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, NativeMethods.SetWindowPosFlags.NoZOrder | NativeMethods.SetWindowPosFlags.NoMove | NativeMethods.SetWindowPosFlags.NoActivate);
            }
            else
            {
                NativeMethods.SetWindowPos(handle, IntPtr.Zero, 0, 0, width, height, NativeMethods.SetWindowPosFlags.NoZOrder);
            }
        }
    }
}
