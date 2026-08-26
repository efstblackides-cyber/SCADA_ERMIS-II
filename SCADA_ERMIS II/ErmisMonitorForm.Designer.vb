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
    Friend WithEvents statusGroup As GroupBox
    Friend WithEvents _statusTool As SensorStatusPanel

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

    Friend WithEvents missionGroup As GroupBox
    Friend WithEvents startMeasurementButton As Button
    Friend WithEvents stopMeasurementButton As Button
    Friend WithEvents singleMeasurementButton As Button
    Friend WithEvents clearBufferButton As Button
    Friend WithEvents startSpectrumButton As Button
    Friend WithEvents stopSpectrumButton As Button
    Friend WithEvents clearSpectrumButton As Button
    Friend WithEvents resetAcquisitionButton As Button
    Friend WithEvents liveSpectrogram As LiveSpectrogramControl
    Friend WithEvents spectrumStatusLabel As Label

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    Private Sub InitializeComponent()

        components = New System.ComponentModel.Container()

        mainLayout = New TableLayoutPanel()

        topArea = New Panel()
        headerPanel = New Panel()
        titleLabel = New Label()
        subtitleLabel = New Label()

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

        dashboardPanel = New Panel()

        missionGroup = New GroupBox()
        startMeasurementButton = New Button()
        stopMeasurementButton = New Button()
        singleMeasurementButton = New Button()
        clearBufferButton = New Button()
        startSpectrumButton = New Button()
        stopSpectrumButton = New Button()
        clearSpectrumButton = New Button()
        resetAcquisitionButton = New Button()
        liveSpectrogram = New LiveSpectrogramControl()
        spectrumStatusLabel = New Label()
        contentLayout = New TableLayoutPanel()

        GroupBox1 = New GroupBox()
        rightLayout = New TableLayoutPanel()
        GPSBox = New GroupBox()
        statusGroup = New GroupBox()
        _statusTool = New SensorStatusPanel()

        TempName = New Label()
        Temp1 = New Label()
        Temp2 = New Label()
        Temp3 = New Label()
        tempHeader1 = New Label()
        tempHeader2 = New Label()
        tempHeader3 = New Label()

        PressName = New Label()
        Press1 = New Label()
        Press2 = New Label()
        Press3 = New Label()
        pressureUnit = New Label()
        altitudeUnit = New Label()

        PartName = New Label()
        Pm1 = New Label()
        Pm25 = New Label()
        Pm5 = New Label()
        Pm10 = New Label()
        pmHeader1 = New Label()
        pmHeader25 = New Label()
        pmHeader4 = New Label()
        pmHeader10 = New Label()

        CO2Name = New Label()
        Label2 = New Label()
        VOCName = New Label()
        VOC = New Label()
        RHName = New Label()
        RH = New Label()

        XName = New Label()
        X = New Label()
        YName = New Label()
        Y = New Label()
        ZName = New Label()
        Z = New Label()

        consolePanel = New Panel()
        consoleTitle = New Label()
        outputConsole = New RichTextBox()

        Label1 = New Label()

        CType(baudBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(slaveBox, System.ComponentModel.ISupportInitialize).BeginInit()

        SuspendLayout()

        ' ============================================================
        ' FORM
        ' ============================================================
        AutoScaleDimensions = New SizeF(8.0F, 20.0F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(242, 245, 249)
        ClientSize = New Size(1500, 900)
        MinimumSize = New Size(1180, 760)
        Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        StartPosition = FormStartPosition.CenterScreen
        Text = "ERMIS II | Telemetry Control Center"

        ' ============================================================
        ' MAIN LAYOUT
        ' ============================================================
        mainLayout.ColumnCount = 1
        mainLayout.RowCount = 4
        mainLayout.Dock = DockStyle.Fill
        mainLayout.Margin = New Padding(0)
        mainLayout.Padding = New Padding(0)
        mainLayout.BackColor = Color.FromArgb(242, 245, 249)
        mainLayout.ColumnStyles.Add(
            New ColumnStyle(SizeType.Percent, 100.0F))
        mainLayout.RowStyles.Add(
            New RowStyle(SizeType.Absolute, 175.0F))
        mainLayout.RowStyles.Add(
            New RowStyle(SizeType.Absolute, 160.0F))
        mainLayout.RowStyles.Add(
            New RowStyle(SizeType.Percent, 100.0F))
        mainLayout.RowStyles.Add(
            New RowStyle(SizeType.Absolute, 230.0F))

        ' ============================================================
        ' TOP AREA
        ' ============================================================
        topArea.Dock = DockStyle.Fill
        topArea.BackColor = Color.FromArgb(242, 245, 249)
        topArea.Padding = New Padding(14, 12, 14, 8)

        headerPanel.BackColor = Color.FromArgb(20, 32, 48)
        headerPanel.Dock = DockStyle.Top
        headerPanel.Height = 68

        titleLabel.AutoSize = True
        titleLabel.ForeColor = Color.White
        titleLabel.Font = New Font(
            "Segoe UI Semibold",
            17.0F,
            FontStyle.Bold)
        titleLabel.Location = New Point(22, 8)
        titleLabel.Text = "ERMIS II"

        subtitleLabel.AutoSize = True
        subtitleLabel.ForeColor = Color.FromArgb(173, 190, 207)
        subtitleLabel.Font = New Font("Segoe UI", 9.0F)
        subtitleLabel.Location = New Point(24, 42)
        subtitleLabel.Text =
            "LoRa Modbus Telemetry · Environmental & Flight Monitoring"

        headerPanel.Controls.Add(titleLabel)
        headerPanel.Controls.Add(subtitleLabel)

        connectionPanel.BackColor = Color.White
        connectionPanel.Dock = DockStyle.Fill

        portLabel.AutoSize = True
        portLabel.Font = New Font("Segoe UI Semibold", 8.5F)
        portLabel.ForeColor = Color.FromArgb(92, 108, 124)
        portLabel.Location = New Point(18, 8)
        portLabel.Text = "COM PORT"

        comList.Font = New Font("Segoe UI", 9.5F)
        comList.BorderStyle = BorderStyle.FixedSingle
        comList.Location = New Point(18, 30)
        comList.Size = New Size(142, 44)

        refreshButton.FlatStyle = FlatStyle.Flat
        refreshButton.FlatAppearance.BorderColor =
            Color.FromArgb(210, 218, 226)
        refreshButton.BackColor = Color.White
        refreshButton.ForeColor = Color.FromArgb(45, 60, 75)
        refreshButton.Font = New Font(
            "Segoe UI Semibold",
            9.0F)
        refreshButton.Location = New Point(170, 30)
        refreshButton.Size = New Size(96, 34)
        refreshButton.Text = "Refresh"

        connectButton.FlatStyle = FlatStyle.Flat
        connectButton.FlatAppearance.BorderSize = 0
        connectButton.BackColor = Color.FromArgb(0, 122, 204)
        connectButton.ForeColor = Color.White
        connectButton.Font = New Font(
            "Segoe UI Semibold",
            9.5F,
            FontStyle.Bold)
        connectButton.Location = New Point(276, 30)
        connectButton.Size = New Size(116, 34)
        connectButton.Text = "Connect"

        baudLabel.AutoSize = True
        baudLabel.Font = New Font("Segoe UI Semibold", 8.5F)
        baudLabel.ForeColor = Color.FromArgb(92, 108, 124)
        baudLabel.Location = New Point(418, 8)
        baudLabel.Text = "BAUD RATE"

        baudBox.BorderStyle = BorderStyle.FixedSingle
        baudBox.Increment =
            New Decimal(New Integer() {9600, 0, 0, 0})
        baudBox.Maximum =
            New Decimal(New Integer() {921600, 0, 0, 0})
        baudBox.Minimum =
            New Decimal(New Integer() {1200, 0, 0, 0})
        baudBox.Location = New Point(418, 31)
        baudBox.Size = New Size(116, 27)
        baudBox.Value =
            New Decimal(New Integer() {115200, 0, 0, 0})

        slaveLabel.AutoSize = True
        slaveLabel.Font = New Font("Segoe UI Semibold", 8.5F)
        slaveLabel.ForeColor = Color.FromArgb(92, 108, 124)
        slaveLabel.Location = New Point(554, 8)
        slaveLabel.Text = "SLAVE ID"

        slaveBox.BorderStyle = BorderStyle.FixedSingle
        slaveBox.Maximum =
            New Decimal(New Integer() {247, 0, 0, 0})
        slaveBox.Minimum =
            New Decimal(New Integer() {1, 0, 0, 0})
        slaveBox.Location = New Point(554, 31)
        slaveBox.Size = New Size(76, 27)
        slaveBox.Value =
            New Decimal(New Integer() {1, 0, 0, 0})

        autoAckBox.AutoSize = True
        autoAckBox.Checked = True
        autoAckBox.CheckState = CheckState.Checked
        autoAckBox.Font = New Font("Segoe UI", 9.0F)
        autoAckBox.ForeColor = Color.FromArgb(45, 60, 75)
        autoAckBox.Location = New Point(654, 33)
        autoAckBox.Text = "Auto ACK"

        clearButton.FlatStyle = FlatStyle.Flat
        clearButton.FlatAppearance.BorderColor =
            Color.FromArgb(210, 218, 226)
        clearButton.BackColor = Color.White
        clearButton.ForeColor = Color.FromArgb(45, 60, 75)
        clearButton.Font = New Font(
            "Segoe UI Semibold",
            9.0F)
        clearButton.Location = New Point(768, 30)
        clearButton.Size = New Size(96, 34)
        clearButton.Text = "Clear log"

        statusDot.AutoSize = False
        statusDot.BackColor = Color.FromArgb(145, 155, 165)
        statusDot.Location = New Point(894, 38)
        statusDot.Size = New Size(10, 10)

        statusLabel.AutoSize = True
        statusLabel.Font = New Font(
            "Segoe UI Semibold",
            9.0F)
        statusLabel.ForeColor = Color.FromArgb(69, 82, 95)
        statusLabel.Location = New Point(914, 32)
        statusLabel.Text = "OFFLINE"

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

        topArea.Controls.Add(connectionPanel)
        topArea.Controls.Add(headerPanel)

        ' ============================================================
        ' DASHBOARD HOST
        ' ============================================================
        dashboardPanel.Dock = DockStyle.Fill
        dashboardPanel.BackColor = Color.FromArgb(242, 245, 249)
        dashboardPanel.Padding = New Padding(14, 10, 14, 10)

        contentLayout.ColumnCount = 2
        contentLayout.RowCount = 1
        contentLayout.Dock = DockStyle.Fill
        contentLayout.BackColor = Color.FromArgb(242, 245, 249)
        contentLayout.Padding = New Padding(0)
        contentLayout.Margin = New Padding(0)
        contentLayout.ColumnStyles.Add(
            New ColumnStyle(SizeType.Percent, 66.0F))
        contentLayout.ColumnStyles.Add(
            New ColumnStyle(SizeType.Percent, 34.0F))
        contentLayout.RowStyles.Add(
            New RowStyle(SizeType.Percent, 100.0F))

        ' ============================================================
        ' LIVE TELEMETRY
        ' ============================================================
        GroupBox1.Dock = DockStyle.Fill
        GroupBox1.Margin = New Padding(0, 0, 8, 0)
        GroupBox1.BackColor = Color.White
        GroupBox1.ForeColor = Color.FromArgb(33, 49, 64)
        GroupBox1.Font = New Font(
            "Segoe UI Semibold",
            10.0F,
            FontStyle.Bold)
        GroupBox1.Text = "  LIVE TELEMETRY  "

        TempName.AutoSize = True
        TempName.Text = "TEMPERATURE"
        TempName.Location = New Point(28, 50)
        TempName.Font = New Font(
            "Segoe UI Semibold",
            8.0F,
            FontStyle.Bold)
        TempName.ForeColor = Color.FromArgb(105, 119, 133)

        tempHeader1.AutoSize = True
        tempHeader1.Text = "SEN66"
        tempHeader1.Location = New Point(220, 38)
        tempHeader1.ForeColor = Color.FromArgb(130, 143, 156)

        tempHeader2.AutoSize = True
        tempHeader2.Text = "BMP280"
        tempHeader2.Location = New Point(430, 38)
        tempHeader2.ForeColor = Color.FromArgb(130, 143, 156)

        tempHeader3.AutoSize = True
        tempHeader3.Text = "SHT21"
        tempHeader3.Location = New Point(640, 38)
        tempHeader3.ForeColor = Color.FromArgb(130, 143, 156)

        Temp1.Text = "--.- °C"
        Temp1.Location = New Point(220, 58)
        Temp1.Size = New Size(170, 34)
        Temp1.Font = New Font(
            "Segoe UI Semibold",
            14.0F,
            FontStyle.Bold)
        Temp1.ForeColor = Color.FromArgb(25, 42, 58)

        Temp2.Text = "--.- °C"
        Temp2.Location = New Point(430, 58)
        Temp2.Size = New Size(170, 34)
        Temp2.Font = New Font(
            "Segoe UI Semibold",
            14.0F,
            FontStyle.Bold)
        Temp2.ForeColor = Color.FromArgb(25, 42, 58)

        Temp3.Text = "--.- °C"
        Temp3.Location = New Point(640, 58)
        Temp3.Size = New Size(170, 34)
        Temp3.Font = New Font(
            "Segoe UI Semibold",
            14.0F,
            FontStyle.Bold)
        Temp3.ForeColor = Color.FromArgb(25, 42, 58)

        PressName.AutoSize = True
        PressName.Text = "PRESSURE / ALTITUDE"
        PressName.Location = New Point(28, 125)
        PressName.Font = New Font(
            "Segoe UI Semibold",
            8.0F,
            FontStyle.Bold)
        PressName.ForeColor = Color.FromArgb(105, 119, 133)

        pressureUnit.AutoSize = True
        pressureUnit.Text = "BAROMETER"
        pressureUnit.Location = New Point(220, 112)
        pressureUnit.ForeColor = Color.FromArgb(130, 143, 156)

        altitudeUnit.AutoSize = True
        altitudeUnit.Text = "ALTITUDE"
        altitudeUnit.Location = New Point(500, 112)
        altitudeUnit.ForeColor = Color.FromArgb(130, 143, 156)

        Press1.Text = "----.-- hPa"
        Press1.Location = New Point(220, 132)
        Press1.Size = New Size(230, 34)
        Press1.Font = New Font(
            "Segoe UI Semibold",
            14.0F,
            FontStyle.Bold)
        Press1.ForeColor = Color.FromArgb(25, 42, 58)

        Press2.Text = "---- m"
        Press2.Location = New Point(500, 132)
        Press2.Size = New Size(180, 34)
        Press2.Font = New Font(
            "Segoe UI Semibold",
            14.0F,
            FontStyle.Bold)
        Press2.ForeColor = Color.FromArgb(25, 42, 58)

        Press3.Visible = False

        PartName.AutoSize = True
        PartName.Text = "PARTICULATE MATTER"
        PartName.Location = New Point(28, 205)
        PartName.Font = New Font(
            "Segoe UI Semibold",
            8.0F,
            FontStyle.Bold)
        PartName.ForeColor = Color.FromArgb(105, 119, 133)

        pmHeader1.AutoSize = True
        pmHeader1.Text = "PM1.0"
        pmHeader1.Location = New Point(220, 190)
        pmHeader1.ForeColor = Color.FromArgb(130, 143, 156)

        pmHeader25.AutoSize = True
        pmHeader25.Text = "PM2.5"
        pmHeader25.Location = New Point(380, 190)
        pmHeader25.ForeColor = Color.FromArgb(130, 143, 156)

        pmHeader4.AutoSize = True
        pmHeader4.Text = "PM4.0"
        pmHeader4.Location = New Point(540, 190)
        pmHeader4.ForeColor = Color.FromArgb(130, 143, 156)

        pmHeader10.AutoSize = True
        pmHeader10.Text = "PM10"
        pmHeader10.Location = New Point(700, 190)
        pmHeader10.ForeColor = Color.FromArgb(130, 143, 156)

        Pm1.Text = "--.- µg/m³"
        Pm1.Location = New Point(220, 212)
        Pm1.Size = New Size(145, 34)
        Pm1.Font = New Font(
            "Segoe UI Semibold",
            13.0F,
            FontStyle.Bold)
        Pm1.ForeColor = Color.FromArgb(25, 42, 58)

        Pm25.Text = "--.- µg/m³"
        Pm25.Location = New Point(380, 212)
        Pm25.Size = New Size(145, 34)
        Pm25.Font = New Font(
            "Segoe UI Semibold",
            13.0F,
            FontStyle.Bold)
        Pm25.ForeColor = Color.FromArgb(25, 42, 58)

        Pm5.Text = "--.- µg/m³"
        Pm5.Location = New Point(540, 212)
        Pm5.Size = New Size(145, 34)
        Pm5.Font = New Font(
            "Segoe UI Semibold",
            13.0F,
            FontStyle.Bold)
        Pm5.ForeColor = Color.FromArgb(25, 42, 58)

        Pm10.Text = "--.- µg/m³"
        Pm10.Location = New Point(700, 212)
        Pm10.Size = New Size(145, 34)
        Pm10.Font = New Font(
            "Segoe UI Semibold",
            13.0F,
            FontStyle.Bold)
        Pm10.ForeColor = Color.FromArgb(25, 42, 58)

        CO2Name.AutoSize = True
        CO2Name.Text = "CO₂"
        CO2Name.Location = New Point(28, 300)
        CO2Name.Font = New Font(
            "Segoe UI Semibold",
            8.0F,
            FontStyle.Bold)
        CO2Name.ForeColor = Color.FromArgb(105, 119, 133)

        VOCName.AutoSize = True
        VOCName.Text = "VOC INDEX"
        VOCName.Location = New Point(330, 300)
        VOCName.Font = New Font(
            "Segoe UI Semibold",
            8.0F,
            FontStyle.Bold)
        VOCName.ForeColor = Color.FromArgb(105, 119, 133)

        RHName.AutoSize = True
        RHName.Text = "REL. HUMIDITY"
        RHName.Location = New Point(610, 300)
        RHName.Font = New Font(
            "Segoe UI Semibold",
            8.0F,
            FontStyle.Bold)
        RHName.ForeColor = Color.FromArgb(105, 119, 133)

        Label2.Text = "---- ppm"
        Label2.Location = New Point(28, 325)
        Label2.Size = New Size(230, 42)
        Label2.Font = New Font(
            "Segoe UI Semibold",
            17.0F,
            FontStyle.Bold)
        Label2.ForeColor = Color.FromArgb(0, 102, 179)

        VOC.Text = "---.-"
        VOC.Location = New Point(330, 325)
        VOC.Size = New Size(200, 42)
        VOC.Font = New Font(
            "Segoe UI Semibold",
            17.0F,
            FontStyle.Bold)
        VOC.ForeColor = Color.FromArgb(0, 102, 179)

        RH.Text = "--.- %RH"
        RH.Location = New Point(610, 325)
        RH.Size = New Size(220, 42)
        RH.Font = New Font(
            "Segoe UI Semibold",
            17.0F,
            FontStyle.Bold)
        RH.ForeColor = Color.FromArgb(0, 102, 179)

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

        ' ============================================================
        ' RIGHT SIDE LAYOUT
        ' ============================================================
        rightLayout.ColumnCount = 1
        rightLayout.RowCount = 2
        rightLayout.Dock = DockStyle.Fill
        rightLayout.Margin = New Padding(8, 0, 0, 0)
        rightLayout.ColumnStyles.Add(
            New ColumnStyle(SizeType.Percent, 100.0F))
        rightLayout.RowStyles.Add(
            New RowStyle(SizeType.Percent, 34.0F))
        rightLayout.RowStyles.Add(
            New RowStyle(SizeType.Percent, 66.0F))

        ' ============================================================
        ' GPS
        ' ============================================================
        GPSBox.Dock = DockStyle.Fill
        GPSBox.Margin = New Padding(0, 0, 0, 8)
        GPSBox.BackColor = Color.White
        GPSBox.ForeColor = Color.FromArgb(33, 49, 64)
        GPSBox.Font = New Font(
            "Segoe UI Semibold",
            10.0F,
            FontStyle.Bold)
        GPSBox.Text = "  GPS / POSITION  "

        XName.AutoSize = True
        XName.Text = "LATITUDE"
        XName.Location = New Point(24, 42)
        XName.ForeColor = Color.FromArgb(105, 119, 133)

        X.Text = "NO FIX"
        X.Location = New Point(24, 66)
        X.Size = New Size(210, 40)
        X.Font = New Font(
            "Segoe UI Semibold",
            14.0F,
            FontStyle.Bold)
        X.ForeColor = Color.FromArgb(185, 62, 62)

        YName.AutoSize = True
        YName.Text = "LONGITUDE"
        YName.Location = New Point(255, 42)
        YName.ForeColor = Color.FromArgb(105, 119, 133)

        Y.Text = "NO FIX"
        Y.Location = New Point(255, 66)
        Y.Size = New Size(220, 40)
        Y.Font = New Font(
            "Segoe UI Semibold",
            14.0F,
            FontStyle.Bold)
        Y.ForeColor = Color.FromArgb(185, 62, 62)

        ZName.AutoSize = True
        ZName.Text = "ALTITUDE / SATELLITES"
        ZName.Location = New Point(24, 122)
        ZName.ForeColor = Color.FromArgb(105, 119, 133)

        Z.Text = "NO DATA"
        Z.Location = New Point(24, 146)
        Z.Size = New Size(450, 36)
        Z.Font = New Font(
            "Segoe UI Semibold",
            14.0F,
            FontStyle.Bold)
        Z.ForeColor = Color.FromArgb(105, 119, 133)

        GPSBox.Controls.Add(XName)
        GPSBox.Controls.Add(X)
        GPSBox.Controls.Add(YName)
        GPSBox.Controls.Add(Y)
        GPSBox.Controls.Add(ZName)
        GPSBox.Controls.Add(Z)

        ' ============================================================
        ' SYSTEM STATUS
        ' ============================================================
        statusGroup.Dock = DockStyle.Fill
        statusGroup.Margin = New Padding(0, 8, 0, 0)
        statusGroup.BackColor = Color.White
        statusGroup.ForeColor = Color.FromArgb(33, 49, 64)
        statusGroup.Font = New Font(
            "Segoe UI Semibold",
            10.0F,
            FontStyle.Bold)
        statusGroup.Text = "  ON-BOARD STATUS  "
        statusGroup.Padding = New Padding(8, 22, 8, 8)

        _statusTool.Dock = DockStyle.Fill
        _statusTool.Margin = New Padding(0)
        _statusTool.BackColor = Color.FromArgb(245, 247, 250)

        statusGroup.Controls.Add(_statusTool)

        rightLayout.Controls.Add(GPSBox, 0, 0)
        rightLayout.Controls.Add(statusGroup, 0, 1)

        contentLayout.Controls.Add(GroupBox1, 0, 0)
        contentLayout.Controls.Add(rightLayout, 1, 0)

        dashboardPanel.Controls.Add(contentLayout)


        ' ============================================================
        ' MISSION CONTROL + LIVE SPECTROGRAM
        ' ============================================================
        missionGroup.Dock = DockStyle.Fill
        missionGroup.Margin = New Padding(14, 4, 14, 4)
        missionGroup.BackColor = Color.White
        missionGroup.ForeColor = Color.FromArgb(33, 49, 64)
        missionGroup.Font = New Font(
            "Segoe UI Semibold",
            10.0F,
            FontStyle.Bold)
        missionGroup.Text = "  MISSION CONTROL  "
        missionGroup.Padding = New Padding(12)

        startMeasurementButton.Text = "START MEASUREMENT"
        startMeasurementButton.Location = New Point(18, 34)
        startMeasurementButton.Size = New Size(155, 34)

        stopMeasurementButton.Text = "STOP MEASUREMENT"
        stopMeasurementButton.Location = New Point(181, 34)
        stopMeasurementButton.Size = New Size(155, 34)

        singleMeasurementButton.Text = "SINGLE SAMPLE"
        singleMeasurementButton.Location = New Point(344, 34)
        singleMeasurementButton.Size = New Size(130, 34)

        clearBufferButton.Text = "CLEAR BUFFER"
        clearBufferButton.Location = New Point(482, 34)
        clearBufferButton.Size = New Size(120, 34)

        startSpectrumButton.Text = "START SPECTRUM"
        startSpectrumButton.Location = New Point(18, 80)
        startSpectrumButton.Size = New Size(145, 34)

        stopSpectrumButton.Text = "STOP SPECTRUM"
        stopSpectrumButton.Location = New Point(171, 80)
        stopSpectrumButton.Size = New Size(145, 34)

        clearSpectrumButton.Text = "CLEAR SPECTRUM"
        clearSpectrumButton.Location = New Point(324, 80)
        clearSpectrumButton.Size = New Size(145, 34)

        resetAcquisitionButton.Text = "RESET"
        resetAcquisitionButton.Location = New Point(477, 80)
        resetAcquisitionButton.Size = New Size(90, 34)

        spectrumStatusLabel.AutoSize = True
        spectrumStatusLabel.Location = New Point(18, 122)
        spectrumStatusLabel.ForeColor = Color.FromArgb(105, 119, 133)
        spectrumStatusLabel.Text = "SPECTRUM STOPPED"

        liveSpectrogram.Location = New Point(625, 28)
        liveSpectrogram.Size = New Size(815, 115)
        liveSpectrogram.Anchor =
            AnchorStyles.Top Or
            AnchorStyles.Bottom Or
            AnchorStyles.Left Or
            AnchorStyles.Right

        missionGroup.Controls.Add(startMeasurementButton)
        missionGroup.Controls.Add(stopMeasurementButton)
        missionGroup.Controls.Add(singleMeasurementButton)
        missionGroup.Controls.Add(clearBufferButton)
        missionGroup.Controls.Add(startSpectrumButton)
        missionGroup.Controls.Add(stopSpectrumButton)
        missionGroup.Controls.Add(clearSpectrumButton)
        missionGroup.Controls.Add(resetAcquisitionButton)
        missionGroup.Controls.Add(spectrumStatusLabel)
        missionGroup.Controls.Add(liveSpectrogram)

        ' ============================================================
        ' CONSOLE
        ' ============================================================
        consolePanel.Dock = DockStyle.Fill
        consolePanel.BackColor = Color.FromArgb(20, 27, 35)
        consolePanel.Padding = New Padding(18, 8, 18, 14)

        consoleTitle.AutoSize = True
        consoleTitle.Font = New Font(
            "Segoe UI Semibold",
            9.0F,
            FontStyle.Bold)
        consoleTitle.ForeColor = Color.FromArgb(150, 165, 180)
        consoleTitle.Location = New Point(18, 8)
        consoleTitle.Text = "COMMUNICATION / SYSTEM LOG"

        outputConsole.BackColor = Color.FromArgb(14, 20, 27)
        outputConsole.ForeColor = Color.FromArgb(111, 230, 149)
        outputConsole.BorderStyle = BorderStyle.None
        outputConsole.DetectUrls = False
        outputConsole.Font = New Font("Cascadia Mono", 9.5F)
        outputConsole.Location = New Point(18, 34)
        outputConsole.Size = New Size(1450, 178)
        outputConsole.Anchor =
            AnchorStyles.Top Or
            AnchorStyles.Bottom Or
            AnchorStyles.Left Or
            AnchorStyles.Right
        outputConsole.ReadOnly = True
        outputConsole.WordWrap = False

        consolePanel.Controls.Add(consoleTitle)
        consolePanel.Controls.Add(outputConsole)

        Label1.Visible = False

        ' ============================================================
        ' ROOT
        ' ============================================================
        mainLayout.Controls.Add(topArea, 0, 0)
        mainLayout.Controls.Add(missionGroup, 0, 1)
        mainLayout.Controls.Add(dashboardPanel, 0, 2)
        mainLayout.Controls.Add(consolePanel, 0, 3)

        Controls.Add(mainLayout)

        CType(baudBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(slaveBox, System.ComponentModel.ISupportInitialize).EndInit()

        ResumeLayout(False)

    End Sub

End Class
