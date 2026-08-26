using System.Windows.Forms;

namespace ErmisSensorStatusTool
{
    public partial class SensorStatusPanel : UserControl
    {
        public SensorStatusPanel()
        {
            InitializeComponent();
        }

        public SensorStatusItem Sen66 => sen66Item;
        public SensorStatusItem Bmp280 => bmp280Item;
        public SensorStatusItem Sht21 => sht21Item;
        public SensorStatusItem Gps => gpsItem;
        public SensorStatusItem Esp32P4 => esp32P4Item;
        public SensorStatusItem LoraLink => loraItem;

        public void SetAllOffline()
        {
            sen66Item.State = SensorState.Offline;
            bmp280Item.State = SensorState.Offline;
            sht21Item.State = SensorState.Offline;
            gpsItem.State = SensorState.Offline;
            esp32P4Item.State = SensorState.Offline;
            loraItem.State = SensorState.Offline;
        }
    }
}
