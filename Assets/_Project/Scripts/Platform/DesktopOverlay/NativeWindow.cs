#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;

namespace TaskBarFisher.Platform.DesktopOverlay
{
    /// <summary>
    /// P/Invoke crudo a user32.dll/dwmapi.dll (game_design_overview.md §9). Envuelto en el mismo
    /// #if que <see cref="DesktopOverlayController"/> a propósito: esta clase NO existe fuera de un
    /// build standalone de Windows, así que nada del resto del juego puede terminar dependiendo de
    /// ella por accidente.
    ///
    /// Fase 0 = firmas correctas + wrapper administrado; la lógica real de "aplicar overlay" (calcular
    /// región click-through, manejar cambios de escalado de pantalla, etc.) es Fase 1, Módulo C --
    /// y arrancarla temprano, porque es el módulo de mayor riesgo técnico del proyecto.
    /// </summary>
    internal static class NativeWindow
    {
        const int GWL_EXSTYLE = -20;
        const int WS_EX_LAYERED = 0x00080000;
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int WS_EX_TOOLWINDOW = 0x00000080;

        static readonly IntPtr HWND_TOPMOST = new(-1);
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOSIZE = 0x0001;

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [StructLayout(LayoutKind.Sequential)]
        struct MARGINS { public int Left, Right, Top, Bottom; }

        [DllImport("dwmapi.dll")]
        static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

        /// <summary>Pone la ventana siempre-encima, sin barra de título. No toca click-through todavía (ver <see cref="SetClickThrough"/>).</summary>
        public static void MakeTopmostOverlay(IntPtr hWnd)
        {
            var exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, exStyle | WS_EX_LAYERED | WS_EX_TOOLWINDOW);
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        /// <summary>Extiende el frame DWM para transparencia real (no solo color-key).</summary>
        public static void EnableRealTransparency(IntPtr hWnd)
        {
            var margins = new MARGINS { Left = -1, Right = -1, Top = -1, Bottom = -1 };
            DwmExtendFrameIntoClientArea(hWnd, ref margins);
        }

        /// <summary>
        /// Activa/desactiva click-through global de la ventana. La versión final necesita
        /// click-through POR ZONA (game_design_overview.md §9: "zonas click-through selectivas"),
        /// que no es esto -- esto es el mecanismo de base sobre el que Fase 1 construye esa lógica.
        /// </summary>
        public static void SetClickThrough(IntPtr hWnd, bool enabled)
        {
            var exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            exStyle = enabled ? exStyle | WS_EX_TRANSPARENT : exStyle & ~WS_EX_TRANSPARENT;
            SetWindowLong(hWnd, GWL_EXSTYLE, exStyle);
        }
    }
}
#endif
