// Form1.cs (WinForms .NET Framework)
// 必要コントロール（Form Designerで配置）:
// - Button: buttonCreate
// - Button: buttonUnlock
// - Button: buttonDelete
// - TextBox: textBox2 (Vault path 表示用、読み取り専用でも可)
// - ListBox: listBox2 (ログ出力)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HiddenValult.CredentialManager;

namespace HiddenValult
{
    public partial class Form1 : Form
    {
        private readonly string appFolderName = "HiddenVault"; // config 保管場所
        private string userConfigPath;
        private HiddenLaunchForm hiddenForm;
        private bool startWithPassWordPage;
        public bool IsUnLocked { get; set; } = false;
        private string configDir;

        public Form1(bool startWithPW = false)
        {
            InitializeComponent();
            startWithPassWordPage = startWithPW;
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\HiddenVault";
            string configDir = Path.Combine(basePath, appFolderName);
            userConfigPath = Path.Combine(configDir, "config.json");
            this.configDir = configDir;
            this.Load += new EventHandler(Form1_Load);
            if(!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Form1 の描画が始まる前に、認証画面を表示する
            if (startWithPassWordPage) // Program.cs で true で呼ばれた場合など
            {
                // 認証ロジックを実行
                ShowLoginOnly();
            }
            else
            {
                // Vault の存在チェックなど、通常の Form1 初期化処理
                // (アンロック済みの状態での初期化を想定)
                ShowHomePanelOrInit();
            }
        }

        private void Log(string msg)
        {
            listBox2.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " : " + msg);
        }

        public void ShowLoginOnly()
        {
            this.Hide();
            { 
                this.Show();
                ButtonUnlock_Click(this, EventArgs.Empty); // アンロック処理を呼ぶ
                ShowHomePanelOrInit();
            }
        }

        private void DirectOpenVault()
        {
            try
            {
                string vaultConfigPath = Path.Combine(configDir, "vault.json");
            }
            catch (Exception ex)
            {
                Log("Unlock エラー: " + ex.Message);
            }
        }

        private string DirectoryOpenVault(string vaultPath)
        {
            if (Directory.Exists(vaultPath))
            {
                Process.Start("explorer.exe", $"\"{vaultPath}\"");
            }

            return vaultPath;
        }


        // ヘルパー（既存の ShowHomePanel と統一）
        private void ShowHomePanelOrInit()
        {
            // ここはあなたの既存のホーム初期化処理を呼ぶ
            // 例:
            ButtonCreate.Enabled = true;
            ButtonUnlock.Enabled = true;
            ButtonDelete.Enabled = true;
            // 必要なら RefreshList();

        }



        // Create vault
        private void ButtonCreate_Click(object sender, EventArgs e)
        {
            try
            {
                string basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\HiddenVault";
                string vaultPath = Path.Combine(basePath, "System_" + Guid.NewGuid().ToString("N"));

                // フォルダ作成
                Directory.CreateDirectory(vaultPath);
                File.SetAttributes(vaultPath, File.GetAttributes(vaultPath) | FileAttributes.Hidden | FileAttributes.System);

                // config dir
                string configDir = Path.Combine(basePath, appFolderName);
                Directory.CreateDirectory(configDir);

                // key file path
                string keyPath = Path.Combine(configDir, "key.bin");

                // 1) 鍵生成して DPAPI で保護して保存
                CryptoKeyManager.GenerateAndSaveProtectedKey(keyPath);

                // 2) 鍵ロード（復号）
                var (key, iv) = CryptoKeyManager.LoadProtectedKey(keyPath);

                // 3) vaultPath を AES で暗号化して vault.config に保存
                string encryptedVaultPath = Crypto.Encrypt(vaultPath, key, iv);
                string vaultConfigPath = Path.Combine(configDir,"vault.json");
                File.WriteAllText(vaultConfigPath, encryptedVaultPath, Encoding.UTF8);

                Log("Vault 作成成功（パスは非表示）。");

                // 4) ユーザーの認証情報が未設定なら設定を促す
                //string userConfigPath = Path.Combine(configDir, "config.json");
                    Log("初回設定：ID と パスワードを設定してください。");
                    using (var dlg = new CredentialsForm())
                    {
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            // 保存
                            CredentialManager.AddVault(userConfigPath, dlg.UserId, dlg.Password);
                            Log("認証情報を保存しました。");
                        }
                        else
                        {
                            Log("認証情報が未設定です。作成は完了しましたが、後で設定してください。");
                        }
                    }
                
            }
            catch (Exception ex)
            {
                Log("作成エラー: " + ex.Message);
            }


        }

        private void RefreshList()
        {
            listBox2.Items.Clear();

            if (!File.Exists(userConfigPath))
                return;

            var jsonText = File.ReadAllText(userConfigPath, Encoding.UTF8);

            if (string.IsNullOrWhiteSpace(jsonText))
            {
                File.WriteAllText(userConfigPath, "{\"Vaults\":[]}", Encoding.UTF8);
                jsonText = "{\"Vaults\":[]}";
            }

            var vaultConfig = JsonSerializer.Deserialize<VaultConfig>(jsonText);

            foreach (var vault in vaultConfig.Vaults)
            {
                listBox2.Items.Add(vault.UserId);
            }
        }

        // Unlock vault (authenticate + open)
        private void ButtonUnlock_Click(object sender, EventArgs e)
        {
            try
            {
                string basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\HiddenVault";
                string configDir = Path.Combine(basePath, appFolderName);
                string vaultConfigPath = Path.Combine(configDir, "vault.json");
                string keyPath = Path.Combine(configDir, "key.bin");
                //string userConfigPath = Path.Combine(configDir, "config.json");

                if (!File.Exists(vaultConfigPath) || !File.Exists(keyPath))
                {
                    Log("Vault が見つかりません。最初に作成してください。");
                    return;
                }

                // 認証ダイアログ
                using (var dlg = new CredentialsForm(isForValidate: true))
                {
                    if (dlg.ShowDialog() != DialogResult.OK)
                    {
                        Log("認証キャンセル。");
                        return;
                    }

                    // 認証チェック
                    if (!CredentialManager.ValidateVault(userConfigPath, dlg.UserId, dlg.Password))
                    {
                        Log("認証失敗：ID または パスワードが違います。");
                        return;
                    }
                }

                // 認証成功 → 鍵を読み、vaultPathを復号
                var (key, iv) = CryptoKeyManager.LoadProtectedKey(keyPath);
                string encryptedVaultPath = File.ReadAllText(vaultConfigPath, Encoding.UTF8);
                string vaultPath = Crypto.Decrypt(encryptedVaultPath, key, iv);

                if (!Directory.Exists(vaultPath))
                {
                    Log("Vault フォルダが見つかりません（消失している可能性があります）。");
                    return;
                }

                // UI 表示
                textBox2.Text = vaultPath;
                Log("Vault をアンロックしました。Explorer で開きます。");

                // Explorer で開く（Hidden 属性のままでも指定されたフォルダを開ける）
                Process.Start("explorer.exe", $"\"{vaultPath}\"");
            }
            catch (Exception ex)
            {
                Log("Unlock エラー: " + ex.Message);
            }
        }

        private void ButtonLock_Click_Click(object sender, EventArgs e)
        {
            // メインフォームを隠す
            this.Hide();

            // HiddenLaunchForm を作って待機させる（既にあるなら再利用）
            if (Program.hiddenForm == null || Program.hiddenForm.IsDisposed)
                Program.hiddenForm = new HiddenLaunchForm();

            Program.hiddenForm.Show();
        }

        // Delete (destroy vault) — 安全モードは鍵削除（復元不能）
        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            

            if (listBox2 == null)
            {
                MessageBox.Show("リストが初期化されていません。Form1.Desiner.csを確認してください");
            }

        
            if (listBox2.SelectedItem == null)
            {
                MessageBox.Show("削除する項目を選択してください。");
                return;
            }
            
            string pwName = listBox2.SelectedItem.ToString();


            //Jsonをロード
            var config = JsonSerializer.Deserialize<VaultConfig>(File.ReadAllText(userConfigPath, Encoding.UTF8));

            var target = config.Vaults
                .FirstOrDefault(v => v.UserId == pwName);


            if (target == null)
            {
                MessageBox.Show("対象のデータが見つかりません。");
                return;
            }

            // 削除確認
            var result = MessageBox.Show(
                $"{pwName} を削除しますか？",
                "確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                config.Vaults.Remove(target);
                //Json保存
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(userConfigPath, json, Encoding.UTF8);

                RefreshList();
            }
        }

        public void ShowAuthScreen()
        {
            try
            {
                string basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\HiddenVault";
                string configDir = Path.Combine(basePath, appFolderName);
                string vaultConfigPath = Path.Combine(configDir, "vault.json"); // あなたが使っているファイル名
                string keyPath = Path.Combine(configDir, "key.bin");
                string userConfigPath = Path.Combine(configDir, "config.json");

                if (!File.Exists(vaultConfigPath) || !File.Exists(keyPath))
                {
                    Log("Vault が見つかりません。最初に作成してください。");
                    MessageBox.Show("Vault が見つかりません。まずは作成してください。");
                    return;
                }

                // 認証ダイアログ（既存 CredentialsForm を使う）
                using (var dlg = new CredentialsForm(isForValidate: true))
                {
                    if (dlg.ShowDialog() != DialogResult.OK)
                    {
                        Log("認証キャンセル。");
                        return;
                    }

                    if (!CredentialManager.ValidateVault(userConfigPath, dlg.UserId, dlg.Password))
                    {
                        Log("認証失敗：ID または パスワードが違います。");
                        MessageBox.Show("ID またはパスワードが違います。");
                        return;
                    }
                }

                // 認証成功 → 鍵を読み、vaultPathを復号して開く
                var (key, iv) = CryptoKeyManager.LoadProtectedKey(keyPath);
                string encryptedVaultPath = File.ReadAllText(vaultConfigPath, Encoding.UTF8);
                string vaultPath = Crypto.Decrypt(encryptedVaultPath, key, iv);

                if (!Directory.Exists(vaultPath))
                {
                    Log("Vault フォルダが見つかりません（消失している可能性があります）。");
                    MessageBox.Show("Vaultフォルダが見つかりません。");
                    return;
                }

                textBox2.Text = vaultPath;
                Log("Vault をアンロックしました。Explorer で開きます。");

                Process.Start("explorer.exe", $"\"{vaultPath}\"");
            }
            catch (Exception ex)
            {
                Log("ShowAuthScreen エラー: " + ex.Message);
                MessageBox.Show("エラー: " + ex.Message);
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Application.Exit();
            base.OnFormClosing(e);
        }
    }

    // --- Crypto: AES encrypt/decrypt using provided key/iv (byte[])
    public static class Crypto
    {
        public static string Encrypt(string plainText, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                using (var encryptor = aes.CreateEncryptor())
                {
                    var plain = Encoding.UTF8.GetBytes(plainText);
                    var cipher = encryptor.TransformFinalBlock(plain, 0, plain.Length);
                    return Convert.ToBase64String(cipher);
                }
            }
        }

        public static string Decrypt(string cipherTextBase64, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                using (var decryptor = aes.CreateDecryptor())
                {
                    var cipher = Convert.FromBase64String(cipherTextBase64);
                    var plain = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
                    return Encoding.UTF8.GetString(plain);
                }
            }
        }
    }

    // --- CryptoKeyManager: AES鍵を生成し DPAPI(ProtectedData) で保護して保存／読み込み
    public static class CryptoKeyManager
    {
        // keyPath: 保存先ファイル（例: %LocalAppData%\HiddenVault\key.bin）
        public static void GenerateAndSaveProtectedKey(string keyPath)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.GenerateKey();
                aes.GenerateIV();
                var obj = new Dictionary<string, string>
                {
                    { "Key", Convert.ToBase64String(aes.Key) },
                    { "IV", Convert.ToBase64String(aes.IV) }
                };
                var json = JsonSerializer.Serialize(obj);
                var bytes = Encoding.UTF8.GetBytes(json);

                // DPAPI で保護（CurrentUser スコープ）
                var protectedBytes = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
                File.WriteAllBytes(keyPath, protectedBytes);
            }
        }

        // keyPath: 読み込み
        public static (byte[] key, byte[] iv) LoadProtectedKey(string keyPath)
        {
            var protectedBytes = File.ReadAllBytes(keyPath);
            var bytes = ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser);
            var json = Encoding.UTF8.GetString(bytes);
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            var key = Convert.FromBase64String(dict["Key"]);
            var iv = Convert.FromBase64String(dict["IV"]);
            return (key, iv);
        }
    }

    // --- CredentialManager: ID/Password のハッシュ保存と検証（PBKDF2）
    public static class CredentialManager
    {
        public class CredData
        {
            public string UserId { get; set; }
            public string PasswordHash { get; set; } // base64
            public string Salt { get; set; } // base64
            public int Iterations { get; set; }
        }

        public class VaultConfig
        {
          public List<CredData> Vaults { get; set; } = new List<CredData>();
        }
        


        private static VaultConfig LoadConfig(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return new VaultConfig();

                var json = File.ReadAllText(path, Encoding.UTF8);

                // 空ファイル対策
                if (string.IsNullOrWhiteSpace(json))
                    return new VaultConfig();

                return JsonSerializer.Deserialize<VaultConfig>(json) ?? new VaultConfig();
            }
            catch
            {
                // JSON が壊れている場合も新規として扱う
                return new VaultConfig();
            }
        }

        // --- config.json を保存 ---
        private static void SaveConfig(string path, VaultConfig cfg)
        {
            var json = JsonSerializer.Serialize(cfg, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json, Encoding.UTF8);
        }

        //Vaultの追加
        public static void AddVault(string path, string userId, string password)
        {
            var config = LoadConfig(path);

            if (config.Vaults.Any(VaultConfig => VaultConfig.UserId == userId))
                throw new Exception("すでに同じIDのVaultが存在します");
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            int iterations = 100_000;
            var hash = PBKDF2(password, salt, iterations);
            var data = new CredData
            {
                UserId = userId,
                PasswordHash = Convert.ToBase64String(hash),
                Salt = Convert.ToBase64String(salt),
                Iterations = iterations
            };
            config.Vaults.Add(data);

            SaveConfig(path, config);
        }

        //認証用
        public static bool ValidateVault(string path, string userId, string password)
        {
            if (!File.Exists(path)) return false;
            var config = LoadConfig(path);
            var data = config.Vaults.FirstOrDefault(v => v.UserId == userId);
            if (data == null) return false;
            var salt = Convert.FromBase64String(data.Salt);
            var expected = Convert.FromBase64String(data.PasswordHash);
            var hash = PBKDF2(password, salt, data.Iterations);
            return SlowEquals(expected, hash);
        }

        //特定のVaultを削除
        public static void DeleteVault(string path, string userId)
        {
            var config = LoadConfig(path);

            var target = config.Vaults.FirstOrDefault(v => v.UserId == userId);
            if (target == null) return;

            config.Vaults.Remove(target);

            SaveConfig(path,config);
        }


        public static void SaveCredentials(string path, string userId, string password)
        {
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            int iterations = 100_000;
            var hash = PBKDF2(password, salt, iterations);

            var data = new CredData
            {
                UserId = userId,
                PasswordHash = Convert.ToBase64String(hash),
                Salt = Convert.ToBase64String(salt),
                Iterations = iterations
            };

            var json = JsonSerializer.Serialize(data);
            File.WriteAllText(path, json, Encoding.UTF8);
        }

        public static bool ValidateCredentials(string path, string userId, string password)
        {
            if (!File.Exists(path)) return false;
            var json = File.ReadAllText(path, Encoding.UTF8);
            var data = JsonSerializer.Deserialize<CredData>(json);
            if (data.UserId != userId) return false;

            var salt = Convert.FromBase64String(data.Salt);
            var expected = Convert.FromBase64String(data.PasswordHash);
            var hash = PBKDF2(password, salt, data.Iterations);

            return SlowEquals(expected, hash);
        }

        private static byte[] PBKDF2(string password, byte[] salt, int iterations)
        {
            using (var k = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return k.GetBytes(32); // 256bit
            }
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }

    }

    // --- Simple CredentialsForm: ユーザーにIDとPasswordを入力させる（Register と Login 用）
    public class CredentialsForm : Form
    {
        public string UserId { get; private set; }
        public string Password { get; private set; }

        private TextBox tbId;
        private TextBox tbPw;
        private Button btnOk;
        private Button btnCancel;
        private Label lblId;
        private Label lblPw;

        // isForValidate = true の場合は「既存のID/PWを入力して認証する」モード
        public CredentialsForm(bool isForValidate = false)
        {
            this.Text = isForValidate ? "Unlock - Enter credentials" : "Set credentials";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ClientSize = new Size(360, 140);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            lblId = new Label() { Left = 12, Top = 14, Text = "ID", Width = 80 };
            tbId = new TextBox() { Left = 100, Top = 10, Width = 240 };

            lblPw = new Label() { Left = 12, Top = 50, Text = "Password", Width = 80 };
            tbPw = new TextBox() { Left = 100, Top = 46, Width = 240, UseSystemPasswordChar = true };

            btnOk = new Button() { Text = "OK", Left = 180, Width = 75, Top = 90, DialogResult = DialogResult.OK };
            btnCancel = new Button() { Text = "Cancel", Left = 260, Width = 75, Top = 90, DialogResult = DialogResult.Cancel };

            btnOk.Click += ButtonCreate_Click;

            this.Controls.Add(lblId);
            this.Controls.Add(tbId);
            this.Controls.Add(lblPw);
            this.Controls.Add(tbPw);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }

        private void ButtonCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbId.Text) || string.IsNullOrEmpty(tbPw.Text))
            {
                MessageBox.Show("ID と Password を入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            UserId = tbId.Text.Trim();
            Password = tbPw.Text;
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }
    }

}

