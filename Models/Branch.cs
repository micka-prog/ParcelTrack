namespace ParcelTrack.Models
{
    // One ParcelTrack branch, where customers collect their parcels: Half Way Tree, Montego Bay...
    public class Branch
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        // The parish the branch is in
        public string Parish { get; set; } = "";

        // Navigation property: the parcels going to this branch. Filled by Include.
        public List<Parcel> Parcels { get; set; } = new List<Parcel>();
    }
}
