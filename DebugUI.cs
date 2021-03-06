using Unity.Entities;
using VisualPinball.Unity.Physics.DebugUI;
using VisualPinball.Unity.VPT.Table;
using ImGuiNET;
using ImGuiNET.Unity;
using UnityEngine;

namespace VisualPinball.Engine.Unity.ImgGUI
{

    public class DebugUI : IDebugUI
    {
        public string Name => "ImgGUI";

        public bool showOverlayWindow = true;
        public bool showDebugWindow = true;
        public bool showDemoWindow = false;

        FlipperMonitor _flippers = new FlipperMonitor();
        BallMonitor _balls = null;
        PerformanceMonitor _performance = new PerformanceMonitor();
        VPEUtilities _VPEUtilities;
        DebugProperties _debugProperties = new DebugProperties();

        public FlipperMonitor Flippers { get => _flippers; }
        public BallMonitor Balls { get => _balls; }
        public PerformanceMonitor Performance { get => _performance; }
        public VPEUtilities VPE { get => _VPEUtilities; }
        public DebugProperties Properties { get => _debugProperties; }

        DebugOverlay _debugOverlay;
        DebugWindow _debugWindow;
        

        // ==================================================================== IDebugUI ===

        public void Init(TableBehavior tableBehavior)
        {
            // add component if not already added in editor
            var dearImGUIComponent = tableBehavior.gameObject.GetComponent<DearImGui>();
            if (dearImGUIComponent == null)
            {
                dearImGUIComponent = tableBehavior.gameObject.AddComponent<DearImGui>();
            }

            var debugUIComponent = tableBehavior.gameObject.GetComponent<DebugUIComponent>();
            if (debugUIComponent == null)
            {
                debugUIComponent = tableBehavior.gameObject.AddComponent<DebugUIComponent>();
            }

            debugUIComponent.debugUI = this;
            _balls = new BallMonitor(this);
            _debugOverlay = new DebugOverlay(this);
            _debugWindow = new DebugWindow(this);
            _VPEUtilities = new VPEUtilities(this, tableBehavior);
        }

        public void OnPhysicsUpdate(double physicClockMilliseconds, int numSteps, float processingTimeMilliseconds)
        {
            _performance.OnPhysicsUpdate(physicClockMilliseconds, numSteps, processingTimeMilliseconds);
            _flippers.OnPhysicsUpdate(physicClockMilliseconds, numSteps, processingTimeMilliseconds);
            _balls.OnPhysicsUpdate(physicClockMilliseconds, numSteps, processingTimeMilliseconds);
        }

        public void OnRegisterFlipper(Entity entity, string name)
        {
            _flippers.Register(entity, name);
        }

        public void OnCreateBall(Entity entity)
        {
            _balls.Register(entity, null);
        }

        public int AddProperty<T>(int parentIdx, string name, T currentValue, string tip)
        {
            return Properties.AddProperty(parentIdx, name, currentValue, tip);
        }

        public bool GetProperty<T>(int propIdx, ref T val)
        {
            return Properties.GetValue(propIdx, ref val);
        }

        public void SetProperty<T>(int propIdx, T value)
        {
            Properties.SetValue(propIdx, value);
        }

        public bool QuickPropertySync<T>(string name, ref T value, string tip)
        {
            return Properties.QuickPropertySync(name, ref value, tip);
        }
        // ==================================================================== DebugUI ===

        public void OnDraw()
        {
            _performance.OnUpdateBeforeDraw();

            if (showOverlayWindow)
                _debugOverlay.Draw();

            if (showDemoWindow)
                ImGui.ShowDemoWindow(ref showDemoWindow);

            if (showDebugWindow)
                _debugWindow.Draw();
        }
    }

}