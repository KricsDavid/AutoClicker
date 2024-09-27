﻿using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace AutoClicker
{
    public partial class MainWindow : Window
    {
        private POINT[] clickPositions = new POINT[3];
        private Key[] hotkeys = new Key[3];

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Fetching positions
                clickPositions[0] = new POINT(int.Parse(Pos1X.Text), int.Parse(Pos1Y.Text));
                clickPositions[1] = new POINT(int.Parse(Pos2X.Text), int.Parse(Pos2Y.Text));
                clickPositions[2] = new POINT(int.Parse(Pos3X.Text), int.Parse(Pos3Y.Text));

                // Fetching hotkeys
                hotkeys[0] = (Key)Enum.Parse(typeof(Key), Hotkey1.Text, true);
                hotkeys[1] = (Key)Enum.Parse(typeof(Key), Hotkey2.Text, true);
                hotkeys[2] = (Key)Enum.Parse(typeof(Key), Hotkey3.Text, true);

                for (int i = 0; i < hotkeys.Length; i++)
                {
                    RegisterHotKey(new WindowInteropHelper(this).Handle, i + 1, 0, KeyInterop.VirtualKeyFromKey(hotkeys[i]));
                }

                ComponentDispatcher.ThreadPreprocessMessage += HandleHotkeyPress;
                MessageBox.Show("Hotkeys registered. Ready to click!", "Auto Clicker");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        private void HandleHotkeyPress(ref MSG msg, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg.message == WM_HOTKEY)
            {
                int id = msg.wParam.ToInt32() - 1;
                if (id >= 0 && id < clickPositions.Length)
                {
                    SetCursorPos(clickPositions[id].X, clickPositions[id].Y);
                    MouseClick();
                }
            }
        }

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll", SetLastError = true)]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        private void MouseClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ComponentDispatcher.ThreadPreprocessMessage -= HandleHotkeyPress;
            for (int i = 1; i <= 3; i++)
            {
                UnregisterHotKey(new WindowInteropHelper(this).Handle, i);
            }
        }

        private struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
    }
}
