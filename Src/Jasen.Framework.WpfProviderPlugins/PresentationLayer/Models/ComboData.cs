using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jasen.Framework.WpfProviderPlugins.PresentationLayer.Models
{
    public class ComboData
    {
       
        public ComboData(string data, int id)
        {
            this.Data = data;
            this.ID = id;
        }

      
        public string Data { get; set; }

       
        public int ID { get; set; }
    }
}
