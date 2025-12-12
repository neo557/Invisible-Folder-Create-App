using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;	

namespace HiddenValult
{
	public partial class HiddenLaunchForm : Form
	{
		[DllImport("user32.dll")]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

		[DllImport("user32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		private const int HOTKEY_ID = 1;
		private const uint MOD_CONTROL = 0x2;
		private const uint MOD_SHIFT = 0x4;

        

        public HiddenLaunchForm()
		{
			this.FormBorderStyle = FormBorderStyle.None;
			this.WindowState = FormWindowState.Maximized;
			this.BackColor = Color.Black;
			this.TransparencyKey = Color.Black;
            this.TopMost = true;
			this.ShowInTaskbar = false;
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			this.AllowTransparency = true;
			this.BackColor = Color.Black;
			this.TransparencyKey = Color.Black;

            // Ctrl + Shift + L
            RegisterHotKey(this.Handle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, (uint)Keys.L);
		}

		protected override void WndProc(ref Message m)
		{
			const int WM_HOTKEY = 0x0312;

			if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
			{
				this.Close();
				using (var unlock = new UnlockForm())
				{
					var dr = unlock.ShowDialog();

					if (dr == DialogResult.OK && unlock.SpecialKeyValidated)
					{
						// special key 成功
						// mainForm を表示させ、Form1 の ShowLoginOnly を呼ぶ
						if (Program.mainForm != null && !Program.mainForm.IsDisposed)
						{
							Program.mainForm.BeginInvoke((Action)(() =>
							{
								Program.mainForm.ShowLoginOnly();
							}));
						}

						else
						{
							this.Show();
						}

					}
					else
					{
						// 失敗またはキャンセルなら再度 HiddenLaunchForm を表示して待機
						this.Show();
					}
				}
			}

			base.WndProc(ref m);
		}

		public void LockScreen()
		{
			// Form1 が閉じられた後などに呼ばれる
			this.Show();
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			UnregisterHotKey(this.Handle, HOTKEY_ID);
			base.OnFormClosing(e);
		}
       
        

    }
}
