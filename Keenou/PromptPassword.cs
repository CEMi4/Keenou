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

namespace Keenou
{
    public partial class PromptPassword : Form
    {

        // Constructor //
        public PromptPassword()
        {
            InitializeComponent();
        }
        // * //



        // Public accessor for password field //
        public string CloudPassword
        {
            get { return t_cloudPW.Text; }
            set { t_cloudPW.Text = value; }
        }
        // * //



        // Submit button click handlers //
        private void b_encryptCloud_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
        // * //


    }  // End PromptPassword class 

    // End namespace 
}
