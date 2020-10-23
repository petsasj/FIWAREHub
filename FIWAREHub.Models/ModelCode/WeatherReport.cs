using DevExpress.Xpo;

namespace FIWAREHub.Models.Sql
{

    public partial class WeatherReport
    {
        public WeatherReport(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
