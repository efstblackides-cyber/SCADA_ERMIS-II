Imports System.Drawing
Imports System.Windows.Forms
Imports ErmisSensorStatusTool

Partial Public Class ErmisMonitorForm

    Private components As System.ComponentModel.IContainer

    Friend WithEvents mainLayout As TableLayoutPanel

    Friend WithEvents topArea As Panel
    Friend WithEvents headerPanel As Panel
    Friend WithEvents titleLabel As Label
    Friend WithEvents subtitleLabel As Label

    Friend WithEvents connectionPanel As Panel
    Friend WithEvents comList As ListBox
    Friend WithEvents refreshButton As Button
    Friend WithEvents connectButton As Button
    Friend WithEvents clearButton As Button
    Friend WithEvents baudBox As NumericUpDown
    Friend WithEvents slaveBox As NumericUpDown
    Friend WithEvents autoAckBox As CheckBox
    Friend WithEvents statusLabel As Label
    Friend WithEvents statusDot As Label
    Friend WithEvents portLabel As Label
    Friend WithEvents baudLabel As Label
    Friend WithEvents slaveLabel As Label

    Friend WithEvents contentLayout As TableLayoutPanel
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents rightLayout As TableLayoutPanel
    Friend WithEvents GPSBox As GroupBox

    Friend WithEvents TempName As Label
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
    Friend WithEvents Label1 As Label

    Friend WithEvents XName As Label
    Friend WithEvents YName As Label
    Friend WithEvents Y As Label
    Friend WithEvents X As Label
    Friend WithEvents ZName As Label
    Friend WithEvents Z As Label

    Friend WithEvents tempHeader1 As Label
    Friend WithEvents tempHeader2 As Label
    Friend WithEvents tempHeader3 As Label
    Friend WithEvents pmHeader1 As Label
    Friend WithEvents pmHeader25 As Label
    Friend WithEvents pmHeader4 As Label
    Friend WithEvents pmHeader10 As Label
    Friend WithEvents pressureUnit As Label
    Friend WithEvents altitudeUnit As Label

    Friend WithEvents consolePanel As Panel
    Friend WithEvents consoleTitle As Label
    Friend WithEvents outputConsole As RichTextBox

    Friend WithEvents dashboardPanel As Panel

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    Private Sub InitializeComponent()
        mainLayout = New TableLayoutPanel()
        topArea = New Panel()
        connectionPanel = New Panel()
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
        statusDot = New Label()
        statusLabel = New Label()
        headerPanel = New Panel()
        titleLabel = New Label()
        subtitleLabel = New Label()
        dashboardPanel = New Panel()
        contentLayout = New TableLayoutPanel()
        GroupBox1 = New GroupBox()
        TempName = New Label()
        tempHeader1 = New Label()
        tempHeader2 = New Label()
        tempHeader3 = New Label()
        Temp1 = New Label()
        Temp2 = New Label()
        Temp3 = New Label()
        PressName = New Label()
        pressureUnit = New Label()
        altitudeUnit = New Label()
        Press1 = New Label()
        Press2 = New Label()
        PartName = New Label()
        pmHeader1 = New Label()
        pmHeader25 = New Label()
        pmHeader4 = New Label()
        pmHeader10 = New Label()
        Pm1 = New Label()
        Pm25 = New Label()
        Pm5 = New Label()
        Pm10 = New Label()
        CO2Name = New Label()
        VOCName = New Label()
        RHName = New Label()
        Label2 = New Label()
        VOC = New Label()
        RH = New Label()
        rightLayout = New TableLayoutPanel()
        GPSBox = New GroupBox()
        XName = New Label()
        X = New Label()
        YName = New Label()
        Y = New Label()
        ZName = New Label()
        Z = New Label()
        consolePanel = New Panel()
        consoleTitle = New Label()
        outputConsole = New RichTextBox()
        Press3 = New Label()
        Label1 = New Label()
        _statusTool = New SensorStatusPanel()
        statusGroup = New GroupBox()
        mainLayout.SuspendLayout()
        topArea.SuspendLayout()
        connectionPanel.SuspendLayout()
        CType(baudBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(slaveBox, System.ComponentModel.ISupportInitialize).BeginInit()
        headerPanel.SuspendLayout()
        dashboardPanel.SuspendLayout()
        contentLayout.SuspendLayout()
        GroupBox1.SuspendLayout()
        rightLayout.SuspendLayout()
        GPSBox.SuspendLayout()
        consolePanel.SuspendLayout()
        statusGroup.SuspendLayout()
        SuspendLayout()
        ' 
        ' mainLayout
        ' 
        mainLayout.BackColor = Color.FromArgb(CByte(242), CByte(245), CByte(249))
        mainLayout.ColumnCount = 1
        mainLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        mainLayout.Controls.Add(topArea, 0, 0)
        mainLayout.Controls.Add(dashboardPanel, 0, 1)
        mainLayout.Controls.Add(consolePanel, 0, 2)
        mainLayout.Dock = DockStyle.Fill
        mainLayout.Location = New Point(0, 0)
        mainLayout.Margin = New Padding(0)
        mainLayout.Name = "mainLayout"
        mainLayout.RowCount = 3
        mainLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 175.0F))
        mainLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        mainLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 230.0F))
        mainLayout.Size = New Size(1500, 900)
        mainLayout.TabIndex = 0
        ' 
        ' topArea
        ' 
        topArea.BackColor = Color.FromArgb(CByte(242), CByte(245), CByte(249))
        topArea.Controls.Add(connectionPanel)
        topArea.Controls.Add(headerPanel)
        topArea.Dock = DockStyle.Fill
        topArea.Location = New Point(3, 3)
        topArea.Name = "topArea"
        topArea.Padding = New Padding(14, 12, 14, 8)
        topArea.Size = New Size(1494, 169)
        topArea.TabIndex = 0
        ' 
        ' connectionPanel
        ' 
        connectionPanel.BackColor = Color.White
        connectionPanel.Controls.Add(portLabel)
        connectionPanel.Controls.Add(comList)
        connectionPanel.Controls.Add(refreshButton)
        connectionPanel.Controls.Add(connectButton)
        connectionPanel.Controls.Add(baudLabel)
        connectionPanel.Controls.Add(baudBox)
        connectionPanel.Controls.Add(slaveLabel)
        connectionPanel.Controls.Add(slaveBox)
        connectionPanel.Controls.Add(autoAckBox)
        connectionPanel.Controls.Add(clearButton)
        connectionPanel.Controls.Add(statusDot)
        connectionPanel.Controls.Add(statusLabel)
        connectionPanel.Dock = DockStyle.Fill
        connectionPanel.Location = New Point(14, 80)
        connectionPanel.Name = "connectionPanel"
        connectionPanel.Size = New Size(1466, 81)
        connectionPanel.TabIndex = 0
        ' 
        ' portLabel
        ' 
        portLabel.AutoSize = True
        portLabel.Font = New Font("Segoe UI Semibold", 8.5F)
        portLabel.ForeColor = Color.FromArgb(CByte(92), CByte(108), CByte(124))
        portLabel.Location = New Point(18, 8)
        portLabel.Name = "portLabel"
        portLabel.Size = New Size(84, 20)
        portLabel.TabIndex = 0
        portLabel.Text = "COM PORT"
        ' 
        ' comList
        ' 
        comList.BorderStyle = BorderStyle.FixedSingle
        comList.Font = New Font("Segoe UI", 9.5F)
        comList.Location = New Point(18, 30)
        comList.Name = "comList"
        comList.Size = New Size(142, 44)
        comList.TabIndex = 1
        ' 
        ' refreshButton
        ' 
        refreshButton.BackColor = Color.White
        refreshButton.FlatAppearance.BorderColor = Color.FromArgb(CByte(210), CByte(218), CByte(226))
        refreshButton.FlatStyle = FlatStyle.Flat
        refreshButton.Font = New Font("Segoe UI Semibold", 9.0F)
        refreshButton.ForeColor = Color.FromArgb(CByte(45), CByte(60), CByte(75))
        refreshButton.Location = New Point(170, 30)
        refreshButton.Name = "refreshButton"
        refreshButton.Size = New Size(96, 34)
        refreshButton.TabIndex = 2
        refreshButton.Text = "Refresh"
        refreshButton.UseVisualStyleBackColor = False
        ' 
        ' connectButton
        ' 
        connectButton.BackColor = Color.FromArgb(CByte(0), CByte(122), CByte(204))
        connectButton.FlatAppearance.BorderSize = 0
        connectButton.FlatStyle = FlatStyle.Flat
        connectButton.Font = New Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
        connectButton.ForeColor = Color.White
        connectButton.Location = New Point(276, 30)
        connectButton.Name = "connectButton"
        connectButton.Size = New Size(116, 34)
        connectButton.TabIndex = 3
        connectButton.Text = "Connect"
        connectButton.UseVisualStyleBackColor = False
        ' 
        ' baudLabel
        ' 
        baudLabel.AutoSize = True
        baudLabel.Font = New Font("Segoe UI Semibold", 8.5F)
        baudLabel.ForeColor = Color.FromArgb(CByte(92), CByte(108), CByte(124))
        baudLabel.Location = New Point(418, 8)
        baudLabel.Name = "baudLabel"
        baudLabel.Size = New Size(88, 20)
        baudLabel.TabIndex = 4
        baudLabel.Text = "BAUD RATE"
        ' 
        ' baudBox
        ' 
        baudBox.BorderStyle = BorderStyle.FixedSingle
        baudBox.Increment = New Decimal(New Integer() {9600, 0, 0, 0})
        baudBox.Location = New Point(418, 31)
        baudBox.Maximum = New Decimal(New Integer() {921600, 0, 0, 0})
        baudBox.Minimum = New Decimal(New Integer() {1200, 0, 0, 0})
        baudBox.Name = "baudBox"
        baudBox.Size = New Size(116, 27)
        baudBox.TabIndex = 5
        baudBox.Value = New Decimal(New Integer() {115200, 0, 0, 0})
        ' 
        ' slaveLabel
        ' 
        slaveLabel.AutoSize = True
        slaveLabel.Font = New Font("Segoe UI Semibold", 8.5F)
        slaveLabel.ForeColor = Color.FromArgb(CByte(92), CByte(108), CByte(124))
        slaveLabel.Location = New Point(554, 8)
        slaveLabel.Name = "slaveLabel"
        slaveLabel.Size = New Size(70, 20)
        slaveLabel.TabIndex = 6
        slaveLabel.Text = "SLAVE ID"
        ' 
        ' slaveBox
        ' 
        slaveBox.BorderStyle = BorderStyle.FixedSingle
        slaveBox.Location = New Point(554, 31)
        slaveBox.Maximum = New Decimal(New Integer() {247, 0, 0, 0})
        slaveBox.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        slaveBox.Name = "slaveBox"
        slaveBox.Size = New Size(76, 27)
        slaveBox.TabIndex = 7
        slaveBox.Value = New Decimal(New Integer() {1, 0, 0, 0})
        ' 
        ' autoAckBox
        ' 
        autoAckBox.AutoSize = True
        autoAckBox.Checked = True
        autoAckBox.CheckState = CheckState.Checked
        autoAckBox.Font = New Font("Segoe UI", 9.0F)
        autoAckBox.ForeColor = Color.FromArgb(CByte(45), CByte(60), CByte(75))
        autoAckBox.Location = New Point(654, 33)
        autoAckBox.Name = "autoAckBox"
        autoAckBox.Size = New Size(95, 24)
        autoAckBox.TabIndex = 8
        autoAckBox.Text = "Auto ACK"
        ' 
        ' clearButton
        ' 
        clearButton.BackColor = Color.White
        clearButton.FlatAppearance.BorderColor = Color.FromArgb(CByte(210), CByte(218), CByte(226))
        clearButton.FlatStyle = FlatStyle.Flat
        clearButton.Font = New Font("Segoe UI Semibold", 9.0F)
        clearButton.ForeColor = Color.FromArgb(CByte(45), CByte(60), CByte(75))
        clearButton.Location = New Point(768, 30)
        clearButton.Name = "clearButton"
        clearButton.Size = New Size(96, 34)
        clearButton.TabIndex = 9
        clearButton.Text = "Clear log"
        clearButton.UseVisualStyleBackColor = False
        ' 
        ' statusDot
        ' 
        statusDot.BackColor = Color.FromArgb(CByte(145), CByte(155), CByte(165))
        statusDot.Location = New Point(894, 38)
        statusDot.Name = "statusDot"
        statusDot.Size = New Size(10, 10)
        statusDot.TabIndex = 10
        ' 
        ' statusLabel
        ' 
        statusLabel.AutoSize = True
        statusLabel.Font = New Font("Segoe UI Semibold", 9.0F)
        statusLabel.ForeColor = Color.FromArgb(CByte(69), CByte(82), CByte(95))
        statusLabel.Location = New Point(914, 32)
        statusLabel.Name = "statusLabel"
        statusLabel.Size = New Size(67, 20)
        statusLabel.TabIndex = 11
        statusLabel.Text = "OFFLINE"
        ' 
        ' headerPanel
        ' 
        headerPanel.BackColor = Color.FromArgb(CByte(20), CByte(32), CByte(48))
        headerPanel.Controls.Add(titleLabel)
        headerPanel.Controls.Add(subtitleLabel)
        headerPanel.Dock = DockStyle.Top
        headerPanel.Location = New Point(14, 12)
        headerPanel.Name = "headerPanel"
        headerPanel.Size = New Size(1466, 68)
        headerPanel.TabIndex = 1
        ' 
        ' titleLabel
        ' 
        titleLabel.AutoSize = True
        titleLabel.Font = New Font("Segoe UI Semibold", 17.0F, FontStyle.Bold)
        titleLabel.ForeColor = Color.White
        titleLabel.Location = New Point(22, 8)
        titleLabel.Name = "titleLabel"
        titleLabel.Size = New Size(125, 40)
        titleLabel.TabIndex = 0
        titleLabel.Text = "ERMIS II"
        ' 
        ' subtitleLabel
        ' 
        subtitleLabel.AutoSize = True
        subtitleLabel.Font = New Font("Segoe UI", 9.0F)
        subtitleLabel.ForeColor = Color.FromArgb(CByte(173), CByte(190), CByte(207))
        subtitleLabel.Location = New Point(24, 42)
        subtitleLabel.Name = "subtitleLabel"
        subtitleLabel.Size = New Size(398, 20)
        subtitleLabel.TabIndex = 1
        subtitleLabel.Text = "LoRa Modbus Telemetry · Environmental & Flight Monitoring"
        ' 
        ' dashboardPanel
        ' 
        dashboardPanel.BackColor = Color.FromArgb(CByte(242), CByte(245), CByte(249))
        dashboardPanel.Controls.Add(contentLayout)
        dashboardPanel.Dock = DockStyle.Fill
        dashboardPanel.Location = New Point(3, 178)
        dashboardPanel.Name = "dashboardPanel"
        dashboardPanel.Padding = New Padding(14, 10, 14, 10)
        dashboardPanel.Size = New Size(1494, 489)
        dashboardPanel.TabIndex = 1
        ' 
        ' contentLayout
        ' 
        contentLayout.BackColor = Color.FromArgb(CByte(242), CByte(245), CByte(249))
        contentLayout.ColumnCount = 2
        contentLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 66.0F))
        contentLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 34.0F))
        contentLayout.Controls.Add(GroupBox1, 0, 0)
        contentLayout.Controls.Add(rightLayout, 1, 0)
        contentLayout.Dock = DockStyle.Fill
        contentLayout.Location = New Point(14, 10)
        contentLayout.Margin = New Padding(0)
        contentLayout.Name = "contentLayout"
        contentLayout.RowCount = 1
        contentLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        contentLayout.Size = New Size(1466, 469)
        contentLayout.TabIndex = 0
        ' 
        ' GroupBox1
        ' 
        GroupBox1.BackColor = Color.White
        GroupBox1.Controls.Add(TempName)
        GroupBox1.Controls.Add(tempHeader1)
        GroupBox1.Controls.Add(tempHeader2)
        GroupBox1.Controls.Add(tempHeader3)
        GroupBox1.Controls.Add(Temp1)
        GroupBox1.Controls.Add(Temp2)
        GroupBox1.Controls.Add(Temp3)
        GroupBox1.Controls.Add(PressName)
        GroupBox1.Controls.Add(pressureUnit)
        GroupBox1.Controls.Add(altitudeUnit)
        GroupBox1.Controls.Add(Press1)
        GroupBox1.Controls.Add(Press2)
        GroupBox1.Controls.Add(PartName)
        GroupBox1.Controls.Add(pmHeader1)
        GroupBox1.Controls.Add(pmHeader25)
        GroupBox1.Controls.Add(pmHeader4)
        GroupBox1.Controls.Add(pmHeader10)
        GroupBox1.Controls.Add(Pm1)
        GroupBox1.Controls.Add(Pm25)
        GroupBox1.Controls.Add(Pm5)
        GroupBox1.Controls.Add(Pm10)
        GroupBox1.Controls.Add(CO2Name)
        GroupBox1.Controls.Add(VOCName)
        GroupBox1.Controls.Add(RHName)
        GroupBox1.Controls.Add(Label2)
        GroupBox1.Controls.Add(VOC)
        GroupBox1.Controls.Add(RH)
        GroupBox1.Dock = DockStyle.Fill
        GroupBox1.Font = New Font("Segoe UI Semibold", 10.0F, FontStyle.Bold)
        GroupBox1.ForeColor = Color.FromArgb(CByte(33), CByte(49), CByte(64))
        GroupBox1.Location = New Point(0, 0)
        GroupBox1.Margin = New Padding(0, 0, 8, 0)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(959, 469)
        GroupBox1.TabIndex = 0
        GroupBox1.TabStop = False
        GroupBox1.Text = "  LIVE TELEMETRY  "
        ' 
        ' TempName
        ' 
        TempName.AutoSize = True
        TempName.Font = New Font("Segoe UI Semibold", 8.0F, FontStyle.Bold)
        TempName.ForeColor = Color.FromArgb(CByte(105), CByte(119), CByte(133))
        TempName.Location = New Point(28, 50)
        TempName.Name = "TempName"
        TempName.Size = New Size(103, 19)
        TempName.TabIndex = 0
        TempName.Text = "TEMPERATURE"
        ' 
        ' tempHeader1
        ' 
        tempHeader1.AutoSize = True
        tempHeader1.ForeColor = Color.FromArgb(CByte(130), CByte(143), CByte(156))
        tempHeader1.Location = New Point(220, 38)
        tempHeader1.Name = "tempHeader1"
        tempHeader1.Size = New Size(59, 23)
        tempHeader1.TabIndex = 1
        tempHeader1.Text = "SEN66"
        ' 
        ' tempHeader2
        ' 
        tempHeader2.AutoSize = True
        tempHeader2.ForeColor = Color.FromArgb(CByte(130), CByte(143), CByte(156))
        tempHeader2.Location = New Point(430, 38)
        tempHeader2.Name = "tempHeader2"
        tempHeader2.Size = New Size(73, 23)
        tempHeader2.TabIndex = 2
        tempHeader2.Text = "BMP280"
        ' 
        ' tempHeader3
        ' 
        tempHeader3.AutoSize = True
        tempHeader3.ForeColor = Color.FromArgb(CByte(130), CByte(143), CByte(156))
        tempHeader3.Location = New Point(640, 38)
        tempHeader3.Name = "tempHeader3"
        tempHeader3.Size = New Size(57, 23)
        tempHeader3.TabIndex = 3
        tempHeader3.Text = "SHT21"
        ' 
        ' Temp1
        ' 
        Temp1.Font = New Font("Segoe UI Semibold", 14.0F, FontStyle.Bold)
        Temp1.ForeColor = Color.FromArgb(CByte(25), CByte(42), CByte(58))
        Temp1.Location = New Point(220, 58)
        Temp1.Name = "Temp1"
        Temp1.Size = New Size(170, 34)
        Temp1.TabIndex = 4
        Temp1.Text = "--.- °C"
        ' 
        ' Temp2
        ' 
        Temp2.Font = New Font("Segoe UI Semibold", 14.0F, FontStyle.Bold)
        Temp2.ForeColor = Color.FromArgb(CByte(25), CByte(42), CByte(58))
        Temp2.Location = New Point(430, 58)
        Temp2.Name = "Temp2"
        Temp2.Size = New Size(170, 34)
        Temp2.TabIndex = 5
        Temp2.Text = "--.- °C"
        ' 
        ' Temp3
        ' 
        Temp3.Font = New Font("Segoe UI Semibold", 14.0F, FontStyle.Bold)
        Temp3.ForeColor = Color.FromArgb(CByte(25), CByte(42), CByte(58))
        Temp3.Location = New Point(640, 58)
        Temp3.Name = "Temp3"
        Temp3.Size = New Size(170, 34)
        Temp3.TabIndex = 6
        Temp3.Text = "--.- °C"
        ' 
        ' PressName
        ' 
        PressName.AutoSize = True
        PressName.Font = New Font("Segoe UI Semibold", 8.0F, FontStyle.Bold)
        PressName.ForeColor = Color.FromArgb(CByte(105), CByte(119), CByte(133))
        PressName.Location = New Point(28, 125)
        PressName.Name = "PressName"
        PressName.Size = New Size(151, 19)
        PressName.TabIndex = 7
        PressName.Text = "PRESSURE / ALTITUDE"
        ' 
        ' pressureUnit
        ' 
        pressureUnit.AutoSize = True
        pressureUnit.ForeColor = Color.FromArgb(CByte(130), CByte(143), CByte(156))
        pressureUnit.Location = New Point(220, 112)
        pressureUnit.Name = "pressureUnit"
        pressureUnit.Size = New Size(109, 23)
        pressureUnit.TabIndex = 8
        pressureUnit.Text = "BAROMETER"
        ' 
        ' altitudeUnit
        ' 
        altitudeUnit.AutoSize = True
        altitudeUnit.ForeColor = Color.FromArgb(CByte(130), CByte(143), CByte(156))
        altitudeUnit.Location = New Point(500, 112)
        altitudeUnit.Name = "altitudeUnit"
        altitudeUnit.Size = New Size(84, 23)
        altitudeUnit.TabIndex = 9
        altitudeUnit.Text = "ALTITUDE"
        ' 
        ' Press1
        ' 
        Press1.Font = New Font("Segoe UI Semibold", 14.0F, FontStyle.Bold)
        Press1.ForeColor = Color.FromArgb(CByte(25), CByte(42), CByte(58))
        Press1.Location = New Point(220, 132)
        Press1.Name = "Press1"
        Press1.Size = New Size(230, 34)
        Press1.TabIndex = 10
        Press1.Text = "----.-- hPa"
        ' 
        ' Press2
        ' 
        Press2.Font = New Font("Segoe UI Semibold", 14.0F, FontStyle.Bold)
        Press2.ForeColor = Color.FromArgb(CByte(25), CByte(42), CByte(58))
        Press2.Location = New Point(500, 132)
        Press2.Name = "Press2"
        Press2.Size = New Size(180, 34)
        Press2.TabIndex = 11
        Press2.Text = "---- m"
        ' 
        ' PartName
        ' 
        PartName.AutoSize = True
        PartName.Font = New Font("Segoe UI Semibold", 8.0F, FontStyle.Bold)
        PartName.ForeColor = Color.FromArgb(CByte(105), CByte(119), CByte(133))
        PartName.Location = New Point(28, 205)
        PartName.Name = "PartName"
        PartName.Size = New Size(152, 19)
        PartName.TabIndex = 12
        PartName.Text = "PARTICULATE MATTER"
        ' 
        ' pmHeader1
        ' 
        pmHeader1.AutoSize = True
        pmHeader1.ForeColor = Color.FromArgb(CByte(130), CByte(143), CByte(156))
        pmHeader1.Location = New Point(220, 190)
        pmHeader1.Name = "pmHeader1"
        pmHeader1.Size = New Size(56, 23)
        pmHeader1.TabIndex = 13
        pmHeader1.Text = "PM1.0"
        ' 
        ' pmHeader25
        ' 
        pmHeader25.AutoSize = True
        pmHeader25.ForeColor = Color.FromArgb(CByte(130), CByte(143), CByte(156))
        pmHeader25.Location = New Point(380, 190)
        pmHeader25.Name = "pmHeader25"
        pmHeader25.Size = New Size(58, 23)
        pmHeader25.TabIndex = 14
        pmHeader25.Text = "PM2.5"
        ' 
        ' pmHeader4
        ' 
        pmHeader4.AutoSize = True
        pmHeader4.ForeColor = Color.FromArgb(CByte(130), CByte(143), CByte(156))
        pmHeader4.Location = New Point(540, 190)
        pmHeader4.Name = "pmHeader4"
        pmHeader4.Size = New Size(59, 23)
        pmHeader4.TabIndex = 15
        pmHeader4.Text = "PM4.0"
        ' 
        ' pmHeader10
        ' 
        pmHeader10.AutoSize = True
        pmHeader10.ForeColor = Color.FromArgb(CByte(130), CByte(143), CByte(156))
        pmHeader10.Location = New Point(700, 190)
        pmHeader10.Name = "pmHeader10"
        pmHeader10.Size = New Size(52, 23)
        pmHeader10.TabIndex = 16
        pmHeader10.Text = "PM10"
        ' 
        ' Pm1
        ' 
        Pm1.Font = New Font("Segoe UI Semibold", 13.0F, FontStyle.Bold)
        Pm1.ForeColor = Color.FromArgb(CByte(25), CByte(42), CByte(58))
        Pm1.Location = New Point(220, 212)
        Pm1.Name = "Pm1"
        Pm1.Size = New Size(145, 34)
        Pm1.TabIndex = 17
        Pm1.Text = "--.- µg/m³"
        ' 
        ' Pm25
        ' 
        Pm25.Font = New Font("Segoe UI Semibold", 13.0F, FontStyle.Bold)
        Pm25.ForeColor = Color.FromArgb(CByte(25), CByte(42), CByte(58))
        Pm25.Location = New Point(380, 212)
        Pm25.Name = "Pm25"
        Pm25.Size = New Size(145, 34)
        Pm25.TabIndex = 18
        Pm25.Text = "--.- µg/m³"
        ' 
        ' Pm5
        ' 
        Pm5.Font = New Font("Segoe UI Semibold", 13.0F, FontStyle.Bold)
        Pm5.ForeColor = Color.FromArgb(CByte(25), CByte(42), CByte(58))
        Pm5.Location = New Point(540, 212)
        Pm5.Name = "Pm5"
        Pm5.Size = New Size(145, 34)
        Pm5.TabIndex = 19
        Pm5.Text = "--.- µg/m³"
        ' 
        ' Pm10
        ' 
        Pm10.Font = New Font("Segoe UI Semibold", 13.0F, FontStyle.Bold)
        Pm10.ForeColor = Color.FromArgb(CByte(25), CByte(42), CByte(58))
        Pm10.Location = New Point(700, 212)
        Pm10.Name = "Pm10"
        Pm10.Size = New Size(145, 34)
        Pm10.TabIndex = 20
        Pm10.Text = "--.- µg/m³"
        ' 
        ' CO2Name
        ' 
        CO2Name.AutoSize = True
        CO2Name.Font = New Font("Segoe UI Semibold", 8.0F, FontStyle.Bold)
        CO2Name.ForeColor = Color.FromArgb(CByte(105), CByte(119), CByte(133))
        CO2Name.Location = New Point(28, 300)
        CO2Name.Name = "CO2Name"
        CO2Name.Size = New Size(34, 19)
        CO2Name.TabIndex = 21
        CO2Name.Text = "CO₂"
        ' 
        ' VOCName
        ' 
        VOCName.AutoSize = True
        VOCName.Font = New Font("Segoe UI Semibold", 8.0F, FontStyle.Bold)
        VOCName.ForeColor = Color.FromArgb(CByte(105), CByte(119), CByte(133))
        VOCName.Location = New Point(330, 300)
        VOCName.Name = "VOCName"
        VOCName.Size = New Size(83, 19)
        VOCName.TabIndex = 22
        VOCName.Text = "VOC INDEX"
        ' 
        ' RHName
        ' 
        RHName.AutoSize = True
        RHName.Font = New Font("Segoe UI Semibold", 8.0F, FontStyle.Bold)
        RHName.ForeColor = Color.FromArgb(CByte(105), CByte(119), CByte(133))
        RHName.Location = New Point(610, 300)
        RHName.Name = "RHName"
        RHName.Size = New Size(106, 19)
        RHName.TabIndex = 23
        RHName.Text = "REL. HUMIDITY"
        ' 
        ' Label2
        ' 
        Label2.Font = New Font("Segoe UI Semibold", 17.0F, FontStyle.Bold)
        Label2.ForeColor = Color.FromArgb(CByte(0), CByte(102), CByte(179))
        Label2.Location = New Point(28, 325)
        Label2.Name = "Label2"
        Label2.Size = New Size(230, 42)
        Label2.TabIndex = 24
        Label2.Text = "---- ppm"
        ' 
        ' VOC
        ' 
        VOC.Font = New Font("Segoe UI Semibold", 17.0F, FontStyle.Bold)
        VOC.ForeColor = Color.FromArgb(CByte(0), CByte(102), CByte(179))
        VOC.Location = New Point(330, 325)
        VOC.Name = "VOC"
        VOC.Size = New Size(200, 42)
        VOC.TabIndex = 25
        VOC.Text = "---.-"
        ' 
        ' RH
        ' 
        RH.Font = New Font("Segoe UI Semibold", 17.0F, FontStyle.Bold)
        RH.ForeColor = Color.FromArgb(CByte(0), CByte(102), CByte(179))
        RH.Location = New Point(610, 325)
        RH.Name = "RH"
        RH.Size = New Size(220, 42)
        RH.TabIndex = 26
        RH.Text = "--.- %RH"
        ' 
        ' rightLayout
        ' 
        rightLayout.ColumnCount = 1
        rightLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        rightLayout.Controls.Add(GPSBox, 0, 0)
        rightLayout.Controls.Add(statusGroup, 0, 1)
        rightLayout.Dock = DockStyle.Fill
        rightLayout.Location = New Point(975, 0)
        rightLayout.Margin = New Padding(8, 0, 0, 0)
        rightLayout.Name = "rightLayout"
        rightLayout.RowCount = 2
        rightLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 34.0F))
        rightLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 66.0F))
        rightLayout.Size = New Size(491, 469)
        rightLayout.TabIndex = 1
        ' 
        ' GPSBox
        ' 
        GPSBox.BackColor = Color.White
        GPSBox.Controls.Add(XName)
        GPSBox.Controls.Add(X)
        GPSBox.Controls.Add(YName)
        GPSBox.Controls.Add(Y)
        GPSBox.Controls.Add(ZName)
        GPSBox.Controls.Add(Z)
        GPSBox.Dock = DockStyle.Fill
        GPSBox.Font = New Font("Segoe UI Semibold", 10.0F, FontStyle.Bold)
        GPSBox.ForeColor = Color.FromArgb(CByte(33), CByte(49), CByte(64))
        GPSBox.Location = New Point(0, 0)
        GPSBox.Margin = New Padding(0, 0, 0, 8)
        GPSBox.Name = "GPSBox"
        GPSBox.Size = New Size(491, 151)
        GPSBox.TabIndex = 0
        GPSBox.TabStop = False
        GPSBox.Text = "  GPS / POSITION  "
        ' 
        ' XName
        ' 
        XName.AutoSize = True
        XName.ForeColor = Color.FromArgb(CByte(105), CByte(119), CByte(133))
        XName.Location = New Point(24, 42)
        XName.Name = "XName"
        XName.Size = New Size(84, 23)
        XName.TabIndex = 0
        XName.Text = "LATITUDE"
        ' 
        ' X
        ' 
        X.Font = New Font("Segoe UI Semibold", 14.0F, FontStyle.Bold)
        X.ForeColor = Color.FromArgb(CByte(185), CByte(62), CByte(62))
        X.Location = New Point(24, 66)
        X.Name = "X"
        X.Size = New Size(210, 40)
        X.TabIndex = 1
        X.Text = "NO FIX"
        ' 
        ' YName
        ' 
        YName.AutoSize = True
        YName.ForeColor = Color.FromArgb(CByte(105), CByte(119), CByte(133))
        YName.Location = New Point(255, 42)
        YName.Name = "YName"
        YName.Size = New Size(102, 23)
        YName.TabIndex = 2
        YName.Text = "LONGITUDE"
        ' 
        ' Y
        ' 
        Y.Font = New Font("Segoe UI Semibold", 14.0F, FontStyle.Bold)
        Y.ForeColor = Color.FromArgb(CByte(185), CByte(62), CByte(62))
        Y.Location = New Point(255, 66)
        Y.Name = "Y"
        Y.Size = New Size(220, 40)
        Y.TabIndex = 3
        Y.Text = "NO FIX"
        ' 
        ' ZName
        ' 
        ZName.AutoSize = True
        ZName.ForeColor = Color.FromArgb(CByte(105), CByte(119), CByte(133))
        ZName.Location = New Point(24, 122)
        ZName.Name = "ZName"
        ZName.Size = New Size(186, 23)
        ZName.TabIndex = 4
        ZName.Text = "ALTITUDE / SATELLITES"
        ' 
        ' Z
        ' 
        Z.Font = New Font("Segoe UI Semibold", 14.0F, FontStyle.Bold)
        Z.ForeColor = Color.FromArgb(CByte(105), CByte(119), CByte(133))
        Z.Location = New Point(24, 146)
        Z.Name = "Z"
        Z.Size = New Size(450, 36)
        Z.TabIndex = 5
        Z.Text = "NO DATA"
        ' 
        ' consolePanel
        ' 
        consolePanel.BackColor = Color.FromArgb(CByte(20), CByte(27), CByte(35))
        consolePanel.Controls.Add(consoleTitle)
        consolePanel.Controls.Add(outputConsole)
        consolePanel.Dock = DockStyle.Fill
        consolePanel.Location = New Point(3, 673)
        consolePanel.Name = "consolePanel"
        consolePanel.Padding = New Padding(18, 8, 18, 14)
        consolePanel.Size = New Size(1494, 224)
        consolePanel.TabIndex = 2
        ' 
        ' consoleTitle
        ' 
        consoleTitle.AutoSize = True
        consoleTitle.Font = New Font("Segoe UI Semibold", 9.0F, FontStyle.Bold)
        consoleTitle.ForeColor = Color.FromArgb(CByte(150), CByte(165), CByte(180))
        consoleTitle.Location = New Point(18, 8)
        consoleTitle.Name = "consoleTitle"
        consoleTitle.Size = New Size(238, 20)
        consoleTitle.TabIndex = 0
        consoleTitle.Text = "COMMUNICATION / SYSTEM LOG"
        ' 
        ' outputConsole
        ' 
        outputConsole.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        outputConsole.BackColor = Color.FromArgb(CByte(14), CByte(20), CByte(27))
        outputConsole.BorderStyle = BorderStyle.None
        outputConsole.DetectUrls = False
        outputConsole.Font = New Font("Cascadia Mono", 9.5F)
        outputConsole.ForeColor = Color.FromArgb(CByte(111), CByte(230), CByte(149))
        outputConsole.Location = New Point(18, 34)
        outputConsole.Name = "outputConsole"
        outputConsole.ReadOnly = True
        outputConsole.Size = New Size(2744, 302)
        outputConsole.TabIndex = 1
        outputConsole.Text = ""
        outputConsole.WordWrap = False
        ' 
        ' Press3
        ' 
        Press3.Location = New Point(0, 0)
        Press3.Name = "Press3"
        Press3.Size = New Size(100, 23)
        Press3.TabIndex = 0
        Press3.Visible = False
        ' 
        ' Label1
        ' 
        Label1.Location = New Point(0, 0)
        Label1.Name = "Label1"
        Label1.Size = New Size(100, 23)
        Label1.TabIndex = 0
        Label1.Visible = False
        ' 
        ' _statusTool
        ' 
        _statusTool.BackColor = Color.FromArgb(CByte(245), CByte(247), CByte(250))
        _statusTool.Dock = DockStyle.Fill
        _statusTool.Location = New Point(8, 43)
        _statusTool.Margin = New Padding(0)
        _statusTool.MinimumSize = New Size(300, 300)
        _statusTool.Name = "_statusTool"
        _statusTool.Padding = New Padding(18)
        _statusTool.Size = New Size(475, 300)
        _statusTool.TabIndex = 0
        ' 
        ' statusGroup
        ' 
        statusGroup.BackColor = Color.White
        statusGroup.Controls.Add(_statusTool)
        statusGroup.Dock = DockStyle.Fill
        statusGroup.Font = New Font("Segoe UI Semibold", 10.0F, FontStyle.Bold)
        statusGroup.ForeColor = Color.FromArgb(CByte(33), CByte(49), CByte(64))
        statusGroup.Location = New Point(0, 167)
        statusGroup.Margin = New Padding(0, 8, 0, 0)
        statusGroup.Name = "statusGroup"
        statusGroup.Padding = New Padding(8, 20, 8, 8)
        statusGroup.Size = New Size(491, 302)
        statusGroup.TabIndex = 1
        statusGroup.TabStop = False
        statusGroup.Text = "  SYSTEM STATUS  "
        ' 
        ' ErmisMonitorForm
        ' 
        AutoScaleDimensions = New SizeF(8.0F, 20.0F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(242), CByte(245), CByte(249))
        ClientSize = New Size(1500, 900)
        Controls.Add(mainLayout)
        Font = New Font("Segoe UI", 9.0F)
        MinimumSize = New Size(1180, 760)
        Name = "ErmisMonitorForm"
        StartPosition = FormStartPosition.CenterScreen
        Text = "ERMIS II | Telemetry Control Center"
        mainLayout.ResumeLayout(False)
        topArea.ResumeLayout(False)
        connectionPanel.ResumeLayout(False)
        connectionPanel.PerformLayout()
        CType(baudBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(slaveBox, System.ComponentModel.ISupportInitialize).EndInit()
        headerPanel.ResumeLayout(False)
        headerPanel.PerformLayout()
        dashboardPanel.ResumeLayout(False)
        contentLayout.ResumeLayout(False)
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        rightLayout.ResumeLayout(False)
        GPSBox.ResumeLayout(False)
        GPSBox.PerformLayout()
        consolePanel.ResumeLayout(False)
        consolePanel.PerformLayout()
        statusGroup.ResumeLayout(False)
        ResumeLayout(False)

    End Sub

    Friend WithEvents statusGroup As GroupBox
    Friend WithEvents _statusTool As SensorStatusPanel

End Class
