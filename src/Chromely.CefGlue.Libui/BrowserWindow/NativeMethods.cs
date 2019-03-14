namespace Chromely.CefGlue.Libui.BrowserWindow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    public static class NativeMethods
    {
        const string DllName = "chromely_libui";

        #region Application
        [StructLayout(LayoutKind.Sequential)]
        public struct uiInitOptions
        {
            public UIntPtr Size;
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern string uiInit(ref uiInitOptions options);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uiUninit();
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uiFreeInitError(string err);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uiMain();
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool uiMainStep(bool wait);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uiQuit();
        #endregion

        #region Control

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uiControlDestroy(IntPtr control);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr uiControlHandle(IntPtr control);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr uiControlParent(IntPtr control);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uiControlSetParent(IntPtr control, IntPtr parent);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool uiControlToplevel(IntPtr control);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool uiControlVisible(IntPtr control);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uiControlShow(IntPtr control);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uiControlHide(IntPtr control);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool uiControlEnabled(IntPtr control);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uiControlEnable(IntPtr control);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uiControlDisable(IntPtr control);
        #endregion

        #region Dialogs
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern string uiOpenFile(IntPtr window);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern string uiSaveFile(IntPtr window);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uiMsgBox(IntPtr window, string title, string desc);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uiMsgBoxError(IntPtr window, string title, string desc);
        #endregion

        #region Window
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern string uiWindowTitle(IntPtr control);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uiWindowSetTitle(IntPtr control, string title);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uiWindowSetChild(IntPtr window, IntPtr child);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uiWindowMargined(IntPtr control);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uiWindowSetMargined(IntPtr control, int margin);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr uiNewWindow(string title, int width, int height, bool hasMenubar);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr uiCefHostHandle(IntPtr w);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr uiCefContentHandle(IntPtr w);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr uiWindowSetContentSize(IntPtr w, int width, int height);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uiWindowOnClosing(IntPtr b, uiWindowOnClosingDelegate f, IntPtr data);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uiWindowOnContentSizeChanged(IntPtr b, uiWindowOnContentSizeChangedDelegate f, IntPtr data);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void uiWindowContentSize(IntPtr w, out int width, out int height);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void uiWindowOnClosingDelegate(IntPtr b, IntPtr data);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void uiWindowOnContentSizeChangedDelegate(IntPtr b, IntPtr data);
        #endregion

        #region Windows

        [Flags]
        public enum SetWindowPosFlags : uint
        {
            /// <summary>
            /// If the calling thread and the thread that owns the window are attached to different input queues, 
            /// the system posts the request to the thread that owns the window. This prevents the calling thread from 
            /// blocking its execution while other threads process the request.
            /// </summary>
            /// <remarks>SWP_ASYNCWINDOWPOS</remarks>
            AsyncWindowPosition = 0x4000,

            /// <summary>
            /// Prevents generation of the WM_SYNCPAINT message.
            /// </summary>
            /// <remarks>SWP_DEFERERASE</remarks>
            DeferErase = 0x2000,

            /// <summary>
            /// Draws a frame (defined in the window's class description) around the window.
            /// </summary>
            /// <remarks>SWP_DRAWFRAME</remarks>
            DrawFrame = 0x0020,

            /// <summary>
            /// Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to 
            /// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE 
            /// is sent only when the window's size is being changed.
            /// </summary>
            /// <remarks>SWP_FRAMECHANGED</remarks>
            FrameChanged = 0x0020,

            /// <summary>
            /// Hides the window.
            /// </summary>
            /// <remarks>SWP_HIDEWINDOW</remarks>
            HideWindow = 0x0080,

            /// <summary>
            /// Does not activate the window. If this flag is not set, the window is activated and moved to the 
            /// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
            /// </summary>
            /// <remarks>SWP_NOACTIVATE</remarks>
            NoActivate = 0x0010,

            /// <summary>
            /// Discards the entire contents of the client area. If this flag is not specified, the valid contents 
            /// of the client area are saved and copied back into the client area after the window is sized or repositioned.
            /// </summary>
            /// <remarks>SWP_NOCOPYBITS</remarks>
            NoCopyBits = 0x0100,

            /// <summary>
            /// Retains the current position (ignores X and Y parameters).
            /// </summary>
            /// <remarks>SWP_NOMOVE</remarks>
            NoMove = 0x0002,

            /// <summary>
            /// Does not change the owner window's position in the Z order.
            /// </summary>
            /// <remarks>SWP_NOOWNERZORDER</remarks>
            NoOwnerZOrder = 0x0200,

            /// <summary>
            /// Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to 
            /// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent 
            /// window uncovered as a result of the window being moved. When this flag is set, the application must 
            /// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
            /// </summary>
            /// <remarks>SWP_NOREDRAW</remarks>
            NoRedraw = 0x0008,

            /// <summary>
            /// Same as the SWP_NOOWNERZORDER flag.
            /// </summary>
            /// <remarks>SWP_NOREPOSITION</remarks>
            NoReposition = 0x0200,

            /// <summary>
            /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
            /// </summary>
            /// <remarks>SWP_NOSENDCHANGING</remarks>
            NoSendChanging = 0x0400,

            /// <summary>
            /// Retains the current size (ignores the cx and cy parameters).
            /// </summary>
            /// <remarks>SWP_NOSIZE</remarks>
            NoSize = 0x0001,

            /// <summary>
            /// Retains the current Z order (ignores the hWndInsertAfter parameter).
            /// </summary>
            /// <remarks>SWP_NOZORDER</remarks>
            NoZOrder = 0x0004,

            /// <summary>
            /// Displays the window.
            /// </summary>
            /// <remarks>SWP_SHOWWINDOW</remarks>
            ShowWindow = 0x0040,
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        // ReSharper disable once InconsistentNaming
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);
        #endregion
    }
}

