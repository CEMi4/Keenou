using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace Keenou
{
    public class EncryptHome
    {




        // Create new encrypted volume //
        public static BooleanResult CreateEncryptedVolume(string hash, string volumeLoc, string targetDrive, string password, string cipherChosen, long volumeSize)
        {

            using (Process process = new Process())
            {

                // GET VeraCrypt DIRECTORY
                string programDir = (Environment.GetEnvironmentVariable("PROGRAMFILES(X86)") ?? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) + @"\VeraCrypt\";

                // Make sure veracrypt is installed
                if (!Directory.Exists(programDir))
                {
                    return new BooleanResult() { Success = false, Message = "ERROR: VeraCrypt inaccessible!" };
                }


                // MOUNT ENCRYPTED CONTAINER (TODO: sanitize password?)
                ProcessStartInfo startInfo = new ProcessStartInfo();
                try
                {
                    //.\"VeraCrypt Format.exe" /create test.hc /password testing /size 10M /hash whirlpool /encryption AES(Twofish(Serpent)) /filesystem NTFS /force /silent

                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C \"\"" + programDir + "VeraCrypt Format.exe\" /create \"" + volumeLoc + "\" /size " + volumeSize + "M /hash " + hash + " /encryption " + cipherChosen + " /password \"" + password + "\" /filesystem NTFS /force /dynamic /silent\"";
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
                        return new BooleanResult() { Success = false, Message = "ERROR: Error while creating encrypted file" };
                    }

                }
                catch (Exception err)
                {
                    return new BooleanResult() { Success = false, Message = "ERROR: Failed to create encrypted volume! " + err.Message };
                }

            }

            return new BooleanResult() { Success = true };
        }
        // * //



        // Mount home folder's encrypted file as targetDrive //
        public static BooleanResult MountEncryptedVolume(string hash, string volumeLoc, string targetDrive, string password)
        {

            using (Process process = new Process())
            {

                // GET VeraCrypt DIRECTORY
                string programDir = (Environment.GetEnvironmentVariable("PROGRAMFILES(X86)") ?? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) + @"\VeraCrypt\";

                // Make sure veracrypt is installed
                if (!Directory.Exists(programDir))
                {
                    return new BooleanResult() { Success = false, Message = "ERROR: VeraCrypt inaccessible!" };
                }


                // MOUNT ENCRYPTED CONTAINER (TODO: sanitize password?)
                ProcessStartInfo startInfo = new ProcessStartInfo();
                try
                {
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C \"\"" + programDir + "VeraCrypt.exe\" /hash " + hash + " /v \"" + volumeLoc + "\" /l " + targetDrive + " /f /h n /p \"" + password + "\" /q /s\"";
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();

                    // Ensure no errors were thrown 
                    if (process.ExitCode != 0)
                    {
                        return new BooleanResult() { Success = false, Message = "ERROR: Error while mounting encrypted file!" };
                    }

                    //m_logger.InfoFormat("CMD Argument: {0}", startInfo.Arguments);
                }
                catch (Exception err)
                {
                    return new BooleanResult() { Success = false, Message = "ERROR: Failed to mount encrypted home volume. " + err.Message };
                }

            }
            // * //



            // Make sure encrypted system was mounted //
            int cnt = 10;
            while (!Directory.Exists(targetDrive + @":\") && cnt-- > 0)
            {
                Thread.Sleep(1000);
            }
            if (!Directory.Exists(targetDrive + @":\"))
            {
                return new BooleanResult() { Success = false, Message = "ERROR: Could not mount encrypted drive!" };
            }


            return new BooleanResult() { Success = true };
        }
        // * //



        // Robocopy everything over from home directory to encrypted container //
        public static BooleanResult CopyDataFromHomeFolder(string homeFolder, string targetDrive)
        {

            // Make sure old location exists (before moving files over to new location) 
            if (!Directory.Exists(homeFolder) || !File.Exists(homeFolder + @"\" + "NTUSER.DAT"))
            {
                return new BooleanResult() { Success = false, Message = "ERROR: Current home directory inaccessible!" };
            }
            // * //



            using (Process process = new Process())
            {

                ProcessStartInfo startInfo = new ProcessStartInfo();
                try
                {
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C \"robocopy \"" + homeFolder + "\" " + targetDrive + ":\\ /MIR /copyall /sl /xj /r:0\"";
                    process.StartInfo = startInfo;
                    process.Start(); // this may take a while! 
                    process.WaitForExit();

                    // Ensure no errors were thrown 
                    if (process.ExitCode == 16)
                    {
                        return new BooleanResult() { Success = false, Message = "ERROR: Error while copying files over!" };
                    }

                }
                catch (Exception err)
                {
                    return new BooleanResult() { Success = false, Message = "ERROR: Failed to finish moving home volume. " + err.Message };
                }
            }


            return new BooleanResult() { Success = true };
        }
        // * //



    } // End ThreadedProcess class 

    // End namespace 
}
