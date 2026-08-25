using System.Drawing;
using System.Windows.Forms;

namespace ErmisSensorStatusTool
{
    public class SensorStatusPanel : UserControl
    {
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
            Padding = new Padding(8);

            _list = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            Controls.Add(_list);

            Sen66 = CreateItem(
                "SEN66",
                "Air quality sensor");

            Bmp280 = CreateItem(
                "BMP280",
                "Pressure / altitude");

            Sht21 = CreateItem(
                "SHT21",
                "Temperature / humidity");

            Gps = CreateItem(
                "GPS",
                "Position / satellite fix");

            Esp32P4 = CreateItem(
                "ESP32-P4",
                "USB Modbus telemetry");

            LoraLink = CreateItem(
                "LoRa LINK",
                "Ground station connection");

            _list.Controls.Add(Sen66);
            _list.Controls.Add(Bmp280);
            _list.Controls.Add(Sht21);
            _list.Controls.Add(Gps);
            _list.Controls.Add(Esp32P4);
            _list.Controls.Add(LoraLink);

            Resize += (_, _) => UpdateItemWidths();

            UpdateItemWidths();
        }

        private SensorStatusItem CreateItem(
            string name,
            string description)
        {
            return new SensorStatusItem
            {
                SensorName = name,
                Description = description,
                State = SensorState.Offline,
                Height = 50,
                Margin = new Padding(0, 0, 0, 6)
            };
        }

        private void UpdateItemWidths()
        {
            int scrollbarAllowance =
                _list.VerticalScroll.Visible ? 26 : 10;

            int itemWidth =
                System.Math.Max(
                    220,
                    _list.ClientSize.Width -
                    scrollbarAllowance);

            foreach (Control control in _list.Controls)
            {
                control.Width = itemWidth;
            }
        }

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
