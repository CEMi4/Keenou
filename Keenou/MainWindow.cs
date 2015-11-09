using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Principal;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Threading;

namespace Keenou
{
    public partial class MainWindow : Form
    {
        const int CIPHER_C_DEFAULT = 0;
        const int HASH_C_DEFAULT = 2;
        const int VOLUME_SIZE_MULT_DEFAULT = 3;

        protected string[] ciphers = { "AES", "Serpent", "Twofish", "AES(Twofish)", "AES(Twofish(Serpent))", "Serpent(AES)", "Serpent(Twofish(AES))", "Twofish(Serpent)" };
        protected string[] hashes = { "sha256", "sha512", "whirlpool", "ripemd160" };

        protected string defaultVolumeLoc = string.Empty;
        protected string homeFolder = string.Empty;
        protected string username = string.Empty;
        protected string usrSID = string.Empty;
        protected long homeDirSize = 0;




        // Constructor //
        public MainWindow()
        {
            InitializeComponent();


            // Get user name
            this.username = Environment.UserName.ToString();

            // Get user SID
            NTAccount acct = new NTAccount(username);
            SecurityIdentifier s = (SecurityIdentifier)acct.Translate(typeof(SecurityIdentifier));
            this.usrSID = s.ToString();

            // Get user home directory 
            this.homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // Get volume location (default) 
            this.defaultVolumeLoc = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)) + this.username + ".hc";




            // Figure out where the home folder's encrypted file is located for this user //
            string encContainerLoc = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Keenou\" + this.usrSID, "encContainerLoc", null);
            if (string.IsNullOrEmpty(encContainerLoc))
            {
                g_homeDirectory.Location = new Point(22, 27);
            }
            // * //



            l_statusLabel.Text = "Ready ...";
            Application.DoEvents();
        }
        // * //



        // Get SHA-512 signature from input text //
        public static string SHA512_Base64(string input)
        {
            using (SHA512 alg = SHA512.Create())
            {
                byte[] result = alg.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(result);
            }
        }
        // * //



        // Calculate the available free space for the drive that the given path resides on //
        private long GetAvailableFreeSpace(string path)
        {
            string targetDrive = Path.GetPathRoot(path);
            DriveInfo di = new DriveInfo(targetDrive);
            long targetSpace = 0;
            if (di != null && di.IsReady)
            {
                targetSpace = di.AvailableFreeSpace / (1024 * 1024);
            }

            return targetSpace;
        }
        // * //



        // Use powershell to get a rough estimate of the directory size //
        private long GetDirectorySize(string directory)
        {
            long size = 0;

            using (Process process = new Process())
            {

                ProcessStartInfo startInfo = new ProcessStartInfo();
                try
                {
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.CreateNoWindow = true;
                    startInfo.FileName = "cmd.exe";
                    startInfo.RedirectStandardOutput = true;
                    startInfo.UseShellExecute = false;
                    startInfo.Arguments = "/C powershell -Command \"& {Get-ChildItem '" + directory + "' -recurse | Measure-Object -property length -sum}\"";
                    process.StartInfo = startInfo;
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    // Ensure no errors were thrown 
                    if (process.ExitCode != 0)
                    {
                        MessageBox.Show("ERROR: Error while determining directory size!");
                        throw new Exception("ERROR: Error while determining directory size!");
                    }


                    string[] tokenize = output.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string token in tokenize)
                    {
                        if (token.StartsWith("Sum"))
                        {
                            Match m = new Regex(@"\d+$").Match(token);
                            if (m.Success)
                            {
                                size = Int64.Parse(m.Value);
                            }
                            break;
                        }
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("ERROR: Failed to determine directory size!");
                    throw new Exception("ERROR: Failed to determine directory size!", err);
                }

            }

            return size;
        }
        // * //



        // Internal events  // 
        private void MainWindow_Load(object sender, EventArgs e)
        {

            // Choose defaults  
            c_cipher.SelectedIndex = CIPHER_C_DEFAULT;
            c_hash.SelectedIndex = HASH_C_DEFAULT;


            // Fill out user name and SID
            t_userName.Text = this.username;
            t_sid.Text = this.usrSID;


            // Set output volume location
            t_volumeLoc.Text = this.defaultVolumeLoc;
            
        }
        // * //



        // When user hits the encrypt button for Home Folder //
        private void b_encrypt_Click(object sender, EventArgs e)
        {

            // Sanity checks //
            if (string.IsNullOrWhiteSpace(t_volumeSize.Text))
            {
                MessageBox.Show("ERROR: Must specify a volume size!");
                return;
            }
            if (t_password.Text.Length <= 0 || !string.Equals(t_password.Text, t_passwordConf.Text))
            {
                MessageBox.Show("ERROR: Passwords provided must match and be non-zero in length!");
                return;
            }
            if (t_volumeLoc.Text.Length <= 0)
            {
                MessageBox.Show("ERROR: You must specify a encrypted volume location!");
                return;
            }
            if (t_volumeLoc.Text.Contains(this.homeFolder))
            {
                MessageBox.Show("ERROR: You cannot put your encrypted volume in your home directory!");
                return;
            }
            if (c_hash.SelectedIndex < 0)
            {
                MessageBox.Show("ERROR: Please choose a hash!");
                return;
            }
            if (c_cipher.SelectedIndex < 0)
            {
                MessageBox.Show("ERROR: Please choose a cipher!");
                return;
            }
            // TODO: warn user if volume size will not fit home directory
            // * //



            // Get user-specified values 
            string hashChosen = c_hash.SelectedItem.ToString();
            string cipherChosen = c_cipher.SelectedItem.ToString();
            long volSize = Int64.Parse(t_volumeSize.Text);



            // Ensure there will be enough space for the enc volume
            if (volSize > GetAvailableFreeSpace(t_volumeLoc.Text))
            {
                MessageBox.Show("ERROR: Your encrypted volume will not fit on the chosen target drive!");
                return;
            }



            // Disable while we calcualte stuff 
            this.Cursor = Cursors.WaitCursor;
            g_homeDirectory.Enabled = false;



            // GET NEXT FREE DRIVE LETTER 
            string targetDrive = null;

            char[] alpha = "VTHEDFGIJKLMNOPQRSUWXYZC".ToCharArray();
            string[] taken = Directory.GetLogicalDrives();
            foreach (char dL in alpha)
            {
                int pos = Array.IndexOf(taken, dL + @":\");
                if (pos == -1)
                {
                    targetDrive = dL.ToString();
                    break;
                }
            }
            if (targetDrive == null)
            {
                MessageBox.Show("ERROR: Cannot find a free drive letter!");
                throw new Exception("ERROR: Cannot find a free drive letter!");
            }
            // * //
            

            
            // Create new encrypted volume //
            using (Process process = new Process())
            {
                // Give user a status update
                l_statusLabel.Text = "Creating encrypted volume ...";
                Application.DoEvents();


                // GET VeraCrypt DIRECTORY
                string programDir = (Environment.GetEnvironmentVariable("PROGRAMFILES(X86)") ?? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) + @"\VeraCrypt\";

                // Make sure veracrypt is installed
                if (!Directory.Exists(programDir))
                {
                    MessageBox.Show("ERROR: VeraCrypt inaccessible!");
                    throw new Exception("ERROR: VeraCrypt inaccessible!");
                }


                // MOUNT ENCRYPTED CONTAINER (TODO: sanitize password?)
                ProcessStartInfo startInfo = new ProcessStartInfo();
                try
                {
                    //.\"VeraCrypt Format.exe" /create test.hc /password testing /size 10M /hash whirlpool /encryption AES(Twofish(Serpent)) /filesystem NTFS /force /silent

                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C \"\"" + programDir + "VeraCrypt Format.exe\" /create \"" + t_volumeLoc.Text + "\" /size " + volSize + "M /hash " + hashChosen + " /encryption " + cipherChosen + " /password \"" + SHA512_Base64(t_password.Text) + "\" /filesystem NTFS /force /dynamic /silent\"";
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit(); // this does not work, since "VeraCrypt Format.exe" exits but child continues running 

                    // Wait until the child process dies 
                    while (Process.GetProcessesByName("VeraCrypt Format").Length > 0)
                    {
                        Thread.Sleep(2000);
                    }

                    // Ensure no errors were thrown 
                    if (process.ExitCode != 0)
                    {
                        MessageBox.Show("ERROR: Error while creating encrypted file!");
                        throw new Exception("ERROR: Error while creating encrypted file!");
                    }

                }
                catch (Exception err)
                {
                    MessageBox.Show("ERROR: Failed to create encrypted volume!");
                    throw new Exception("ERROR: Failed to create encrypted volume!", err);
                }

            }
            // * //
            


            // Mount home folder's encrypted file as targetDrive //
            using (Process process = new Process())
            {
                // Give user a status update
                l_statusLabel.Text = "Mounting encrypted volume ...";
                Application.DoEvents();


                // GET VeraCrypt DIRECTORY
                string programDir = (Environment.GetEnvironmentVariable("PROGRAMFILES(X86)") ?? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) + @"\VeraCrypt\";

                // Make sure veracrypt is installed
                if (!Directory.Exists(programDir))
                {
                    MessageBox.Show("ERROR: VeraCrypt inaccessible!");
                    throw new Exception("ERROR: VeraCrypt inaccessible!");
                }


                // MOUNT ENCRYPTED CONTAINER (TODO: sanitize password?)
                ProcessStartInfo startInfo = new ProcessStartInfo();
                try
                {
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C \"\"" + programDir + "VeraCrypt.exe\" /hash " + hashChosen + " /v \"" + t_volumeLoc.Text + "\" /l " + targetDrive + " /f /h n /p \"" + SHA512_Base64(t_password.Text) + "\" /q /s\"";
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();

                    // Ensure no errors were thrown 
                    if (process.ExitCode != 0)
                    {
                        MessageBox.Show("ERROR: Error while mounting encrypted file!");
                        throw new Exception("ERROR: Error while mounting encrypted file!");
                    }

                    //m_logger.InfoFormat("CMD Argument: {0}", startInfo.Arguments);
                }
                catch (Exception err)
                {
                    MessageBox.Show("ERROR: Failed to mount encrypted home volume. " + err.Message);
                    throw new Exception("ERROR: Failed to mount encrypted home volume. " + err.Message);
                }

            }
            // * //



            // Make sure encrypted system was mounted //
            int cnt = 10;
            while (!Directory.Exists(targetDrive + @":\") && cnt-- > 0 )
            {
                Thread.Sleep(2000);
            }
            if (!Directory.Exists(targetDrive + @":\"))
            {
                MessageBox.Show("ERROR: Could not mount encrypted drive!");
                throw new Exception("ERROR: Could not mount encrypted drive!");
            }
            // * //

            return;

            // Make sure old location exists (before moving files over to new location) 
            if (!Directory.Exists(this.homeFolder) || !File.Exists(this.homeFolder + @"\" + "NTUSER.DAT"))
            {
                MessageBox.Show("ERROR: Current home directory inaccessible!");
                throw new Exception("ERROR: Current home directory inaccessible!");
            }
            // * //



            // Robocopy everything over from home directory to encrypted container //
            using (Process process = new Process())
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                try
                {
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C \"robocopy \"" + this.homeFolder + "\" " + targetDrive + ":\\ /MIR /copyall /sl /xj /r:0\"";
                    process.StartInfo = startInfo;
                    process.Start(); // this may take a while! 
                    process.WaitForExit();

                    // Ensure no errors were thrown 
                    if (process.ExitCode >= 4)
                    {
                        MessageBox.Show("ERROR: Error while copying files over!");
                        throw new Exception("ERROR: Error while copying files over!");
                    }

                }
                catch (Exception err)
                {
                    MessageBox.Show("ERROR: Failed to finish moving home volume. " + err.Message);
                    throw new Exception("ERROR: Failed to finish moving home volume. " + err.Message);
                }
            }
            // * //

            

            // Set necessary registry values //
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Keenou\" + this.usrSID, "encContainerLoc", t_volumeLoc.Text);
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Keenou\" + this.usrSID, "firstBoot", true);
            // * //



            // Re-enable everything //
            this.Cursor = Cursors.Default;
            g_homeDirectory.Enabled = true;
            l_statusLabel.Text = "Ready ...";
            Application.DoEvents();
            // * //



            // Inform user of the good news 
            MessageBox.Show("Almost done!  You must log out and log back in to finish the migration!");
        }
        // * //


        // When user hits "Choose" box to override default volume location //
        private void b_volumeLoc_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog outputFileDialog = new OpenFileDialog())
            {
                outputFileDialog.InitialDirectory = this.defaultVolumeLoc;
                outputFileDialog.FilterIndex = 0;
                outputFileDialog.CheckFileExists = false;
                outputFileDialog.RestoreDirectory = true;

                try
                {
                    if (outputFileDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        t_volumeLoc.Text = outputFileDialog.FileName;

                        if (File.Exists(outputFileDialog.FileName))
                        {
                            MessageBox.Show("Warning: File already exists and will be overwritten!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("[" + ex.GetType().Name + "] " + ex.Message + ex.StackTrace);
                }
            }
        }
        // * //



        // User wants us to suggest a volume size to them //
        private void b_setVolumeSize_Click(object sender, EventArgs e)
        {
            // Disable while we calcualte stuff 
            this.Cursor = Cursors.WaitCursor;
            g_homeDirectory.Enabled = false;
            l_statusLabel.Text = "Calculating your home directory size  ...";
            Application.DoEvents();


            // Determine free space on enc volume target drive 
            long targetSpace = GetAvailableFreeSpace(t_volumeLoc.Text);


            // Do calculation of current size (if not already done) 
            if (this.homeDirSize <= 0)
            {
                this.homeDirSize = GetDirectorySize(this.homeFolder);
            }


            // Show suggested volume size 
            long volSizeSuggested = (VOLUME_SIZE_MULT_DEFAULT * this.homeDirSize / (1024 * 1024));
            t_volumeSize.Text = volSizeSuggested.ToString();


            // If not enough space, alert user
            if (volSizeSuggested >= targetSpace)
            {
                string targetDrive = Path.GetPathRoot(t_volumeLoc.Text);
                MessageBox.Show("Warning: There is not enough space on the " + targetDrive + " drive! " );
            }


            // Re-enable everything 
            this.Cursor = Cursors.Default;
            g_homeDirectory.Enabled = true;
            l_statusLabel.Text = "Ready ...";
            Application.DoEvents();
        }
        // * //



    } // End MainWindow class 

    // End namespace 
}
