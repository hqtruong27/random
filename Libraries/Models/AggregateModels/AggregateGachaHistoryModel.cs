using Hoyoverse.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.AggregateModels
{
    public class AggregateGachaHistoryModel : GachaHistory
    {
        public int PullIndex { get; set; } = default!;
    }
}
