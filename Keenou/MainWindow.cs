/*
 * Keenou
 * Copyright (C) 2015  Charles Munson
 * 
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along
 * with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using System;
using System.Windows.Forms;
using System.Security.Principal;
using System.IO;
using Microsoft.Win32;
using System.Threading;
using System.Threading.Tasks;
using Org.BouncyCastle.Security;
using System.Diagnostics;

namespace Keenou
{
    public partial class MainWindow : Form
    {
        // General random number generator instance 
        private static readonly SecureRandom Random = new SecureRandom();

        // Various globals used throughout routines 
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
            string encDrive = (string)Registry.GetValue(Config.LOCAL_MACHINE_REG_ROOT + this.usrSID, "encDrive", string.Empty);
            string encContainerLoc = (string)Registry.GetValue(Config.LOCAL_MACHINE_REG_ROOT + this.usrSID, "encContainerLoc", string.Empty);
            if (!string.IsNullOrWhiteSpace(encContainerLoc) && !string.IsNullOrWhiteSpace(encDrive) && Directory.Exists(encDrive + @":\"))
            {
                // We're already running in an encrypted home directory environment! 
                g_tabContainer.Controls[0].Enabled = false;
                l_homeAlreadyEncrypted.Visible = true;
                l_homeAlreadyEncrypted.Enabled = true;
            }
            // * //



            l_statusLabel.Text = "Ready ...";
            Application.DoEvents();
        }
        // * //



        // Internal events  // 
        private void MainWindow_Load(object sender, EventArgs e)
        {

            // Choose defaults  
            c_cipher.SelectedIndex = Config.CIPHER_C_DEFAULT;
            c_hash.SelectedIndex = Config.HASH_C_DEFAULT;


            // Fill out user name and SID
            t_userName.Text = this.username;
            t_sid.Text = this.usrSID;


            // Set output volume location
            t_volumeLoc.Text = this.defaultVolumeLoc;

        }
        // * //



        // When user hits the encrypt button for Home Folder //
        private void ReportEncryptHomeError(BooleanResult res)
        {
            if (res.Message != null)
            {
                MessageBox.Show(res.Message);
            }

            // Reset state of window, and display error conditions 
            this.Cursor = Cursors.Default;
            g_tabContainer.Controls[0].Enabled = true;
            l_statusLabel.Text = "ERROR";
            s_progress.Value = 0;
            s_progress.Visible = false;
        }
        private void b_encrypt_Click(object sender, EventArgs e)
        {

            // Sanity checks //
            if (string.IsNullOrWhiteSpace(t_volumeSize.Text))
            {
                ReportEncryptHomeError(new BooleanResult() { Success = false, Message = "Please specify a volume size!" });
                return;
            }
            if (t_password.Text.Length <= 0 || !string.Equals(t_password.Text, t_passwordConf.Text))
            {
                ReportEncryptHomeError(new BooleanResult() { Success = false, Message = "Passwords provided must match and be non-zero in length!" });
                return;
            }
            if (t_volumeLoc.Text.Length <= 0)
            {
                ReportEncryptHomeError(new BooleanResult() { Success = false, Message = "Please specify a encrypted volume location!" });
                return;
            }
            if (t_volumeLoc.Text.Contains(this.homeFolder))
            {
                ReportEncryptHomeError(new BooleanResult() { Success = false, Message = "You cannot store your encrypted home volume in your home directory!" });
                return;
            }
            if (c_hash.SelectedIndex < 0)
            {
                ReportEncryptHomeError(new BooleanResult() { Success = false, Message = "Please choose a hash!" });
                return;
            }
            if (c_cipher.SelectedIndex < 0)
            {
                ReportEncryptHomeError(new BooleanResult() { Success = false, Message = "Please choose a cipher!" });
                return;
            }
            // TODO: warn user if volume size will not fit home directory
            // * //



            // Helper result object
            BooleanResult res = null;


            // Get user-specified values 
            string hashChosen = c_hash.SelectedItem.ToString();
            string cipherChosen = c_cipher.SelectedItem.ToString();
            long volSize = Int64.Parse(t_volumeSize.Text);


            // Progress bar 
            s_progress.Value = 0;
            s_progress.Visible = true;
            Application.DoEvents();
            s_progress.ProgressBar.Refresh();


            // Ensure there will be enough space for the enc volume
            if (volSize > Toolbox.GetAvailableFreeSpace(t_volumeLoc.Text))
            {
                ReportEncryptHomeError(new BooleanResult() { Success = false, Message = "ERROR: Your encrypted volume will not fit on the chosen target drive!" });
                return;
            }



            // Disable while we calcualte stuff 
            this.Cursor = Cursors.WaitCursor;
            g_tabContainer.Controls[0].Enabled = false;



            // GET NEXT FREE DRIVE LETTER 
            string targetDrive = Toolbox.GetNextFreeDriveLetter();
            if (targetDrive == null)
            {
                ReportEncryptHomeError(new BooleanResult() { Success = false, Message = "ERROR: Cannot find a free drive letter!" });
                return;
            }
            // * //



            // Generate master key & protect with user password //
            l_statusLabel.Text = "Generating encryption key ...";
            Application.DoEvents();

            string masterKey = Toolbox.GenerateKey(Config.MASTERKEY_PW_CHAR_COUNT);
            string encMasterKey = Toolbox.PasswordEncryptKey(masterKey, t_password.Text);

            // Ensure we got good stuff back 
            if (masterKey == null || encMasterKey == null)
            {
                ReportEncryptHomeError(new BooleanResult() { Success = false, Message = "ERROR: Cannot generate master key!" });
                return;
            }
            // * //




            // Run work-heavy tasks in a separate thread 
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken cancelToken = cts.Token;
            var workerThread = Task.Factory.StartNew(() =>
            {

                // Update UI 
                this.Invoke((MethodInvoker)delegate
                {
                    s_progress.Value = 17;
                    l_statusLabel.Text = "Creating encrypted volume ...";
                    Application.DoEvents();
                    s_progress.ProgressBar.Refresh();
                });

                // Create new encrypted volume //
                res = EncryptDirectory.CreateEncryptedVolume(hashChosen, t_volumeLoc.Text, masterKey, cipherChosen, volSize);
                if (res == null || !res.Success)
                {
                    return res;
                }
                // * //



                // Update UI 
                this.Invoke((MethodInvoker)delegate
                {
                    s_progress.Value = 33;
                    l_statusLabel.Text = "Mounting encrypted volume ...";
                    Application.DoEvents();
                    s_progress.ProgressBar.Refresh();
                });

                // Mount home folder's encrypted file as targetDrive //
                res = EncryptDirectory.MountEncryptedVolume(hashChosen, t_volumeLoc.Text, targetDrive, masterKey);
                if (res == null || !res.Success)
                {
                    return res;
                }
                // * //



                // Update UI 
                this.Invoke((MethodInvoker)delegate
                {
                    s_progress.Value = 50;
                    l_statusLabel.Text = "Copying home directory to encrypted container ...";
                    Application.DoEvents();
                    s_progress.ProgressBar.Refresh();
                });

                // Copy everything over from home directory to encrypted container //
                res = EncryptDirectory.CopyDataFromHomeFolder(this.homeFolder, targetDrive);
                if (res == null || !res.Success)
                {
                    return res;
                }
                // * //



                // Update UI 
                this.Invoke((MethodInvoker)delegate
                {
                    s_progress.Value = 67;
                    l_statusLabel.Text = "Unmounting encrypted drive ...";
                    Application.DoEvents();
                    s_progress.ProgressBar.Refresh();
                });

                // unmount so we can mount upon login 
                res = EncryptDirectory.UnmountEncryptedVolume(targetDrive);
                if (res == null || !res.Success)
                {
                    return res;
                }
                // * //



                // Update UI 
                this.Invoke((MethodInvoker)delegate
                {
                    s_progress.Value = 84;
                    l_statusLabel.Text = "Installing Keenou-pGina ...";
                    Application.DoEvents();
                    s_progress.ProgressBar.Refresh();
                });

                // Install Keenou-pGina 
                using (Process process = new Process())
                {

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    try
                    {
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.FileName = "cmd.exe";
                        startInfo.Arguments = "/C \"\"" + Config.KeenouProgramDirectory + "\\Keenou-pGina-setup.exe\"\"";
                        process.StartInfo = startInfo;
                        process.Start(); // this may take a while! 
                        process.WaitForExit();

                        // Ensure no errors were thrown 
                        if (process.ExitCode > 0)
                        {
                            return new BooleanResult() { Success = false, Message = "ERROR: Error while installing Keenou-pGina!" };
                        }
                    }
                    catch (Exception err)
                    {
                        return new BooleanResult() { Success = false, Message = "ERROR: Failed to install Keenou-pGina. " + err.Message };
                    }
                }


                return new BooleanResult() { Success = true };

            }, TaskCreationOptions.LongRunning);



            // When threaded tasks finish, check for errors and continue (if appropriate) 
            workerThread.ContinueWith((antecedent) =>
            {
                // Block until we get a result back from previous thread
                BooleanResult result = antecedent.Result;


                // Check if there was an error in previous thread
                if (result == null || !result.Success)
                {
                    ReportEncryptHomeError(result);
                    return;
                }



                // Set necessary registry values //
                Registry.SetValue(Config.LOCAL_MACHINE_REG_ROOT + this.usrSID, "encContainerLoc", t_volumeLoc.Text);
                Registry.SetValue(Config.LOCAL_MACHINE_REG_ROOT + this.usrSID, "firstBoot", true, RegistryValueKind.DWord);
                Registry.SetValue(Config.LOCAL_MACHINE_REG_ROOT + this.usrSID, "hash", hashChosen);
                Registry.SetValue(Config.LOCAL_MACHINE_REG_ROOT + this.usrSID, "encHeader", encMasterKey);
                // * //



                // Re-enable everything //
                this.Cursor = Cursors.Default;
                l_statusLabel.Text = "Log out and back in to finish ...";
                s_progress.Value = 100;
                Application.DoEvents();
                // * //



                // Inform user of the good news 
                MessageBox.Show("Almost done!  You must log out and log back in via Keenou-pGina to finish the migration!");

            },
            cancelToken,
            TaskContinuationOptions.OnlyOnRanToCompletion,
            TaskScheduler.FromCurrentSynchronizationContext()
            );

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



        // When user hits "Encrypt" button for Cloud service //
        private void ReportEncryptCloudError(BooleanResult res)
        {
            if (res.Message != null)
            {
                MessageBox.Show(res.Message);
            }

            // Reset state of window, and display error conditions 
            this.Cursor = Cursors.Default;
            g_tabContainer.Controls[1].Enabled = true;
            l_statusLabel.Text = "ERROR";
            s_progress.Value = 0;
            s_progress.Visible = false;
        }
        private void b_encryptCloud_Click(object sender, EventArgs e)
        {
            // Sanity checks //
            if (t_cloudPW.Text.Length < Config.MIN_PASSWORD_LEN || !string.Equals(t_cloudPW.Text, t_cloudPWConf.Text))
            {
                ReportEncryptHomeError(new BooleanResult() { Success = false, Message = "Passwords provided must match and be non-zero in length!" });
                return;
            }
            // * //



            // GET NEXT FREE DRIVE LETTER 
            string targetDrive = Toolbox.GetNextFreeDriveLetter();
            if (targetDrive == null)
            {
                ReportEncryptCloudError(new BooleanResult() { Success = false, Message = "ERROR: Cannot find a free drive letter!" });
                return;
            }
            // * //



            // Disable while we calcualte stuff 
            this.Cursor = Cursors.WaitCursor;
            g_tabContainer.Controls[1].Enabled = false;


            // Generate a new GUID to identify this FS
            string guid = Guid.NewGuid().ToString();



            // Helper result object
            BooleanResult res = null;



            // Figure out where the cloud's folder is on this computer 
            string type = Config.CLOUD_SERVICES[0];
            string cloudPath = EncryptFS.GetCloudServicePath(type);
            if (cloudPath == null)
            {
                ReportEncryptCloudError(new BooleanResult() { Success = false, Message = "ERROR: Cannot find folder path for cloud service " + type });
                return;
            }
            // * //



            // Generate master key & protect with user password //
            l_statusLabel.Text = "Generating encryption key ...";
            Application.DoEvents();

            string masterKey = Toolbox.GenerateKey(Config.MASTERKEY_PW_CHAR_COUNT);
            string encMasterKey = Toolbox.PasswordEncryptKey(masterKey, t_cloudPW.Text);

            // Ensure we got good stuff back 
            if (masterKey == null)
            {
                ReportEncryptCloudError(new BooleanResult() { Success = false, Message = "ERROR: Cannot generate master key!" });
                return;
            }
            if (encMasterKey == null)
            {
                ReportEncryptCloudError(new BooleanResult() { Success = false, Message = "ERROR: Cannot encrypt master key!" });
                return;
            }

            Registry.SetValue(Config.CURR_USR_REG_DRIVE_ROOT + guid, "encHeader", encMasterKey);
            // * //



            // Generate temporary location to hold enc data
            string tempFolderName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + Path.GetRandomFileName());
            Directory.CreateDirectory(tempFolderName);
            l_statusLabel.Text = "Temp folder: " + tempFolderName;


            // Create new EncFS
            l_statusLabel.Text = "Creating EncFS drive";
            res = EncryptFS.CreateEncryptedFS(guid, tempFolderName, targetDrive, masterKey, "Secure " + type, true);
            if (res == null || !res.Success)
            {
                ReportEncryptCloudError(res);
                return;
            }
            // * //


            // Copy cloud data over 
            l_statusLabel.Text = "Copying data from Cloud folder to encrypted drive";
            res = EncryptFS.CopyDataFromFolder(cloudPath, targetDrive, tempFolderName);
            if (res == null || !res.Success)
            {
                ReportEncryptCloudError(res);
                return;
            }
            // * //


            // Unmount encrypted folder (prepare to move it) 
            res = EncryptFS.UnmountEncryptedFS(targetDrive);
            if (res == null || !res.Success)
            {
                ReportEncryptCloudError(res);
                return;
            }
            // * //


            // TODO: RENAME (FOR NOW) CLOUD FOLDER TO XXXX.BACKUP -- switch to remove after beta 
            try
            {
                Directory.Move(cloudPath, cloudPath + ".backup-" + Path.GetRandomFileName());
            }
            catch (Exception err)
            {
                ReportEncryptCloudError(new BooleanResult() { Success = false, Message = "ERROR: Cannot remove old Cloud folder, maybe syncing was not pause or a file is in use? " + err.Message });
                return;
            }
            // * //


            // Move the encrypted cloud folder to the old cloud path (Cloud provider should re-sync with encrypted files) 
            try
            {
                Directory.Move(tempFolderName, cloudPath);
            }
            catch (Exception err)
            {
                ReportEncryptCloudError(new BooleanResult() { Success = false, Message = "ERROR: Cannot move Cloud folder, maybe syncing was not paused or a file is in use? " + err.Message });
                return;
            }
            // * //


            // Update the location of the encrypted volume (was temp, now is the cloud path) 
            res = EncryptFS.ChangeEncryptedFSParams(guid, cloudPath);
            if (res == null || !res.Success)
            {
                ReportEncryptCloudError(res);
                return;
            }
            // * //



            // TODO: Recover from errors along the way 



            // GET A NEW FREE DRIVE LETTER 
            targetDrive = Toolbox.GetNextFreeDriveLetter();
            if (targetDrive == null)
            {
                ReportEncryptCloudError(new BooleanResult() { Success = false, Message = "ERROR: Cannot find a free drive letter!" });
                return;
            }
            // * //


            // Mount their freshly-created encrypted drive 
            res = EncryptFS.MountEncryptedFS(guid, targetDrive, masterKey, "Secure " + type);
            if (res == null || !res.Success)
            {
                ReportEncryptCloudError(res);
                return;
            }
            // * //



            // Re-enable everything //
            this.Cursor = Cursors.Default;
            g_tabContainer.Controls[1].Enabled = true;
            l_statusLabel.Text = "Successfully moved your cloud folder!";
            s_progress.Value = 0;
            s_progress.Visible = false;
            Application.DoEvents();
            // * //

        }

        private void b_mountDropbox_Click(object sender, EventArgs e)
        {

            // GET NEXT FREE DRIVE LETTER 
            string targetDrive = Toolbox.GetNextFreeDriveLetter();
            if (targetDrive == null)
            {
                MessageBox.Show("ERROR: Cannot find a free drive letter!");
                return;
            }
            // * //


            BooleanResult res = null;



            // Figure out where the cloud's folder is on this computer 
            string type = Config.CLOUD_SERVICES[0];
            string cloudPath = EncryptFS.GetCloudServicePath(type);
            if (cloudPath == null || !Directory.Exists(cloudPath))
            {
                MessageBox.Show("ERROR: Cannot find folder path for cloud service " + type);
                return;
            }
            // * //



            // Find guid of desired cloud folder //
            string guid = null;
            RegistryKey OurKey = Registry.CurrentUser;
            OurKey = OurKey.OpenSubKey(@"Software\Keenou");
            foreach (string Keyname in OurKey.GetSubKeyNames())
            {
                RegistryKey key = OurKey.OpenSubKey(Keyname);
                if (key.GetValue("encContainerLoc").ToString() == cloudPath)
                {
                    guid = Keyname.ToString();
                }
            }
            if (guid == null)
            {
                MessageBox.Show("Cannot find encrypted cloud folder!");
                return;
            }
            // * //



            // Get and decrypt user's master key (using user password) //
            string masterKey = null;
            string encHeader = (string)Registry.GetValue(Config.CURR_USR_REG_DRIVE_ROOT + guid, "encHeader", null);
            if (string.IsNullOrEmpty(encHeader))
            {
                MessageBox.Show("User's header information could not be found.");
                return;
            }

            masterKey = Toolbox.PasswordDecryptKey(encHeader, this.t_mountDropboxPW.Text);

            // Make sure we got a key back 
            if (masterKey == null)
            {
                MessageBox.Show("Failed to decrypt master key!");
                return;
            }
            // * //



            // Mount their freshly-created encrypted drive 
            res = EncryptFS.MountEncryptedFS(guid, targetDrive, masterKey, "Secure " + type);
            if (res == null || !res.Success)
            {
                MessageBox.Show(res.Message);
                return;
            }
            // * //


        }

        private void b_unmountDropbox_Click(object sender, EventArgs e)
        {

            BooleanResult res = null;



            // Figure out where the cloud's folder is on this computer 
            string type = Config.CLOUD_SERVICES[0];
            string cloudPath = EncryptFS.GetCloudServicePath(type);
            if (cloudPath == null)
            {
                MessageBox.Show("ERROR: Cannot find folder path for cloud service " + type);
                return;
            }
            // * //



            // Find guid of desired cloud folder //
            string guid = null;
            RegistryKey OurKey = Registry.CurrentUser;
            OurKey = OurKey.OpenSubKey(@"Software\Keenou");
            foreach (string Keyname in OurKey.GetSubKeyNames())
            {
                RegistryKey key = OurKey.OpenSubKey(Keyname);
                if (key.GetValue("encContainerLoc").ToString() == cloudPath)
                {
                    guid = Keyname.ToString();
                }
            }
            if (guid == null)
            {
                MessageBox.Show("Cannot find encrypted cloud folder!");
                return;
            }
            // * //


            // Determine where this cloud is mounted to //
            string targetDrive = (string)Registry.GetValue(Config.CURR_USR_REG_DRIVE_ROOT + guid, "encDrive", null);
            if (string.IsNullOrEmpty(targetDrive))
            {
                MessageBox.Show("Target drive not found!  Is cloud mounted?");
                return;
            }
            // * //


            // Unmount encrypted drive 
            res = EncryptFS.UnmountEncryptedFS(targetDrive);
            if (res == null || !res.Success)
            {
                MessageBox.Show(res.Message);
                return;
            }
            // * //

        }
        // * //



        // User wants us to suggest a volume size to them //
        private void b_setVolumeSize_Click(object sender, EventArgs e)
        {
            // Disable while we calcualte stuff 
            this.Cursor = Cursors.WaitCursor;
            g_tabContainer.Controls[0].Enabled = false;
            l_statusLabel.Text = "Calculating your home directory size  ...";
            Application.DoEvents();


            // Do calculation of current size (if not already done) 
            if (this.homeDirSize <= 0)
            {
                var taskA = Task.Factory.StartNew(() => Toolbox.GetDirectorySize(this.homeFolder), TaskCreationOptions.LongRunning);
                CancellationTokenSource cts = new CancellationTokenSource();
                CancellationToken cancelToken = cts.Token;

                taskA.ContinueWith((antecedent) =>
                {
                    this.homeDirSize = antecedent.Result;
                    this.b_setVolumeSize_Click_Callback();
                },
                cancelToken,
                TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.FromCurrentSynchronizationContext()
                );
            }
            else
            {
                this.b_setVolumeSize_Click_Callback();
            }
        }

        private void b_setVolumeSize_Click_Callback()
        {
            // Determine free space on enc volume target drive 
            long targetSpace = Toolbox.GetAvailableFreeSpace(t_volumeLoc.Text);


            // Show suggested volume size 
            long volSizeSuggested = (Config.VOLUME_SIZE_MULT_DEFAULT * this.homeDirSize / (1024 * 1024));
            t_volumeSize.Text = volSizeSuggested.ToString();


            // If not enough space, alert user
            if (volSizeSuggested >= targetSpace)
            {
                string targetDrive = Path.GetPathRoot(t_volumeLoc.Text);
                MessageBox.Show("Warning: There is not enough space on the " + targetDrive + " drive! ");
            }


            // Re-enable everything 
            this.Cursor = Cursors.Default;
            g_tabContainer.Controls[0].Enabled = true;
            l_statusLabel.Text = "Ready ...";
            Application.DoEvents();
        }
        // * //



        // MENU ITEMS //

        // User clicks the "About" menu item 
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }

        // User clicks "Add New Personal Folder" menu item
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddPersonalFolder pf = new AddPersonalFolder();
            pf.ShowDialog();
        }
        // * //



    } // End MainWindow class 

    // End namespace 
}
