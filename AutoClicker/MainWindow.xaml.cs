using System;
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
        private bool isRunning = false;

        public MainWindow()
        {
            InitializeComponent();

            
            Pos1X.Text = "2735";
            Pos1Y.Text = "400";
            Hotkey1.Text = "A";

            Pos2X.Text = "2857";
            Pos2Y.Text = "400";
            Hotkey2.Text = "S";

            Pos3X.Text = "3000";
            Pos3Y.Text = "400";
            Hotkey3.Text = "D";

           
            RegisterHotKey(new WindowInteropHelper(this).Handle, 10, 0, KeyInterop.VirtualKeyFromKey(Key.F));
            RegisterHotKey(new WindowInteropHelper(this).Handle, 11, 0, KeyInterop.VirtualKeyFromKey(Key.G));

            ComponentDispatcher.ThreadPreprocessMessage += HandleHotkeyPress;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
           

            try
            {
               
                clickPositions[0] = new POINT(int.Parse(Pos1X.Text), int.Parse(Pos1Y.Text));
                clickPositions[1] = new POINT(int.Parse(Pos2X.Text), int.Parse(Pos2Y.Text));
                clickPositions[2] = new POINT(int.Parse(Pos3X.Text), int.Parse(Pos3Y.Text));

                
                hotkeys[0] = (Key)Enum.Parse(typeof(Key), Hotkey1.Text, true);
                hotkeys[1] = (Key)Enum.Parse(typeof(Key), Hotkey2.Text, true);
                hotkeys[2] = (Key)Enum.Parse(typeof(Key), Hotkey3.Text, true);

                for (int i = 0; i < hotkeys.Length; i++)
                {
                    RegisterHotKey(new WindowInteropHelper(this).Handle, i + 1, 0, KeyInterop.VirtualKeyFromKey(hotkeys[i]));
                }

                isRunning = true;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                
                for (int i = 1; i <= 3; i++)
                {
                    UnregisterHotKey(new WindowInteropHelper(this).Handle, i);
                }

                isRunning = false;
            }
        }

        private void HandleHotkeyPress(ref MSG msg, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg.message == WM_HOTKEY)
            {
                int id = msg.wParam.ToInt32();

                if (id >= 1 && id <= 3)
                {
                    int clickIndex = id - 1;
                    SetCursorPos(clickPositions[clickIndex].X, clickPositions[clickIndex].Y);
                    MouseClick();
                    SetCursorPos(960,520);
                }
                else if (id == 10) 
                {
                    StartButton_Click(null, null);
                }
                else if (id == 11) 
                {
                    StopButton_Click(null, null);
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

            
            for (int i = 1; i <= 3; i++)
            {
                UnregisterHotKey(new WindowInteropHelper(this).Handle, i);
            }

            UnregisterHotKey(new WindowInteropHelper(this).Handle, 10); 
            UnregisterHotKey(new WindowInteropHelper(this).Handle, 11); 
            
            ComponentDispatcher.ThreadPreprocessMessage -= HandleHotkeyPress;
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
