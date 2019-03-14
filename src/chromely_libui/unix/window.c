// 11 june 2015
#include "uipriv_unix.h"

// Signatures
int x11_error_handler(Display *display, XErrorEvent *event);
int x11_io_error_handler(Display *display);

struct uiWindow {
    uiUnixControl c;

    GtkWidget *widget;
    GtkContainer *container;
    GtkWindow *window;

    int margined;

    int(*onClosing)(uiWindow *, void *);
    void *onClosingData;
    void(*onContentSizeChanged)(uiWindow *, void *);
    void *onContentSizeChangedData;
    gboolean fullscreen;
};

static gboolean onClosing(GtkWidget *win, GdkEvent *e, gpointer data)
{
    uiWindow *w = uiWindow(data);

    // manually destroy the window ourselves; don't let the delete-event handler do it
    if ((*(w->onClosing))(w, w->onClosingData))
        uiControlDestroy(uiControl(w));
    // don't continue to the default delete-event handler; we destroyed the window by now
    return TRUE;
}

static void onSizeAllocate(GtkWidget *widget, GdkRectangle *allocation, gpointer data)
{
    uiWindow *w = uiWindow(data);

    // TODO deal with spurious size-allocates
    (*(w->onContentSizeChanged))(w, w->onContentSizeChangedData);
}

static int defaultOnClosing(uiWindow *w, void *data)
{
    return 0;
}

static void defaultOnPositionContentSizeChanged(uiWindow *w, void *data)
{
    // do nothing
}

static void uiWindowDestroy(uiControl *c)
{
    uiWindow *w = uiWindow(c);

    // first hide ourselves
    gtk_widget_hide(w->widget);

    // and finally free ourselves
    // use gtk_widget_destroy() instead of g_object_unref() because GTK+ has internal references (see #165)
    gtk_widget_destroy(w->widget);
    uiFreeControl(uiControl(w));
}

uiUnixControlDefaultHandle(uiWindow)

uiControl *uiWindowParent(uiControl *c)
{
    return NULL;
}

void uiWindowSetParent(uiControl *c, uiControl *parent)
{
    uiUserBugCannotSetParentOnToplevel("uiWindow");
}

static int uiWindowToplevel(uiControl *c)
{
    return 1;
}

uiUnixControlDefaultVisible(uiWindow)

static void uiWindowShow(uiControl *c)
{
    uiWindow *w = uiWindow(c);

    // don't use gtk_widget_show_all() as that will show all children, regardless of user settings
    // don't use gtk_widget_show(); that doesn't bring to front or give keyboard focus
    // (gtk_window_present() does call gtk_widget_show() though)
    gtk_window_present(w->window);
}

uiUnixControlDefaultHide(uiWindow)
uiUnixControlDefaultEnabled(uiWindow)
uiUnixControlDefaultEnable(uiWindow)
uiUnixControlDefaultDisable(uiWindow)
// TODO?
uiUnixControlDefaultSetContainer(uiWindow)

char *uiWindowTitle(uiWindow *w)
{
    return uiUnixStrdupText(gtk_window_get_title(w->window));
}

void uiWindowSetTitle(uiWindow *w, const char *title)
{
    gtk_window_set_title(w->window, title);
}

void uiWindowContentSize(uiWindow *w, int *width, int *height)
{
    gtk_window_get_size(w->window, width, height);
}

void uiWindowSetContentSize(uiWindow *w, int width, int height)
{
    gtk_widget_set_size_request(w->widget, width, height);
}

int uiWindowFullscreen(uiWindow *w)
{
    return w->fullscreen;
}

// TODO use window-state-event to track
// TODO does this send an extra size changed?
// TODO what behavior do we want?
void uiWindowSetFullscreen(uiWindow *w, int fullscreen)
{
    w->fullscreen = fullscreen;
    if (w->fullscreen)
        gtk_window_fullscreen(w->window);
    else
        gtk_window_unfullscreen(w->window);
}

void uiWindowOnContentSizeChanged(uiWindow *w, void(*f)(uiWindow *, void *), void *data)
{
    w->onContentSizeChanged = f;
    w->onContentSizeChangedData = data;
}

void uiWindowOnClosing(uiWindow *w, int(*f)(uiWindow *, void *), void *data)
{
    w->onClosing = f;
    w->onClosingData = data;
}

int uiWindowBorderless(uiWindow *w)
{
    return gtk_window_get_decorated(w->window) == FALSE;
}

void uiWindowSetBorderless(uiWindow *w, int borderless)
{
    gtk_window_set_decorated(w->window, borderless == 0);
}

// TODO save and restore expands and aligns
void uiWindowSetChild(uiWindow *w, uiControl *child)
{
}

int uiWindowMargined(uiWindow *w)
{
    return w->margined;
}

void uiWindowSetMargined(uiWindow *w, int margined)
{
    w->margined = margined;
}

uiWindow *uiNewWindow(const char *title, int width, int height, int hasMenubar)
{
    uiWindow *w;

    uiUnixNewControl(uiWindow, w);

    w->widget = gtk_window_new(GTK_WINDOW_TOPLEVEL);
    w->container = GTK_CONTAINER(w->widget);
    w->window = GTK_WINDOW(w->widget);

    gtk_window_set_title(w->window, title);
    gtk_window_resize(w->window, width, height);

    // and connect our events
    g_signal_connect(w->widget, "delete-event", G_CALLBACK(onClosing), w);
    uiWindowOnClosing(w, defaultOnClosing, NULL);
    uiWindowOnContentSizeChanged(w, defaultOnPositionContentSizeChanged, NULL);

    // normally it's SetParent() that does this, but we can't call SetParent() on a uiWindow
    // TODO we really need to clean this up, especially since see uiWindowDestroy() above
    g_object_ref(w->widget);

    return w;
}

// Chromely add-ons

void *uiCefHostHandle(uiWindow *w)
{
    return w->window;
}

void *uiCefContentHandle(uiWindow *w)
{
    // Copied from upstream cefclient. Install xlib error
   // handlers so that the application won't be terminated on
   // non-fatal errors. Must be done after initializing GTK.
    XSetErrorHandler(x11_error_handler);
    XSetIOErrorHandler(x11_io_error_handler);

    GdkWindow *gdkwin = gtk_widget_get_window(w->widget);
    return gdk_x11_window_get_xid(gdkwin);
}

int x11_error_handler(Display *display, XErrorEvent *event) {
    printf("X11 error: type=%d, serial=%lu, code=%d\n",
        event->type, event->serial, (int)event->error_code);
    return 0;
}

int x11_io_error_handler(Display *display) {
    return 0;
}