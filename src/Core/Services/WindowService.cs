using Blish_HUD;
using Microsoft.Xna.Framework;
using System;

namespace Nekres.WindowResize.Core.Services {

    internal class WindowService : IDisposable {
        public event EventHandler<ValueEventArgs<bool>> IsWindowedModeChanged;

        private bool _isWindowedMode;
        public bool IsWindowedMode {
            get => _isWindowedMode;
            set {
                if (_isWindowedMode != value) {
                    _isWindowedMode = value;
                    IsWindowedModeChanged?.Invoke(this, new ValueEventArgs<bool>(value));
                }
            }
        }

        private       double elapsedTime;
        private const double UPDATE_INTERVAL = 200;

        public WindowService() {
            IsWindowedModeChanged += OnIsWindowedModeChanged;
        }

        private void OnIsWindowedModeChanged(object sender, ValueEventArgs<bool> e) {
            if (!e.Value) {
                return; // Not in windowed mode.
            }

            var size = WindowUtil.ParseSize(WindowResize.Instance.WindowSize.Value);
            WindowUtil.ResizeAndCenterWindow(GameService.GameIntegration.Gw2Instance.Gw2WindowHandle, size.Width, size.Height);

            /*if (WindowResize.Instance.Borderless.Value) {
                WindowUtil.RemoveBorder(GameService.GameIntegration.Gw2Instance.Gw2WindowHandle);
            }*/
        }

        public void Update(GameTime gameTime) {
            elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime < UPDATE_INTERVAL) {
                return;
            }
            elapsedTime = 0;
            CheckWindowMode(GameService.GameIntegration.Gw2Instance.Gw2WindowHandle);
        }

        private void CheckWindowMode(IntPtr hWnd) {
            if (hWnd == IntPtr.Zero) {
                return;
            }

            IsWindowedMode = WindowUtil.IsWindowedMode(hWnd);
        }

        public void Dispose() {
            IsWindowedModeChanged -= OnIsWindowedModeChanged;
        }
    }
}
