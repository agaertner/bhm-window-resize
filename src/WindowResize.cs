using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Nekres.WindowResize.Core;
using Nekres.WindowResize.Core.Services;
using System;
using System.ComponentModel.Composition;

namespace Nekres.WindowResize {
    [Export(typeof(Module))]
    public class WindowResize : Module {
        internal static readonly Logger Logger = Logger.GetLogger<WindowResize>();

        internal static WindowResize Instance { get; private set; }

        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion

        [ImportingConstructor]
        public WindowResize([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) => Instance = this;

        internal WindowService Window;

        internal SettingEntry<WindowUtil.WindowSize> WindowSize;
        internal SettingEntry<bool>                  Borderless;
        protected override void DefineSettings(SettingCollection settings) {
            var styleCol = settings.AddSubCollection("window_style", true, () => "Window Appearance");
            WindowSize = styleCol.DefineSetting("size", WindowUtil.WindowSize._1920x1080, () => "Size");
            //Borderless = styleCol.DefineSetting("borderless", true, () => "Borderless");
        }

        protected override void Initialize() {
            Window = new WindowService();
        }

        protected override void OnModuleLoaded(EventArgs e) {
            //Borderless.SettingChanged += OnBorderlessChanged;
            WindowSize.SettingChanged += OnWindowSizeChanged;

            // Base handler must be called.
            base.OnModuleLoaded(e);
        }

        private void OnWindowSizeChanged(object sender, ValueChangedEventArgs<WindowUtil.WindowSize> e) {
            if (Window.IsWindowedMode) {
                var size = WindowUtil.ParseSize(WindowSize.Value);
                WindowUtil.ResizeAndCenterWindow(GameService.GameIntegration.Gw2Instance.Gw2WindowHandle, size.Width, size.Height);
            }
        }

        private void OnBorderlessChanged(object sender, ValueChangedEventArgs<bool> e) {
            if (Window.IsWindowedMode) {
                if (e.NewValue) {
                    WindowUtil.RemoveBorder(GameService.GameIntegration.Gw2Instance.Gw2WindowHandle);
                } else {
                    WindowUtil.AddBorder(GameService.GameIntegration.Gw2Instance.Gw2WindowHandle);
                }
            }
        }

        protected override void Update(GameTime gameTime) {
            Window.Update(gameTime);
        }

        /// <inheritdoc />
        protected override void Unload() {
            WindowSize.SettingChanged -= OnWindowSizeChanged;
            //Borderless.SettingChanged -= OnBorderlessChanged;
            // All static members must be manually unset
            Instance = null;
        }
    }
}
