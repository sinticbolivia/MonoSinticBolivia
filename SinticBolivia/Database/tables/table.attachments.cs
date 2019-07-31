using System;
using SinticBolivia;
using SinticBolivia.Database;

namespace SinticBolivia.Database.Tables
{
	public class SBTableAttachments : SBTable
	{
		protected long attachment_id;
		protected string object_type;
		protected string object_id;
		protected string title;
		protected string description;
		protected string type;
		protected string mime;
		protected string file;
		protected string size;
		protected long parent;
		protected DateTime last_modification_date;
		protected DateTime creation_date;

		public SBTableAttachments() : base("attachments", "attachment_id")
		{
		}
	}
}

