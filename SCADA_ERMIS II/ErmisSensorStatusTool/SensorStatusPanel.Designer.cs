using System.Drawing;
using System.Windows.Forms;

namespace ErmisSensorStatusTool
{
    partial class SensorStatusPanel
    {
        private System.ComponentModel.IContainer? components = null;

        private Panel headerPanel = null!;
        private Label titleLabel = null!;
        private Label subtitleLabel = null!;
        private Panel listPanel = null!;

        private SensorStatusItem sen66Item = null!;
        private SensorStatusItem bmp280Item = null!;
        private SensorStatusItem sht21Item = null!;
        private SensorStatusItem gpsItem = null!;
        private SensorStatusItem esp32P4Item = null!;
        private SensorStatusItem loraItem = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            headerPanel = new Panel();
            titleLabel = new Label();
            subtitleLabel = new Label();

            listPanel = new Panel();

            sen66Item = new SensorStatusItem();
            bmp280Item = new SensorStatusItem();
            sht21Item = new SensorStatusItem();
            gpsItem = new SensorStatusItem();
            esp32P4Item = new SensorStatusItem();
            loraItem = new SensorStatusItem();

            SuspendLayout();

            // ========================================================
            // SENSOR STATUS PANEL
            // ========================================================
            BackColor = Color.FromArgb(245, 247, 250);
            MinimumSize = new Size(320, 300);
            Name = "SensorStatusPanel";
            Size = new Size(500, 360);

            // ========================================================
            // HEADER
            // ========================================================
            headerPanel.BackColor = Color.Transparent;
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 48;
            headerPanel.Name = "headerPanel";

            titleLabel.AutoSize = true;
            titleLabel.Font = new Font(
                "Segoe UI Semibold",
                12.0F,
                FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(24, 38, 55);
            titleLabel.Location = new Point(8, 3);
            titleLabel.Name = "titleLabel";
            titleLabel.Text = "SENSOR STATUS";

            subtitleLabel.AutoSize = true;
            subtitleLabel.Font = new Font(
                "Segoe UI",
                8.0F,
                FontStyle.Regular);
            subtitleLabel.ForeColor = Color.FromArgb(116, 129, 145);
            subtitleLabel.Location = new Point(10, 27);
            subtitleLabel.Name = "subtitleLabel";
            subtitleLabel.Text = "On-board devices & communication";

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);

            // ========================================================
            // LIST PANEL
            // ========================================================
            listPanel.AutoScroll = true;
            listPanel.BackColor = Color.Transparent;
            listPanel.Dock = DockStyle.Fill;
            listPanel.Name = "listPanel";
            listPanel.Padding = new Padding(6, 4, 6, 6);

            // ========================================================
            // SEN66
            // ========================================================
            sen66Item.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;
            sen66Item.Location = new Point(6, 4);
            sen66Item.Name = "sen66Item";
            sen66Item.Size = new Size(470, 44);
            sen66Item.SensorName = "SEN66";
            sen66Item.Description = "Air quality sensor";
            sen66Item.State = SensorState.Offline;

            // ========================================================
            // BMP280
            // ========================================================
            bmp280Item.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;
            bmp280Item.Location = new Point(6, 52);
            bmp280Item.Name = "bmp280Item";
            bmp280Item.Size = new Size(470, 44);
            bmp280Item.SensorName = "BMP280";
            bmp280Item.Description = "Pressure / altitude";
            bmp280Item.State = SensorState.Offline;

            // ========================================================
            // SHT21
            // ========================================================
            sht21Item.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;
            sht21Item.Location = new Point(6, 100);
            sht21Item.Name = "sht21Item";
            sht21Item.Size = new Size(470, 44);
            sht21Item.SensorName = "SHT21";
            sht21Item.Description = "Temperature / humidity";
            sht21Item.State = SensorState.Offline;

            // ========================================================
            // GPS
            // ========================================================
            gpsItem.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;
            gpsItem.Location = new Point(6, 148);
            gpsItem.Name = "gpsItem";
            gpsItem.Size = new Size(470, 44);
            gpsItem.SensorName = "GPS";
            gpsItem.Description = "Position / satellite fix";
            gpsItem.State = SensorState.Offline;

            // ========================================================
            // ESP32-P4
            // ========================================================
            esp32P4Item.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;
            esp32P4Item.Location = new Point(6, 196);
            esp32P4Item.Name = "esp32P4Item";
            esp32P4Item.Size = new Size(470, 44);
            esp32P4Item.SensorName = "ESP32-P4";
            esp32P4Item.Description = "USB Modbus telemetry";
            esp32P4Item.State = SensorState.Offline;

            // ========================================================
            // LORA LINK
            // ========================================================
            loraItem.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;
            loraItem.Location = new Point(6, 244);
            loraItem.Name = "loraItem";
            loraItem.Size = new Size(470, 44);
            loraItem.SensorName = "LoRa LINK";
            loraItem.Description = "Ground station connection";
            loraItem.State = SensorState.Offline;

            listPanel.Controls.Add(sen66Item);
            listPanel.Controls.Add(bmp280Item);
            listPanel.Controls.Add(sht21Item);
            listPanel.Controls.Add(gpsItem);
            listPanel.Controls.Add(esp32P4Item);
            listPanel.Controls.Add(loraItem);

            Controls.Add(listPanel);
            Controls.Add(headerPanel);

            ResumeLayout(false);
        }
    }
}
