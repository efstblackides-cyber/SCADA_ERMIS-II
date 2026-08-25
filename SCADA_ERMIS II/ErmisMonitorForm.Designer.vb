Imports System.Drawing
Imports System.Windows.Forms

Partial Public Class ErmisMonitorForm

    Private components As System.ComponentModel.IContainer

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
    Friend WithEvents outputConsole As RichTextBox
    Friend WithEvents statusLabel As Label
    Friend WithEvents statusDot As Label
    Friend WithEvents portLabel As Label
    Friend WithEvents baudLabel As Label
    Friend WithEvents slaveLabel As Label

    Friend WithEvents GroupBox1 As GroupBox
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

    Friend WithEvents GPSBox As GroupBox
    Friend WithEvents XName As Label
    Friend WithEvents YName As Label
    Friend WithEvents Y As Label
    Friend WithEvents X As Label
    Friend WithEvents ZName As Label
    Friend WithEvents Z As Label

    Friend WithEvents dashboardPanel As Panel
    Friend WithEvents tempHeader1 As Label
    Friend WithEvents tempHeader2 As Label
    Friend WithEvents tempHeader3 As Label
    Friend WithEvents pmHeader1 As Label
    Friend WithEvents pmHeader25 As Label
    Friend WithEvents pmHeader4 As Label
    Friend WithEvents pmHeader10 As Label
    Friend WithEvents pressureUnit As Label
    Friend WithEvents altitudeUnit As Label
    Friend WithEvents consoleTitle As Label
    Friend WithEvents consolePanel As Panel

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    Private Sub InitializeComponent()
        components = New System.ComponentModel.Container()

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
        GroupBox1 = New GroupBox()
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
        GPSBox = New GroupBox()
        XName = New Label()
        YName = New Label()
        ZName = New Label()
        X = New Label()
        Y = New Label()
        Z = New Label()
        Label1 = New Label()

        consolePanel = New Panel()
        consoleTitle = New Label()
        outputConsole = New RichTextBox()

        CType(baudBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(slaveBox, System.ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()

        ' ===== FORM =====
        AutoScaleDimensions = New SizeF(8.0F, 20.0F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(242, 245, 249)
        ClientSize = New Size(1240, 800)
        MinimumSize = New Size(1050, 700)
        Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        StartPosition = FormStartPosition.CenterScreen
        Text = "ERMIS II | Telemetry Control Center"

        ' ===== TOP AREA =====
        topArea.BackColor = Color.FromArgb(242, 245, 249)
        topArea.Dock = DockStyle.Top
        topArea.Height = 178
        topArea.Padding = New Padding(18, 14, 18, 10)

        headerPanel.BackColor = Color.FromArgb(20, 32, 48)
        headerPanel.Dock = DockStyle.Top
        headerPanel.Height = 68
        headerPanel.Padding = New Padding(20, 10, 20, 8)

        titleLabel.AutoSize = True
        titleLabel.ForeColor = Color.White
        titleLabel.Font = New Font("Segoe UI Semibold", 17.0F, FontStyle.Bold)
        titleLabel.Location = New Point(20, 8)
        titleLabel.Text = "ERMIS II"

        subtitleLabel.AutoSize = True
        subtitleLabel.ForeColor = Color.FromArgb(173, 190, 207)
        subtitleLabel.Font = New Font("Segoe UI", 9.0F)
        subtitleLabel.Location = New Point(22, 42)
        subtitleLabel.Text = "LoRa Modbus Telemetry · Environmental & Flight Monitoring"

        headerPanel.Controls.Add(titleLabel)
        headerPanel.Controls.Add(subtitleLabel)

        connectionPanel.BackColor = Color.White
        connectionPanel.Dock = DockStyle.Fill
        connectionPanel.Padding = New Padding(16, 10, 16, 10)

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
        refreshButton.FlatAppearance.BorderColor = Color.FromArgb(210, 218, 226)
        refreshButton.BackColor = Color.White
        refreshButton.ForeColor = Color.FromArgb(45, 60, 75)
        refreshButton.Font = New Font("Segoe UI Semibold", 9.0F)
        refreshButton.Location = New Point(170, 30)
        refreshButton.Size = New Size(96, 34)
        refreshButton.Text = "Refresh"

        connectButton.FlatStyle = FlatStyle.Flat
        connectButton.FlatAppearance.BorderSize = 0
        connectButton.BackColor = Color.FromArgb(0, 122, 204)
        connectButton.ForeColor = Color.White
        connectButton.Font = New Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
        connectButton.Location = New Point(276, 30)
        connectButton.Size = New Size(116, 34)
        connectButton.Text = "Connect"

        baudLabel.AutoSize = True
        baudLabel.Font = New Font("Segoe UI Semibold", 8.5F)
        baudLabel.ForeColor = Color.FromArgb(92, 108, 124)
        baudLabel.Location = New Point(418, 8)
        baudLabel.Text = "BAUD RATE"

        baudBox.BorderStyle = BorderStyle.FixedSingle
        baudBox.Increment = New Decimal(New Integer() {9600, 0, 0, 0})
        baudBox.Maximum = New Decimal(New Integer() {921600, 0, 0, 0})
        baudBox.Minimum = New Decimal(New Integer() {1200, 0, 0, 0})
        baudBox.Location = New Point(418, 31)
        baudBox.Size = New Size(116, 27)
        baudBox.Value = New Decimal(New Integer() {115200, 0, 0, 0})

        slaveLabel.AutoSize = True
        slaveLabel.Font = New Font("Segoe UI Semibold", 8.5F)
        slaveLabel.ForeColor = Color.FromArgb(92, 108, 124)
        slaveLabel.Location = New Point(554, 8)
        slaveLabel.Text = "SLAVE ID"

        slaveBox.BorderStyle = BorderStyle.FixedSingle
        slaveBox.Maximum = New Decimal(New Integer() {247, 0, 0, 0})
        slaveBox.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        slaveBox.Location = New Point(554, 31)
        slaveBox.Size = New Size(76, 27)
        slaveBox.Value = New Decimal(New Integer() {1, 0, 0, 0})

        autoAckBox.AutoSize = True
        autoAckBox.Checked = True
        autoAckBox.CheckState = CheckState.Checked
        autoAckBox.Font = New Font("Segoe UI", 9.0F)
        autoAckBox.ForeColor = Color.FromArgb(45, 60, 75)
        autoAckBox.Location = New Point(654, 33)
        autoAckBox.Text = "Auto ACK"

        clearButton.FlatStyle = FlatStyle.Flat
        clearButton.FlatAppearance.BorderColor = Color.FromArgb(210, 218, 226)
        clearButton.BackColor = Color.White
        clearButton.ForeColor = Color.FromArgb(45, 60, 75)
        clearButton.Font = New Font("Segoe UI Semibold", 9.0F)
        clearButton.Location = New Point(768, 30)
        clearButton.Size = New Size(96, 34)
        clearButton.Text = "Clear log"

        statusDot.AutoSize = False
        statusDot.BackColor = Color.FromArgb(145, 155, 165)
        statusDot.Location = New Point(894, 38)
        statusDot.Size = New Size(10, 10)

        statusLabel.AutoSize = True
        statusLabel.Font = New Font("Segoe UI Semibold", 9.0F)
        statusLabel.ForeColor = Color.FromArgb(69, 82, 95)
        statusLabel.Location = New Point(914, 32)
        statusLabel.MaximumSize = New Size(300, 45)
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

        ' ===== DASHBOARD =====
        dashboardPanel.Dock = DockStyle.Fill
        dashboardPanel.BackColor = Color.FromArgb(242, 245, 249)
        dashboardPanel.Padding = New Padding(18, 6, 18, 10)

        GroupBox1.BackColor = Color.White
        GroupBox1.ForeColor = Color.FromArgb(33, 49, 64)
        GroupBox1.Font = New Font("Segoe UI Semibold", 10.0F, FontStyle.Bold)
        GroupBox1.Location = New Point(18, 8)
        GroupBox1.Size = New Size(770, 375)
        GroupBox1.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        GroupBox1.Text = "  LIVE TELEMETRY  "

        ' row labels
        StyleSectionLabel(TempName, "TEMPERATURE", 26, 48)
        StyleSectionLabel(PressName, "PRESSURE / ALTITUDE", 26, 115)
        StyleSectionLabel(PartName, "PARTICULATE MATTER", 26, 182)
        StyleSectionLabel(CO2Name, "CO₂", 26, 272)
        StyleSectionLabel(VOCName, "VOC INDEX", 270, 272)
        StyleSectionLabel(RHName, "REL. HUMIDITY", 514, 272)

        StyleSmallHeader(tempHeader1, "SEN66", 195, 37)
        StyleSmallHeader(tempHeader2, "BMP280", 385, 37)
        StyleSmallHeader(tempHeader3, "SHT21", 575, 37)
        StyleValueLabel(Temp1, "--.- °C", 195, 57, 150)
        StyleValueLabel(Temp2, "--.- °C", 385, 57, 150)
        StyleValueLabel(Temp3, "--.- °C", 575, 57, 150)

        StyleSmallHeader(pressureUnit, "BAROMETER", 195, 104)
        StyleSmallHeader(altitudeUnit, "ALTITUDE", 385, 104)
        StyleSmallHeader(Press3, "REFERENCE", 575, 104)
        StyleValueLabel(Press1, "----.-- hPa", 195, 124, 160)
        StyleValueLabel(Press2, "---- m", 385, 124, 150)
        StyleValueLabel(Press3, "1013.25 hPa", 575, 124, 165)

        StyleSmallHeader(pmHeader1, "PM1.0", 195, 171)
        StyleSmallHeader(pmHeader25, "PM2.5", 335, 171)
        StyleSmallHeader(pmHeader4, "PM4.0", 475, 171)
        StyleSmallHeader(pmHeader10, "PM10", 615, 171)
        StyleValueLabel(Pm1, "--.- µg/m³", 195, 191, 130)
        StyleValueLabel(Pm25, "--.- µg/m³", 335, 191, 130)
        StyleValueLabel(Pm5, "--.- µg/m³", 475, 191, 130)
        StyleValueLabel(Pm10, "--.- µg/m³", 615, 191, 130)

        StyleMetricValue(Label2, "---- ppm", 26, 300, 200)
        StyleMetricValue(VOC, "---.-", 270, 300, 180)
        StyleMetricValue(RH, "--.- %RH", 514, 300, 200)

        GroupBox1.Controls.Add(TempName)
        GroupBox1.Controls.Add(PressName)
        GroupBox1.Controls.Add(PartName)
        GroupBox1.Controls.Add(CO2Name)
        GroupBox1.Controls.Add(VOCName)
        GroupBox1.Controls.Add(RHName)
        GroupBox1.Controls.Add(tempHeader1)
        GroupBox1.Controls.Add(tempHeader2)
        GroupBox1.Controls.Add(tempHeader3)
        GroupBox1.Controls.Add(Temp1)
        GroupBox1.Controls.Add(Temp2)
        GroupBox1.Controls.Add(Temp3)
        GroupBox1.Controls.Add(pressureUnit)
        GroupBox1.Controls.Add(altitudeUnit)
        GroupBox1.Controls.Add(Press3)
        GroupBox1.Controls.Add(Press1)
        GroupBox1.Controls.Add(Press2)
        GroupBox1.Controls.Add(pmHeader1)
        GroupBox1.Controls.Add(pmHeader25)
        GroupBox1.Controls.Add(pmHeader4)
        GroupBox1.Controls.Add(pmHeader10)
        GroupBox1.Controls.Add(Pm1)
        GroupBox1.Controls.Add(Pm25)
        GroupBox1.Controls.Add(Pm5)
        GroupBox1.Controls.Add(Pm10)
        GroupBox1.Controls.Add(Label2)
        GroupBox1.Controls.Add(VOC)
        GroupBox1.Controls.Add(RH)

        ' GPS card
        GPSBox.BackColor = Color.White
        GPSBox.ForeColor = Color.FromArgb(33, 49, 64)
        GPSBox.Font = New Font("Segoe UI Semibold", 10.0F, FontStyle.Bold)
        GPSBox.Location = New Point(806, 8)
        GPSBox.Size = New Size(416, 375)
        GPSBox.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        GPSBox.Text = "  GPS / POSITION  "

        StyleSectionLabel(XName, "LATITUDE", 28, 54)
        StyleMetricValue(X, "NO FIX", 28, 78, 340)
        X.Font = New Font("Segoe UI Semibold", 20.0F, FontStyle.Bold)

        StyleSectionLabel(YName, "LONGITUDE", 28, 146)
        StyleMetricValue(Y, "NO FIX", 28, 170, 340)
        Y.Font = New Font("Segoe UI Semibold", 20.0F, FontStyle.Bold)

        StyleSectionLabel(ZName, "ALTITUDE", 28, 238)
        StyleMetricValue(Z, "NO DATA", 28, 262, 340)
        Z.Font = New Font("Segoe UI Semibold", 20.0F, FontStyle.Bold)

        GPSBox.Controls.Add(XName)
        GPSBox.Controls.Add(X)
        GPSBox.Controls.Add(YName)
        GPSBox.Controls.Add(Y)
        GPSBox.Controls.Add(ZName)
        GPSBox.Controls.Add(Z)

        dashboardPanel.Controls.Add(GroupBox1)
        dashboardPanel.Controls.Add(GPSBox)

        ' ===== CONSOLE =====
        consolePanel.Dock = DockStyle.Bottom
        consolePanel.Height = 220
        consolePanel.BackColor = Color.FromArgb(20, 27, 35)
        consolePanel.Padding = New Padding(18, 8, 18, 14)

        consoleTitle.AutoSize = True
        consoleTitle.Font = New Font("Segoe UI Semibold", 9.0F, FontStyle.Bold)
        consoleTitle.ForeColor = Color.FromArgb(150, 165, 180)
        consoleTitle.Location = New Point(18, 8)
        consoleTitle.Text = "SYSTEM LOG"

        outputConsole.BackColor = Color.FromArgb(14, 20, 27)
        outputConsole.ForeColor = Color.FromArgb(111, 230, 149)
        outputConsole.BorderStyle = BorderStyle.None
        outputConsole.DetectUrls = False
        outputConsole.Font = New Font("Cascadia Mono", 9.5F)
        outputConsole.Location = New Point(18, 32)
        outputConsole.Size = New Size(1204, 172)
        outputConsole.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        outputConsole.ReadOnly = True
        outputConsole.WordWrap = False

        consolePanel.Controls.Add(consoleTitle)
        consolePanel.Controls.Add(outputConsole)

        ' unused legacy placeholder
        Label1.Visible = False

        Controls.Add(dashboardPanel)
        Controls.Add(consolePanel)
        Controls.Add(topArea)

        CType(baudBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(slaveBox, System.ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Private Shared Sub StyleSectionLabel(label As Label, textValue As String, x As Integer, y As Integer)
        label.AutoSize = True
        label.Text = textValue
        label.Location = New Point(x, y)
        label.Font = New Font("Segoe UI Semibold", 8.0F, FontStyle.Bold)
        label.ForeColor = Color.FromArgb(105, 119, 133)
    End Sub

    Private Shared Sub StyleSmallHeader(label As Label, textValue As String, x As Integer, y As Integer)
        label.AutoSize = True
        label.Text = textValue
        label.Location = New Point(x, y)
        label.Font = New Font("Segoe UI", 8.0F, FontStyle.Regular)
        label.ForeColor = Color.FromArgb(130, 143, 156)
    End Sub

    Private Shared Sub StyleValueLabel(label As Label, textValue As String, x As Integer, y As Integer, width As Integer)
        label.AutoSize = False
        label.Text = textValue
        label.Location = New Point(x, y)
        label.Size = New Size(width, 34)
        label.Font = New Font("Segoe UI Semibold", 14.0F, FontStyle.Bold)
        label.ForeColor = Color.FromArgb(25, 42, 58)
    End Sub

    Private Shared Sub StyleMetricValue(label As Label, textValue As String, x As Integer, y As Integer, width As Integer)
        label.AutoSize = False
        label.Text = textValue
        label.Location = New Point(x, y)
        label.Size = New Size(width, 42)
        label.Font = New Font("Segoe UI Semibold", 17.0F, FontStyle.Bold)
        label.ForeColor = Color.FromArgb(0, 102, 179)
    End Sub

End Class
