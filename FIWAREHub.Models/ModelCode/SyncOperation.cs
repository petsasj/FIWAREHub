using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
namespace FIWAREHub.Models.Sql
{

    public partial class SyncOperation
    {
        public SyncOperation(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction(); 
            this.DateStarted = DateTime.UtcNow;
        }

        public string PercentageString => this.Percentage.ToString("P2");

        public string EstimatedTimeLeft()
        {
            if (DateFinished.HasValue)
                return "Operation has already finished";

            if (!DateModified.HasValue || !CurrentItem.HasValue || !TotalItemCount.HasValue)
                return "Please try again later for this calculation";

            var timeDifference = DateModified.Value - DateStarted;
            var timePerItem = timeDifference.Milliseconds / CurrentItem.Value;
            var remainingTime = (timePerItem) / TotalItemCount.Value - CurrentItem.Value;
            var timeSpan = TimeSpan.FromMilliseconds(remainingTime);
            var humanReadableRemainingTime = $"{timeSpan.Hours:D2}h:{timeSpan.Minutes:D2}m:{timeSpan.Seconds:D2}s:{timeSpan.Milliseconds:D3}ms";

            return humanReadableRemainingTime;
        }
    }

}
