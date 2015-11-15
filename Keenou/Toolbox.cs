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
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Security.Cryptography;

namespace Keenou
{
    static class Toolbox
    {

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
        public static long GetAvailableFreeSpace(string path)
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
        public static long GetDirectorySize(string directory)
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
                        return 0;
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
                    MessageBox.Show("ERROR: Failed to determine directory size! " + err.Message);
                    return 0;
                }

            }

            return size;
        }
        // * //


    } // End Toolbox class 

    // End namespace 
}
