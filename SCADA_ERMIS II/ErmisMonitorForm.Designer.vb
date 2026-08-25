Imports System.Drawing
Imports System.Windows.Forms

Partial Public Class ErmisMonitorForm

    Private components As System.ComponentModel.IContainer

    Friend WithEvents topArea As Panel
    Friend WithEvents comList As ListBox
    Friend WithEvents refreshButton As Button
    Friend WithEvents connectButton As Button
    Friend WithEvents clearButton As Button
    Friend WithEvents baudBox As NumericUpDown
    Friend WithEvents slaveBox As NumericUpDown
    Friend WithEvents autoAckBox As CheckBox
    Friend WithEvents outputConsole As RichTextBox
    Friend WithEvents statusLabel As Label
    Friend WithEvents portLabel As Label
    Friend WithEvents baudLabel As Label
    Friend WithEvents slaveLabel As Label

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If

        MyBase.Dispose(disposing)
    End Sub

    Private Sub InitializeComponent()
        topArea = New Panel()
        portLabel = New Label()
        comList = New ListBox()
        refreshButton = New Button()
        connectButton = New Button()
        baudLabel = New Label()
        baudBox = New NumericUpDown()
        slaveLabel = New Label()
        slaveBox = New NumericUpDown()
        autoAckBox = New CheckBox()
        clearButton = New Button()
        statusLabel = New Label()
        outputConsole = New RichTextBox()
        GroupBox1 = New GroupBox()
        Label1 = New Label()
        TempName = New Label()
        Temp1 = New Label()
        Temp2 = New Label()
        Temp3 = New Label()
        PressName = New Label()
        Press1 = New Label()
        Press2 = New Label()
        Press3 = New Label()
        PartName = New Label()
        Pm1 = New Label()
        Pm25 = New Label()
        Pm5 = New Label()
        Pm10 = New Label()
        CO2Name = New Label()
        Label2 = New Label()
        VOCName = New Label()
        VOC = New Label()
        RHName = New Label()
        RH = New Label()
        GPSBox = New GroupBox()
        XName = New Label()
        YName = New Label()
        ZName = New Label()
        X = New Label()
        Y = New Label()
        Z = New Label()
        topArea.SuspendLayout()
        CType(baudBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(slaveBox, System.ComponentModel.ISupportInitialize).BeginInit()
        GroupBox1.SuspendLayout()
        GPSBox.SuspendLayout()
        SuspendLayout()
        ' 
        ' topArea
        ' 
        topArea.BackColor = SystemColors.Control
        topArea.Controls.Add(portLabel)
        topArea.Controls.Add(comList)
        topArea.Controls.Add(refreshButton)
        topArea.Controls.Add(connectButton)
        topArea.Controls.Add(baudLabel)
        topArea.Controls.Add(baudBox)
        topArea.Controls.Add(slaveLabel)
        topArea.Controls.Add(slaveBox)
        topArea.Controls.Add(autoAckBox)
        topArea.Controls.Add(clearButton)
        topArea.Controls.Add(statusLabel)
        topArea.Dock = DockStyle.Top
        topArea.Location = New Point(0, 0)
        topArea.Name = "topArea"
        topArea.Size = New Size(1084, 140)
        topArea.TabIndex = 0
        ' 
        ' portLabel
        ' 
        portLabel.AutoSize = True
        portLabel.Location = New Point(12, 12)
        portLabel.Name = "portLabel"
        portLabel.Size = New Size(88, 20)
        portLabel.TabIndex = 0
        portLabel.Text = "Θύρες COM"
        ' 
        ' comList
        ' 
        comList.Font = New Font("Segoe UI", 10.0F)
        comList.FormattingEnabled = True
        comList.Location = New Point(12, 34)
        comList.Name = "comList"
        comList.Size = New Size(180, 73)
        comList.TabIndex = 0
        ' 
        ' refreshButton
        ' 
        refreshButton.Location = New Point(205, 34)
        refreshButton.Name = "refreshButton"
        refreshButton.Size = New Size(115, 32)
        refreshButton.TabIndex = 1
        refreshButton.Text = "Ανανέωση"
        refreshButton.UseVisualStyleBackColor = True
        ' 
        ' connectButton
        ' 
        connectButton.Location = New Point(205, 78)
        connectButton.Name = "connectButton"
        connectButton.Size = New Size(115, 46)
        connectButton.TabIndex = 2
        connectButton.Text = "Σύνδεση"
        connectButton.UseVisualStyleBackColor = True
        ' 
        ' baudLabel
        ' 
        baudLabel.AutoSize = True
        baudLabel.Location = New Point(345, 12)
        baudLabel.Name = "baudLabel"
        baudLabel.Size = New Size(73, 20)
        baudLabel.TabIndex = 3
        baudLabel.Text = "Baud rate"
        ' 
        ' baudBox
        ' 
        baudBox.Increment = New Decimal(New Integer() {9600, 0, 0, 0})
        baudBox.Location = New Point(345, 34)
        baudBox.Maximum = New Decimal(New Integer() {921600, 0, 0, 0})
        baudBox.Minimum = New Decimal(New Integer() {1200, 0, 0, 0})
        baudBox.Name = "baudBox"
        baudBox.Size = New Size(120, 27)
        baudBox.TabIndex = 3
        baudBox.Value = New Decimal(New Integer() {115200, 0, 0, 0})
        ' 
        ' slaveLabel
        ' 
        slaveLabel.AutoSize = True
        slaveLabel.Location = New Point(485, 12)
        slaveLabel.Name = "slaveLabel"
        slaveLabel.Size = New Size(63, 20)
        slaveLabel.TabIndex = 4
        slaveLabel.Text = "Slave ID"
        ' 
        ' slaveBox
        ' 
        slaveBox.Location = New Point(485, 34)
        slaveBox.Maximum = New Decimal(New Integer() {247, 0, 0, 0})
        slaveBox.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        slaveBox.Name = "slaveBox"
        slaveBox.Size = New Size(85, 27)
        slaveBox.TabIndex = 4
        slaveBox.Value = New Decimal(New Integer() {1, 0, 0, 0})
        ' 
        ' autoAckBox
        ' 
        autoAckBox.AutoSize = True
        autoAckBox.Checked = True
        autoAckBox.CheckState = CheckState.Checked
        autoAckBox.Location = New Point(345, 82)
        autoAckBox.Name = "autoAckBox"
        autoAckBox.Size = New Size(131, 24)
        autoAckBox.TabIndex = 5
        autoAckBox.Text = "Αυτόματο ACK"
        autoAckBox.UseVisualStyleBackColor = True
        ' 
        ' clearButton
        ' 
        clearButton.Location = New Point(590, 34)
        clearButton.Name = "clearButton"
        clearButton.Size = New Size(125, 32)
        clearButton.TabIndex = 6
        clearButton.Text = "Καθαρισμός"
        clearButton.UseVisualStyleBackColor = True
        ' 
        ' statusLabel
        ' 
        statusLabel.AutoSize = True
        statusLabel.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        statusLabel.Location = New Point(590, 86)
        statusLabel.Name = "statusLabel"
        statusLabel.Size = New Size(148, 23)
        statusLabel.TabIndex = 7
        statusLabel.Text = "Αποσυνδεδεμένο"
        ' 
        ' outputConsole
        ' 
        outputConsole.BackColor = Color.Black
        outputConsole.BorderStyle = BorderStyle.None
        outputConsole.DetectUrls = False
        outputConsole.Dock = DockStyle.Bottom
        outputConsole.Font = New Font("Consolas", 10.0F)
        outputConsole.ForeColor = Color.Lime
        outputConsole.Location = New Point(0, 587)
        outputConsole.Name = "outputConsole"
        outputConsole.ReadOnly = True
        outputConsole.Size = New Size(1084, 133)
        outputConsole.TabIndex = 1
        outputConsole.Text = ""
        outputConsole.WordWrap = False
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(RH)
        GroupBox1.Controls.Add(RHName)
        GroupBox1.Controls.Add(VOC)
        GroupBox1.Controls.Add(VOCName)
        GroupBox1.Controls.Add(Label2)
        GroupBox1.Controls.Add(CO2Name)
        GroupBox1.Controls.Add(Pm10)
        GroupBox1.Controls.Add(Pm5)
        GroupBox1.Controls.Add(Pm25)
        GroupBox1.Controls.Add(Pm1)
        GroupBox1.Controls.Add(PartName)
        GroupBox1.Controls.Add(Press3)
        GroupBox1.Controls.Add(Press2)
        GroupBox1.Controls.Add(Press1)
        GroupBox1.Controls.Add(PressName)
        GroupBox1.Controls.Add(Temp3)
        GroupBox1.Controls.Add(Temp2)
        GroupBox1.Controls.Add(Temp1)
        GroupBox1.Controls.Add(TempName)
        GroupBox1.Location = New Point(54, 174)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(684, 309)
        GroupBox1.TabIndex = 2
        GroupBox1.TabStop = False
        GroupBox1.Text = "DATA"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(0, 0)
        Label1.Name = "Label1"
        Label1.Size = New Size(53, 20)
        Label1.TabIndex = 3
        Label1.Text = "Label1"
        ' 
        ' TempName
        ' 
        TempName.AutoSize = True
        TempName.Font = New Font("Showcard Gothic", 9.0F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        TempName.Location = New Point(31, 23)
        TempName.Name = "TempName"
        TempName.Size = New Size(121, 18)
        TempName.TabIndex = 0
        TempName.Text = "Temperature:"
        ' 
        ' Temp1
        ' 
        Temp1.AutoSize = True
        Temp1.Location = New Point(167, 21)
        Temp1.Name = "Temp1"
        Temp1.Size = New Size(18, 20)
        Temp1.TabIndex = 1
        Temp1.Text = "C"
        ' 
        ' Temp2
        ' 
        Temp2.AutoSize = True
        Temp2.Location = New Point(291, 21)
        Temp2.Name = "Temp2"
        Temp2.Size = New Size(18, 20)
        Temp2.TabIndex = 2
        Temp2.Text = "C"
        ' 
        ' Temp3
        ' 
        Temp3.AutoSize = True
        Temp3.Location = New Point(415, 23)
        Temp3.Name = "Temp3"
        Temp3.Size = New Size(18, 20)
        Temp3.TabIndex = 3
        Temp3.Text = "C"
        ' 
        ' PressName
        ' 
        PressName.AutoSize = True
        PressName.Font = New Font("Showcard Gothic", 9.0F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        PressName.Location = New Point(31, 65)
        PressName.Name = "PressName"
        PressName.Size = New Size(88, 18)
        PressName.TabIndex = 4
        PressName.Text = "Pressure:"
        ' 
        ' Press1
        ' 
        Press1.AutoSize = True
        Press1.Location = New Point(167, 64)
        Press1.Name = "Press1"
        Press1.Size = New Size(24, 20)
        Press1.TabIndex = 5
        Press1.Text = "Pa"
        ' 
        ' Press2
        ' 
        Press2.AutoSize = True
        Press2.Location = New Point(285, 63)
        Press2.Name = "Press2"
        Press2.Size = New Size(24, 20)
        Press2.TabIndex = 6
        Press2.Text = "Pa"
        ' 
        ' Press3
        ' 
        Press3.AutoSize = True
        Press3.Location = New Point(415, 65)
        Press3.Name = "Press3"
        Press3.Size = New Size(24, 20)
        Press3.TabIndex = 7
        Press3.Text = "Pa"
        ' 
        ' PartName
        ' 
        PartName.AutoSize = True
        PartName.Font = New Font("Showcard Gothic", 9.0F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        PartName.Location = New Point(31, 115)
        PartName.Name = "PartName"
        PartName.Size = New Size(90, 18)
        PartName.TabIndex = 8
        PartName.Text = "Particles:"
        ' 
        ' Pm1
        ' 
        Pm1.AutoSize = True
        Pm1.Location = New Point(167, 113)
        Pm1.Name = "Pm1"
        Pm1.Size = New Size(54, 20)
        Pm1.TabIndex = 9
        Pm1.Text = "μg/m3"
        ' 
        ' Pm25
        ' 
        Pm25.AutoSize = True
        Pm25.Location = New Point(291, 113)
        Pm25.Name = "Pm25"
        Pm25.Size = New Size(54, 20)
        Pm25.TabIndex = 10
        Pm25.Text = "μg/m3"
        ' 
        ' Pm5
        ' 
        Pm5.AutoSize = True
        Pm5.Location = New Point(415, 113)
        Pm5.Name = "Pm5"
        Pm5.Size = New Size(54, 20)
        Pm5.TabIndex = 11
        Pm5.Text = "μg/m3"
        ' 
        ' Pm10
        ' 
        Pm10.AutoSize = True
        Pm10.Location = New Point(536, 113)
        Pm10.Name = "Pm10"
        Pm10.Size = New Size(54, 20)
        Pm10.TabIndex = 12
        Pm10.Text = "μg/m3"
        ' 
        ' CO2Name
        ' 
        CO2Name.AutoSize = True
        CO2Name.Font = New Font("Showcard Gothic", 9.0F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        CO2Name.Location = New Point(31, 164)
        CO2Name.Name = "CO2Name"
        CO2Name.Size = New Size(35, 18)
        CO2Name.TabIndex = 13
        CO2Name.Text = "CO2"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(167, 164)
        Label2.Name = "Label2"
        Label2.Size = New Size(40, 20)
        Label2.TabIndex = 14
        Label2.Text = "ppm"
        ' 
        ' VOCName
        ' 
        VOCName.AutoSize = True
        VOCName.Font = New Font("Showcard Gothic", 9.0F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        VOCName.Location = New Point(31, 214)
        VOCName.Name = "VOCName"
        VOCName.Size = New Size(38, 18)
        VOCName.TabIndex = 15
        VOCName.Text = "VOC"
        ' 
        ' VOC
        ' 
        VOC.AutoSize = True
        VOC.Location = New Point(167, 212)
        VOC.Name = "VOC"
        VOC.Size = New Size(78, 20)
        VOC.TabIndex = 16
        VOC.Text = "VOC Index"
        ' 
        ' RHName
        ' 
        RHName.AutoSize = True
        RHName.Font = New Font("Showcard Gothic", 9.0F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        RHName.Location = New Point(31, 264)
        RHName.Name = "RHName"
        RHName.Size = New Size(118, 18)
        RHName.TabIndex = 17
        RHName.Text = "Rel. Humidity"
        ' 
        ' RH
        ' 
        RH.AutoSize = True
        RH.Location = New Point(167, 262)
        RH.Name = "RH"
        RH.Size = New Size(29, 20)
        RH.TabIndex = 18
        RH.Text = "RH"
        ' 
        ' GPSBox
        ' 
        GPSBox.Controls.Add(Z)
        GPSBox.Controls.Add(Y)
        GPSBox.Controls.Add(X)
        GPSBox.Controls.Add(ZName)
        GPSBox.Controls.Add(YName)
        GPSBox.Controls.Add(XName)
        GPSBox.Location = New Point(57, 505)
        GPSBox.Name = "GPSBox"
        GPSBox.Size = New Size(686, 76)
        GPSBox.TabIndex = 4
        GPSBox.TabStop = False
        GPSBox.Text = "GPS"
        ' 
        ' XName
        ' 
        XName.AutoSize = True
        XName.Font = New Font("Showcard Gothic", 9.0F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        XName.Location = New Point(14, 33)
        XName.Name = "XName"
        XName.Size = New Size(24, 18)
        XName.TabIndex = 19
        XName.Text = "X:"
        ' 
        ' YName
        ' 
        YName.AutoSize = True
        YName.Font = New Font("Showcard Gothic", 9.0F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        YName.Location = New Point(194, 33)
        YName.Name = "YName"
        YName.Size = New Size(22, 18)
        YName.TabIndex = 20
        YName.Text = "Y:"
        ' 
        ' ZName
        ' 
        ZName.AutoSize = True
        ZName.Font = New Font("Showcard Gothic", 9.0F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        ZName.Location = New Point(359, 33)
        ZName.Name = "ZName"
        ZName.Size = New Size(22, 18)
        ZName.TabIndex = 21
        ZName.Text = "Z:"
        ' 
        ' X
        ' 
        X.AutoSize = True
        X.Location = New Point(44, 31)
        X.Name = "X"
        X.Size = New Size(44, 20)
        X.TabIndex = 19
        X.Text = "00.00"
        ' 
        ' Y
        ' 
        Y.AutoSize = True
        Y.Location = New Point(222, 32)
        Y.Name = "Y"
        Y.Size = New Size(44, 20)
        Y.TabIndex = 20
        Y.Text = "00.00"
        ' 
        ' Z
        ' 
        Z.AutoSize = True
        Z.Location = New Point(392, 31)
        Z.Name = "Z"
        Z.Size = New Size(44, 20)
        Z.TabIndex = 20
        Z.Text = "00.00"
        ' 
        ' ErmisMonitorForm
        ' 
        AutoScaleDimensions = New SizeF(8.0F, 20.0F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1084, 720)
        Controls.Add(GPSBox)
        Controls.Add(Label1)
        Controls.Add(GroupBox1)
        Controls.Add(outputConsole)
        Controls.Add(topArea)
        MinimumSize = New Size(850, 550)
        Name = "ErmisMonitorForm"
        StartPosition = FormStartPosition.CenterScreen
        Text = "ERMIS II - LoRa Modbus SCADA Monitor"
        topArea.ResumeLayout(False)
        topArea.PerformLayout()
        CType(baudBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(slaveBox, System.ComponentModel.ISupportInitialize).EndInit()
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        GPSBox.ResumeLayout(False)
        GPSBox.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents TempName As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Temp3 As Label
    Friend WithEvents Temp2 As Label
    Friend WithEvents Temp1 As Label
    Friend WithEvents PressName As Label
    Friend WithEvents PartName As Label
    Friend WithEvents Press3 As Label
    Friend WithEvents Press2 As Label
    Friend WithEvents Press1 As Label
    Friend WithEvents Pm1 As Label
    Friend WithEvents Pm25 As Label
    Friend WithEvents CO2Name As Label
    Friend WithEvents Pm10 As Label
    Friend WithEvents Pm5 As Label
    Friend WithEvents VOCName As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents RH As Label
    Friend WithEvents RHName As Label
    Friend WithEvents VOC As Label
    Friend WithEvents GPSBox As GroupBox
    Friend WithEvents XName As Label
    Friend WithEvents YName As Label
    Friend WithEvents Y As Label
    Friend WithEvents X As Label
    Friend WithEvents ZName As Label
    Friend WithEvents Z As Label

End Class
