﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.FriendsVersus.Dto
{
    public class UpdateMMRRequest
    {
        public int LeaderboardId { get; set; }
        public int NewMmr { get; set; }
    }
}
