using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
namespace FIWAREHub.Models.Sql
{

    public partial class ClusterCentroid
    {
        public ClusterCentroid(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
