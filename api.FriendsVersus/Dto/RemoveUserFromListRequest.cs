﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.FriendsVersus.Dto
{
    public class RemoveUserFromListRequest
    {
        public int UserId { get; set; }
        public int LeaderboardId { get; set; }
    }
}
