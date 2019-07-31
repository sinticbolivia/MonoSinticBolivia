using System;
using Gtk;
namespace SinticBolivia
{
	public interface SBIGtkApplication : SBIApplication
	{
		Gtk.Window AppWindow{get;}
		Gtk.MenuBar MainMenu{get;}
		Gtk.VBox MainPanel{get;}
		Gtk.Statusbar StatusBar{get;}
		Gtk.Notebook	Notebook{ get;}
		void RegisterMenu();
		int addNewPage(string title, Gtk.Widget widget);
		bool PageExists(string title, out int index);
	}
    // delegate declaration
    public delegate void SBMatchSelected (object sender, TreeIter iter, TreeModel model); 
    public class SBEntryCompletion : Gtk.EntryCompletion
    {
        public event SBMatchSelected OnSBMatchSelected;
        protected override void OnActionActivated(int index_)
        {
            Console.WriteLine("action activated");
            base.OnActionActivated(index_);
        }
        protected override bool OnCursorOnMatch(TreeModel model, TreeIter iter)
        {
            Console.WriteLine("cursor_match: {0}", model.GetValue(iter, 0));
            return base.OnCursorOnMatch(model, iter);
        }
        protected override bool OnMatchSelected(TreeModel model, TreeIter iter)
        {
            //bool res = base.OnMatchSelected(model, iter);

            return true;
        }
    }
}

