using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SerialCommunication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string[] portNames = SerialPort.GetPortNames().Distinct().ToArray();
                comboBoxPoort.Items.Clear();
                comboBoxPoort.Items.AddRange(portNames);
                if (comboBoxPoort.Items.Count > 0) comboBoxPoort.SelectedIndex = 0;

                comboBoxBaudrate.SelectedIndex = comboBoxBaudrate.Items.IndexOf("115200");
            }
            catch (Exception)
            { }
        }

        private void cboPoort_DropDown(object sender, EventArgs e)
        {
            try
            {
                string selected = (string)comboBoxPoort.SelectedItem;
                string[] portNames = SerialPort.GetPortNames().Distinct().ToArray();

                comboBoxPoort.Items.Clear();
                comboBoxPoort.Items.AddRange(portNames);

                comboBoxPoort.SelectedIndex = comboBoxPoort.Items.IndexOf(selected);
            }
            catch (Exception)
            {
                if (comboBoxPoort.Items.Count > 0) comboBoxPoort.SelectedIndex = 0;
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    DisconnectSerialPort();
                }
                else
                {
                    ConnectSerialPort();
                }
            }
            catch (Exception exception)
            {
                labelStatus.Text = "Error: " + exception.Message;
            }
        }

        private void ConnectSerialPort()
        {
            // Set SerialPort properties from UI controls
            serialPortArduino.PortName = (string)comboBoxPoort.SelectedItem;
            serialPortArduino.BaudRate = int.Parse((string)comboBoxBaudrate.SelectedItem);
            serialPortArduino.DataBits = (int)numericUpDownDatabits.Value;
            serialPortArduino.ReadTimeout = 1000;
            serialPortArduino.WriteTimeout = 1000;

            // Set Parity
            if (radioButtonParityNone.Checked) serialPortArduino.Parity = Parity.None;
            else if (radioButtonParityOdd.Checked) serialPortArduino.Parity = Parity.Odd;
            else if (radioButtonParityEven.Checked) serialPortArduino.Parity = Parity.Even;
            else if (radioButtonParityMark.Checked) serialPortArduino.Parity = Parity.Mark;
            else if (radioButtonParitySpace.Checked) serialPortArduino.Parity = Parity.Space;

            // Set StopBits
            if (radioButtonStopbitsNone.Checked) serialPortArduino.StopBits = StopBits.None;
            else if (radioButtonStopbitsOne.Checked) serialPortArduino.StopBits = StopBits.One;
            else if (radioButtonStopbitsOnePointFive.Checked) serialPortArduino.StopBits = StopBits.OnePointFive;
            else if (radioButtonStopbitsTwo.Checked) serialPortArduino.StopBits = StopBits.Two;

            // Set Handshake
            if (radioButtonHandshakeNone.Checked) serialPortArduino.Handshake = Handshake.None;
            else if (radioButtonHandshakeRTS.Checked) serialPortArduino.Handshake = Handshake.RequestToSend;
            else if (radioButtonHandshakeRTSXonXoff.Checked) serialPortArduino.Handshake = Handshake.RequestToSendXOnXOff;
            else if (radioButtonHandshakeXonXoff.Checked) serialPortArduino.Handshake = Handshake.XOnXOff;

            // Set RTS and DTR
            serialPortArduino.RtsEnable = checkBoxRtsEnable.Checked;
            serialPortArduino.DtrEnable = checkBoxDtrEnable.Checked;

            // Register DataReceived event handler
            serialPortArduino.DataReceived += SerialPortArduino_DataReceived;

            // Open the connection
            serialPortArduino.Open();

            // Update UI
            radioButtonVerbonden.Checked = true;
            buttonConnect.Text = "Disconnect";
            labelStatus.Text = "Connected to " + serialPortArduino.PortName;
        }

        private void DisconnectSerialPort()
        {
            // Unregister DataReceived event handler
            serialPortArduino.DataReceived -= SerialPortArduino_DataReceived;

            // Close the connection
            serialPortArduino.Close();

            // Update UI
            radioButtonVerbonden.Checked = false;
            buttonConnect.Text = "Connect";
            labelStatus.Text = "Disconnected";
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    string command = textBoxCommand.Text.Trim();
                    if (!string.IsNullOrEmpty(command))
                    {
                        serialPortArduino.WriteLine(command);
                        labelStatus.Text = "Sent: " + command;
                    }
                    else
                    {
                        labelStatus.Text = "Error: Command cannot be empty";
                    }
                }
                else
                {
                    labelStatus.Text = "Error: Not connected to serial port";
                }
            }
            catch (Exception exception)
            {
                labelStatus.Text = "Error: " + exception.Message;
            }
        }

        private void SerialPortArduino_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPortArduino.ReadLine();
                this.Invoke(new MethodInvoker(delegate {
                    listBoxResponses.Items.Add("Response: " + data);
                    listBoxResponses.TopIndex = listBoxResponses.Items.Count - 1;
                }));
            }
            catch { }
        }
    }
}
