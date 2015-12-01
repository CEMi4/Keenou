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
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Data.SQLite;

namespace Keenou
{

    /// <summary>
    /// Helper class to access Dropbox's JSON configuration file 
    /// </summary>
    public class DropboxJSON
    {
        public DropboxJSONItem business;
        public DropboxJSONItem personal;
    }
    public class DropboxJSONItem
    {
        public long host;
        public string path;
    }
    // * //


    class EncryptFS
    {

        // Get folder path for cloud service //
        public static string GetCloudServicePath(Config.Clouds type)
        {
            string folderPath = null;
            string configPath, configFilePath;

            switch (type)
            {
                case Config.Clouds.Dropbox:
                    configPath = @"Dropbox\info.json";

                    // Find config file -- first try %APPDATA%, then %LOCALAPPDATA% 
                    configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), configPath);
                    if (!File.Exists(configFilePath))
                    {
                        configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), configPath);
                        if (!File.Exists(configFilePath))
                        {
                            return null;
                        }
                    }

                    // Read and parse config file 
                    string personalPath = null;
                    string businessPath = null;
                    using (StreamReader r = new StreamReader(configFilePath))
                    {
                        var serializer = new JavaScriptSerializer();
                        DropboxJSON accounts = serializer.Deserialize<DropboxJSON>(r.ReadToEnd());

                        if (accounts.personal != null)
                        {
                            personalPath = accounts.personal.path;
                        }
                        if (accounts.business != null)
                        {
                            businessPath = accounts.business.path;
                        }

                        // Only support personal folder for now 
                        folderPath = personalPath;
                    }


                    break;


                case Config.Clouds.GoogleDrive:
                    configPath = @"Google\Drive\user_default\sync_config.db";

                    // Find config file -- first try Google\Drive\user_default\sync_config.db, then Google\Drive\sync_config.db
                    configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), configPath);
                    if (!File.Exists(configFilePath))
                    {
                        configPath = @"Google\Drive\sync_config.db";
                        configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), configPath);
                        if (!File.Exists(configFilePath))
                        {
                            return null;
                        }
                    }

                    using (SQLiteConnection con = new SQLiteConnection(@"Data Source=" + configFilePath + ";Version=3;New=False;Compress=True;"))
                    {
                        con.Open();
                        using (SQLiteCommand sqLitecmd = new SQLiteCommand(con))
                        {
                            sqLitecmd.CommandText = "select * from data where entry_key='local_sync_root_path'";

                            using (SQLiteDataReader reader = sqLitecmd.ExecuteReader())
                            {
                                reader.Read();

                                // data_value is in format "\\?\<path>" 
                                folderPath = reader["data_value"].ToString().Substring(4);
                            }
                        }
                    }

                    break;


                case Config.Clouds.OneDrive:

                    // Find config file -- first try SkyDrive (old name), then OneDrive
                    folderPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\SkyDrive", "UserFolder", null);
                    if (folderPath == null)
                    {
                        folderPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\OneDrive", "UserFolder", null);
                    }

                    break;
            }


            return folderPath;
        }
        // * //





        // Change the parameters for this EncFS //
        public static BooleanResult ChangeEncryptedFSParams(string guid, string volumeLoc)
        {

            // Update volume location (after migrating enc volume to cloud location, for instance) 
            if (!string.IsNullOrEmpty(volumeLoc))
            {
                Registry.SetValue(Config.CURR_USR_REG_DRIVE_ROOT + guid, "encContainerLoc", volumeLoc);
            }

            return new BooleanResult() { Success = true };
        }
        // * //





        // Create new encrypted filesystem //
        public static BooleanResult CreateEncryptedFS(string guid, string volumeLoc, string targetDrive, string masterKey, string label, bool keepMounted = false)
        {

            // Path cannot end with a slash due to CMD usage 
            volumeLoc = volumeLoc.TrimEnd(new[] { '/', '\\' });


            // GET EncFS DIRECTORY
            string programDir = Toolbox.GetSoftwareDirectory(Config.Software.EncFS);
            if (programDir == null)
            {
                return new BooleanResult() { Success = false, Message = "ERROR: EncFS inaccessible!" };
            }


            // GET Dokan DIRECTORY
            string dokanDir = Toolbox.GetSoftwareDirectory(Config.Software.Dokan);
            if (dokanDir == null)
            {
                return new BooleanResult() { Success = false, Message = "ERROR: Dokan inaccessible!" };
            }



            // Determine Keenou config file location for this volume 
            string configLoc = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Keenou\" + guid + @"\";
            if (Directory.Exists(configLoc))
            {
                // TODO: auto-recovery? 
                return new BooleanResult() { Success = false, Message = "ERROR: Possible GUID collision!" };
            }


            // Create config file directory for this FS
            Directory.CreateDirectory(configLoc);


            // Save setting values to registry  
            Registry.SetValue(Config.CURR_USR_REG_DRIVE_ROOT + guid, "encContainerLoc", volumeLoc);
            Registry.SetValue(Config.CURR_USR_REG_DRIVE_ROOT + guid, "configLoc", configLoc);


            using (Process process = new Process())
            {

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
                    startInfo.EnvironmentVariables["ENCFS6_CONFIG"] = configLoc + Config.ENCFS_CONFIG_FILENAME;
                    startInfo.Arguments = "/C \"\"" + programDir + "encfs.exe\" --stdinpass -o volname=\"" + label + "\" \"" + volumeLoc + "\" \"" + targetDrive + ":\"\"";
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
                        // Auto-unmount unless they asked us not to 
                        if (!keepMounted)
                        {
                            BooleanResult res = EncryptFS.UnmountEncryptedFS(targetDrive);
                            if (res == null || !res.Success)
                            {
                                return res;
                            }
                        }
                        else
                        {
                            // Save where we mounted the encrypted volume 
                            Registry.SetValue(Config.CURR_USR_REG_DRIVE_ROOT + guid, "encDrive", targetDrive);
                        }
                    }
                    else
                    {
                        // If it exited by itself, it was in error 
                        return new BooleanResult() { Success = false, Message = "ERROR: Error while creating encrypted FS! " + process.ExitCode };
                    }


                    // Process will block indefinitely (until unmount called), so just return 
                    //process.WaitForExit();
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
        public static BooleanResult MountEncryptedFS(string guid, string targetDrive, string masterKey, string label)
        {

            // Determine location of configuration file 
            string configLoc = (string)Registry.GetValue(Config.CURR_USR_REG_DRIVE_ROOT + guid, "configLoc", string.Empty);
            if (string.IsNullOrEmpty(configLoc))
            {
                return new BooleanResult() { Success = false, Message = "ERROR: Invalid GUID given to mount!" };
            }


            // Pull enc volume location from registry for this GUID 
            string volumeLoc = (string)Registry.GetValue(Config.CURR_USR_REG_DRIVE_ROOT + guid, "encContainerLoc", string.Empty);
            if (string.IsNullOrEmpty(volumeLoc))
            {
                return new BooleanResult() { Success = false, Message = "ERROR: Invalid GUID given to mount!" };
            }

            // Path cannot end with a slash due to CMD usage 
            volumeLoc = volumeLoc.TrimEnd(new[] { '/', '\\' });




            using (Process process = new Process())
            {

                // GET EncFS DIRECTORY
                string programDir = Toolbox.GetSoftwareDirectory(Config.Software.EncFS);
                if (programDir == null)
                {
                    return new BooleanResult() { Success = false, Message = "ERROR: EncFS inaccessible!" };
                }


                // GET Dokan DIRECTORY
                string dokanDir = Toolbox.GetSoftwareDirectory(Config.Software.Dokan);
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
                    startInfo.EnvironmentVariables["ENCFS6_CONFIG"] = configLoc + Config.ENCFS_CONFIG_FILENAME;
                    startInfo.Arguments = "/C \"\"" + programDir + "encfs.exe\" --require-macs --stdinpass -o volname=\"" + label + "\" \"" + volumeLoc + "\" \"" + targetDrive + ":\"\"";
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


                    // Save where we mounted the encrypted volume 
                    Registry.SetValue(Config.CURR_USR_REG_DRIVE_ROOT + guid, "encDrive", targetDrive);


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
                string programDir = Toolbox.GetSoftwareDirectory(Config.Software.Dokan);
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
                    startInfo.Arguments = "/C \"\"" + programDir + "dokanctl.exe\"  /u \"" + targetDrive + ":\"\"";
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





        // Copy files over from old folder to new, enc folder //
        public static BooleanResult MoveDataFromFolder(string sourceFolder, string targetDrive)
        {
            // First copy files over
            using (Process process = new Process())
            {

                ProcessStartInfo startInfo = new ProcessStartInfo();
                try
                {
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C \"FOR %i IN (\"" + sourceFolder + "\\*\") DO MOVE /Y \"%i\" \"" + targetDrive + "\\%~nxi\" \"";
                    process.StartInfo = startInfo;
                    process.Start(); // this may take a while! 
                    process.WaitForExit();

                    // Ensure no errors were thrown 
                    if (process.ExitCode > 0)
                    {
                        return new BooleanResult() { Success = false, Message = "ERROR: Error while moving root files! " + process.ExitCode };
                    }

                }
                catch (Exception err)
                {
                    return new BooleanResult() { Success = false, Message = "ERROR: Failed to finish moving root cloud volume. " + err.Message };
                }
            }

            // Then clear files out of old folder 
            using (Process process = new Process())
            {

                ProcessStartInfo startInfo = new ProcessStartInfo();
                try
                {
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C \"FOR /D %i IN (\"" + sourceFolder + "\\*\") DO ROBOCOPY /MOVE /MIR /sl /xj /r:0 \"%i\" \"" + targetDrive + "\\%~nxi\" \"";
                    process.StartInfo = startInfo;
                    process.Start(); // this may take a while! 
                    process.WaitForExit();

                    // Ensure no errors were thrown 
                    if (process.ExitCode > 7)
                    {
                        return new BooleanResult() { Success = false, Message = "ERROR: Error while moving subfiles! " + process.ExitCode };
                    }

                }
                catch (Exception err)
                {
                    return new BooleanResult() { Success = false, Message = "ERROR: Failed to finish moving cloud subvolume. " + err.Message };
                }
            }

            return new BooleanResult() { Success = true };
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
                    startInfo.Arguments = "/C \"robocopy \"" + sourceFolder + "\" " + targetDrive + " /MIR /sl /xj /r:0\"";
                    process.StartInfo = startInfo;
                    process.Start(); // this may take a while! 
                    process.WaitForExit();

                    // Ensure no errors were thrown 
                    if (process.ExitCode > 7)
                    {
                        return new BooleanResult() { Success = false, Message = "ERROR: Error while copying files over! " + process.ExitCode };
                    }

                }
                catch (Exception err)
                {
                    return new BooleanResult() { Success = false, Message = "ERROR: Failed to finish copying cloud volume. " + err.Message };
                }
            }


            return new BooleanResult() { Success = true };
        }
        // * //


    } // End EncryptFS class 

    // End namespace 
}
