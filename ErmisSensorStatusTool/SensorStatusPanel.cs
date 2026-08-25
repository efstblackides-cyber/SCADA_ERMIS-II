using System.Drawing;
using System.Windows.Forms;

namespace ErmisSensorStatusTool
{
    public class SensorStatusPanel : UserControl
    {
        private readonly Label _title;
        private readonly Label _subtitle;
        private readonly FlowLayoutPanel _list;

        public SensorStatusItem Sen66 { get; }
        public SensorStatusItem Bmp280 { get; }
        public SensorStatusItem Sht21 { get; }
        public SensorStatusItem Gps { get; }
        public SensorStatusItem Esp32P4 { get; }
        public SensorStatusItem LoraLink { get; }

        public SensorStatusPanel()
        {
            DoubleBuffered = true;
            BackColor = Color.FromArgb(245, 247, 250);
            Size = new Size(390, 520);
            MinimumSize = new Size(320, 420);
            Padding = new Padding(18);

            _title = new Label
            {
                AutoSize = true,
                Text = "SYSTEM STATUS",
                Font = new Font(
                    "Segoe UI",
                    14F,
                    FontStyle.Bold),
                ForeColor = Color.FromArgb(24, 38, 55),
                Location = new Point(18, 16)
            };

            _subtitle = new Label
            {
                AutoSize = true,
                Text = "Sensors & communication",
                Font = new Font(
                    "Segoe UI",
                    8.5F,
                    FontStyle.Regular),
                ForeColor = Color.FromArgb(116, 129, 145),
                Location = new Point(20, 45)
            };

            _list = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Location = new Point(18, 76),
                Anchor = AnchorStyles.Top |
                         AnchorStyles.Bottom |
                         AnchorStyles.Left |
                         AnchorStyles.Right
            };

            Controls.Add(_title);
            Controls.Add(_subtitle);
            Controls.Add(_list);

            Sen66 = CreateItem(
                "SEN66",
                "Air quality sensor",
                SensorState.Online);

            Bmp280 = CreateItem(
                "BMP280",
                "Pressure / altitude",
                SensorState.Online);

            Sht21 = CreateItem(
                "SHT21",
                "Temperature / humidity",
                SensorState.Online);

            Gps = CreateItem(
                "GPS",
                "Position / satellite fix",
                SensorState.Warning);

            Esp32P4 = CreateItem(
                "ESP32-P4",
                "USB Modbus telemetry",
                SensorState.Online);

            LoraLink = CreateItem(
                "LoRa LINK",
                "Ground station connection",
                SensorState.Offline);

            _list.Controls.Add(Sen66);
            _list.Controls.Add(Bmp280);
            _list.Controls.Add(Sht21);
            _list.Controls.Add(Gps);
            _list.Controls.Add(Esp32P4);
            _list.Controls.Add(LoraLink);

            Resize += (_, _) => UpdateLayout();
            UpdateLayout();
        }

        private SensorStatusItem CreateItem(
            string name,
            string description,
            SensorState state)
        {
            return new SensorStatusItem
            {
                SensorName = name,
                Description = description,
                State = state
            };
        }

        private void UpdateLayout()
        {
            _list.Size = new Size(
                Math.Max(200, Width - 36),
                Math.Max(180, Height - 94));

            int itemWidth = Math.Max(
                250,
                _list.ClientSize.Width - 22);

            foreach (Control control in _list.Controls)
            {
                control.Width = itemWidth;
            }
        }

        // Optional helper for later real data integration.
        public void SetAllOffline()
        {
            Sen66.State = SensorState.Offline;
            Bmp280.State = SensorState.Offline;
            Sht21.State = SensorState.Offline;
            Gps.State = SensorState.Offline;
            Esp32P4.State = SensorState.Offline;
            LoraLink.State = SensorState.Offline;
        }
    }
}
