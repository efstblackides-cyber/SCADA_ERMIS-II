using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ErmisSensorStatusTool
{
    public class SensorStatusItem : Control
    {
        private string _sensorName = "SENSOR";
        private string _description = "Waiting for data";
        private SensorState _state = SensorState.Offline;

        public SensorStatusItem()
        {
            DoubleBuffered = true;
            Height = 62;
            MinimumSize = new Size(250, 62);
            Margin = new Padding(0, 0, 0, 8);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            ForeColor = Color.FromArgb(26, 36, 50);
            BackColor = Color.White;
            Cursor = Cursors.Default;
        }

        public string SensorName
        {
            get => _sensorName;
            set
            {
                _sensorName = value ?? string.Empty;
                Invalidate();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value ?? string.Empty;
                Invalidate();
            }
        }

        public SensorState State
        {
            get => _state;
            set
            {
                _state = value;
                Invalidate();
            }
        }

        public string StateText => _state switch
        {
            SensorState.Online => "ONLINE",
            SensorState.Warning => "WARNING",
            _ => "OFFLINE"
        };

        private Color StateColor => _state switch
        {
            SensorState.Online => Color.FromArgb(39, 174, 96),
            SensorState.Warning => Color.FromArgb(242, 153, 74),
            _ => Color.FromArgb(149, 165, 166)
        };

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint =
                System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var rect = ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;

            using var path = RoundedRectangle(rect, 10);
            using var cardBrush = new SolidBrush(Color.White);
            using var borderPen = new Pen(Color.FromArgb(223, 229, 236), 1);

            e.Graphics.FillPath(cardBrush, path);
            e.Graphics.DrawPath(borderPen, path);

            // Left status line
            using var lineBrush = new SolidBrush(StateColor);
            using var linePath = RoundedRectangle(
                new Rectangle(0, 0, 5, Height - 1), 4);
            e.Graphics.FillPath(lineBrush, linePath);

            // Status dot
            var dotRect = new Rectangle(18, 21, 16, 16);
            using var dotOuter = new SolidBrush(Color.FromArgb(235, 238, 242));
            using var dotInner = new SolidBrush(StateColor);

            e.Graphics.FillEllipse(dotOuter,
                new Rectangle(dotRect.X - 3, dotRect.Y - 3, 22, 22));
            e.Graphics.FillEllipse(dotInner, dotRect);

            // Name
            using var nameFont = new Font(
                "Segoe UI", 10F, FontStyle.Bold);

            using var descriptionFont = new Font(
                "Segoe UI", 8.5F, FontStyle.Regular);

            using var statusFont = new Font(
                "Segoe UI", 8F, FontStyle.Bold);

            using var nameBrush =
                new SolidBrush(Color.FromArgb(25, 37, 52));

            using var descriptionBrush =
                new SolidBrush(Color.FromArgb(110, 123, 138));

            using var statusBrush =
                new SolidBrush(StateColor);

            e.Graphics.DrawString(
                SensorName,
                nameFont,
                nameBrush,
                new PointF(48, 11));

            e.Graphics.DrawString(
                Description,
                descriptionFont,
                descriptionBrush,
                new PointF(48, 34));

            var statusSize =
                e.Graphics.MeasureString(StateText, statusFont);

            e.Graphics.DrawString(
                StateText,
                statusFont,
                statusBrush,
                new PointF(
                    Width - statusSize.Width - 18,
                    21));
        }

        private static GraphicsPath RoundedRectangle(
            Rectangle bounds,
            int radius)
        {
            int diameter = radius * 2;

            var path = new GraphicsPath();

            path.AddArc(
                bounds.Left,
                bounds.Top,
                diameter,
                diameter,
                180,
                90);

            path.AddArc(
                bounds.Right - diameter,
                bounds.Top,
                diameter,
                diameter,
                270,
                90);

            path.AddArc(
                bounds.Right - diameter,
                bounds.Bottom - diameter,
                diameter,
                diameter,
                0,
                90);

            path.AddArc(
                bounds.Left,
                bounds.Bottom - diameter,
                diameter,
                diameter,
                90,
                90);

            path.CloseFigure();

            return path;
        }
    }
}
