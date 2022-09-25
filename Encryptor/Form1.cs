using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace Encryptor
{
    public partial class Encryptor : Form
    {
        public Encryptor()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Add files
            OpenFileDialog filepath = new OpenFileDialog();
            filepath.Title = "Select File";
            filepath.InitialDirectory = @"C:\";
            filepath.Filter = "All files (*.*)|*.*";
            filepath.Multiselect = true;
            filepath.FilterIndex = 1;
            filepath.ShowDialog();
            foreach(String file in filepath.FileNames)
            {
                listBox1.Items.Add(file); //add file path to the listbox
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Add folders
            FolderBrowserDialog folderpath = new FolderBrowserDialog();
            folderpath.ShowDialog();
            listBox2.Items.Add(folderpath.SelectedPath);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                listBox1.Items.Remove(listBox1.SelectedItem);
            }
            catch { }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                listBox2.Items.Remove(listBox2.SelectedItem);
            }
            catch { }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //Clear values from listbox1, 2 and password text line
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            textBox1.Text = "";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var msgbox = MessageBox.Show("This feature will encrypt selected files. Please write down your password or else you cannot revert this action. Continue?", "FileProtect™",MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
            if (msgbox == DialogResult.Yes)
            {
                //This function will encrypt selected files
                //Password must have 8 characters!
                try
                {
                    if (textBox1.Text.Length < 8)
                    {
                        MessageBox.Show("Password must have 8 characters!", "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    //This is for selected files
                    if (listBox1.Items.Count > 0)
                    {
                        for (int num = 0; num < listBox1.Items.Count; num++)
                        {
                            string e_file = "" + listBox1.Items[num];
                            if (!e_file.Trim().EndsWith(".!LOCKED") && File.Exists(e_file))
                            {
                                EncryptFile("" + listBox1.Items[num], "" + listBox1.Items[num] + ".!LOCKED", textBox1.Text);
                                File.Delete("" + listBox1.Items[num]);
                            }
                        }
                    }
                    //This is for selected folders
                    if (listBox2.Items.Count > 0)
                    {
                        for (int num = 0; num < listBox2.Items.Count; num++)
                        {
                            string d_file = "" + listBox2.Items[num];
                            string[] get_files = Directory.GetFiles(d_file);
                            foreach (string name_file in get_files)
                            {
                                if (!name_file.Trim().EndsWith(".!LOCKED") && File.Exists(name_file))
                                {
                                    EncryptFile(name_file, name_file + ".!LOCKED", textBox1.Text);
                                    File.Delete(name_file);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    
                }
                MessageBox.Show("File encription finished", "FileProtect™", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else { }
        }

        char[] mychar = { '!', '.', 'L', 'O', 'C', 'K', 'E', 'D' };

        private void button6_Click(object sender, EventArgs e)
        {
            var msgbox = MessageBox.Show("This will return the files you encrypted before. Please double check your password, because entering a wrong password will result in damaged files. Continue?", "FileProtect™",MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
            if (msgbox == DialogResult.Yes)
            {
                //This function will decrypt selected files
                //Password must have 8 characters!
                //Password must be correct!
                try
                {
                    if (textBox1.Text.Length < 8)
                    {
                        MessageBox.Show("Password must have 8 characters!", "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    //This is for selected files
                    if (listBox1.Items.Count > 0)
                    {
                        for (int num = 0; num < listBox1.Items.Count; num++)
                        {
                            string e_file = "" + listBox1.Items[num];
                            if (e_file.Trim().EndsWith(".!LOCKED") && File.Exists(e_file))
                            {
                                DecryptFile(e_file, e_file.TrimEnd(mychar), textBox1.Text);
                                File.Delete(e_file);
                            }
                        }
                    }
                    //This is for selected folders
                    if (listBox2.Items.Count > 0)
                    {
                        for (int num = 0; num < listBox2.Items.Count; num++)
                        {
                            string d_file = "" + listBox2.Items[num];
                            string[] get_files = Directory.GetFiles(d_file);
                            foreach (string name_file in get_files)
                            {
                                if (name_file.Trim().EndsWith(".!LOCKED") && File.Exists(name_file))
                                {
                                    DecryptFile(name_file, name_file.TrimEnd(mychar), textBox1.Text);
                                    File.Delete(name_file);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    
                }
                MessageBox.Show("File decription finished", "FileProtect™",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        private void DecryptFile(string inputFile, string outputFile, string password)
        {
            try
            {
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateDecryptor(key, key),
                    CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(outputFile, FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch { }
        }

        private void EncryptFile(string inputFile, string outputFile, string password)
        {
            try
            {
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = outputFile;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);


                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch { }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The generate password feature works only on the encrypting phase. Do not use with decryption!!!", "FileProtect™",MessageBoxButtons.OK,MessageBoxIcon.Information);
            textBox1.Text = GenerateRandomAlphanumericString(8);
        }
        public static string GenerateRandomAlphanumericString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            var random = new Random();
            var randomString = new string(Enumerable.Repeat(chars, length)
                                                    .Select(s => s[random.Next(s.Length)]).ToArray());
            return randomString;
        }
    }
}
