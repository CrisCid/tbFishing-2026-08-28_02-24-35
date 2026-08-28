using UnityEngine;

namespace TaskBarFisher.Platform.DesktopOverlay
{
    /// <summary>
    /// Punto de entrada del overlay de escritorio (game_design_overview.md §9). Vive en una escena
    /// de bootstrap, un único objeto. Fuera de un build standalone de Windows (es decir, en el
    /// Editor, o en un futuro build de otra plataforma) se comporta como una ventana común -- el
    /// resto del equipo puede desarrollar y probar su módulo sin tener el overlay funcionando.
    ///
    /// Fase 0 = wiring correcto de la llamada nativa; ajustar la región de click-through real por
    /// zona de UI, y probar en distintas configuraciones de escalado de pantalla de Windows, es
    /// trabajo de Fase 1 (Módulo C) -- marcado en game_design_overview.md §10 como el riesgo técnico
    /// más alto del proyecto.
    /// </summary>
    public class DesktopOverlayController : MonoBehaviour
    {
        [SerializeField] bool enableOverlayInEditor = false;

        void Start()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            ApplyOverlay();
#else
            if (enableOverlayInEditor)
                Debug.LogWarning("[DesktopOverlayController] El overlay nativo solo corre en un build Standalone Windows; en Editor esta ventana se queda normal.");
#endif
        }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        void ApplyOverlay()
        {
            var hWnd = GetUnityWindowHandle();
            if (hWnd == System.IntPtr.Zero)
            {
                Debug.LogError("[DesktopOverlayController] No se pudo resolver el handle de la ventana. Overlay no aplicado.");
                return;
            }

            NativeWindow.MakeTopmostOverlay(hWnd);
            NativeWindow.EnableRealTransparency(hWnd);
            // Click-through selectivo por zona: TODO Fase 1, Módulo C. De momento queda con
            // click-through apagado (ventana totalmente interactiva) para no bloquear el resto del
            // desarrollo mientras esa lógica se construye.
            NativeWindow.SetClickThrough(hWnd, enabled: false);
        }

        static System.IntPtr GetUnityWindowHandle()
        {
            // Placeholder de estructura -- la resolución real del HWND del proceso de Unity
            // (vía GetActiveWindow/FindWindow, con reintentos si la ventana todavía no tiene foco
            // al primer frame) se implementa en Fase 1, Módulo C.
            return System.IntPtr.Zero;
        }
#endif
    }
}
