using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HiddenValult
{
    public static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        /// 
         // 秘密コマンド（自由に変更OK）
        public const string SecretCmd = "awakeme";
        public static Form1 mainForm;
        public static HiddenLaunchForm hiddenForm;

        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool needsAuthOnStart = true;
            mainForm = new Form1();
            Application.Run(mainForm);
            var args = Environment.GetCommandLineArgs();

            // --- A: コマンドライン解錠 ---
            if (args.Length >= 3 && args[1] == "/unlock")
            {
                if (args[2] == SecretCmd)
                {
                    // 正しい → アンロック画面を表示
                    Application.Run(new HiddenLaunchForm());
                    return;
                }
                else
                {
                    // 間違っていれば何も表示せず終了
                    return;
                }
                

            }

            // --- D: 通常起動 → 隠しコマンド入力待機フォームへ ---
            Application.Run(new Form1());
        }
    }
}

