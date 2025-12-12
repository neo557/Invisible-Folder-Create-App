using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HiddenValult
{
    public partial class UnlockForm : Form
    {
        // 呼び出し元に結果を渡す（special コマンド検証フラグ）
        public bool SpecialKeyValidated { get; private set; } = false;


        public UnlockForm()
        {
            InitializeComponent();
            this.KeyPreview = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                TryUnlock();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void UnlockForm_Load(object sender, EventArgs e)
        {
            FadeIn();
        }

        private void FadeIn()
        {
            var timer = new Timer();
            timer.Interval = 20;
            timer.Tick += (s, ev) =>
            {
                if (this.Opacity < 1.0)
                    this.Opacity += 0.05;
                else
                    timer.Stop();
            };
            timer.Start();
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            TryUnlock();
        }

        private void TryUnlock()
        {
            // special コマンド検証（Program.SecretCmd を使う）
            var input = textBoxPW.Text?.Trim() ?? "";

            if (string.Equals(input, Program.SecretCmd, StringComparison.Ordinal))
            {
                // special コマンド合格（ここでは awakening）
                SpecialKeyValidated = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            // もし special コマンドでなく「直接ID/PW入力をしたい」仕様ならここに分岐を追加
            MessageBox.Show("特殊コマンドが違います。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            SpecialKeyValidated = false;
            // stay open or close depending on desired UX; ここでは閉じないで再入力を促す
        }
    
    }
}
