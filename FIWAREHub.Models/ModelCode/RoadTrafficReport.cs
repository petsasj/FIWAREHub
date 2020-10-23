using DevExpress.Xpo;

namespace FIWAREHub.Models.Sql
{

    public partial class RoadTrafficReport
    {
        public RoadTrafficReport(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
