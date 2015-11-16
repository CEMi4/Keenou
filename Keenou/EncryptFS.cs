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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace Keenou
{
    class EncryptFS
    {
        // ENCFS6_CONFIG environment variable to change config file location 



        // Create new encrypted filesystem //
        public static BooleanResult CreateEncryptedFS(string volumeLoc, string targetDrive, string masterKey)
        {

            // Path cannot end with a slash due to CMD usage 
            volumeLoc = volumeLoc.TrimEnd(new[] { '/', '\\' });


            using (Process process = new Process())
            {

                // GET EncFS DIRECTORY
                string programDir = Toolbox.GetSoftwareDirectory("EncFS");
                if (programDir == null)
                {
                    return new BooleanResult() { Success = false, Message = "ERROR: EncFS inaccessible!" };
                }


                // GET Dokan DIRECTORY
                string dokanDir = Toolbox.GetSoftwareDirectory("Dokan");
                if (dokanDir == null)
                {
                    return new BooleanResult() { Success = false, Message = "ERROR: Dokan inaccessible!" };
                }



                // MOUNT ENCRYPTED CONTAINER 
                ProcessStartInfo startInfo = new ProcessStartInfo();
                try
                {
                    //.\encfs.exe "C:\Users\jetwhiz\Desktop\encfs4win\testing" "Z:" --stdinpass -o volname="Secure Dropbox"

                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.CreateNoWindow = true;
                    startInfo.RedirectStandardInput = true;
                    startInfo.UseShellExecute = false;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C \"\"" + programDir + "encfs.exe\" --stdinpass -o volname=\"Secure Dropbox\" \"" + volumeLoc + "\" \"" + targetDrive + "\"\"";
                    process.StartInfo = startInfo;
                    process.Start();

                    // Send stdin inputs to program 
                    StreamWriter myStreamWriter = process.StandardInput;
                    myStreamWriter.WriteLine("p"); // paranoia mode
                    myStreamWriter.WriteLine(masterKey); // user's password
                    myStreamWriter.Close();



                    // Give it a few seconds to finish 
                    // TODO: Figure out better way to determine when EncFS has finished 
                    process.WaitForExit(10000);



                    // Unmount drive (forcing the closing of encfs process) if not already exited 
                    if (!process.HasExited)
                    {
                        BooleanResult res = EncryptFS.UnmountEncryptedFS(targetDrive);
                        if (res == null || !res.Success)
                        {
                            return res;
                        }
                    }
                    else
                    {
                        // If it exited by itself, it was in error 
                        return new BooleanResult() { Success = false, Message = "ERROR: Error while creating encrypted FS! " + process.ExitCode };
                    }


                    // Wait for EncFS to die 
                    process.WaitForExit();
                }
                catch (Exception err)
                {
                    return new BooleanResult() { Success = false, Message = "ERROR: Failed to create encrypted FS! " + err.Message };
                }

            }

            return new BooleanResult() { Success = true };
        }
        // * //





        // Mount encrypted filesystem //
        public static BooleanResult MountEncryptedFS(string volumeLoc, string targetDrive, string masterKey)
        {

            // Path cannot end with a slash due to CMD usage 
            volumeLoc = volumeLoc.TrimEnd(new[] { '/', '\\' });


            using (Process process = new Process())
            {

                // GET EncFS DIRECTORY
                string programDir = Toolbox.GetSoftwareDirectory("EncFS");
                if (programDir == null)
                {
                    return new BooleanResult() { Success = false, Message = "ERROR: EncFS inaccessible!" };
                }


                // GET Dokan DIRECTORY
                string dokanDir = Toolbox.GetSoftwareDirectory("Dokan");
                if (dokanDir == null)
                {
                    return new BooleanResult() { Success = false, Message = "ERROR: Dokan inaccessible!" };
                }



                // MOUNT ENCRYPTED CONTAINER 
                ProcessStartInfo startInfo = new ProcessStartInfo();
                try
                {
                    //.\encfs.exe "C:\Users\jetwhiz\Desktop\encfs4win\testing" "Z:" --stdinpass -o volname="Secure Dropbox"

                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.CreateNoWindow = true;
                    startInfo.RedirectStandardInput = true;
                    startInfo.UseShellExecute = false;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C \"\"" + programDir + "encfs.exe\" --stdinpass -o volname=\"Secure Dropbox\" \"" + volumeLoc + "\" \"" + targetDrive + "\"\"";
                    process.StartInfo = startInfo;
                    process.Start();

                    // Send stdin inputs to program 
                    StreamWriter myStreamWriter = process.StandardInput;
                    myStreamWriter.WriteLine(masterKey); // user's password
                    myStreamWriter.Close();



                    // Give it a few seconds to finish 
                    // TODO: Figure out better way to determine when EncFS has finished 
                    process.WaitForExit(10000);



                    // If it exited by itself, it was in error 
                    if (process.HasExited)
                    {
                        return new BooleanResult() { Success = false, Message = "ERROR: Error while mounting encrypted FS! " + process.ExitCode };
                    }


                    // Process will block indefinitely (until unmount called), so just return 
                    //process.WaitForExit();
                }
                catch (Exception err)
                {
                    return new BooleanResult() { Success = false, Message = "ERROR: Failed to mount encrypted FS! " + err.Message };
                }

            }

            return new BooleanResult() { Success = true };
        }
        // * //





        // Unmount encrypted filesystem //
        public static BooleanResult UnmountEncryptedFS(string targetDrive)
        {

            using (Process process = new Process())
            {

                // GET Dokan DIRECTORY
                string programDir = Toolbox.GetSoftwareDirectory("Dokan");
                if (programDir == null)
                {
                    return new BooleanResult() { Success = false, Message = "ERROR: Dokan inaccessible!" };
                }



                // UNMOUNT ENCRYPTED FS 
                ProcessStartInfo startInfo = new ProcessStartInfo();
                try
                {
                    //.\dokanctl.exe /u Z:

                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.CreateNoWindow = true;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C \"\"" + programDir + "dokanctl.exe\"  /u \"" + targetDrive + "\"\"";
                    process.StartInfo = startInfo;
                    process.Start();

                    process.WaitForExit();
                }
                catch (Exception err)
                {
                    return new BooleanResult() { Success = false, Message = "ERROR: Failed to unmount encrypted FS! " + err.Message };
                }

            }

            return new BooleanResult() { Success = true };
        }
        // * //



    } // End EncryptFS class 

    // End namespace 
}
