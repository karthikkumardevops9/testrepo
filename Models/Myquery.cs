using System;
using System.Collections.Generic;
using System.Linq;
// MY QUERY MODEL RETURN DOM OBJECT
namespace MSRecordsEngine.Models
{
    public class Myquery
    {
        public List<queryList> list { get; set; }
        public string Msg { get; set; }
        public bool IsError { get; set; }
        public bool isNameExist { get; set; }
        public string uiparam { get; set; }
    }
    
}