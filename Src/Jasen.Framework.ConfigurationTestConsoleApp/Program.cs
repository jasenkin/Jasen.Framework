using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.Common;
using System.Data; 

namespace Jasen.Framework.ConfigurationTestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            DbContext context = new OracleDbContext();
            DataTable T=  context.ExecuteDataTable("SELECT * FROM LAB_MENU",false);
        }

    }

 

    class MySection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]

        public MyCollec MyElements

        {

            get { return (MyCollec)this[""]; }

        }

    }
 

    class MyElemenet : ConfigurationElement

    {

        [ConfigurationProperty("id", IsKey = true)]

        public int Id

        {

            get { return (int)this["id"]; }

        }

 

        [ConfigurationProperty("name")]

        public string Name

        {

            get { return (string)this["name"]; }

        }
    }


    [ConfigurationCollection(typeof(MyElemenet))]
    class MyCollec : ConfigurationElementCollection
    {
 

        protected override string ElementName
        {

            get
            {

                return "ele";

            }

        }


        protected override ConfigurationElement CreateNewElement()
        {

            return new MyElemenet();

        }
        protected override object GetElementKey(ConfigurationElement element)
        {

            return ((MyElemenet)element).Id;

        }

    }


  
}
