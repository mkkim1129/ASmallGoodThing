// Guids.cs
// MUST match guids.h
using System;

namespace myungk2gmailcom.ASmallGoodThing
{
    static class GuidList
    {
        public const string guidASmallGoodThingPkgString = "12797036-b265-4427-8b94-b864f24503ac";
        public const string guidASmallGoodThingCmdSetString = "830f5b67-73d7-4027-917a-a0926ed28567";

        public static readonly Guid guidASmallGoodThingCmdSet = new Guid(guidASmallGoodThingCmdSetString);
    };
}