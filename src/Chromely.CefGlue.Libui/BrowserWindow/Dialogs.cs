using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Chromely.CefGlue.Libui.BrowserWindow
{
    /// <summary>
    /// Provides functions to open dialog boxes.
    /// </summary>
    public static class Dialogs
    {
        /// <summary>
        /// Pops up an open file dialog.
        /// </summary>
        /// <param name="owner">The window that owns this dialog.</param>
        /// <returns>What the user picked; null if cancelled.</returns>
        public static string OpenFileDialog(this Window owner)
        {
            return NativeMethods.uiOpenFile(NativeWindow.Handle);
        }

        /// <summary>
        /// Pops up a save file dialog.
        /// </summary>
        /// <param name="owner">The window that owns this dialog.</param>
        /// <returns>What the user picked; null if cancelled.</returns>
        public static string SaveFileDialog(this Window owner)
        {
            return NativeMethods.uiSaveFile(NativeWindow.Handle);
        }

        /// <summary>
        /// Pops up a message dialog.
        /// </summary>
        /// <param name="owner">The window that owns this dialog.</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="description">The message.</param>
        /// <param name="error">If this dialog should be shown as an error dialog.</param>
        public static void MessageBox(this Window owner,
            string title, string description, bool error = false)
        {
            if (error)
                NativeMethods.uiMsgBoxError(NativeWindow.Handle, title, description);
            else
                NativeMethods.uiMsgBox(NativeWindow.Handle, title, description);
        }
    }
}
