using System;
using Gtk;

namespace SinticBolivia
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class SBDatePicker : Gtk.Bin
    {
        public int Height
        {
            set{this.fixed1.HeightRequest = value;}
            get{return this.fixed1.HeightRequest;}
        }
        public int Width
        {
            set{this.fixed1.WidthRequest = value;}
            get{return this.fixed1.WidthRequest;}
        }
        public Calendar TheCalendar
        {
            get{return this._calendar;}
        }
        public string Date
        {
            set{this.entryDate.Text = value;}
            get{return this.entryDate.Text.Trim();}
        }

        protected Calendar _calendar = null;
        public string DateFormat = "dd/MM/yyyy";

        public SBDatePicker()
        {
            this.Build();

            this.buttonShowCalendar.Clicked += delegate(object sender, EventArgs e)
            {
                this.ShowCalendar();
            };
        }
        public void ShowCalendar()
        {
            if (this._calendar != null)
            {
                this._calendar.Destroy();
                this._calendar = null;
                return;
            }
            this._calendar = new Calendar();
            this._calendar.WidthRequest = 200;
            this._calendar.HeightRequest = 200;
            this._calendar.DaySelected += new EventHandler(this.OnDaySelected);
            this._calendar.DaySelectedDoubleClick += new EventHandler(this.OnDaySelectedDoubleClicked);
            int x, y;
            this.entryDate.GdkWindow.GetOrigin(out x, out y);
            this.fixed1.Put(this._calendar, 0, 25);
            this.fixed1.HeightRequest = 29;
            this.fixed1.WidthRequest = 197;
            this.fixed1.ShowAll();
        }
        protected void OnDaySelected(object sender, EventArgs args)
        {
            DateTime date = this._calendar.GetDate();
            this.entryDate.Text = date.ToString(this.DateFormat);
        }
        [GLib.ConnectBefore]
        protected void OnDaySelectedDoubleClicked(object sender, EventArgs args)
        {
            DateTime date = this._calendar.GetDate();
            this.entryDate.Text = date.ToString(this.DateFormat);
            this._calendar.Hide();
            this._calendar.Destroy();
            this._calendar = null;
            (SBGlobals.getVar("app") as SBApplication).logString("day double clicked");
        }
    }
}

