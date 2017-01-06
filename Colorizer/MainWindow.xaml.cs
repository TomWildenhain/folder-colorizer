using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using System.Threading;

namespace Colorizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string path2 = "";
        int index2 = 0;
        string Hash1 = "";
        string Hash2 = "";
        string HashText = "No Match";
        string Hash1Text = "Hash 1";
        string Hash2Text = "Hash 2";

        public MainWindow()
        {
            InitializeComponent();
            //File.SetAttributes(@"C:\Users\Wildenhain\Documents\Tom's Stuff\Random Stuff\Colorizer\Default.ico", FileAttributes.Normal);
            string[] myArray = { "Red", "Orange", "Green", "Blue", "Violet" };
            foreach (string rectName in myArray)
            {
                ((TextBlock)FindName("Rect" + rectName)).AllowDrop = true;
                ((TextBlock)FindName("Rect" + rectName)).DragEnter += new DragEventHandler(Form1_DragEnter);
                ((TextBlock)FindName("Rect" + rectName)).Drop += new DragEventHandler(Form1_DragDrop);
                ((TextBlock)FindName("Rect" + rectName)).Foreground = Brushes.White;
                ((TextBlock)FindName("Rect" + rectName)).TextAlignment = TextAlignment.Center;
            }
            RectClear.AllowDrop = true;
            RectClear.DragEnter += new DragEventHandler(Form1_DragEnter);
            RectClear.Drop += new DragEventHandler(Form1_DragDrop);
            RectClear.TextAlignment = TextAlignment.Center;
            RectHash1.AllowDrop = true;
            RectHash1.DragEnter += new DragEventHandler(Form1_DragEnter);
            RectHash1.Drop += new DragEventHandler(Hash_DragDrop);
            RectHash1.Foreground = Brushes.White;
            RectHash1.TextAlignment = TextAlignment.Center;
            RectHash2.AllowDrop = true;
            RectHash2.DragEnter += new DragEventHandler(Form1_DragEnter);
            RectHash2.Drop += new DragEventHandler(Hash_DragDrop);
            RectHash2.Foreground = Brushes.White;
            RectHash2.TextAlignment = TextAlignment.Center;
            HashResult.AllowDrop = true;
            HashResult.Foreground = Brushes.White;
            HashResult.TextAlignment = TextAlignment.Center;
        }
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effects = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files) Console.WriteLine(file);
            int rectIndex = 0;
            string[] myArray = { "Red", "Orange", "Green", "Blue", "Violet", "Clear" };
            for (int i = 0; i < myArray.Length; i++)
            {
                if ((sender as TextBlock).Name == "Rect" + myArray[i])
                {
                    rectIndex = i;
                }
            }
            int[] myArray2 = { 3, 2, 6, 1, 4, 7 };
            index2 = myArray2[rectIndex];
            
            foreach (string file in files)
            {
                StartTheThread(file, index2);
                
            }
                
        }
        public Thread StartTheThread(string path, int index)
        {
            var t = new Thread(() => tryChangeIcon(path, index));
            t.Start();
            return t;
        }
        private void tryChangeIcon(string path, int index)
        {
            try
            {
                changeIcon2(path, index);
            }
            catch { }
        }
        private void changeIcon(string path, int index)
        {
            //FileInfo fileInfo = new FileInfo(@"D:\New folder\desktop.ini");
            FileInfo fileInfo = new FileInfo(path+@"\desktop.ini");
            FileAttributes fa = fileInfo.Attributes;
            FileStream fs = fileInfo.OpenWrite();
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("[.ShellClassInfo]");
            sw.WriteLine(@"IconResource=C:\colorIcons.icl," + index);
            sw.Flush();
            sw.Close();
            fs.Close();
            fileInfo.Attributes = FileAttributes.Hidden | FileAttributes.Archive | FileAttributes.System;
            RefreshIconCache();
        }
        private void changeIcon2(string path, int index)
        {
            try
            {
                if (File.Exists(path + @"\desktop.ini"))
                {
                    File.Delete(path + @"\desktop.ini");
                }
            }
            catch { }
            if (index == 7)
            {
                LPSHFOLDERCUSTOMSETTINGS FolderSettings = new LPSHFOLDERCUSTOMSETTINGS();
                FolderSettings.dwMask = 0x10;
                FolderSettings.pszIconFile = @"C:\colorIcons.icl";
                FolderSettings.iIconIndex = 0;

                UInt32 FCS_READ = 0x00000001;
                UInt32 FCS_FORCEWRITE = 0x00000002;
                UInt32 FCS_WRITE = FCS_READ | FCS_FORCEWRITE;

                string pszPath = path;
                UInt32 HRESULT = SHGetSetFolderCustomSettings(ref FolderSettings, pszPath, FCS_WRITE);


                try
                {
                    if (File.Exists(path + @"\desktop.ini"))
                    {
                        File.Delete(path + @"\desktop.ini");
                    }
                }
                catch { }
            }
            else
            {
                LPSHFOLDERCUSTOMSETTINGS FolderSettings = new LPSHFOLDERCUSTOMSETTINGS();
                FolderSettings.dwMask = 0x10;
                FolderSettings.pszIconFile = @"C:\colorIcons.icl";
                FolderSettings.iIconIndex = index;

                UInt32 FCS_READ = 0x00000001;
                UInt32 FCS_FORCEWRITE = 0x00000002;
                UInt32 FCS_WRITE = FCS_READ | FCS_FORCEWRITE;

                string pszPath = path;
                UInt32 HRESULT = SHGetSetFolderCustomSettings(ref FolderSettings, pszPath, FCS_WRITE);
                //Console.WriteLine(HRESULT.ToString("x"));
                //Console.ReadLine();
            }
        }
        private void Hash_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (sender == RectHash1)
            {
                RectHash1.Text = "Hashing...";
                Hash1Text = "Hashing...";
            }
            else
            {
                RectHash2.Text = "Hashing...";
                Hash2Text = "Hashing...";
            }
            StartTheHash(sender, files[0]);
        }
        public Thread StartTheHash(object sender, string file)
        {
            var t = new Thread(() => HashAsync(sender, file));
            t.Start();
            return t;
        }
        private void HashAsync(object sender, string file)
        {
            try
            {
                if (sender == RectHash1)
                {
                    Hash1 = MD5HashFile(file);
                    Hash1Text = "Hashed";
                }
                else
                {
                    Hash2 = MD5HashFile(file);
                    Hash2Text = "Hashed";
                }
                if (Hash1 == Hash2)
                {
                    HashText = "Match";
                }
                else
                {
                    HashText = "No Match";
                }
            }
            catch { }
            var d = Application.Current.Dispatcher;
            if (d.CheckAccess())
                displayHash();
            else
                d.BeginInvoke((Action)displayHash);
        }
        private void displayHash()
        {
            RectHash1.Text = Hash1Text;
            RectHash2.Text = Hash2Text;
            HashResult.Text = HashText;
        }
        public string MD5HashFile(string fn)
        {
            byte[] hash = System.Security.Cryptography.MD5.Create().ComputeHash(File.ReadAllBytes(fn));
            return BitConverter.ToString(hash).Replace("-", "");
        }



        [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
        static extern UInt32 SHGetSetFolderCustomSettings(ref LPSHFOLDERCUSTOMSETTINGS pfcs, string pszPath, UInt32 dwReadWrite);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct LPSHFOLDERCUSTOMSETTINGS
        {
            public UInt32 dwSize;
            public UInt32 dwMask;
            public IntPtr pvid;
            public string pszWebViewTemplate;
            public UInt32 cchWebViewTemplate;
            public string pszWebViewTemplateVersion;
            public string pszInfoTip;
            public UInt32 cchInfoTip;
            public IntPtr pclsid;
            public UInt32 dwFlags;
            public string pszIconFile;
            public UInt32 cchIconFile;
            public int iIconIndex;
            public string pszLogo;
            public UInt32 cchLogo;
        }
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr SendMessageTimeout(
            int windowHandle,
            int Msg,
            int wParam,
            String lParam,
            SendMessageTimeoutFlags flags,
            int timeout,
            out int result);
        [Flags]
        enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8
        }

        static void RefreshIconCache()
        {

            // get the the original Shell Icon Size registry string value
            RegistryKey k = Registry.CurrentUser.OpenSubKey("Control Panel").OpenSubKey("Desktop").OpenSubKey("WindowMetrics", true);
            Object OriginalIconSize = k.GetValue("Shell Icon Size");

            // set the Shell Icon Size registry string value
            k.SetValue("Shell Icon Size", (Convert.ToInt32(OriginalIconSize) + 1).ToString());
            k.Flush(); k.Close();

            // broadcast WM_SETTINGCHANGE to all window handles
            int res = 0;
            SendMessageTimeout(0xffff, 0x001A, 0, "", SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 5000, out res);
            //SendMessageTimeout(HWD_BROADCAST,WM_SETTINGCHANGE,0,"",SMTO_ABORTIFHUNG,5 seconds, return result to res)

            // set the Shell Icon Size registry string value to original value
            k = Registry.CurrentUser.OpenSubKey("Control Panel").OpenSubKey("Desktop").OpenSubKey("WindowMetrics", true);
            k.SetValue("Shell Icon Size", OriginalIconSize);
            k.Flush(); k.Close();

            SendMessageTimeout(0xffff, 0x001A, 0, "", SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 5000, out res);
        }
    }
}
