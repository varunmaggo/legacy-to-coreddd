﻿namespace CoreDddShared.Dtos
{
    public class ShipDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Tonnage { get; set; }
        public string ImoNumber { get; set; }
        public bool HasImoNumberBeenVerified { get; set; }
        public bool IsImoNumberValid { get; set; }
    }
}