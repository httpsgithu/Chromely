using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Chromely.CefGlue.Libui.BrowserWindow
{
    using Xilium.CefGlue;

    /// <summary>
    /// Functions to initialize the library and control the event flow of the application.
    /// </summary>
    public static class Application
    {
        /// <summary>
        /// Initializes LibUI.
        /// </summary>
        /// <exception cref="ExternalException">
        /// Thrown if LibUI encounters an error.
        /// </exception>
        public static void Init()
        {
            NativeMethods.uiInitOptions o = new NativeMethods.uiInitOptions();
            o.Size = UIntPtr.Zero;
            var err = NativeMethods.uiInit(ref o);
            if (!string.IsNullOrEmpty(err))
            {
                NativeMethods.uiFreeInitError(err);
                throw new ExternalException(err);
            }
        }

        /// <summary>
        /// Starts the LibUI main event loop.
        /// </summary>
        public static void Main()
        {
            if (HostRuntime.HostPlatform == CefRuntimePlatform.Windows)
            {
                NativeMethods.uiMain();
            }
            else
            {
                CefRuntime.RunMessageLoop();
            }
        }
        
        /// <summary>
        /// Manually steps through an event loop to process UI messages.
        /// </summary>
        /// <param name="wait">
        /// If true, block until a message is recieved. If false and a
        /// message is available If false, and a message is available,
        /// process it immediately (returning either true or false
        /// depending on the message), otherwise return true.
        /// </param>
        /// <returns>
        /// Returns true if a message was processed, false if the main
        /// loop has quit.
        /// </returns>
        public static bool Step(bool wait)
        {
            return NativeMethods.uiMainStep(wait);
        }

        /// <summary>
        /// Quits the application and thus, the event loop.
        /// </summary>
        public static void Quit()
        {
            if (HostRuntime.HostPlatform == CefRuntimePlatform.Windows)
            {
                NativeMethods.uiQuit();
            }
            else
            {
                CefRuntime.QuitMessageLoop();
            }

            // Perform cleanup tasks after the main event loop has finished.
            Task.Run(() => NativeMethods.uiUninit());
        }
    }
}
