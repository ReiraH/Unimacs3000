//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Unimacs_3000.Models
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public partial class ScreenSetting
    {
        public int id { get; set; }
        public System.DateTime timestamp { get; set; }
        public Nullable<int> page_id { get; set; }
        public Nullable<int> screen_id { get; set; }

        public virtual Page Page { get; set; }
        public virtual Screen Screen { get; set; }
        public List<SelectListItem> SelectListItems = new List<SelectListItem>();

    }
}
