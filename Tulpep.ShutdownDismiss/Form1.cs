using System;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;

namespace Tulpep.ShutdownDismiss
{
    public partial class Form1 : Form
    {

        private const string SubkeyName = @"Software\Tulpep\ShutdownDismiss";
        private const string KeyName = "NumberOfDismisses";
        public Form1()
        {
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            InitializeComponent();
            //30 minutes
            // Thise site helps in the conversion http://www.calculateme.com/Time/Minutes/ToMilliseconds.htm
            int delayMiliseconds = 1800000;

            var datetime = DateTime.Now.AddMilliseconds(delayMiliseconds);
            label2.Text = label2.Text + datetime.ToString("hh:mm tt");

            new Thread(() =>
            {

                Thread.CurrentThread.IsBackground = true;
                Thread.Sleep(delayMiliseconds);
                SetNumberOfDismisses(1);
                Process.Start("shutdown", "/s /f /t 00");
            }).Start();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            SetNumberOfDismisses(1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int numberOfDismisses  = GetNumberOfDismisses();
            if (numberOfDismisses > 3)
            {
                MessageBox.Show("Usted ha cancelado el apagado diario por mas de tres dias. No lo puede cancelar de nuevo");
            }
            else
            {
                SetNumberOfDismisses(numberOfDismisses++);
                this.Close();
            }

        }

        private int GetNumberOfDismisses()
        {
            int existingNumber = 1;
            using (var key = Registry.CurrentUser.CreateSubKey(SubkeyName))
            {
                var existing = (string) key.GetValue(KeyName);
                if (existing != null)
                {
                    existingNumber = Int32.Parse(existing) + 1;
                }
            };
            return existingNumber;
        }

        private void SetNumberOfDismisses(int number)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(SubkeyName))
            {
                key.SetValue(KeyName, number, RegistryValueKind.String);
            };
        }
    }
}
