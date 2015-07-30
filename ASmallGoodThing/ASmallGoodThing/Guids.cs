// Guids.cs
// MUST match guids.h
using System;

namespace mkkim1129.ASmallGoodThing
{
    static class GuidList
    {
        public const string guidASmallGoodThingPkgString = "34cd23bf-1b96-4640-ba6e-4a553302af9a";
        public const string guidASmallGoodThingCmdSetString = "86a8bb61-742f-41e5-8e2c-f3b9647cc751";

        public static readonly Guid guidASmallGoodThingCmdSet = new Guid(guidASmallGoodThingCmdSetString);
    };
}