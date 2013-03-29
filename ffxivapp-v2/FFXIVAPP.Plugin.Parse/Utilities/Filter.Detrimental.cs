﻿// FFXIVAPP.Plugin.Parse
// Filter.Detrimental.cs
//  
// Created by Ryan Wilson.
// Copyright © 2007-2013 Ryan Wilson - All Rights Reserved

#region Usings

using System.Text.RegularExpressions;
using FFXIVAPP.Plugin.Parse.Enums;
using FFXIVAPP.Plugin.Parse.Helpers;
using FFXIVAPP.Plugin.Parse.Models;
using FFXIVAPP.Plugin.Parse.Models.Events;
using NLog;

#endregion

namespace FFXIVAPP.Plugin.Parse.Utilities
{
    public static partial class Filter
    {
        private static void ProcessDetrimental(Event e, Expressions exp)
        {
            var line = new Line();
            var detrimental = Regex.Match("ph", @"^\.$");
            switch (e.Subject)
            {
                case EventSubject.You:
                case EventSubject.Party:
                case EventSubject.Other:
                case EventSubject.NPC:
                case EventSubject.Engaged:
                case EventSubject.UnEngaged:
                    break;
            }
            if (detrimental.Success)
            {
                return;
            }
            ClearLast();
            ParsingLogHelper.Log(LogManager.GetCurrentClassLogger(), "Detrimental", e, exp);
        }
    }
}
