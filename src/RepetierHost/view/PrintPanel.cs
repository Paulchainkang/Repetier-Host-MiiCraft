﻿/*
   Copyright 2011 repetier repetierdev@googlemail.com

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RepetierHost.model;
using RepetierHost.view.utils;

namespace RepetierHost.view
{
    public enum PrinterStatus {disconnected,idle,heatingExtruder,heatingBed,motorStopped,jobPaused,jobKilled,jobFinsihed }
    public partial class PrintPanel : UserControl
    {
        PrinterConnection con;
        GCodeAnalyzer ann;
        LinkedList<string> commands = new LinkedList<string>();
        int commandPos = 0;
        bool createCommands = true;
        float lastx = -1000, lasty = -1000, lastz = -1000;
        private PrinterStatus status = PrinterStatus.disconnected;
        private long statusSet=0;
        public PrintPanel()
        {
            InitializeComponent();
            con = Main.conn;
            ann = con.analyzer;
            con.eventConnectionChange += ConnectionChanged;
            ann.fanVoltage = trackFanVoltage.Value;
            con.eventTempChange += tempUpdate;
          //  ann.eventPosChanged += coordUpdate;
            ann.eventChange += analyzerChange;
            UpdateConStatus(false);
            textExtrudeSpeed.Text = RegMemory.GetString("panelExtrudeSpeed", textExtrudeSpeed.Text);
            textExtrudeAmount.Text = RegMemory.GetString("panelExtrudeAmount", textExtrudeAmount.Text);
            textRetractAmount.Text = RegMemory.GetString("panelRetractAmount", textRetractAmount.Text);

            float volt = 100f * trackFanVoltage.Value / 255;
            labelVoltage.Text = "Output " + volt.ToString("0.0") + "%";
            arrowButtonXMinus.PossibleValues = Custom.GetString("xyMoveDistances", arrowButtonXMinus.PossibleValues);
            arrowButtonXPlus.PossibleValues = Custom.GetString("xyMoveDistances", arrowButtonXPlus.PossibleValues);
            arrowButtonYMinus.PossibleValues = Custom.GetString("xyMoveDistances", arrowButtonYMinus.PossibleValues);
            arrowButtonYPlus.PossibleValues = Custom.GetString("xyMoveDistances", arrowButtonYPlus.PossibleValues);
            if (Custom.GetBool("noPowerControlButton", false))
                switchPower.Visible = false;
            timer.Start();
        }
        public void updateStatus()
        {
            TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            long timestamp = (long)t.TotalSeconds;
            long diff = timestamp - statusSet;
            if (Main.conn.connected == false)
            {
                if (status != PrinterStatus.disconnected)
                    Status = PrinterStatus.disconnected;
            }
            else if (ann.extruderTemp > 15 && ann.extruderTemp - con.extruderTemp > 5)
                Status = PrinterStatus.heatingExtruder;
            else if (ann.bedTemp > 15 && ann.bedTemp - con.bedTemp > 5 && con.bedTemp > 15) // only if has bed
                Status = PrinterStatus.heatingBed;
            else if (status == PrinterStatus.heatingBed || status == PrinterStatus.heatingExtruder)
                Status = PrinterStatus.idle;
            else if (Main.conn.paused && status != PrinterStatus.jobPaused)
                Status = PrinterStatus.jobPaused;
            else if (status == PrinterStatus.jobPaused && !Main.conn.paused)
                Status = PrinterStatus.idle;
            else if (status == PrinterStatus.idle && diff > 0)
                Status = PrinterStatus.idle;
            else if (status == PrinterStatus.motorStopped || status == PrinterStatus.jobKilled || status == PrinterStatus.jobFinsihed)
            {
                if (diff > 30) // remove message after 30 seconds
                    Status = PrinterStatus.idle;
            }
            else if (status == PrinterStatus.disconnected && Main.conn.connected)
                Status = PrinterStatus.idle;
        }
        public MethodInvoker SetStatusJobFinished = delegate {Main.main.printPanel.Status = PrinterStatus.jobFinsihed;};
        public PrinterStatus Status {
            set
            {
                TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                long timestamp = (long)t.TotalSeconds;
                statusSet = timestamp;
                status = value;
                switch (value)
                {
                    case PrinterStatus.disconnected:
                        labelStatus.Text = "Disconnected";
                        break;
                    case PrinterStatus.heatingBed:
                        labelStatus.Text = "Heating bed";
                        break;
                    case PrinterStatus.heatingExtruder:
                        labelStatus.Text = "Heating extruder";
                        break;
                    case PrinterStatus.jobKilled:
                        labelStatus.Text = "Print job killed";
                        break;
                    case PrinterStatus.jobPaused:
                        labelStatus.Text = "Print job paused";
                        break;
                    case PrinterStatus.jobFinsihed:
                        labelStatus.Text = "Print job finished";
                        break;
                    default:
                    case PrinterStatus.idle:
                        if (Main.conn.job.mode==1)
                        {
                            if (Main.conn.analyzer.uploading)
                                labelStatus.Text = "Uploading ...";
                            else
                                labelStatus.Text = "Printing job ETA " + Main.conn.job.ETA;
                        }
                        else
                        {
                            labelStatus.Text = "Idle";
                        }
                        break;
                }
            }
        }
        public void ConnectionChanged(string msg) {
            UpdateConStatus(Main.conn.serial != null || Main.conn.isVirtualActive);
        }
        private void tempUpdate(int extruder, int printbed)
        {
            labelExtruderTemp.Text = extruder.ToString() + "°C /";
            labelPrintbedTemp.Text = printbed.ToString() + "°C /";
            string tr = "Extruder: " + con.extruderTemp.ToString();
            if (switchExtruderHeatOn.On) tr += "/" + ann.extruderTemp.ToString() + "°C";
            else tr += "°C/Off";
            if (con.bedTemp > 0)
            {
                tr += " Bed: " + con.bedTemp.ToString();
                if (ann.bedTemp > 0) tr += "/" + ann.bedTemp.ToString() + "°C ";
                else tr += "°C/Off ";
            }
            Main.main.toolTempReading.Text = tr;
        }
        public void analyzerChange() {
            createCommands = false;
            if (ann.extruderTemp > 0)
                numericUpDownExtruder.Value = ann.extruderTemp;
            //    textExtruderSetTemp.Text = ann.extruderTemp.ToString();
            if (ann.bedTemp > 0)
                numericPrintBed.Value = ann.bedTemp;
            //    textPrintbedTemp.Text = ann.bedTemp.ToString();
            switchExtruderHeatOn.On = ann.extruderTemp > 0;
            switchFanOn.On = ann.fanOn;
            trackFanVoltage.Value = ann.fanVoltage;
            switchBedHeat.On = ann.bedTemp > 0;
            switchPower.On = ann.powerOn;
            sliderSpeed.Value = con.speedMultiply;
            numericUpDownSpeed.Value = con.speedMultiply;
            //labelSpeed.Text = sliderSpeed.Value.ToString() + "%";
            tempUpdate(con.extruderTemp, con.bedTemp);
            createCommands = true;
        }
        private void coordUpdate(GCode code,float x,float y,float z) {
            if (x != -lastx || x==0)
            {
                labelX.Text = "X=" + x.ToString("0.00");
                if (ann.hasXHome)
                    labelX.ForeColor = SystemColors.ControlText;
                else
                    labelX.ForeColor = Color.Red;
                lastx = x;
            }
            if (y != lasty || y==0)
            {
                labelY.Text = "Y=" + y.ToString("0.00");
                if (ann.hasYHome)
                    labelY.ForeColor = SystemColors.ControlText;
                else
                    labelY.ForeColor = Color.Red;
                lasty = y;
            }
            if (z != lastz || z==0)
            {
                labelZ.Text = "Z=" + z.ToString("0.00");
                if (ann.hasZHome)
                    labelZ.ForeColor = SystemColors.ControlText;
                else
                    labelZ.ForeColor = Color.Red;
                lastz = z;
            }
        }
        public void UpdateConStatus(bool c)
        {
            Main.main.toolRunJob.Enabled = c;
            Main.main.toolStripSDCard.Enabled = c;
            Main.main.menuSDCardManager.Enabled = c;
           // switchConnect.On = c;
            textGCode.Enabled = c;
            switchBedHeat.Enabled = c;
            switchFanOn.Enabled = c;
            switchExtruderHeatOn.Enabled = c;
            trackFanVoltage.Enabled = c;
            numericUpDownExtruder.Enabled = c;
            buttonExtrude.Enabled = c;
            numericPrintBed.Enabled = c;
            buttonSend.Enabled = c;
            buttonHomeAll.Enabled = c;
            buttonHomeX.Enabled = c;
            buttonHomeY.Enabled = c;
            buttonHomeZ.Enabled = c;
            buttonStopMotor.Enabled = c;
            switchPower.Enabled = c;
            textRetractAmount.Enabled = c;
            textExtrudeSpeed.Enabled = c;
            textExtrudeAmount.Enabled = c;
            buttonRetract.Enabled = c;
            switchEcho.Enabled = c;
            switchInfo.Enabled = c;
            switchDryRun.Enabled = c;
            switchErrors.Enabled = c;
            buttonGoDisposeArea.Enabled = c;
            buttonSimulateOK.Enabled = c;
            //buttonJobStatus.Enabled = c;
            arrowButtonXMinus.Enabled = c;
            arrowButtonXPlus.Enabled = c;
            arrowButtonYMinus.Enabled = c;
            arrowButtonYPlus.Enabled = c;
            arrowButtonZMinus.Enabled = c;
            arrowButtonZPlus.Enabled = c;
            sliderSpeed.Enabled = c && (con.isMarlin || con.isRepetier);
            numericUpDownSpeed.Enabled = c && (con.isMarlin || con.isRepetier);
            if (c) sendDebug();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (textGCode.Text.Length < 2) return;
            con.injectManualCommand(textGCode.Text);
            commands.AddLast(textGCode.Text);
            if (commands.Count > 100)
                commands.RemoveFirst();
            commandPos = commands.Count;
            textGCode.Text = "";
        }

        private void sendDebug()
        {
            if (con.serial == null && !con.isVirtualActive) return;
            int v = 0;
            if (switchEcho.On) v += 1;
            if (switchInfo.On) v += 2;
            if (switchErrors.On) v += 4;
            if (switchDryRun.On) v += 8;
            con.GetInjectLock();
            con.injectManualCommand("M111 S" + v);
            con.ReturnInjectLock();
        }

        private void buttonHomeX_Click(object sender, EventArgs e)
        {
            con.GetInjectLock();
            con.injectManualCommand("G28 X0");
            con.ReturnInjectLock();
        }

        private void buttonHomeY_Click(object sender, EventArgs e)
        {
            con.GetInjectLock();
            con.injectManualCommand("G28 Y0");
            con.ReturnInjectLock();
        }

        private void buttonHomeZ_Click(object sender, EventArgs e)
        {
            con.GetInjectLock();
            con.injectManualCommand("G28 Z0");
            con.ReturnInjectLock();
        }

        private void buttonHomeAll_Click(object sender, EventArgs e)
        {
            con.GetInjectLock();
            con.injectManualCommand("G28 X0 Y0 Z0");
            con.ReturnInjectLock();
        }
        private void moveHead(string axis,float amount) {
            con.GetInjectLock();
            bool wasrel = con.analyzer.relative;
            //if(!wasrel) 
                con.injectManualCommand("G91");
            if(axis.Equals("Z"))
                con.injectManualCommand("G1 " + axis + amount.ToString(GCode.format) + " F" + con.maxZFeedRate.ToString(GCode.format));
            else
                con.injectManualCommand("G1 " + axis + amount.ToString(GCode.format) + " F" + con.travelFeedRate.ToString(GCode.format));
            //if (!wasrel) 
                con.injectManualCommand("G90");
            con.ReturnInjectLock();
        }

        private void buttonXM100_Click(object sender, EventArgs e)
        {
            moveHead("X", -100);
        }

        private void buttonXM10_Click(object sender, EventArgs e)
        {
            moveHead("X", -10);
        }

        private void buttonXM1_Click(object sender, EventArgs e)
        {
            moveHead("X", -1);
        }

        private void buttonXM01_Click(object sender, EventArgs e)
        {
            moveHead("X", -0.1f);
        }

        private void buttonXP01_Click(object sender, EventArgs e)
        {
            moveHead("X", 0.1f);
        }

        private void buttonXP1_Click(object sender, EventArgs e)
        {
            moveHead("X", 1);
        }

        private void buttonXP10_Click(object sender, EventArgs e)
        {
            moveHead("X", 10);
        }

        private void buttonXP100_Click(object sender, EventArgs e)
        {
            moveHead("X", 100);
        }

        private void buttonYM100_Click(object sender, EventArgs e)
        {
            moveHead("Y", -100);
        }

        private void buttonYM10_Click(object sender, EventArgs e)
        {
            moveHead("Y", -10);
        }

        private void buttonYM1_Click(object sender, EventArgs e)
        {
            moveHead("Y", -1);
        }

        private void buttonYM01_Click(object sender, EventArgs e)
        {
            moveHead("Y", -0.1f);
        }

        private void buttonYP01_Click(object sender, EventArgs e)
        {
            moveHead("Y", 0.1f);
        }

        private void buttonYP1_Click(object sender, EventArgs e)
        {
            moveHead("Y", 1);
        }

        private void buttonYP10_Click(object sender, EventArgs e)
        {
            moveHead("Y", 10);
        }

        private void buttonYP100_Click(object sender, EventArgs e)
        {
            moveHead("Y", 100);
        }

        private void buttonZM100_Click(object sender, EventArgs e)
        {
            moveHead("Z", -100);
        }

        private void buttonZM10_Click(object sender, EventArgs e)
        {
            moveHead("Z", -10);
        }

        private void buttonZM1_Click(object sender, EventArgs e)
        {
            moveHead("Z", -1);
        }

        private void buttonZM01_Click(object sender, EventArgs e)
        {
            moveHead("Z", -0.1f);
        }

        private void buttonZP01_Click(object sender, EventArgs e)
        {
            moveHead("Z", 0.1f);
        }

        private void buttonZP1_Click(object sender, EventArgs e)
        {
            moveHead("Z", 1);
        }

        private void buttonZP10_Click(object sender, EventArgs e)
        {
            moveHead("Z", 10);
        }

        private void buttonZP100_Click(object sender, EventArgs e)
        {
            moveHead("Z", 100);
        }

        private void switchFanOn_Change(SwitchButton b)
        {
            if (Main.conn.connected == false) return;
            if (!createCommands) return;
            con.GetInjectLock();
            if (switchFanOn.On)
            {
                if(ann.fanVoltage!=trackFanVoltage.Value)
                    con.injectManualCommand("M106 S" + trackFanVoltage.Value);
            }
            else
            {
                con.injectManualCommand("M107");
            }
            con.ReturnInjectLock();
        }

        private void trackFanVoltage_ValueChanged(object sender, EventArgs e)
        {
            float volt = 100f*trackFanVoltage.Value/255;
            labelVoltage.Text = "Output " + volt.ToString("0.0") + "%";
            if (!createCommands) return;
            //switchFanOn.On = true;
            if(switchFanOn.On)
                switchFanOn_Change(null);
        }

        private void buttonExtrude_Click(object sender, EventArgs e)
        {
            con.GetInjectLock();
            bool wasrel = con.analyzer.relative;
            if (!wasrel) con.injectManualCommand("G91");
            con.injectManualCommand("G1 E" + textExtrudeAmount.Text.Trim() + " F"+textExtrudeSpeed.Text.Trim());
            if (!wasrel) con.injectManualCommand("G90");
            con.ReturnInjectLock();
        }

        private void switchExtruderHeatOn_Change(SwitchButton b)
        {
            if (Main.conn.connected == false) return;
            if (!createCommands) return;
            //int temp = 0;
            //int.TryParse(textExtruderSetTemp.Text,out temp);
            con.GetInjectLock();
            if (switchExtruderHeatOn.On)
            {
                con.injectManualCommand("M104 S" + numericUpDownExtruder.Value);
            }
            else
            {
                con.injectManualCommand("M104 S0");
            }
            con.ReturnInjectLock();
        }

        private void switchBedHeat_Change(SwitchButton b)
        {
            if (Main.conn.connected == false) return;
            if (!createCommands) return;
            //int temp = 0;
            //int.TryParse(textPrintbedTemp.Text, out temp);
            con.GetInjectLock();
            if (switchBedHeat.On)
            {
                con.injectManualCommand("M140 S" + numericPrintBed.Value);
            }
            else
            {
                con.injectManualCommand("M140 S0");
            }
            con.ReturnInjectLock();
        }

        private void switchEcho_Change(SwitchButton b)
        {
            sendDebug();
        }

        private void switchInfo_Change(SwitchButton b)
        {
            sendDebug();
        }

        private void switchErrors_Change(SwitchButton b)
        {
            sendDebug();
        }

        private void switchDryRun_Change(SwitchButton b)
        {
            sendDebug();
        }

        private void switchPower_Change(SwitchButton b)
        {
            if (Main.conn.connected == false) return;
            con.GetInjectLock();
            if (switchPower.On)
            {
                con.injectManualCommand("M80");
            }
            else
            {
                con.injectManualCommand("M81");
            }
            con.ReturnInjectLock();
        }

        private void textGCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                buttonSend_Click(null, null);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up)
            {
                commandPos--;
                if (commandPos < 0) commandPos = 0;
                if (commandPos < commands.Count)
                    textGCode.Text = commands.ElementAt(commandPos);
                textGCode.SelectionLength = 0;
                textGCode.SelectionStart = textGCode.Text.Length;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                commandPos++;
                if (commandPos > commands.Count)
                    commandPos = commands.Count;
                if (commandPos < commands.Count)
                    textGCode.Text = commands.ElementAt(commandPos);
                textGCode.SelectionLength = 0;
                textGCode.SelectionStart = textGCode.Text.Length;
                e.Handled = true;
            }
        }
        private void float_Validating(object sender, CancelEventArgs e)
        {
            TextBox box = (TextBox)sender;
            try
            {
                float.Parse(box.Text);
                errorProvider.SetError(box, "");
            }
            catch
            {
                errorProvider.SetError(box, "Not a number.");
            }
        }
        private void floatPos_Validating(object sender, CancelEventArgs e)
        {
            TextBox box = (TextBox)sender;
            try
            {
                float x = float.Parse(box.Text);
                if (x >= 0)
                    errorProvider.SetError(box, "");
                else
                    errorProvider.SetError(box, "Positive number required.");
            }
            catch
            {
                errorProvider.SetError(box, "Not a number.");
            }
        }
        private void int_Validating(object sender, CancelEventArgs e)
        {
            TextBox box = (TextBox)sender;
            try
            {
                int.Parse(box.Text);
                errorProvider.SetError(box, "");
            }
            catch
            {
                errorProvider.SetError(box, "Not an integer.");
            }
        }

        private void buttonGoDisposeArea_Click(object sender, EventArgs e)
        {
            con.doDispose();
        }

        private void buttonSimulateOK_Click(object sender, EventArgs e)
        {
            con.analyzeResponse("ok");
        }

        private void buttonJobStatus_Click(object sender, EventArgs e)
        {
            JobStatus.ShowStatus();
        }

        private void textGCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            {
                if (e.KeyChar == '\r')
                    e.Handled = true;
            }
        }

        private void buttonStopMotor_Click(object sender, EventArgs e)
        {
            con.injectManualCommand("M84");
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            coordUpdate(null, ann.x, ann.y, ann.z);
            updateStatus();
        }

        private void buttonRetract_Click(object sender, EventArgs e)
        {
            con.GetInjectLock();
            bool wasrel = con.analyzer.relative;
            if (!wasrel) con.injectManualCommand("G91");
            con.injectManualCommand("G1 E-" + textRetractAmount.Text.Trim() + " F" + textExtrudeSpeed.Text.Trim());
            if (!wasrel) con.injectManualCommand("G90");
            con.ReturnInjectLock();
        }

        private void textExtrudeSpeed_TextChanged(object sender, EventArgs e)
        {
            RegMemory.SetString("panelExtrudeSpeed", textExtrudeSpeed.Text);
        }

        private void textExtrudeAmount_TextChanged(object sender, EventArgs e)
        {
            RegMemory.SetString("panelExtrudeAmount", textExtrudeAmount.Text);
        }

        private void textRetractAmount_TextChanged(object sender, EventArgs e)
        {
            RegMemory.SetString("panelRetractAmount", textRetractAmount.Text);
        }

        private void sliderSpeed_ValueChanged(object sender, EventArgs e)
        {
            if (!createCommands) return;
            //labelSpeed.Text = sliderSpeed.Value.ToString() + "%";
            int oldcon = con.speedMultiply;
            if (sender == sliderSpeed)
            {
                if (con.speedMultiply != sliderSpeed.Value)
                {
                    con.speedMultiply = sliderSpeed.Value;
                    numericUpDownSpeed.Value = con.speedMultiply;
                }
            }
            else
            {
                if (con.speedMultiply != numericUpDownSpeed.Value)
                {
                    con.speedMultiply = (int)numericUpDownSpeed.Value;
                    sliderSpeed.Value = con.speedMultiply;
                }
            }
            if(oldcon != con.speedMultiply && con.connected && (con.isMarlin || con.isRepetier))
                con.injectManualCommand("M220 S"+sliderSpeed.Value.ToString());
        }

        private void numericUpDownExtruder_ValueChanged(object sender, EventArgs e)
        {
            if (!createCommands) return;
            if (switchExtruderHeatOn.On)
            {
                con.GetInjectLock();
                con.injectManualCommand("M104 S" + numericUpDownExtruder.Value.ToString("0"));
                con.ReturnInjectLock();
            }
        }

        private void numericPrintBed_ValueChanged(object sender, EventArgs e)
        {
            if (!createCommands) return;
            if (switchBedHeat.On)
            {
                con.GetInjectLock();
                con.injectManualCommand("M140 S" + numericPrintBed.Value.ToString("0"));
                con.ReturnInjectLock();
            }

        }

        private void XY_arrowValueChanged(ArrowButton sender, string value)
        {
            if (value.Length == 0)
                labelMoveDist.Text = "";
            else
                labelMoveDist.Text = value + " mm";
        }
        private void Z_arrowValueChanged(ArrowButton sender, string value)
        {
            if (value.Length == 0)
                labelZDiff.Text = "";
            else
                labelZDiff.Text = value + " mm";
        }

        private void arrowButtonXPlus_Click(object sender, EventArgs e)
        {
            float d = ((ArrowButton)sender).CurrentValueF;
            if (ann.hasXHome && d + ann.x > Main.printerSettings.PrintAreaWidth) d = Main.printerSettings.PrintAreaWidth - ann.x;
            moveHead("X", d);
        }

        private void arrowButtonXMinus_Click(object sender, EventArgs e)
        {
            float d = -((ArrowButton)sender).CurrentValueF;
            if (ann.hasXHome && d + ann.x <0) d = -ann.x;
            moveHead("X", d);
        }

        private void arrowButtonYPlus_Click(object sender, EventArgs e)
        {
            float d = ((ArrowButton)sender).CurrentValueF;
            if (ann.hasYHome && d + ann.y > Main.printerSettings.PrintAreaDepth) d = Main.printerSettings.PrintAreaDepth - ann.y;
            moveHead("Y",d);

        }

        private void arrowButtonYMinus_Click(object sender, EventArgs e)
        {
            float d = -((ArrowButton)sender).CurrentValueF;
            if (ann.hasYHome && d + ann.y < 0) d = -ann.y;
            moveHead("Y", d);
        }

        private void arrowButtonZPlus_Click(object sender, EventArgs e)
        {
            float d = ((ArrowButton)sender).CurrentValueF;
            if (ann.hasZHome && d + ann.z > Main.printerSettings.PrintAreaHeight) d = Main.printerSettings.PrintAreaHeight - ann.z;
            moveHead("Z", d);

        }

        private void arrowButtonZMinus_Click(object sender, EventArgs e)
        {
            float d = -((ArrowButton)sender).CurrentValueF;
            if (ann.hasZHome && d + ann.z < 0) d = -ann.z;
            moveHead("Z", d);

        }

     }
}
