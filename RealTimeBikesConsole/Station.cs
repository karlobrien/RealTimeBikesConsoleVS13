﻿using System;

namespace RealTimeBikesConsole
{
    public class Station
    {
        public int Number { get; set; }
        public string ContractName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Pos Position { get; set; }
        public bool Banking { get; set; }
        public string Bonus { get; set; }
        public string Status { get; set; }
        public int BikeStands { get; set; }
        public int AvailableBikeStands { get; set; }
        public int AvailableBikes { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public class Pos
    {
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
    }
}