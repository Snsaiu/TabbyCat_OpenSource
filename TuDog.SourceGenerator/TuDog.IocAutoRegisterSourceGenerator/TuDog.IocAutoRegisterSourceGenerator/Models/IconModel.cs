using System;
using System.Collections.Generic;
using System.Text;

namespace TuDog.IocAutoRegisterSourceGenerator.Models
{   
    public class GlyphsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string icon_id { get; set; }
        /// <summary>
        /// 新增聊天次数
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string font_class { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string unicode { get; set; } 
        /// <summary>
        /// 
        /// </summary>
        public int unicode_decimal { get; set; }
    }

    public class IconModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string font_family { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string css_prefix_text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<GlyphsItem> glyphs { get; set; }
    }

}
