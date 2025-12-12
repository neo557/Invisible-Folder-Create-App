using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace HiddenValult
{
    public partial class StealthForm : Form
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        private const int HOTKEY_ID = 1;
        private const uint MOD_CONTROL = 0x2;
        private const uint MOD_SHIFT = 0x4;

        private void StealthForm_Load(object sender, EventArgs e)
        {

        }



        public StealthForm()
        {
            InitializeComponent();
            // ホットキー登録（Ctrl + Shift + L）
            RegisterHotKey(this.Handle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, (int)Keys.L);
        }


        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;

            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
            {
                // 解除画面を開く
                var unlock = new HiddenLaunchForm();
                unlock.Show();
                unlock.BringToFront();
            }

            base.WndProc(ref m);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID);
            base.OnFormClosing(e);
        }
    }
}
