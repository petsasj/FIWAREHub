﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
namespace FIWAREHub.Models.Sql
{

    public partial class RoadTrafficReport : XPObject
    {
        string fStreet;
        public string Street
        {
            get { return fStreet; }
            set { SetPropertyValue<string>(nameof(Street), ref fStreet, value); }
        }
    }

}
