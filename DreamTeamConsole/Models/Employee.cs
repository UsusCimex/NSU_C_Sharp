namespace DreamTeamConsole.Models
{
    public abstract record Employee(int Id, string Name)
    {
        public abstract List<Employee> GetHackatonList(Hackaton hackaton);

        public Wishlist GenerateRandomWishlist(List<Employee> potentialPartners)
        {
            var random = new Random();
            var shuffledPartners = potentialPartners
                .OrderBy(it => random.Next())
                .Select(it => it.Id)
                .ToArray();

            return new Wishlist(Id, shuffledPartners);
        }

        public void SendWishlistToHrManager(Wishlist wishlist, HrManager hrManager) 
        {
            hrManager.ReceiveWishlist(new Wishlist(wishlist));
        }

        public override string ToString()
        {
            return $"Employee({Id} : {Name})";
        }
    }
}
