/* Created by Mell Rosandich
 * mell@ourace.com
 * 
 * 
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Files2bz2
{
    public partial class Form1 : Form
    {
        string g_filePathFrom = "";     // This is the path that will be read from
        string g_filePathTo = "";       // This is the path the files will be copied to.
        bool g_isprocessing = false;    // The state of processing files
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                g_filePathFrom = folderBrowserDialog1.SelectedPath;
                listBox1.Items.Clear();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (!Directory.Exists(g_filePathFrom))
            {
                MessageBox.Show("You need to set the directory in step 1");
                return;
            }

            List<string> Files;
            if (checkBox1.Checked)
            {
                Files = new List<string>(Directory.EnumerateFiles(g_filePathFrom, "*.*", SearchOption.AllDirectories));
            }
            else
            {
                Files = new List<string>(Directory.EnumerateFiles(g_filePathFrom, "*.*", SearchOption.TopDirectoryOnly));
            }
            
            foreach (var File in Files)
            {
                Application.DoEvents();                     //let the application breath. Allow stopping.
                var temp_Parts = File.Split('.');           //split the file string by a .
                string fileExtension = temp_Parts.Last();   //get the very last element of the array. it will be the file type
                if (!listBox1.Items.Contains(fileExtension))
                {
                    listBox1.Items.Add(fileExtension);
                }
                
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
                g_filePathTo = folderBrowserDialog1.SelectedPath;
                
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if( !File.Exists(textBox3.Text ) )
            {
                MessageBox.Show("Please enter the full path to the 7z.exe.");
                return;
            }
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Please make sure you have set both folders in step 1 and 4.");
                return;
            }
            if (!Directory.Exists(textBox1.Text))
            {
                MessageBox.Show("The directory path in step 1 is wrong.");
                return;
            }
            if (!Directory.Exists(textBox2.Text))
            {
                MessageBox.Show("The directory path in step 4 is wrong.");
                return;
            }

            int SelectedItems = 0;
            for (int x = 0; x < listBox1.Items.Count; x++)
            {
                if (listBox1.GetSelected(x) == true && listBox1.Items[x].ToString() !="")
                {
                    SelectedItems++;
                }

            }

            if (SelectedItems == 0)
            {
                MessageBox.Show("You need to select the file type that you want to bz2 in step 3.");
                return;
            }


            if (g_isprocessing == false)
            {
                g_isprocessing = true;
                DisableEnableAll(false);
                button4.Text = "Stop: BZ Zip and Copy the Files";
                BZCopyFiles();
                
            }
            else
            {
                button4.Text = "Start: BZ Zip and Copy the Files";
                DisableEnableAll(true);
                g_isprocessing = false;
            }
            
        }

        private void DisableEnableAll(bool isEnabled)
        {

            button1.Enabled = isEnabled;
            button2.Enabled = isEnabled;
            checkBox1.Enabled = isEnabled;
            checkBox2.Enabled = isEnabled;
            textBox1.Enabled = isEnabled;
            textBox2.Enabled = isEnabled;
            listBox1.Enabled = isEnabled;
            listBox2.Enabled = isEnabled;

        }

        public void CountFilesToMove()
        {
            List<string> Files;

            if (checkBox1.Checked)
            {
                Files = new List<string>(Directory.EnumerateFiles(g_filePathFrom, "*.*", SearchOption.AllDirectories));
            }
            else
            {
                Files = new List<string>(Directory.EnumerateFiles(g_filePathFrom, "*.*", SearchOption.TopDirectoryOnly));
            }

            int FilesToMoveCnt = 0;
            foreach (var TheFile in Files)
            {
                var temp_Parts = TheFile.Split('.');           //split the file string by a .
                string fileExtension = temp_Parts.Last();   //get the very last element of the array. it will be the file type
                if (isSelectedInList(fileExtension))
                {
                    FilesToMoveCnt++;
                }
                label9.Text = "Total Files: 0 of " + FilesToMoveCnt;
            }

        }


        public void BZCopyFiles()
        {
            List<string> Files;
            int FileCount = 0;
            

            if (checkBox1.Checked)
            {
                Files = new List<string>(Directory.EnumerateFiles(g_filePathFrom, "*.*", SearchOption.AllDirectories));
            }
            else
            {
                Files = new List<string>(Directory.EnumerateFiles(g_filePathFrom, "*.*", SearchOption.TopDirectoryOnly));
            }


            //count all the files usin
            int FilesToMoveCnt = 0;
            foreach (var TheFile in Files)
            {
                var temp_Parts = TheFile.Split('.');           //split the file string by a .
                string fileExtension = temp_Parts.Last();   //get the very last element of the array. it will be the file type
                if (isSelectedInList(fileExtension))
                {
                    FilesToMoveCnt++;
                }
                label9.Text = "Total Files: 0 of " + FilesToMoveCnt;
            }

            foreach (var TheFile in Files)
            {
                Application.DoEvents();                     //let the application breath. Allow stopping.
                var temp_Parts = TheFile.Split('.');           //split the file string by a .
                string fileExtension = temp_Parts.Last();   //get the very last element of the array. it will be the file type
                
                //exit if the button is pressed
                if (g_isprocessing == false)
                {
                    return;
                }

                if (isSelectedInList(fileExtension) )
                {
                    listBox2.Items.Add("FROM: " + TheFile);
                    FileCount++;
                    label9.Text = "Total Files: " + FileCount + " of " + FilesToMoveCnt;

                    //This will chop off the 
                    string temp_Path = TheFile.Replace(g_filePathFrom, "");
                    string new_Path = g_filePathTo + temp_Path;
                    

                    //Build the directory path
                    //We will assume the to path is already built.
                    //We need to break up the temp path into parts and check each part except the file name
                    var Tempparts = temp_Path.Split('\\');
                    string checkpath = "";
                    int PartCnt = Tempparts.Count() - 1; //remove the last item from the list becuase its the file name
                    for (int ix = 0; ix < PartCnt; ix++)
                    {
                        checkpath = checkpath + Tempparts[ix];
                        if (!System.IO.Directory.Exists(g_filePathTo + checkpath ))
                        {
                            System.IO.Directory.CreateDirectory(g_filePathTo + checkpath);
                            //listBox2.Items.Add("CKPath: " + g_filePathTo + checkpath);
                        }
                        checkpath = checkpath + "\\";
                    }


                    if (fileExtension.ToLower() == "bz2") // we are just going to copy the file
                    {
                        if (File.Exists(new_Path) && checkBox2.Checked)
                        {
                            File.Copy(TheFile, new_Path, true);
                            listBox2.Items.Add("TO Overwrite: " + new_Path);
                        }
                        else
                        {
                            if (!File.Exists(new_Path))
                            {
                                File.Copy(TheFile, new_Path, false);
                                listBox2.Items.Add("TO New: " + new_Path);
                            }
                        }
                    }
                    else
                    {
                        if (File.Exists(new_Path) && checkBox2.Checked)
                        {
                            File.Delete(new_Path);
                            //CopyAndArchive(new_Path, TheFile);
                            CopyAndArchive(new_Path + ".bz2", TheFile);
                            listBox2.Items.Add("TO Overwrite: " + new_Path + ".bz2");
                        }
                        else
                        {
                            if (!File.Exists(new_Path))
                            {
                                //CopyAndArchive(new_Path, TheFile);
                                CopyAndArchive(new_Path + ".bz2", TheFile);
                                listBox2.Items.Add("TO New: " + new_Path + ".bz2");
                            }
                        }
                    }

                }//end if file extension
            }//end for each files
        }//End BZCopyFiles


        private void CopyAndArchive(string DestinPath, string FileToBzip)
        {
            //C:\Program Files\7-Zip\7z.exe" u -tbzip2 %~nx1.bz2 %1
            
            ProcessStartInfo bzip2 = new ProcessStartInfo(textBox3.Text);
            bzip2.Arguments = string.Format("u -tbzip2 \"{0}\" \"{1}\"", DestinPath, FileToBzip);
            bzip2.RedirectStandardInput = true;
            bzip2.UseShellExecute = false;
            bzip2.CreateNoWindow = true;
            bzip2.WindowStyle = ProcessWindowStyle.Hidden;
            Process bzip2process = Process.Start(bzip2);
            bzip2process.WaitForExit();

        }





       


        public bool isSelectedInList(string InVal)
        {
            bool temp_RetVal = false;
            for (int x = 0; x < listBox1.Items.Count; x++)
            {
                if (listBox1.GetSelected(x) == true && listBox1.Items[x].ToString() == InVal)
                {
                    return true;
                }
                   
            }
            return temp_RetVal;
        }//end isSelectedInList

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CountFilesToMove();
        }

        private void listBox1_Leave(object sender, EventArgs e)
        {
            CountFilesToMove();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.ourace.com/135-files2bz2");
        }


    }//end class
}//end name space
