using System.ComponentModel;

namespace HiddenValult
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ButtonCreate = new System.Windows.Forms.Button();
            this.ButtonDelete = new System.Windows.Forms.Button();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonUnlock = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.ButtonLock_Click = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ButtonCreate
            // 
            this.ButtonCreate.Location = new System.Drawing.Point(12, 126);
            this.ButtonCreate.Name = "ButtonCreate";
            this.ButtonCreate.Size = new System.Drawing.Size(141, 25);
            this.ButtonCreate.TabIndex = 0;
            this.ButtonCreate.Text = "新規作成";
            this.ButtonCreate.UseVisualStyleBackColor = true;
            this.ButtonCreate.Click += new System.EventHandler(this.ButtonCreate_Click);
            // 
            // ButtonDelete
            // 
            this.ButtonDelete.Location = new System.Drawing.Point(500, 126);
            this.ButtonDelete.Name = "ButtonDelete";
            this.ButtonDelete.Size = new System.Drawing.Size(145, 25);
            this.ButtonDelete.TabIndex = 1;
            this.ButtonDelete.Text = "削除(非推奨)";
            this.ButtonDelete.UseVisualStyleBackColor = true;
            this.ButtonDelete.Click += new System.EventHandler(this.ButtonDelete_Click);
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 18;
            this.listBox2.Location = new System.Drawing.Point(12, 174);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(633, 112);
            this.listBox2.TabIndex = 3;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(12, 51);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(251, 25);
            this.textBox4.TabIndex = 7;
            this.textBox4.Text = "Vaultのパス";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(303, 18);
            this.label1.TabIndex = 8;
            this.label1.Text = "HiddenVault - 暗号化フォルダ管理ツール";
            // 
            // ButtonUnlock
            // 
            this.ButtonUnlock.Location = new System.Drawing.Point(172, 126);
            this.ButtonUnlock.Name = "ButtonUnlock";
            this.ButtonUnlock.Size = new System.Drawing.Size(150, 25);
            this.ButtonUnlock.TabIndex = 9;
            this.ButtonUnlock.Text = "アンロック";
            this.ButtonUnlock.UseVisualStyleBackColor = true;
            this.ButtonUnlock.Click += new System.EventHandler(this.ButtonUnlock_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(295, 51);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(350, 25);
            this.textBox2.TabIndex = 10;
            this.textBox2.Text = "textBoxVaultPath(readOnly)";
            // 
            // ButtonLock_Click
            // 
            this.ButtonLock_Click.Location = new System.Drawing.Point(344, 126);
            this.ButtonLock_Click.Name = "ButtonLock_Click";
            this.ButtonLock_Click.Size = new System.Drawing.Size(150, 25);
            this.ButtonLock_Click.TabIndex = 11;
            this.ButtonLock_Click.Text = "再ロック";
            this.ButtonLock_Click.UseVisualStyleBackColor = true;
            this.ButtonLock_Click.Click += new System.EventHandler(this.ButtonLock_Click_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(672, 311);
            this.Controls.Add(this.ButtonLock_Click);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.ButtonUnlock);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.ButtonDelete);
            this.Controls.Add(this.ButtonCreate);
            this.Name = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonCreate;
        private System.Windows.Forms.Button ButtonDelete;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonUnlock;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button ButtonLock_Click;


    }
        
}

