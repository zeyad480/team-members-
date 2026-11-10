using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace TaskManagerApp
{
    /// <summary>
    /// Task Manager - Team Members screen.
    /// Every control is declared in TaskManagerTeamForm.Designer.cs (visible/editable in the VS designer).
    /// This file only adds runtime polish: rounded corners, double buffering, nav selection and
    /// the e-mail placeholder behaviour (WinForms TextBox has no built-in placeholder).
    /// </summary>
    public partial class TaskManagerTeamForm : Form
    {
        private const string EmailPlaceholder = "friend@example.com";

        private static readonly Color SidebarColor = Color.FromArgb(17, 24, 39);
        private static readonly Color SidebarHover = Color.FromArgb(31, 41, 55);
        private static readonly Color MutedText    = Color.FromArgb(156, 163, 175);
        private static readonly Color InputText    = Color.FromArgb(17, 24, 39);
        private static readonly Color ActiveBlue   = Color.FromArgb(37, 99, 235);
        private static readonly Color ActiveBlueDn = Color.FromArgb(29, 78, 216);

        private Button[] _navButtons;

        public TaskManagerTeamForm()
        {
            InitializeComponent();

            EnableDoubleBuffer(this);
            EnableDoubleBuffer(pnlMain);
            EnableDoubleBuffer(tblMembers);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode) return;   // designer keeps the plain square preview

            ApplyRoundedShapes();
            HookNavigation();
            HookPlaceholder();
        }

        // ---------------------------------------------------------------- rounded shapes

        private void ApplyRoundedShapes()
        {
            foreach (Button b in NavButtons()) MakeRounded(b, 10);
            MakeRounded(pnlBadgeNotifications, 10);
            MakeRounded(pnlStatusDot, 10);

            MakeRounded(btnInviteMember, 8);
            MakeRounded(btnSendInvite, 8);
            MakeRounded(pnlMembersBorder, 12);   // 1px border panel + white card = rounded bordered card
            MakeRounded(pnlInviteBorder, 12);

            // avatars, role badges, status dots inside the table
            foreach (Control cell in tblMembers.Controls)
                foreach (Control child in cell.Controls)
                {
                    if (child.Name.StartsWith("pnlAvatar", StringComparison.Ordinal)) MakeRounded(child, 18);
                    else if (child.Name.StartsWith("pnlBadge", StringComparison.Ordinal)) MakeRounded(child, 12);
                    else if (child.Name.StartsWith("pnlStatusDot", StringComparison.Ordinal)) MakeRounded(child, 5);
                }
        }

        private static void MakeRounded(Control c, int radius)
        {
            Action apply = delegate
            {
                if (c.Width <= 0 || c.Height <= 0) return;
                using (GraphicsPath path = RoundedRect(new Rectangle(0, 0, c.Width, c.Height), radius))
                {
                    Region old = c.Region;
                    c.Region = new Region(path);
                    if (old != null) old.Dispose();
                }
            };
            apply();
            c.SizeChanged += delegate { apply(); };
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            int d = Math.Max(1, Math.Min(radius * 2, Math.Min(r.Width, r.Height)));
            GraphicsPath p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        // ---------------------------------------------------------------- sidebar navigation

        private Button[] NavButtons()
        {
            if (_navButtons == null)
                _navButtons = new[]
                {
                    btnNavDashboard, btnNavProjects, btnNavKanbanBoard,
                    btnNavReports, btnNavNotifications, btnNavProfile
                };
            return _navButtons;
        }

        private void HookNavigation()
        {
            foreach (Button b in NavButtons()) b.Click += NavButton_Click;
        }

        private void NavButton_Click(object sender, EventArgs e)
        {
            Button active = (Button)sender;
            foreach (Button b in NavButtons())
            {
                bool on = ReferenceEquals(b, active);
                b.BackColor = on ? ActiveBlue : SidebarColor;
                b.ForeColor = on ? Color.White : MutedText;
                b.Font = new Font(b.Font, on ? FontStyle.Bold : FontStyle.Regular);
                b.FlatAppearance.MouseOverBackColor = on ? ActiveBlue : SidebarHover;
                b.FlatAppearance.MouseDownBackColor = on ? ActiveBlueDn : SidebarHover;
            }
        }

        // ---------------------------------------------------------------- e-mail placeholder

        private void HookPlaceholder()
        {
            txtInviteEmail.Enter += delegate
            {
                if (txtInviteEmail.Text == EmailPlaceholder && txtInviteEmail.ForeColor == MutedText)
                {
                    txtInviteEmail.Text = string.Empty;
                    txtInviteEmail.ForeColor = InputText;
                }
            };
            txtInviteEmail.Leave += delegate
            {
                if (string.IsNullOrWhiteSpace(txtInviteEmail.Text))
                {
                    txtInviteEmail.Text = EmailPlaceholder;
                    txtInviteEmail.ForeColor = MutedText;
                }
            };
        }

        // ---------------------------------------------------------------- helpers

        private static void EnableDoubleBuffer(Control c)
        {
            typeof(Control).GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic).SetValue(c, true, null);
        }

        private void btnInviteMember_Click(object sender, EventArgs e)
        {

        }
    }
}
