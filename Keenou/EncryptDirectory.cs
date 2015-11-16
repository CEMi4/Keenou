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
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Keenou
{
    public class EncryptDirectory
    {

        // Create new encrypted volume //
        public static BooleanResult CreateEncryptedVolume(string hash, string volumeLoc, string targetDrive, string masterKey, string cipherChosen, long volumeSize)
        {

            using (Process process = new Process())
            {

                // GET VeraCrypt DIRECTORY
                string programDir = Toolbox.GetSoftwareDirectory("VeraCrypt");
                if (programDir == null)
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
                    startInfo.Arguments = "/C \"\"" + programDir + "VeraCrypt Format.exe\" /create \"" + volumeLoc + "\" /size " + volumeSize + "M /hash " + hash + " /encryption " + cipherChosen + " /password \"" + masterKey + "\" /filesystem NTFS /force /dynamic /silent\"";
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
        public static BooleanResult MountEncryptedVolume(string hash, string volumeLoc, string targetDrive, string masterKey)
        {

            using (Process process = new Process())
            {

                // GET VeraCrypt DIRECTORY
                string programDir = Toolbox.GetSoftwareDirectory("VeraCrypt");
                if (programDir == null)
                {
                    return new BooleanResult() { Success = false, Message = "ERROR: VeraCrypt inaccessible!" };
                }


                // MOUNT ENCRYPTED CONTAINER (TODO: sanitize password?)
                ProcessStartInfo startInfo = new ProcessStartInfo();
                try
                {
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C \"\"" + programDir + "VeraCrypt.exe\" /hash " + hash + " /v \"" + volumeLoc + "\" /l " + targetDrive + " /f /h n /p \"" + masterKey + "\" /q /s\"";
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



        // Robocopy everything over from source directory to target encrypted container //
        public static BooleanResult CopyDataFromHomeFolder(string homeFolder, string targetDrive)
        {

            // Make sure old location exists (before moving files over to new location) 
            if (!Directory.Exists(homeFolder) || !File.Exists(homeFolder + @"\" + "NTUSER.DAT"))
            {
                return new BooleanResult() { Success = false, Message = "ERROR: Current home directory inaccessible!" };
            }
            // * //


            return CopyDataFromFolder(homeFolder, targetDrive);
        }
        public static BooleanResult CopyDataFromFolder(string sourceFolder, string targetDrive)
        {

            // Make sure old location exists (before moving files over to new location) 
            if (!Directory.Exists(sourceFolder))
            {
                return new BooleanResult() { Success = false, Message = "ERROR: Source directory inaccessible!" };
            }
            // * //


            // TODO: ensure robocopy exists (Win 7+)


            using (Process process = new Process())
            {

                ProcessStartInfo startInfo = new ProcessStartInfo();
                try
                {
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C \"robocopy \"" + sourceFolder + "\" " + targetDrive + ":\\ /MIR /copyall /sl /xj /r:0\"";
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



    } // End EncryptDirectory class 

    // End namespace 
}
