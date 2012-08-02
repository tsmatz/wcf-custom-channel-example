using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;

namespace SampleClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChannelFactory<SampleServer.ICalcService> factory = new ChannelFactory<SampleServer.ICalcService>("CustomBinding_ICalcService");
            factory.Open();
            SampleServer.ICalcService channel = factory.CreateChannel();
            string result = channel.SetValue(int.Parse(textBox1.Text));
            factory.Close();

            MessageBox.Show(result);
        }
    }
}
